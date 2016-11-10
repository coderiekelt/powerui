//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;
using Dom;


namespace PowerUI{
	
	public class AnimationEvent : DomEvent{
		
		public string animationName;
		public float elapsedTime;
		public string psuedoElement;
		
		
		public AnimationEvent(string type):base(type){}
		public AnimationEvent(string type,object init):base(type,init){}
		
		public override void Setup(object init){
			
			if(init==null){
				return;
			}
			
			// string init.animationName 
			// float init.elapsedTime
			// string init.psuedoElement
			
		}
		
	}
	
	public class AudioProcessingEvent : DomEvent{
		
		public AudioBuffer input;
		public AudioBuffer output;
		public float playbackTime;
		
		
		public AudioProcessingEvent(){}
		public AudioProcessingEvent(string type,object init):base(type,init){}
		
		public override void Setup(object init){
			
			if(init==null){
				return;
			}
			
			// AudioBuffer init.input 
			// AudioBuffer init.output
			// float init.playbackTime
			
		}
		
	}
	
	public class BeforeInputEvent : DomEvent{
		
		public BeforeInputEvent(){}
		public BeforeInputEvent(string type,object init):base(type,init){}
		
	}
	
	public class BeforeUnloadEvent : DomEvent{
		
		public string returnValue;
		
		public BeforeUnloadEvent():base("beforeunload"){
			SetTrusted();
			cancelable=false;
		}
		
	}
	
	public class BlobEvent : DomEvent{
		
		public byte[] data;
		
		public BlobEvent(){}
		public BlobEvent(object init):base(null,init){}
		
		public BlobEvent(string type,object init):base(type,init){}
		
		public override void Setup(object init){
			
			if(init==null){
				return;
			}
			
			// byte[] init.data
			
		}
		
	}
	
	public class ClipboardEvent : DomEvent{
		
		private DataTransfer _ClipboardData;
		
		
		public DataTransfer clipboardData{
			get{
				return _ClipboardData;
			}
		}
		
		public ClipboardEvent(){}
		
		public ClipboardEvent(string type,object init):base(type,init){}
		
		public override void Setup(object init){
			
			if(init==null){
				return;
			}
			
			// string init.data
			// string init.dataType
			_ClipboardData=new DataTransfer();
			
		}
		
	}
	
	public class CloseEvent : DomEvent{
		
		public CloseEvent(){}
		public CloseEvent(string type,object init):base(type,init){}
		
	}
	
	public class CompositionEvent : UIEvent{
		
		public CompositionEvent(){}
		public CompositionEvent(string type,object init):base(type,init){}
		
	}
	
	public class CustomEvent : DomEvent{
		
		public object detail;
		
		public CustomEvent(){}
		public CustomEvent(string type,object init):base(type,init){}
		
	}
	
	public class CSSFontFaceLoadEvent : DomEvent{
		
		public CSSFontFaceLoadEvent(){}
		public CSSFontFaceLoadEvent(string type,object init):base(type,init){}
		
	}
	
	public class DeviceLightEvent : DomEvent{
		
		public DeviceLightEvent(){}
		public DeviceLightEvent(string type,object init):base(type,init){}
		
	}
	
	public class DeviceMotionEvent : DomEvent{
		
		public DeviceMotionEvent(){}
		public DeviceMotionEvent(string type,object init):base(type,init){}
		
	}
	
	public class DeviceOrientationEvent : DomEvent{
		
		public bool absolute;
		public float alpha;
		public float beta;
		public float gamma;
		
		public DeviceOrientationEvent(string type):base(type){}
		public DeviceOrientationEvent(string type,object init):base(type,init){}
		
	}
	
	public class DeviceProximityEvent : DomEvent{
		
		public DeviceProximityEvent(){}
		public DeviceProximityEvent(string type,object init):base(type,init){}
		
	}
	
	public class DOMTransactionEvent : DomEvent{
		
		public DOMTransactionEvent(){}
		public DOMTransactionEvent(string type,object init):base(type,init){}
		
	}
	
	public class DragEvent : UIEvent{
		
		public DragEvent(string type):base(type){}
		public DragEvent(string type,object init):base(type,init){}
		
	}
	
	public class EditingBeforeInputEvent : DomEvent{
		
		public EditingBeforeInputEvent(){}
		public EditingBeforeInputEvent(string type,object init):base(type,init){}
		
	}
	
	public class ErrorEvent : DomEvent{
		
		public ErrorEvent(){}
		public ErrorEvent(string type,object init):base(type,init){}
		
	}
	
	public class FetchEvent : DomEvent{
		
		public FetchEvent(){}
		public FetchEvent(string type,object init):base(type,init){}
		
	}
	
	public class FocusEvent : UIEvent{
		
		/// <summary>The element being focused. May be null.</summary>
		public Element focusing;
		
		public FocusEvent(string type):base(type){}
		public FocusEvent(string type,object init):base(type,init){}
		
	}
	
	public class GamepadEvent : DomEvent{
		
		public GamepadEvent(){}
		public GamepadEvent(string type,object init):base(type,init){}
		
	}
	
	public class IDBVersionChangeEvent : DomEvent{
		
		public IDBVersionChangeEvent():base("versionchange"){}
		public IDBVersionChangeEvent(string type,object init):base(type,init){}
		
	}
	
	public class InputEvent : UIEvent{
		
		public InputEvent():base("input"){}
		public InputEvent(string type,object init):base(type,init){}
		
	}
	
	public class MediaStreamEvent : DomEvent{
		
		public MediaStreamEvent():base("mediastream"){}
		public MediaStreamEvent(string type,object init):base(type,init){}
		
	}
	
	public class MessageEvent : DomEvent{
		
		public object data;
		public string origin;
		
		
		public MessageEvent():base("message"){}
		
		public MessageEvent(object message,string origin):base("message"){
			data=message;
			this.origin=origin;
		}
		
		public MessageEvent(string type,object init):base(type,init){}
		
	}
	
	public class MouseEvent : UIEvent{
		
		/// <summary>True if rayHit was successful and hit something.</summary>
		public bool raySuccess{
			get{
				return PowerUI.Input.LatestRayHitSuccess;
			}
		}
		
		/// <summary>The raycast hit that was sent into the scene whilst looking for WorldUI's.
		/// It's default(RaycastHit) if it didn't hit anything (see raySuccess).</summary>
		public RaycastHit rayHit{
			get{
				return PowerUI.Input.LatestRayHit;
			}
		}
		
		public MouseEvent(){}
		
		public MouseEvent(float x,float y,int button,bool down):base(x,y,down){
			keyCode=button;
		}
		
		public MouseEvent(string type,object init):base(type,init){}
		
		public override int which{
			get{
				return button;
			}
		}
		
	}
	
	public class MutationEvent : UIEvent{
		
		public MutationEvent(){}
		
		public MutationEvent(string type,object init):base(type,init){}
		
	}
	
	public class OfflineAudioCompletionEvent : DomEvent{
		
		public OfflineAudioCompletionEvent(){}
		
		public OfflineAudioCompletionEvent(string type,object init):base(type,init){}
		
	}
	
	public class PageTransitionEvent : DomEvent{
		
		public PageTransitionEvent(){}
		
		public PageTransitionEvent(string type,object init):base(type,init){}
		
	}
	
	public class PointerEvent : DomEvent{
		
		public PointerEvent(){}
		
		public PointerEvent(string type,object init):base(type,init){}
		
	}
	
	public class PopStateEvent : DomEvent{
		
		public object state;
		
		
		public PopStateEvent():base("popstate"){}
		
		public PopStateEvent(string type,object init):base(type,init){}
		
	}
	
	public class ProgressEvent : DomEvent{
		
		public ProgressEvent(){}
		
		public ProgressEvent(string type,object init):base(type,init){}
		
	}
	
	public class RelatedEvent : DomEvent{
		
		public RelatedEvent(){}
		
		public RelatedEvent(string type,object init):base(type,init){}
		
	}
	
	public class RTCDataChannelEvent : DomEvent{
		
		public RTCDataChannelEvent(){}
		
		public RTCDataChannelEvent(string type,object init):base(type,init){}
		
	}
	
	public class RTCIdentityErrorEvent : DomEvent{
		
		public RTCIdentityErrorEvent(){}
		
		public RTCIdentityErrorEvent(string type,object init):base(type,init){}
		
	}
	
	public class RTCIdentityEvent : DomEvent{
		
		public RTCIdentityEvent(){}
		
		public RTCIdentityEvent(string type,object init):base(type,init){}
		
	}
	
	public class RTCPeerConnectionIceEvent : DomEvent{
		
		public RTCPeerConnectionIceEvent(){}
		
		public RTCPeerConnectionIceEvent(string type,object init):base(type,init){}
		
	}
	
	public class SensorEvent : DomEvent{
		
		public SensorEvent(){}
		
		public SensorEvent(string type,object init):base(type,init){}
		
	}
	
	public class StorageEvent : DomEvent{
		
		public StorageEvent(){}
		
		public StorageEvent(string type,object init):base(type,init){}
		
	}
	
	public class TextEvent : DomEvent{
		
		public TextEvent(){}
		
		public TextEvent(string type,object init):base(type,init){}
		
	}
	
	public class TimeEvent : DomEvent{
		
		public TimeEvent(){}
		
		public TimeEvent(string type,object init):base(type,init){}
		
	}
	
	public class TouchEvent : UIEvent{
		
		/// <summary>Cached touches that pressed down on the element that is the target of this event.</summary>
		private TouchList targetTouches_;
		/// <summary>Cached touches regardless of which element they're over.</summary>
		private TouchList touches_;
		
		/// <summary>All touches that pressed down on the element that is the target of this event.</summary>
		public TouchList targetTouches{
			get{
				
				if(targetTouches_==null){
					
					// Create the list:
					targetTouches_=new TouchList();
					
					// For each available input, add it to the list if it's a finger touch:
					for(int i=0;i<InputPointer.PointerCount;i++){
						
						InputPointer pointer=InputPointer.AllRaw[i];
						
						if(pointer.ActivePressed!=target){
							continue;
						}
						
						if(pointer is TouchPointer){
							
							// Add:
							targetTouches_.push(pointer as TouchPointer);
							
						}
						
					}
					
				}
				
				return targetTouches_;
				
			}
		}
		
		/// <summary>All touches regardless of which element they're over.</summary>
		public TouchList touches{
			get{
				
				if(touches_==null){
					
					// Create the list:
					touches_=new TouchList();
					
					// For each available input, add it to the list if it's a finger touch:
					for(int i=0;i<InputPointer.PointerCount;i++){
						
						InputPointer pointer=InputPointer.AllRaw[i];
						
						if(pointer is TouchPointer){
							
							// Add:
							touches_.push(pointer as TouchPointer);
							
						}
						
					}
					
				}
				
				return touches_;
				
			}
		}
		
		public TouchEvent(string type):base(type){}
		
		public TouchEvent(string type,object init):base(type,init){}
		
	}
	
	public class TrackEvent : DomEvent{
		
		public TrackEvent(){}
		
		public TrackEvent(string type,object init):base(type,init){}
		
	}
	
	public class TransitionEvent : DomEvent{
		
		public TransitionEvent(){}
		
		public TransitionEvent(string type,object init):base(type,init){}
		
	}
	
	public class UserProximityEvent : DomEvent{
		
		public UserProximityEvent(){}
		
		public UserProximityEvent(string type,object init):base(type,init){}
		
	}
	
	public class WebGLContextEvent : DomEvent{
		
		public WebGLContextEvent(){}
		
		public WebGLContextEvent(string type,object init):base(type,init){}
		
	}
	
	public class WheelEvent : UIEvent{
		
		public const ushort DOM_DELTA_PIXEL=0;
		public const ushort DOM_DELTA_LINE=1;
		public const ushort DOM_DELTA_PAGE=2;
		
		public float deltaX;
		public float deltaY;
		public float deltaZ;
		/// <summary>Always pixel here.</summary>
		public int deltaMode=DOM_DELTA_PIXEL;
		
		
		public WheelEvent():base("wheel"){}
		
		public WheelEvent(string type,object init):base(type,init){}
		
	}
	
}