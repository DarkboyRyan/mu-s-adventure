using System;
using System.Reflection;
using UnityEngine;
using TMPro;
static class Program {
 static int checks;
 static void Assert(bool result,string message){if(!result)throw new Exception(message);checks++;Console.WriteLine("PASS "+message);}
 static void Call(object o,string method,params object[] args)=>o.GetType().GetMethod(method,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(o,args);
 static void Frame(){Time.frameCount++;Input.keys.Clear();}
 static FileDocument Doc(string title,params string[] pages)=>new(){fileTitle=title,pages=pages};
 static void Main(){
 var controller=new MonoBehaviour();var ladder=new PlayerLadderClimb();controller.gameObject.components[typeof(PlayerLadderClimb)]=ladder;
 var m=new FileUIManager{fileUIPanel=new(),titleText=new(),contentText=new(),pageNumberText=new(),hintText=new(),interactionPrompt=new(),playerController=controller};
 Call(m,"Awake");Assert(!m.fileUIPanel.activeSelf,"reader starts hidden");
 var a=Doc("Blackboard","Notice","Margin");var b=Doc("Other","One","Two","Three");
 m.OpenFile(null);m.OpenFile(Doc("Empty"));m.OpenFile(Doc("Blank"," ",null));Assert(!m.IsOpen && Time.timeScale==1,"invalid documents cannot pause or lock player");
 Time.timeScale=0.5f;m.OpenFile(a);Assert(m.IsOpen && m.contentText.text=="Notice" && m.pageNumberText.text=="1 / 2","valid blackboard opens first page");
 Assert(Time.timeScale==0 && !controller.enabled && !ladder.enabled && FileUIManager.IsInputBlocked,"reading pauses game and locks movement and ladder");
 m.PreviousPage();Assert(m.pageNumberText.text=="1 / 2","first page cannot underflow");
 m.NextPage();m.NextPage();Assert(m.pageNumberText.text=="2 / 2","last page cannot overflow");
 m.OpenFile(a);m.OpenFile(b);Assert(m.titleText.text=="Blackboard" && m.contentText.text=="Margin","repeated E or a second source cannot reset or replace open document");
 m.CloseFile();Assert(Time.timeScale==0.5f && controller.enabled && ladder.enabled,"close restores original time scale and controls");
 Assert(FileUIManager.IsInputBlocked,"close consumes gameplay input for same frame");
 Frame();Assert(!FileUIManager.IsInputBlocked,"gameplay input resumes next frame");m.OpenFile(a);Assert(m.contentText.text=="Margin","reopening resumes bookmark");m.CloseFile();Frame();
 m.OpenFile(b);Assert(m.pageNumberText.text=="1 / 3","bookmarks are separate per document");m.CloseFile();Frame();
 controller.enabled=false;ladder.enabled=false;m.OpenFile(a);m.CloseFile();Assert(!controller.enabled && !ladder.enabled,"previously disabled controls stay disabled");Frame();controller.enabled=true;ladder.enabled=true;
 m.OpenFile(a);Call(m,"OnDisable");Assert(!m.IsOpen && Time.timeScale==0.5f && controller.enabled && ladder.enabled,"disabling manager restores game and controls");Frame();
 m.OpenFile(a);m.fileUIPanel.SetActive(false);Call(m,"Update");Assert(!m.IsOpen && Time.timeScale==0.5f,"external panel hide cannot leave game paused");Frame();
 var old=m.contentText;m.contentText=null;m.OpenFile(a);Assert(!m.IsOpen && Time.timeScale==0.5f,"missing UI reference cannot lock game");m.contentText=old;
 var far=new FileInteractable{document=b};far.transform.position=new Vector3(5,0,0);var near=new FileInteractable{document=a};near.transform.position=new Vector3(1,0,0);
 var p1=new Collider2D();p1.gameObject.tag="Player";var p2=new Collider2D();p2.gameObject.tag="Player";
 Call(far,"OnEnable");Call(near,"OnEnable");Call(far,"OnTriggerEnter2D",p1);Call(near,"OnTriggerEnter2D",p1);Call(near,"OnTriggerEnter2D",p2);
 Assert(FileInteractable.GetNearestAvailable()==near,"overlapping readers choose nearest target");
 Call(near,"OnTriggerExit2D",p1);Assert(FileInteractable.GetNearestAvailable()==near,"one child collider exit does not clear remaining player overlap");
 p2.enabled=false;Assert(FileInteractable.GetNearestAvailable()==far,"disabled player collider is pruned");p2.enabled=true;Call(near,"OnTriggerEnter2D",p2);
 var mini=new MinigameCameraSwitcher();mini.EnterMinigame();m.OpenFile(a);Assert(!m.IsOpen && MinigameCameraSwitcher.IsAnyMinigameActive,"minigame blocks reader");mini.ExitMinigame();Frame();
 Input.keys.Add(KeyCode.E);Call(m,"Update");Assert(m.IsOpen && m.titleText.text=="Blackboard","E opens nearest blackboard through shared manager");
 mini.EnterMinigame();Assert(!MinigameCameraSwitcher.IsAnyMinigameActive,"reader blocks minigame");Frame();Input.keys.Add(KeyCode.LeftArrow);Call(m,"Update");Assert(m.pageNumberText.text=="1 / 2","arrow key flips while paused");Frame();Input.keys.Add(KeyCode.Escape);Call(m,"Update");Assert(!m.IsOpen && Time.timeScale==0.5f,"Esc closes through paused UI update");Frame();Call(m,"Update");Assert(m.interactionPrompt.gameObject.activeSelf,"nearby readable has a prompt");
 Call(near,"OnDisable");Call(far,"OnDisable");Assert(FileInteractable.GetNearestAvailable()==null,"disabled readers are unregistered");
 Frame();m.OpenFile(a);Call(m,"OnDestroy");Assert(FileUIManager.Instance==null && Time.timeScale==0.5f,"destroy restores pause state and clears singleton");
 Console.WriteLine($"All {checks} behavior checks passed (Unity API stubs; no physics/render simulation).");
 }
}
