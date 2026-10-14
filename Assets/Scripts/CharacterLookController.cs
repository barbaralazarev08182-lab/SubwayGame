using UnityEngine;

// Shared look articulation; no skeleton or Animator is required.
public class CharacterLookController : MonoBehaviour {
 public float yawLimit=75f,pitchLimit=55f;
 public bool BodyTurning {get;private set;}
 CharacterMotionController motion;Transform headPivot;float yaw,pitch;int turningFrame=-1;
 void Awake(){motion=GetComponent<CharacterMotionController>();}
 public void AddMouseLook(float horizontal,float vertical){
  if(!motion)motion=GetComponent<CharacterMotionController>();
  float requested=yaw+horizontal;
  if(!motion.Seated&&motion.State!=CharacterMotionState.StandUp){
   float overflow=requested-Mathf.Clamp(requested,-yawLimit,yawLimit);
   if(Mathf.Abs(overflow)>.001f){transform.Rotate(0,overflow,0,Space.Self);turningFrame=Time.frameCount;}
  }
  yaw=Mathf.Clamp(requested,-yawLimit,yawLimit);pitch=Mathf.Clamp(pitch+vertical,-pitchLimit,pitchLimit);
  BodyTurning=turningFrame==Time.frameCount;
  if(motion.aim)motion.aim.localRotation=Quaternion.Euler(pitch,yaw,0);
 }
 public void FollowViewWhileMoving(float maxDegrees){
  if(!motion||motion.Seated||motion.State==CharacterMotionState.StandUp)return;
  float delta=Mathf.Clamp(yaw,-maxDegrees,maxDegrees);
  transform.Rotate(0,delta,0,Space.Self);yaw-=delta;
  if(motion.aim)motion.aim.localRotation=Quaternion.Euler(pitch,yaw,0);
 }
 void LateUpdate(){
  BodyTurning=turningFrame==Time.frameCount;
  if(!motion||!motion.body)return;
  if(!headPivot){
   Transform head=null;foreach(var t in motion.body.GetComponentsInChildren<Transform>())if(t.name=="Head"){head=t;break;}
   if(!head)return;
   var mesh=head.GetComponent<Renderer>();Vector3 neck=mesh?new Vector3(mesh.bounds.center.x,mesh.bounds.min.y,mesh.bounds.center.z):head.position;
   var node=new GameObject("HeadLookPivot");headPivot=node.transform;headPivot.SetPositionAndRotation(neck,transform.rotation);headPivot.SetParent(motion.body,true);
   var parts=motion.body.GetComponentsInChildren<Transform>();
   foreach(var t in parts){string n=t.name;if(n=="Head"||n=="Hat"||n=="Visor"||n.StartsWith("Eye_")||n.StartsWith("Cheek_"))t.SetParent(headPivot,true);}
  }
  headPivot.rotation=transform.rotation*Quaternion.Euler(pitch,yaw,0);
 }
}

