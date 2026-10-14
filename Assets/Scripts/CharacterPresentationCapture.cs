using UnityEngine;
using System.Collections;
using System.IO;
public class CharacterPresentationCapture:MonoBehaviour {
 public CharacterPlayerInput player;public CharacterMotionController npc;public Transform seat;
 void Capture(Camera c,string name){var rt=new RenderTexture(1440,900,24);rt.antiAliasing=4;c.targetTexture=rt;c.Render();var prior=RenderTexture.active;RenderTexture.active=rt;var t=new Texture2D(1440,900,TextureFormat.RGB24,false);t.ReadPixels(new Rect(0,0,1440,900),0,0);t.Apply();File.WriteAllBytes(Path.Combine(Application.dataPath,"../Evidence/"+name),t.EncodeToPNG());c.targetTexture=null;RenderTexture.active=prior;Destroy(t);Destroy(rt);}
 IEnumerator Start(){if(File.Exists(Path.Combine(Application.dataPath,"../Evidence/presentation_fix_v2_done.txt")))yield break;player.automated=true;yield return new WaitForSeconds(1);
  var o=new GameObject("PresentationCamera");var cam=o.AddComponent<Camera>();cam.fieldOfView=52;cam.nearClipPlane=.03f;cam.cullingMask=~(1<<9);cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=new Color(.015f,.02f,.025f);
  o.transform.position=npc.transform.position+npc.transform.forward*2.35f+npc.transform.right*.65f+Vector3.up*.95f;o.transform.LookAt(npc.transform.position+Vector3.up*.82f);Capture(cam,"character_fixed_standing.png");
  npc.SitAt(seat);yield return new WaitForSeconds(1);o.transform.position=seat.position+seat.forward*1.65f+Vector3.forward*.90f+Vector3.up*.98f;o.transform.LookAt(seat.position+Vector3.up*.85f);Capture(cam,"character_fixed_sitting.png");
  npc.StandUp();player.motion.SetHands(true,true);yield return new WaitForSeconds(1);Capture(player.eyes,"character_fixed_firstperson_hands.png");player.motion.SetHands(false,false);player.automated=false;Destroy(o);File.WriteAllText(Path.Combine(Application.dataPath,"../Evidence/presentation_fix_v2_done.txt"),"Three Play mode presentation screenshots captured; human appearance review pending.");
 }
}

