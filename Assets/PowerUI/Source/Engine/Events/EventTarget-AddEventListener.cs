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
using PowerUI;


namespace Dom{
	
	/// <summary>
	/// An event target can receive events and have event handlers.
	/// <summary>
	
	public partial class EventTarget{
		
		// All event-specific addEventListener overloads (except for SVG).
		// This avoids needing to manually create e.g. a KeyboardEventListener object.
		
		public void addEventListener(string name,AnimationEventDelegate method){
			addEventListener(name,new AnimationEventListener(method));
		}

		public void addEventListener(string name,AudioProcessingEventDelegate method){
			addEventListener(name,new AudioProcessingEventListener(method));
		}

		public void addEventListener(string name,BeforeInputEventDelegate method){
			addEventListener(name,new BeforeInputEventListener(method));
		}

		public void addEventListener(string name,BeforeUnloadEventDelegate method){
			addEventListener(name,new BeforeUnloadEventListener(method));
		}

		public void addEventListener(string name,BlobEventDelegate method){
			addEventListener(name,new BlobEventListener(method));
		}

		public void addEventListener(string name,ClipboardEventDelegate method){
			addEventListener(name,new ClipboardEventListener(method));
		}

		public void addEventListener(string name,CloseEventDelegate method){
			addEventListener(name,new CloseEventListener(method));
		}

		public void addEventListener(string name,CompositionEventDelegate method){
			addEventListener(name,new CompositionEventListener(method));
		}

		public void addEventListener(string name,CustomEventDelegate method){
			addEventListener(name,new CustomEventListener(method));
		}

		public void addEventListener(string name,CSSFontFaceLoadEventDelegate method){
			addEventListener(name,new CSSFontFaceLoadEventListener(method));
		}

		public void addEventListener(string name,DeviceLightEventDelegate method){
			addEventListener(name,new DeviceLightEventListener(method));
		}

		public void addEventListener(string name,DeviceMotionEventDelegate method){
			addEventListener(name,new DeviceMotionEventListener(method));
		}

		public void addEventListener(string name,DeviceOrientationEventDelegate method){
			addEventListener(name,new DeviceOrientationEventListener(method));
		}

		public void addEventListener(string name,DeviceProximityEventDelegate method){
			addEventListener(name,new DeviceProximityEventListener(method));
		}

		public void addEventListener(string name,DomEventDelegate method){
			addEventListener(name,new DomEventListener(method));
		}

		public void addEventListener(string name,DOMTransactionEventDelegate method){
			addEventListener(name,new DOMTransactionEventListener(method));
		}

		public void addEventListener(string name,DragEventDelegate method){
			addEventListener(name,new DragEventListener(method));
		}

		public void addEventListener(string name,EditingBeforeInputEventDelegate method){
			addEventListener(name,new EditingBeforeInputEventListener(method));
		}

		public void addEventListener(string name,ErrorEventDelegate method){
			addEventListener(name,new ErrorEventListener(method));
		}

		public void addEventListener(string name,FetchEventDelegate method){
			addEventListener(name,new FetchEventListener(method));
		}

		public void addEventListener(string name,FocusEventDelegate method){
			addEventListener(name,new FocusEventListener(method));
		}

		public void addEventListener(string name,GamepadEventDelegate method){
			addEventListener(name,new GamepadEventListener(method));
		}

		public void addEventListener(string name,HashChangeEventDelegate method){
			addEventListener(name,new HashChangeEventListener(method));
		}

		public void addEventListener(string name,IDBVersionChangeEventDelegate method){
			addEventListener(name,new IDBVersionChangeEventListener(method));
		}

		public void addEventListener(string name,InputEventDelegate method){
			addEventListener(name,new InputEventListener(method));
		}

		public void addEventListener(string name,KeyboardEventDelegate method){
			addEventListener(name,new KeyboardEventListener(method));
		}

		public void addEventListener(string name,MediaStreamEventDelegate method){
			addEventListener(name,new MediaStreamEventListener(method));
		}

		public void addEventListener(string name,MessageEventDelegate method){
			addEventListener(name,new MessageEventListener(method));
		}

		public void addEventListener(string name,MouseEventDelegate method){
			addEventListener(name,new MouseEventListener(method));
		}

		public void addEventListener(string name,MutationEventDelegate method){
			addEventListener(name,new MutationEventListener(method));
		}

		public void addEventListener(string name,OfflineAudioCompletionEventDelegate method){
			addEventListener(name,new OfflineAudioCompletionEventListener(method));
		}

		public void addEventListener(string name,PageTransitionEventDelegate method){
			addEventListener(name,new PageTransitionEventListener(method));
		}

		public void addEventListener(string name,PointerEventDelegate method){
			addEventListener(name,new PointerEventListener(method));
		}

		public void addEventListener(string name,PopStateEventDelegate method){
			addEventListener(name,new PopStateEventListener(method));
		}

		public void addEventListener(string name,ProgressEventDelegate method){
			addEventListener(name,new ProgressEventListener(method));
		}

		public void addEventListener(string name,RelatedEventDelegate method){
			addEventListener(name,new RelatedEventListener(method));
		}

		public void addEventListener(string name,RTCDataChannelEventDelegate method){
			addEventListener(name,new RTCDataChannelEventListener(method));
		}

		public void addEventListener(string name,RTCIdentityErrorEventDelegate method){
			addEventListener(name,new RTCIdentityErrorEventListener(method));
		}

		public void addEventListener(string name,RTCIdentityEventDelegate method){
			addEventListener(name,new RTCIdentityEventListener(method));
		}

		public void addEventListener(string name,RTCPeerConnectionIceEventDelegate method){
			addEventListener(name,new RTCPeerConnectionIceEventListener(method));
		}

		public void addEventListener(string name,SensorEventDelegate method){
			addEventListener(name,new SensorEventListener(method));
		}

		public void addEventListener(string name,StorageEventDelegate method){
			addEventListener(name,new StorageEventListener(method));
		}

		public void addEventListener(string name,TextEventDelegate method){
			addEventListener(name,new TextEventListener(method));
		}

		public void addEventListener(string name,TimeEventDelegate method){
			addEventListener(name,new TimeEventListener(method));
		}

		public void addEventListener(string name,TouchEventDelegate method){
			addEventListener(name,new TouchEventListener(method));
		}

		public void addEventListener(string name,TrackEventDelegate method){
			addEventListener(name,new TrackEventListener(method));
		}

		public void addEventListener(string name,TransitionEventDelegate method){
			addEventListener(name,new TransitionEventListener(method));
		}

		public void addEventListener(string name,UIEventDelegate method){
			addEventListener(name,new UIEventListener(method));
		}

		public void addEventListener(string name,UserProximityEventDelegate method){
			addEventListener(name,new UserProximityEventListener(method));
		}

		public void addEventListener(string name,WebGLContextEventDelegate method){
			addEventListener(name,new WebGLContextEventListener(method));
		}

		public void addEventListener(string name,WheelEventDelegate method){
			addEventListener(name,new WheelEventListener(method));
		}

	}
	
}