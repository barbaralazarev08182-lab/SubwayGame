using UnityEngine;
public class SubwayPlayer : MonoBehaviour {
 public Camera eyes; public bool automated; float pitch; CharacterController cc;
 void Awake(){cc=GetComponent<CharacterController>();}
 void Update(){
  if(automated)return;
  if(Input.GetMouseButtonDown(0)){Cursor.lockState=CursorLockMode.Locked;Cursor.visible=false;}
  if(Input.GetKeyDown(KeyCode.Escape)){Cursor.lockState=CursorLockMode.None;Cursor.visible=true;}
  if(Cursor.lockState==CursorLockMode.Locked){transform.Rotate(0,Input.GetAxis("Mouse X")*2,0);pitch=Mathf.Clamp(pitch-Input.GetAxis("Mouse Y")*2,-80,80);eyes.transform.localRotation=Quaternion.Euler(pitch,0,0);}
  Vector3 v=transform.right*Input.GetAxisRaw("Horizontal")+transform.forward*Input.GetAxisRaw("Vertical");
  cc.Move((Vector3.ClampMagnitude(v,1)*2.4f+Vector3.down*3)*Time.deltaTime);
 }
 void OnGUI(){GUI.Label(new Rect(16,16,650,28),"SUBWAY  |  WASD move  |  Click: mouse look  |  Esc: release cursor");}
}
