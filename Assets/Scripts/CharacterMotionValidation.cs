using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
public class CharacterMotionValidation : MonoBehaviour {
 public CharacterPlayerInput input;public CharacterMotionController npc;public Transform npcSeat;public bool runOnStart=true;
 List<string> checks=new List<string>();
 void Check(string n,bool ok){checks.Add("\""+n+"\":"+ok.ToString().ToLower());if(!ok)Debug.LogError("CHARACTER_CHECK_FAILED "+n);}
 void Place(CharacterMotionController c,Vector3 p,Quaternion r){var cc=c.GetComponent<CharacterController>();cc.enabled=false;c.transform.SetPositionAndRotation(p,r);cc.enabled=true;Physics.SyncTransforms();}
 void Capture(string name,Camera camera){var rt=new RenderTexture(1440,900,24);rt.antiAliasing=4;camera.targetTexture=rt;camera.Render();var old=RenderTexture.active;RenderTexture.active=rt;var t=new Texture2D(1440,900,TextureFormat.RGB24,false);t.ReadPixels(new Rect(0,0,1440,900),0,0);t.Apply();File.WriteAllBytes(Path.Combine(Application.dataPath,"../Evidence/"+name),t.EncodeToPNG());camera.targetTexture=null;RenderTexture.active=old;Destroy(t);Destroy(rt);}
 IEnumerator Start(){if(!runOnStart)yield break;input.automated=true;var player=input.motion;
  yield return new WaitForSeconds(2);Check("shared_controller_type",player.GetType()==npc.GetType());Check("idle",player.State==CharacterMotionState.Idle&&npc.State==CharacterMotionState.Idle);
  Capture("character_idle_player.png",input.eyes);
  Vector3 start=player.transform.position;foreach(var c in new[]{player,npc})c.SetLocomotion(Vector2.up,0,false);
  yield return new WaitForSeconds(.45f);Check("walk",player.State==CharacterMotionState.Walk&&npc.State==CharacterMotionState.Walk);Check("movement",Vector3.Distance(start,player.transform.position)>.3f);Capture("character_walk_player.png",input.eyes);
  foreach(var c in new[]{player,npc})c.SetLocomotion(Vector2.zero,55,false);yield return new WaitForSeconds(.4f);Check("turn",player.State==CharacterMotionState.Turn&&npc.State==CharacterMotionState.Turn);
  foreach(var c in new[]{player,npc})c.SetLocomotion(Vector2.zero,0,true);yield return new WaitForSeconds(.4f);Check("crouch",player.State==CharacterMotionState.Crouch&&npc.State==CharacterMotionState.Crouch);
  foreach(var c in new[]{player,npc})c.SetLocomotion(Vector2.zero,0,false);
  Place(player,new Vector3(0,.025f,-8.65f),Quaternion.identity);Place(npc,new Vector3(0,.025f,-5.5f),Quaternion.Euler(0,180,0));
  yield return new WaitForSeconds(.65f);Vector3 rest=player.leftHand.position;player.SetHands(true,false);npc.SetHands(true,false);yield return new WaitForSeconds(.5f);Check("left_independent",player.LeftRaised&&!player.RightRaised&&Vector3.Distance(rest,player.leftHand.position)>.2f);
  player.SetHands(false,true);npc.SetHands(false,true);yield return new WaitForSeconds(.5f);Check("right_independent",!player.LeftRaised&&player.RightRaised);
  player.SetHands(true,true);npc.SetHands(true,true);yield return new WaitForSeconds(.5f);Capture("character_both_hands_player.png",input.eyes);Check("both_hands",player.LeftRaised&&player.RightRaised);
  player.SetHands(false,false);npc.SetHands(false,false);yield return new WaitForSeconds(.7f);Check("hand_return",Vector3.Distance(rest,player.leftHand.position)<.025f);
  npc.SitAt(npcSeat);player.SitAt(input.seats[0]);yield return new WaitForSeconds(.65f);Check("sit",player.State==CharacterMotionState.Sit&&npc.State==CharacterMotionState.Sit&&npc.PoseAmount>.9f);
  var co=new GameObject("ValidationCamera");var cam=co.AddComponent<Camera>();cam.fieldOfView=58;cam.nearClipPlane=.04f;cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=Color.black;co.transform.position=npcSeat.position+npcSeat.forward*1.4f+Vector3.up*1.0f+Vector3.forward*.8f;co.transform.LookAt(npcSeat.position+Vector3.up*.8f);Capture("character_sit_external.png",cam);Destroy(co);
  bool a=player.StandUp(),b=npc.StandUp();yield return null;Check("stand_up",a&&b&&player.State==CharacterMotionState.StandUp&&npc.State==CharacterMotionState.StandUp);yield return new WaitForSeconds(.7f);Check("return_idle",player.State==CharacterMotionState.Idle&&npc.State==CharacterMotionState.Idle);
  Place(player,new Vector3(0,.025f,-8.65f),Quaternion.identity);Place(npc,new Vector3(0,.025f,-5.5f),Quaternion.Euler(0,180,0));input.automated=false;
  string json="{\n"+string.Join(",\n",checks)+"\n}";File.WriteAllText(Path.Combine(Application.dataPath,"../Evidence/character_motion_validation.json"),json);Debug.Log("CHARACTER_VALIDATION_COMPLETE "+json);
 }
}

