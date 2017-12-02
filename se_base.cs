/**
 * SpaceBase
 *
 * This program provides a collection of base classes to manage grid systems
 * in Space Engineers game from Keen Software House.
 *
 * Space Engineers version: > 1.185.2
 *
 * Author: BitHexed aka Dmitry Matveev
 * Version: 0.1.alpha
 * Licence: MIT
 */

static IMyGridTerminalSystem GTS = null;
static ProgBlock PB = null;
static MyGridProgram PROG = null;

public Program()
{
  PROG = this;
  GTS = GridTerminalSystem;
  PB = new ProgBlock();
  Runtime.UpdateFrequency = UpdateFrequency.Update1 | UpdateFrequency.Update10;
}

public void Save()
{
  // Called when the program needs to save its state. Use
  // this method to save your state to the Storage field
  // or some other means.
  //
  // This method is optional and can be removed if not
  // needed.
}

public void Main(string argument, UpdateType updateSource)
{
  var panel = GTS.GetBlockWithName("Console") as IMyTextPanel;

  var console = new LcdPanel("Console");
  console.PrintLine("Hello World!");

  var cont = new Container("Container");
  var cont2 = new Container("Container02");
  var inventory = cont.GetInventory();

  cont.MoveItemFrom(cont.GetInventory(), 0);

  console.PrintLine(inventory.MaxVolume.ToString());
  
  foreach(var val in PB.customData)
  {
    PROG.Echo(string.Format("block => {0}", val.Key));
    foreach(var d in val.Value)
    {
      PROG.Echo(string.Format("entry => {0}:{1}", d.Key, d.Value));
    }
  }
}

// Base class for all IMyTerminalBlock
class Block
{
  protected IMyTerminalBlock block;
  public Block()
  {
    block = null;
  }
  public Block(string stringId)
  {
    block = GTS.GetBlockWithName(stringId);
  }
  public Block(IMyTerminalBlock block)
  {
    this.block = block;
  }

  public Dictionary<string, Dictionary<string, string>> customData
  {
    get
    {
      return ToCustomDataMap(block.CustomData);
    }
  }
}

class ProgBlock : Block
{
  IMyProgrammableBlock pbInstance
  {
    get {return block as IMyProgrammableBlock;}
  }
  public ProgBlock() : base()
  {
    List<IMyTerminalBlock> pbs = new List<IMyTerminalBlock>();
    GTS.GetBlocksOfType<IMyProgrammableBlock>(pbs, (block) => (block as IMyProgrammableBlock).IsRunning);
    block = pbs.Count == 1 ? pbs[0] as IMyProgrammableBlock : null;
  }
}

class LcdPanel : Block
{
  IMyTextPanel panelInstance
  {
    get {return block as IMyTextPanel;}
  }

  public LcdPanel(string id) : base(id) {}
  public LcdPanel(IMyTerminalBlock block) : base(block) {}

  public void PrintLine(string text)
  {
    panelInstance.WritePublicText(text + "\n");
  }

  public void PrintLine(float num)
  {
    panelInstance.WritePublicText(string.Format("{0:G}", num));
  }
}

class Container : Block
{

  public IMyCargoContainer containerInstance
  {
    get {return block as IMyCargoContainer;}
  }

  public Container(string id) : base(id) {}
  public Container(IMyTerminalBlock block) : base(block) {}

  public IMyInventory GetInventory()
  {
    return containerInstance.GetInventory(0);
  }

  public bool MoveItemFrom(IMyInventory src, int index)
  {
    var item = src.GetItems()[0];
    // PROG.Echo("move item " + item.Amount.ToString());

    return true;
  }
}
// Util stuff

static Dictionary<string, Dictionary<string, string>> ToCustomDataMap(string source)
{
  string BLOCK_START = @":(\w+)";
  string BLOCK_END = @"::$";
  string BLOCK_PARAM = @"(\w+):(.+)";
    
  var dataParams = new Dictionary<string, Dictionary<string, string>>();
  string[] lines = System.Text.RegularExpressions.Regex.Split(source,"\r\n");
  
  var map = new Dictionary<string, string>();
  dataParams.Add("root", map);
    
  for(var i = 0; i < lines.Length; i++)
  {
    var line = lines[i];    
    var match = Match(line, BLOCK_PARAM);

    if (match.Success && map != null)
    {
      map.Add(match.Groups[1].Value.Trim(), match.Groups[2].Value.Trim());
      continue;
    }
    else if (match.Success && map == null)
    {
      throw new Exception("Invalid CustomData string");
    }
  
    match = Match(line, BLOCK_START);
    if (match.Success)
    {        
      map = new Dictionary<string, string>();
      dataParams.Add(match.Groups[1].Value.Trim(), map);
      continue;
    }
  
    if (IsMatch(line, BLOCK_END))
    {
      map = null;
    }
  }
  
  return dataParams;
}

static bool IsMatch(string input, string pattern)
{
  var IgnoreCase = System.Text.RegularExpressions.RegexOptions.IgnoreCase;
  return System.Text.RegularExpressions.Regex.IsMatch(input, pattern, IgnoreCase);
}

static System.Text.RegularExpressions.Match Match(string input, string pattern)
{
  var IgnoreCase = System.Text.RegularExpressions.RegexOptions.IgnoreCase;
  return System.Text.RegularExpressions.Regex.Match(input, pattern, IgnoreCase);
}