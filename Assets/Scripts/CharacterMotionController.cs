using UnityEngine;
public enum CharacterMotionState { Idle, Walk, Turn, Sit, StandUp, Crouch }
[RequireComponent(typeof(CharacterController))]
public class CharacterMotionController : MonoBehaviour {
 public Transform visual,body,upperLeft,upperRight,lowerLeft,lowerRight,footLeft,footRight,leftHand,rightHand,aim;
 public bool firstPersonHands; public float walkSpeed=1.65f;
 public CharacterMotionState State{get;private set;} public bool Seated{get;private set;} public bool LeftRaised{get;private set;} public bool RightRaised{get;private set;} public float PoseAmount=>pose;
 CharacterController capsule;Vector2 move;float turn,phase,pose,transition;bool crouch,rising;Vector3 bodyRest,startPosition,seatPosition,standPosition;Quaternion startRotation,seatRotation;float bodySeatLift=.14f;
 class Leg {public Transform hip,knee,foot;public Vector3 h,k,a;public Quaternion hr,kr,fr;}
 class HandRest {public Transform t,proxy;public Vector3 position,scale;public Quaternion rotation;}
 Leg l,r;HandRest lh,rh;CharacterLookController look;
 Leg CacheLeg(Transform h,Transform k,Transform a){return new Leg{hip=h,knee=k,foot=a,h=transform.InverseTransformPoint(h.position),k=transform.InverseTransformPoint(k.position),a=transform.InverseTransformPoint(a.position),hr=Quaternion.Inverse(transform.rotation)*h.rotation,kr=Quaternion.Inverse(transform.rotation)*k.rotation,fr=Quaternion.Inverse(transform.rotation)*a.rotation};}
 HandRest CacheHand(Transform h){return new HandRest{t=h,position=transform.InverseTransformPoint(h.position),rotation=Quaternion.Inverse(transform.rotation)*h.rotation,scale=h.localScale};}
 void Awake(){look=GetComponent<CharacterLookController>();if(!look)look=gameObject.AddComponent<CharacterLookController>();capsule=GetComponent<CharacterController>();bodyRest=transform.InverseTransformPoint(body.position);l=CacheLeg(upperLeft,lowerLeft,footLeft);r=CacheLeg(upperRight,lowerRight,footRight);
  if(transform.InverseTransformPoint(leftHand.position).x>transform.InverseTransformPoint(rightHand.position).x){var swap=leftHand;leftHand=rightHand;rightHand=swap;}
  lh=CacheHand(leftHand);rh=CacheHand(rightHand); if(firstPersonHands&&aim){ CreateViewHand(lh,"ViewHand_Left"); CreateViewHand(rh,"ViewHand_Right"); }
 }
 void CreateViewHand(HandRest h,string name){var copy=Instantiate(h.t.gameObject,aim);copy.name=name;copy.layer=9;h.proxy=copy.transform;h.proxy.localScale=h.scale*.70f;h.t.gameObject.layer=8;}
 public void SetLocomotion(Vector2 value,float yawRate,bool duck){move=Vector2.ClampMagnitude(value,1);turn=yawRate;crouch=duck;}
 public void SetHands(bool left,bool right){LeftRaised=left;RightRaised=right;}
 public static Vector3 ClosestSeatPoint(Transform seat,Vector3 from){float along=Mathf.Clamp(Vector3.Dot(from-seat.position,seat.right),-.80f,.80f);return seat.position+seat.right*along;}
 public bool SitAt(Transform seat){if(Seated||rising)return false;startPosition=transform.position;startRotation=transform.rotation;seatPosition=ClosestSeatPoint(seat,transform.position);seatRotation=seat.rotation;standPosition=seatPosition+seat.forward*.85f;standPosition.y=.025f;capsule.enabled=false;Seated=true;transition=0;move=Vector2.zero;crouch=false;return true;}
 public string StandUpHint {get;private set;}
 public bool StandUp(){
  StandUpHint="";
  if(!Seated||transition<1)return false;
  Vector3 forward=seatRotation*Vector3.forward,right=seatRotation*Vector3.right;
  foreach(float side in new[]{0f,-.65f,.65f,-1.0f,1.0f}){
   foreach(float distance in new[]{.85f,1.05f}){
    Vector3 candidate=seatPosition+forward*distance+right*side;candidate.y=.025f;
    if(!ClearStandingSpace(candidate)||!ClearExitPath(candidate))continue;
    standPosition=candidate;Seated=false;rising=true;transition=0;startPosition=transform.position;return true;
   }
  }
  StandUpHint="Exit blocked. Clear space beside the seat, then press E again.";
  return false;
 }
 bool OwnCollider(Collider c){return c.transform==transform||c.transform.IsChildOf(transform);}
 bool ClearStandingSpace(Vector3 p){
  foreach(var c in Physics.OverlapCapsule(p+Vector3.up*.26f,p+Vector3.up*1.36f,.23f,~0,QueryTriggerInteraction.Ignore))if(!OwnCollider(c))return false;
  return true;
 }
 bool ClearExitPath(Vector3 target){
  Vector3 a=transform.position+Vector3.up*.26f,b=transform.position+Vector3.up*1.36f,delta=target-transform.position;
  var starting=Physics.OverlapCapsule(a,b,.23f,~0,QueryTriggerInteraction.Ignore);
  foreach(var hit in Physics.CapsuleCastAll(a,b,.23f,delta.normalized,delta.magnitude,~0,QueryTriggerInteraction.Ignore)){
   var c=hit.collider;if(OwnCollider(c))continue;
   // The seated capsule begins inside its bench; leaving that bench is expected.
   bool benchAtStart=false;if(!(c is CharacterController))foreach(var initial in starting)if(initial==c&&c.name.StartsWith("Seat_")){benchAtStart=true;break;}
   if(!benchAtStart)return false;
  }
  return true;
 }
 void Update(){float dt=Time.deltaTime;
  if(Seated||rising){transition=Mathf.Min(1,transition+dt/.7f);float t=Mathf.SmoothStep(0,1,transition);State=Seated?CharacterMotionState.Sit:CharacterMotionState.StandUp;pose=Seated?t:1-t;transform.position=Vector3.Lerp(startPosition,Seated?seatPosition:standPosition,t);if(Seated)transform.rotation=Quaternion.Slerp(startRotation,seatRotation,t);if(rising&&transition>=1){rising=false;capsule.enabled=true;}}
  else {pose=0;transform.Rotate(0,turn*dt,0);var velocity=(transform.right*move.x+transform.forward*move.y)*walkSpeed*(crouch?.5f:1);capsule.Move((velocity+Vector3.down*2.5f)*dt);State=crouch?CharacterMotionState.Crouch:move.sqrMagnitude>.01f?CharacterMotionState.Walk:(Mathf.Abs(turn)>.5f||(look&&look.BodyTurning))?CharacterMotionState.Turn:CharacterMotionState.Idle;}
  capsule.height=crouch&&!Seated?1.45f:1.6f;capsule.center=Vector3.up*capsule.height*.5f;float duck=crouch&&!Seated?1:0;
  phase+=dt*(State==CharacterMotionState.Walk?8:2);float stride=State==CharacterMotionState.Walk?Mathf.Sin(phase)*.075f:State==CharacterMotionState.Turn?Mathf.Sin(phase*2)*.025f:0;
  Vector3 shift=Vector3.up*(pose*bodySeatLift-duck*.10f);body.position=transform.TransformPoint(bodyRest+shift);
  PoseLeg(l,shift,stride,duck);PoseLeg(r,shift,-stride,duck);
  PoseHand(lh,LeftRaised,-1,shift,dt);PoseHand(rh,RightRaised,1,shift,dt);
 }
 void PoseLeg(Leg leg,Vector3 shift,float stride,float duck){
  float thigh=Vector3.Distance(leg.h,leg.k),shin=Vector3.Distance(leg.k,leg.a);
  Vector3 hip=leg.h+shift+Vector3.forward*(pose*.12f);
  Vector3 standingKnee=leg.k+Vector3.forward*stride+Vector3.up*(-duck*.025f);
  Vector3 sittingKnee=hip+new Vector3(0,-.015f,Mathf.Sqrt(Mathf.Max(.001f,thigh*thigh-.000225f)));
  Vector3 knee=Vector3.Lerp(standingKnee,sittingKnee,pose);
  Vector3 ankle=Vector3.Lerp(leg.a+Vector3.forward*stride+Vector3.up*Mathf.Max(0,stride*.25f),knee+Vector3.down*shin,pose);
  Vector3 wh=transform.TransformPoint(hip),wk=transform.TransformPoint(knee),wa=transform.TransformPoint(ankle);
  leg.hip.position=wh;leg.hip.rotation=Quaternion.FromToRotation(transform.TransformDirection(leg.k-leg.h),wk-wh)*transform.rotation*leg.hr;
  leg.knee.position=wk;leg.knee.rotation=Quaternion.FromToRotation(transform.TransformDirection(leg.a-leg.k),wa-wk)*transform.rotation*leg.kr;
  leg.foot.position=wa;leg.foot.rotation=transform.rotation*leg.fr; if(pose>.01f){var shinMesh=leg.knee.GetComponent<Renderer>();var shoeMesh=leg.foot.GetComponent<Renderer>();if(shinMesh&&shoeMesh){float gap=shinMesh.bounds.min.y+.012f-shoeMesh.bounds.max.y;if(gap>0)leg.foot.position+=Vector3.up*gap;}}
 }
 void PoseHand(HandRest hand,bool raised,int side,Vector3 shift,float dt){Vector3 target=transform.TransformPoint(hand.position+shift);Quaternion rotation=transform.rotation*hand.rotation;
  if(hand.proxy&&aim){
   var cam=aim.GetComponent<Camera>();
   Vector3 desired=aim.TransformPoint(raised?new Vector3(side*.205f,.04f,.74f):new Vector3(side*.35f,-.48f,.70f));
   if(cam)desired=cam.ViewportToWorldPoint(new Vector3(side<0?.36f:.64f,raised?.46f:-.02f,.74f));
   hand.proxy.localRotation=Quaternion.Slerp(hand.proxy.localRotation,Quaternion.Euler(raised?-60:0,0,raised?side*-6:0)*hand.rotation,1-Mathf.Exp(-10*dt));
   // Place the visible mesh center, not the imported wrist pivot, at the screen target.
   var renderer=hand.proxy.GetComponent<Renderer>();Vector3 offset=renderer?renderer.bounds.center-hand.proxy.position:Vector3.zero;
   hand.proxy.position=Vector3.Lerp(hand.proxy.position,desired-offset,1-Mathf.Exp(-10*dt));
  }
  if(raised&&aim){target=aim.TransformPoint(new Vector3(side*.23f,-.24f,.56f));rotation=transform.rotation*hand.rotation*Quaternion.Euler(-25,0,0);}
  hand.t.position=Vector3.Lerp(hand.t.position,target,1-Mathf.Exp(-10*dt));hand.t.rotation=Quaternion.Slerp(hand.t.rotation,rotation,1-Mathf.Exp(-10*dt));
 }
}







