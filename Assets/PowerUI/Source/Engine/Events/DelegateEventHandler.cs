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
using System.Collections;
using System.Collections.Generic;
using Dom;

namespace PowerUI{
	
	public delegate void AnimationEventDelegate(AnimationEvent e);
	public delegate void AudioProcessingEventDelegate(AudioProcessingEvent e);
	public delegate void BeforeInputEventDelegate(BeforeInputEvent e);
	public delegate void BeforeUnloadEventDelegate(BeforeUnloadEvent e);
	public delegate void BlobEventDelegate(BlobEvent e);
	public delegate void ClipboardEventDelegate(ClipboardEvent e);
	public delegate void CloseEventDelegate(CloseEvent e);
	public delegate void CompositionEventDelegate(CompositionEvent e);
	public delegate void CustomEventDelegate(CustomEvent e);
	public delegate void CSSFontFaceLoadEventDelegate(CSSFontFaceLoadEvent e);
	public delegate void DeviceLightEventDelegate(DeviceLightEvent e);
	public delegate void DeviceMotionEventDelegate(DeviceMotionEvent e);
	public delegate void DeviceOrientationEventDelegate(DeviceOrientationEvent e);
	public delegate void DeviceProximityEventDelegate(DeviceProximityEvent e);
	public delegate void DOMTransactionEventDelegate(DOMTransactionEvent e);
	public delegate void DragEventDelegate(DragEvent e);
	public delegate void EditingBeforeInputEventDelegate(EditingBeforeInputEvent e);
	public delegate void ErrorEventDelegate(ErrorEvent e);
	public delegate void FetchEventDelegate(FetchEvent e);
	public delegate void FocusEventDelegate(FocusEvent e);
	public delegate void GamepadEventDelegate(GamepadEvent e);
	public delegate void IDBVersionChangeEventDelegate(IDBVersionChangeEvent e);
	public delegate void InputEventDelegate(InputEvent e);
	public delegate void KeyboardEventDelegate(KeyboardEvent e);
	public delegate void MediaStreamEventDelegate(MediaStreamEvent e);
	public delegate void MessageEventDelegate(MessageEvent e);
	public delegate void MouseEventDelegate(MouseEvent e);
	public delegate void MutationEventDelegate(MutationEvent e);
	public delegate void OfflineAudioCompletionEventDelegate(OfflineAudioCompletionEvent e);
	public delegate void PageTransitionEventDelegate(PageTransitionEvent e);
	public delegate void PointerEventDelegate(PointerEvent e);
	public delegate void PopStateEventDelegate(PopStateEvent e);
	public delegate void ProgressEventDelegate(ProgressEvent e);
	public delegate void RelatedEventDelegate(RelatedEvent e);
	public delegate void RTCDataChannelEventDelegate(RTCDataChannelEvent e);
	public delegate void RTCIdentityErrorEventDelegate(RTCIdentityErrorEvent e);
	public delegate void RTCIdentityEventDelegate(RTCIdentityEvent e);
	public delegate void RTCPeerConnectionIceEventDelegate(RTCPeerConnectionIceEvent e);
	public delegate void SensorEventDelegate(SensorEvent e);
	public delegate void StorageEventDelegate(StorageEvent e);
	public delegate void TextEventDelegate(TextEvent e);
	public delegate void TimeEventDelegate(TimeEvent e);
	public delegate void TouchEventDelegate(TouchEvent e);
	public delegate void TrackEventDelegate(TrackEvent e);
	public delegate void TransitionEventDelegate(TransitionEvent e);
	public delegate void UIEventDelegate(UIEvent e);
	public delegate void UserProximityEventDelegate(UserProximityEvent e);
	public delegate void WebGLContextEventDelegate(WebGLContextEvent e);
	public delegate void WheelEventDelegate(WheelEvent e);
	
	/// Handler for UIEvent events.
	public class UIEventListener : EventListener{
		
		public UIEventDelegate Listener;
		
		public UIEventListener(UIEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((UIEvent)e);
		}
		
	}
	
	/// Handler for KeyboardEvent events.
	public class KeyboardEventListener : EventListener{
		
		public KeyboardEventDelegate Listener;
		
		public KeyboardEventListener(KeyboardEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((KeyboardEvent)e);
		}
		
	}
	
	/// Handler for AnimationEvent events
	public class AnimationEventListener : EventListener{
		
		public AnimationEventDelegate Listener;
		
		public AnimationEventListener(AnimationEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((AnimationEvent)e);
		}
		
	}
	
	/// Handler for AudioProcessingEvent events
	public class AudioProcessingEventListener : EventListener{
		
		public AudioProcessingEventDelegate Listener;
		
		public AudioProcessingEventListener(AudioProcessingEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((AudioProcessingEvent)e);
		}
		
	}
	
	/// Handler for BeforeInputEvent events
	public class BeforeInputEventListener : EventListener{
		
		public BeforeInputEventDelegate Listener;
		
		public BeforeInputEventListener(BeforeInputEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((BeforeInputEvent)e);
		}
		
	}
	
	/// Handler for BeforeUnloadEvent events
	public class BeforeUnloadEventListener : EventListener{
		
		public BeforeUnloadEventDelegate Listener;
		
		public BeforeUnloadEventListener(BeforeUnloadEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((BeforeUnloadEvent)e);
		}
		
	}
	
	/// Handler for BlobEvent events
	public class BlobEventListener : EventListener{
		
		public BlobEventDelegate Listener;
		
		public BlobEventListener(BlobEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((BlobEvent)e);
		}
		
	}
	
	/// Handler for ClipboardEvent events
	public class ClipboardEventListener : EventListener{
		
		public ClipboardEventDelegate Listener;
		
		public ClipboardEventListener(ClipboardEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((ClipboardEvent)e);
		}
		
	}
	
	/// Handler for CloseEvent events
	public class CloseEventListener : EventListener{
		
		public CloseEventDelegate Listener;
		
		public CloseEventListener(CloseEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((CloseEvent)e);
		}
		
	}
	
	/// Handler for CompositionEvent events
	public class CompositionEventListener : EventListener{
		
		public CompositionEventDelegate Listener;
		
		public CompositionEventListener(CompositionEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((CompositionEvent)e);
		}
		
	}
	
	/// Handler for CustomEvent events
	public class CustomEventListener : EventListener{
		
		public CustomEventDelegate Listener;
		
		public CustomEventListener(CustomEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((CustomEvent)e);
		}
		
	}
	
	/// Handler for CSSFontFaceLoadEvent events
	public class CSSFontFaceLoadEventListener : EventListener{
		
		public CSSFontFaceLoadEventDelegate Listener;
		
		public CSSFontFaceLoadEventListener(CSSFontFaceLoadEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((CSSFontFaceLoadEvent)e);
		}
		
	}
	
	/// Handler for DeviceLightEvent events
	public class DeviceLightEventListener : EventListener{
		
		public DeviceLightEventDelegate Listener;
		
		public DeviceLightEventListener(DeviceLightEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((DeviceLightEvent)e);
		}
		
	}
	
	/// Handler for DeviceMotionEvent events
	public class DeviceMotionEventListener : EventListener{
		
		public DeviceMotionEventDelegate Listener;
		
		public DeviceMotionEventListener(DeviceMotionEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((DeviceMotionEvent)e);
		}
		
	}
	
	/// Handler for DeviceOrientationEvent events
	public class DeviceOrientationEventListener : EventListener{
		
		public DeviceOrientationEventDelegate Listener;
		
		public DeviceOrientationEventListener(DeviceOrientationEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((DeviceOrientationEvent)e);
		}
		
	}
	
	/// Handler for DeviceProximityEvent events
	public class DeviceProximityEventListener : EventListener{
		
		public DeviceProximityEventDelegate Listener;
		
		public DeviceProximityEventListener(DeviceProximityEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((DeviceProximityEvent)e);
		}
		
	}
	
	/// Handler for DOMTransactionEvent events
	public class DOMTransactionEventListener : EventListener{
		
		public DOMTransactionEventDelegate Listener;
		
		public DOMTransactionEventListener(DOMTransactionEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((DOMTransactionEvent)e);
		}
		
	}
	
	/// Handler for DragEvent events
	public class DragEventListener : EventListener{
		
		public DragEventDelegate Listener;
		
		public DragEventListener(DragEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((DragEvent)e);
		}
		
	}
	
	/// Handler for EditingBeforeInputEvent events
	public class EditingBeforeInputEventListener : EventListener{
		
		public EditingBeforeInputEventDelegate Listener;
		
		public EditingBeforeInputEventListener(EditingBeforeInputEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((EditingBeforeInputEvent)e);
		}
		
	}
	
	/// Handler for ErrorEvent events
	public class ErrorEventListener : EventListener{
		
		public ErrorEventDelegate Listener;
		
		public ErrorEventListener(ErrorEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((ErrorEvent)e);
		}
		
	}
	
	/// Handler for FetchEvent events
	public class FetchEventListener : EventListener{
		
		public FetchEventDelegate Listener;
		
		public FetchEventListener(FetchEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((FetchEvent)e);
		}
		
	}
	
	/// Handler for FocusEvent events
	public class FocusEventListener : EventListener{
		
		public FocusEventDelegate Listener;
		
		public FocusEventListener(FocusEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((FocusEvent)e);
		}
		
	}
	
	/// Handler for GamepadEvent events
	public class GamepadEventListener : EventListener{
		
		public GamepadEventDelegate Listener;
		
		public GamepadEventListener(GamepadEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((GamepadEvent)e);
		}
		
	}
	
	/// Handler for IDBVersionChangeEvent events
	public class IDBVersionChangeEventListener : EventListener{
		
		public IDBVersionChangeEventDelegate Listener;
		
		public IDBVersionChangeEventListener(IDBVersionChangeEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((IDBVersionChangeEvent)e);
		}
		
	}
	
	/// Handler for InputEvent events
	public class InputEventListener : EventListener{
		
		public InputEventDelegate Listener;
		
		public InputEventListener(InputEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((InputEvent)e);
		}
		
	}
	
	/// Handler for MediaStreamEvent events
	public class MediaStreamEventListener : EventListener{
		
		public MediaStreamEventDelegate Listener;
		
		public MediaStreamEventListener(MediaStreamEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((MediaStreamEvent)e);
		}
		
	}
	
	/// Handler for MessageEvent events
	public class MessageEventListener : EventListener{
		
		public MessageEventDelegate Listener;
		
		public MessageEventListener(MessageEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((MessageEvent)e);
		}
		
	}
	
	/// Handler for MouseEvent events
	public class MouseEventListener : EventListener{
		
		public MouseEventDelegate Listener;
		
		public MouseEventListener(MouseEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((MouseEvent)e);
		}
		
	}
	
	/// Handler for MutationEvent events
	public class MutationEventListener : EventListener{
		
		public MutationEventDelegate Listener;
		
		public MutationEventListener(MutationEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((MutationEvent)e);
		}
		
	}
	
	/// Handler for OfflineAudioCompletionEvent events
	public class OfflineAudioCompletionEventListener : EventListener{
		
		public OfflineAudioCompletionEventDelegate Listener;
		
		public OfflineAudioCompletionEventListener(OfflineAudioCompletionEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((OfflineAudioCompletionEvent)e);
		}
		
	}
	
	/// Handler for PageTransitionEvent events
	public class PageTransitionEventListener : EventListener{
		
		public PageTransitionEventDelegate Listener;
		
		public PageTransitionEventListener(PageTransitionEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((PageTransitionEvent)e);
		}
		
	}
	
	/// Handler for PointerEvent events
	public class PointerEventListener : EventListener{
		
		public PointerEventDelegate Listener;
		
		public PointerEventListener(PointerEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((PointerEvent)e);
		}
		
	}
	
	/// Handler for PopStateEvent events
	public class PopStateEventListener : EventListener{
		
		public PopStateEventDelegate Listener;
		
		public PopStateEventListener(PopStateEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((PopStateEvent)e);
		}
		
	}
	
	/// Handler for ProgressEvent events
	public class ProgressEventListener : EventListener{
		
		public ProgressEventDelegate Listener;
		
		public ProgressEventListener(ProgressEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((ProgressEvent)e);
		}
		
	}
	
	/// Handler for RelatedEvent events
	public class RelatedEventListener : EventListener{
		
		public RelatedEventDelegate Listener;
		
		public RelatedEventListener(RelatedEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((RelatedEvent)e);
		}
		
	}
	
	/// Handler for RTCDataChannelEvent events
	public class RTCDataChannelEventListener : EventListener{
		
		public RTCDataChannelEventDelegate Listener;
		
		public RTCDataChannelEventListener(RTCDataChannelEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((RTCDataChannelEvent)e);
		}
		
	}
	
	/// Handler for RTCIdentityErrorEvent events
	public class RTCIdentityErrorEventListener : EventListener{
		
		public RTCIdentityErrorEventDelegate Listener;
		
		public RTCIdentityErrorEventListener(RTCIdentityErrorEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((RTCIdentityErrorEvent)e);
		}
		
	}
	
	/// Handler for RTCIdentityEvent events
	public class RTCIdentityEventListener : EventListener{
		
		public RTCIdentityEventDelegate Listener;
		
		public RTCIdentityEventListener(RTCIdentityEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((RTCIdentityEvent)e);
		}
		
	}
	
	/// Handler for RTCPeerConnectionIceEvent events
	public class RTCPeerConnectionIceEventListener : EventListener{
		
		public RTCPeerConnectionIceEventDelegate Listener;
		
		public RTCPeerConnectionIceEventListener(RTCPeerConnectionIceEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((RTCPeerConnectionIceEvent)e);
		}
		
	}
	
	/// Handler for SensorEvent events
	public class SensorEventListener : EventListener{
		
		public SensorEventDelegate Listener;
		
		public SensorEventListener(SensorEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((SensorEvent)e);
		}
		
	}
	
	/// Handler for StorageEvent events
	public class StorageEventListener : EventListener{
		
		public StorageEventDelegate Listener;
		
		public StorageEventListener(StorageEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((StorageEvent)e);
		}
		
	}
	
	/// Handler for SVGEvent events
	public class SVGEventListener : EventListener{
		
		public SVGEventDelegate Listener;
		
		public SVGEventListener(SVGEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((SVGEvent)e);
		}
		
	}
	
	/// Handler for SVGZoomEvent events
	public class SVGZoomEventListener : EventListener{
		
		public SVGZoomEventDelegate Listener;
		
		public SVGZoomEventListener(SVGZoomEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((SVGZoomEvent)e);
		}
		
	}
	
	/// Handler for TextEvent events
	public class TextEventListener : EventListener{
		
		public TextEventDelegate Listener;
		
		public TextEventListener(TextEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((TextEvent)e);
		}
		
	}
	
	/// Handler for TimeEvent events
	public class TimeEventListener : EventListener{
		
		public TimeEventDelegate Listener;
		
		public TimeEventListener(TimeEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((TimeEvent)e);
		}
		
	}
	
	/// Handler for TouchEvent events
	public class TouchEventListener : EventListener{
		
		public TouchEventDelegate Listener;
		
		public TouchEventListener(TouchEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((TouchEvent)e);
		}
		
	}
	
	/// Handler for TrackEvent events
	public class TrackEventListener : EventListener{
		
		public TrackEventDelegate Listener;
		
		public TrackEventListener(TrackEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((TrackEvent)e);
		}
		
	}
	
	/// Handler for TransitionEvent events
	public class TransitionEventListener : EventListener{
		
		public TransitionEventDelegate Listener;
		
		public TransitionEventListener(TransitionEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((TransitionEvent)e);
		}
		
	}
	
	/// Handler for UserProximityEvent events
	public class UserProximityEventListener : EventListener{
		
		public UserProximityEventDelegate Listener;
		
		public UserProximityEventListener(UserProximityEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((UserProximityEvent)e);
		}
		
	}
	
	/// Handler for WebGLContextEvent events
	public class WebGLContextEventListener : EventListener{
		
		public WebGLContextEventDelegate Listener;
		
		public WebGLContextEventListener(WebGLContextEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((WebGLContextEvent)e);
		}
		
	}
	
	/// Handler for WheelEvent events
	public class WheelEventListener : EventListener{
		
		public WheelEventDelegate Listener;
		
		public WheelEventListener(WheelEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((WheelEvent)e);
		}
		
	}
	
}