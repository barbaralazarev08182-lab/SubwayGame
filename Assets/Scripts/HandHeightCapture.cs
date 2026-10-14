using UnityEngine;
using System.Collections;
using System.IO;
public class HandHeightCapture:MonoBehaviour {
 IEnumerator Start(){var p=FindFirstObjectByType<CharacterPlayerInput>();p.automated=true;p.motion.SetHands(true,true);yield return new WaitForSeconds(1.5f);
  var c=p.eyes;var rt=new RenderTexture(1440,900,24);rt.antiAliasing=4;c.targetTexture=rt;c.Render();var old=RenderTexture.active;RenderTexture.active=rt;var tex=new Texture2D(1440,900,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1440,900),0,0);tex.Apply();File.WriteAllBytes(Path.Combine(Application.dataPath,"../Evidence/hands_height_corrected.png"),tex.EncodeToPNG());c.targetTexture=null;RenderTexture.active=old;Destroy(tex);Destroy(rt);
  string positions="";foreach(var r in c.GetComponentsInChildren<Renderer>())if(r.name.StartsWith("ViewHand"))positions+=r.name+" "+c.WorldToViewportPoint(r.bounds.center)+"\n";File.WriteAllText(Path.Combine(Application.dataPath,"../Evidence/hands_height_positions.txt"),positions);
  p.motion.SetHands(false,false);p.automated=false;Destroy(this);
 }
}
