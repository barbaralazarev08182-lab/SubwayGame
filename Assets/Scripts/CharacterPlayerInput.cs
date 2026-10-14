using UnityEngine;
public class CharacterPlayerInput : MonoBehaviour {
 public CharacterMotionController motion;public Camera eyes;public Transform[] seats;public bool automated;CharacterLookController look;
 void Awake(){look=GetComponent<CharacterLookController>();if(!look)look=gameObject.AddComponent<CharacterLookController>();}
 void Update(){if(automated)return;
  if(Input.GetMouseButtonDown(0)||Input.GetMouseButtonDown(1)){Cursor.lockState=CursorLockMode.Locked;Cursor.visible=false;}
  if(Input.GetKeyDown(KeyCode.Escape)){Cursor.lockState=CursorLockMode.None;Cursor.visible=true;}
  if(Cursor.lockState==CursorLockMode.Locked)look.AddMouseLook(Input.GetAxis("Mouse X")*2,-Input.GetAxis("Mouse Y")*2);
  Vector2 keys=new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
  if(keys.sqrMagnitude>.01f)look.FollowViewWhileMoving(300*Time.deltaTime);
  Vector3 forward=Vector3.ProjectOnPlane(eyes.transform.forward,Vector3.up).normalized;
  Vector3 right=Vector3.Cross(Vector3.up,forward);
  Vector3 worldMove=forward*keys.y+right*keys.x;
  Vector3 localMove=transform.InverseTransformDirection(worldMove);
  motion.SetLocomotion(new Vector2(localMove.x,localMove.z),0,Input.GetKey(KeyCode.LeftControl)||Input.GetKey(KeyCode.C));motion.SetHands(Input.GetMouseButton(0),Input.GetMouseButton(1));
  if(Input.GetKeyDown(KeyCode.E)){if(motion.Seated)motion.StandUp();else{Transform best=null;float d=1.4f;foreach(var seat in seats){float dist=Vector3.Distance(transform.position,CharacterMotionController.ClosestSeatPoint(seat,transform.position));if(dist<d){d=dist;best=seat;}}if(best)motion.SitAt(best);}}
  float target=motion.State==CharacterMotionState.Crouch?1.005f:1.135f+motion.PoseAmount*.105f;var p=eyes.transform.localPosition;p.y=Mathf.Lerp(p.y,target,1-Mathf.Exp(-12*Time.deltaTime));eyes.transform.localPosition=p;
 }
 void OnGUI(){GUI.Label(new Rect(16,16,1000,26),"WASD move | Mouse look | Hold LMB/RMB: left/right hand | E: sit/stand near seat | Ctrl/C: crouch | Esc: cursor");GUI.Label(new Rect(16,42,500,24),"Motion: "+motion.State);if(motion.Seated&&!string.IsNullOrEmpty(motion.StandUpHint))GUI.Label(new Rect(16,68,850,28),motion.StandUpHint);GUI.Label(new Rect(Screen.width/2-5,Screen.height/2-10,20,20),"+");}
}





