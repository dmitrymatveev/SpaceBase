/**
 * SpaceBase
 *
 * This program provides a collection of base classes to manage grid systems
 * in Space Engineers game from Keen Software House.
 * 
 * Space Engineers version: > 1.185.200 beta
 * 
 * Author: BitHexed aka Dmitry Matveev
 * Version: 0.1.alpha 
 * Licence: MIT
 */

static IMyGridTerminalSystem GTS = null;
static IMyProgrammableBlock PB = null;

public Program() 
{
  GTS = GridTerminalSystem;
  PB = getPB();
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

// Returns instance of IMyProgrammableBlock this is running this script
IMyProgrammableBlock getPB() {
  List<IMyTerminalBlock> pbs = new List<IMyTerminalBlock>();
  GTS.GetBlocksOfType<IMyProgrammableBlock>(pbs, (block) => (block as IMyProgrammableBlock).IsRunning);  
  return pbs.Count == 1 ? pbs[0] as IMyProgrammableBlock : null;
}
 
public void Main(string argument, UpdateType updateSource) 
{
  var panel = GTS.GetBlockWithName("Console") as IMyTextPanel;
  
  var console = new Console("Console");
  console.printLine("Hello World!");
  console.printLine("Hello World 2!");
  
   
  // panel.FontSize = 1.0f;
  // var text = string.Format("{0:G}", panel.FontSize);  
  // text = string.Format("{0:G}", PB.CustomData);
  // Echo(text);
  // 
  // panel.WritePublicText("Hello World!");
  
  try {
    throw new Exception("blah");
  }
  catch (Exception e) {
    var text = e.ToString();
    Echo(text);
    console.printLine(text);
  }
}

class Console {
  IMyTextPanel panelInstance;
  public Console(string id) {
    panelInstance = GTS.GetBlockWithName("Console") as IMyTextPanel;
  }
  
  public void printLine(string text) {
    panelInstance.WritePublicText(text + "\n");
  }
  
  public void printLine(float num) {
    panelInstance.WritePublicText(string.Format("{0:G}", num));
  }
}

class InventoryManager {
  public InventoryManager() {
    
  }
}