using UnityEngine;
using System.Collections;
using System.IO;
public class SubwayValidation : MonoBehaviour {
 public SubwayPlayer player; public bool runOnStart=true;
 IEnumerator Start(){if(!runOnStart)yield break;yield return Run();}
 void Place(Vector3 p){var c=player.GetComponent<CharacterController>();c.enabled=false;player.transform.position=p;player.transform.rotation=Quaternion.identity;c.enabled=true;Physics.SyncTransforms();}
 IEnumerator Drive(Vector3 direction,float seconds){var c=player.GetComponent<CharacterController>();float t=0;while(t<seconds){float dt=Mathf.Min(Time.deltaTime,.04f);c.Move((direction+Vector3.down*3)*dt);t+=dt;yield return null;}}
 void Capture(string name){var cam=player.eyes;var rt=new RenderTexture(1440,900,24);cam.targetTexture=rt;cam.Render();var old=RenderTexture.active;RenderTexture.active=rt;var tex=new Texture2D(1440,900,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1440,900),0,0);tex.Apply();File.WriteAllBytes(Path.Combine(Application.dataPath,"../Evidence/"+name),tex.EncodeToPNG());cam.targetTexture=null;RenderTexture.active=old;Destroy(rt);Destroy(tex);}
 IEnumerator Run(){
  Directory.CreateDirectory(Path.Combine(Application.dataPath,"../Evidence"));player.automated=true;
  Place(new Vector3(0,.04f,-8.65f));yield return new WaitForSeconds(2);Capture("unity_player_central.png");
  yield return Drive(Vector3.forward*4,4.35f);float endZ=player.transform.position.z;bool walk=endZ>8.3f&&player.transform.position.y>-.1f;
  Place(new Vector3(0,.04f,6));yield return Drive(Vector3.right*2,1.1f);float seatX=player.transform.position.x;bool seat=seatX<.85f&&seatX>.3f;
  Place(new Vector3(0,.04f,8));yield return Drive(Vector3.right*2,1.4f);float wallX=player.transform.position.x;bool wall=wallX<1.42f&&wallX>1.05f;
  Place(new Vector3(0,.04f,8.65f));yield return Drive(Vector3.forward*2,1);float stopZ=player.transform.position.z;bool end=stopZ<9.3f;
  Place(new Vector3(.50f,.04f,-8.65f));player.eyes.transform.localRotation=Quaternion.Euler(0,-5,0);yield return null;Capture("unity_player_oblique.png");
  Place(new Vector3(0,.04f,-8.65f));player.eyes.transform.localRotation=Quaternion.identity;player.automated=false;
  string json="{\n  \"walk_end_to_end\": "+walk.ToString().ToLower()+",\n  \"seat_blocks\": "+seat.ToString().ToLower()+",\n  \"wall_blocks\": "+wall.ToString().ToLower()+",\n  \"end_wall_blocks\": "+end.ToString().ToLower()+",\n  \"end_z\": "+endZ.ToString("F3",System.Globalization.CultureInfo.InvariantCulture)+",\n  \"seat_stop_x\": "+seatX.ToString("F3",System.Globalization.CultureInfo.InvariantCulture)+",\n  \"wall_stop_x\": "+wallX.ToString("F3",System.Globalization.CultureInfo.InvariantCulture)+",\n  \"player_height_m\": 1.7,\n  \"eye_height_m\": 1.58\n}";
  File.WriteAllText(Path.Combine(Application.dataPath,"../Evidence/playmode_validation.json"),json);Debug.Log("SUBWAY_RUNTIME_COMPLETE "+json);
 }
}
