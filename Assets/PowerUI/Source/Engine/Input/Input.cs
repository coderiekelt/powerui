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

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
	#define MOBILE
#endif

#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
	#define PRE_UNITY3_5
#endif

#if PRE_UNITY3_5 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
	#define PRE_UNITY5
#endif

#if UNITY_4_6 || UNITY_4_7 || !PRE_UNITY5
	#define HANDLE_UNITY_UI
#endif

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Css;
using Dom;


namespace PowerUI{
	
	/// <summary>Used to resolve gameObjects to eventTargets.
	/// Optionally avoids checking MonoBehaviours by cancelling the event.</summary>
	public delegate IEventTarget TargetResolveDelegate(GameObject go,out bool cancelEvent);
	
	/// <summary>
	/// This class manages input such as clicking, hovering and keypresses.
	/// </summary>
	
	public static class Input{
		
		/// <summary>The raw pointer count. See InputPointer.AllRaw for more info.</summary>
		public static int PointerCount{
			get{
				return InputPointer.PointerCount;
			}
		}
		
		/// <summary>The raw set of all current pointing devices. See InputPointer.AllRaw for more info.</summary>
		public static InputPointer[] PointersRaw{
			get{
				return InputPointer.AllRaw;
			}
		}
		
		/// <summary>Clicks, taps and keypresses that pass through the UI and don't hit any WorldUI's will end up being received here.
		/// Note that PowerUI already does raycasting so use the MouseEvent.rayHit property.</summary>
		public static EventTarget Unhandled=new EventTarget();
		/// <summary>Varies based on DPI. On a 96dpi screen, this is 20px. (0.2777 * DPI).</summary>
		public static float MinimumDragStartDistance=20f;
		/// <summary>The system mouse pointer if there is one. Edit this to e.g. fix the mouse in a particular position.</summary>
		public static MousePointer SystemMouse;
		/// <summary>The latest document that got focused.</summary>
		public static HtmlDocument LastFocusedDocument;
		/// <summary>The camera used for 3D input. Defaults to Camera.main if this is null.</summary>
		private static Camera CameraFor3DInput_;
		/// <summary>The camera used for 3D input. Defaults to Camera.main if this is null.</summary>
		public static Camera CameraFor3DInput{
			get{
				
				if(CameraFor3DInput_==null){
					CameraFor3DInput_=Camera.main;
				}
				
				return CameraFor3DInput_;
			}
			set{
				CameraFor3DInput_=value;
			}
		}
		
		/// <summary>True if a system mouse should be created when on desktops.</summary>
		public static bool CreateSystemMouse=true;
		#if HANDLE_UNITY_UI
		/// <summary>True if the input searcher should also hit the Unity UI.</summary>
		public static UnityEngine.UI.GraphicRaycaster UnityUICaster;
		#endif
		
		/// <summary>Sets up the mouse and drag info.</summary>
		public static void Setup(){
			
			// Apply min drag distance:
			MinimumDragStartDistance=0.2777f * ScreenInfo.Dpi;
			
			// Setup the system mouse if this device has one:
			if(SystemInfo.deviceType == DeviceType.Desktop && CreateSystemMouse){
				
				// Create and add a main pointer:
				AddSystemMouse();
			}
			
		}
		
		/// <summary>Adds the system mouse if one is needed.</summary>
		public static void AddSystemMouse(){
			
			if(SystemMouse==null){
				
				Input.SystemMouse=new MousePointer();
				Input.SystemMouse.Add();
				
			}
			
		}
		
		#if MOBILE
		/// <summary>True if autocorrect should be enabled.</summary>
		public static bool Autocorrect;
		/// <summary>The text currently in the keyboard.</summary>
		private static string CachedKeyboardValue;
		/// <summary>The mobile keyboard. This gets opened if the current focused element is a text/password input box.
		/// It gets closed when the element is blurred, or the user closes the keyboard.</summary>
		public static TouchScreenKeyboard MobileKeyboard;
		
		#endif
		
		#if MOBILE
		/// <summary>Handles if the mobile keyboard should get displayed/hidden.</summary>
		/// <param name="mode">The state that defines how the keyboard should open.</param>
		public static bool HandleKeyboard(KeyboardMode mode){
			if(MobileKeyboard!=null){
				MobileKeyboard.active=false;
				MobileKeyboard=null;
			}
			
			if(mode==null){
				return false;
			}else{
				MobileKeyboard=TouchScreenKeyboard.Open(mode.StartText,mode.Type,Autocorrect,mode.Multiline,mode.Secret);
				KeyboardText=null;
				return true;
			}
		}
		
		/// <summary>The mobile keyboard text.</summary>
		public static string KeyboardText{
			get{
				if(MobileKeyboard==null){
					return null;
				}
				return MobileKeyboard.text;
			}
			set{
				if(MobileKeyboard==null){
					return;
				}
				CachedKeyboardValue=value;
				MobileKeyboard.text=value;
			}
		}
		
		/// <summary>Handles input from the onscreen mobile keyboard.</summary>
		private static void HandleMobileKeyboardInput(){
			
			// Did the text change? If so, trigger events.
			if(KeyboardText!=CachedKeyboardValue){
				
				string newText=KeyboardText;
				int original;
				
				if(CachedKeyboardValue==null){
					original=0;
				}else{
					original=CachedKeyboardValue.Length;
				}
				
				int newLength;
				
				if(newText==null){
					newLength=0;
				}else{
					newLength=newText.Length;
				}
				
				CachedKeyboardValue=newText;
				
				// Trigger key presses (twice per character):
				int charDelta=newLength-original;
				
				if(charDelta<0){
					
					// Invert it:
					charDelta=-charDelta;
					
					// Got shorter - add that many backspace presses:
					for(int i=0;i<charDelta;i++){
						
						// Press delete charDelta times:
						OnKeyPress(true,'\0',(int)UnityEngine.KeyCode.Delete);
						OnKeyPress(false,'\0',(int)UnityEngine.KeyCode.Delete);
						
					}
					
				}else{
					
					for(int i=charDelta-1;i>=0;i--){
						
						// Press the character:
						char current=newText[newLength-1-i];
						
						// Get as keycode:
						int code=(int)current;
						
						// Press it:
						OnKeyPress(true,current,code);
						OnKeyPress(false,current,code);
						
					}
					
				}
				
			}
			
			if(MobileKeyboard.done){
				MobileKeyboard=null;
			}
			
		}
		#endif
		
		/// <summary>Either a focused node or the Input.Unhandled set, depending on if anything is actually focused.</summary>
		public static EventTarget ActiveReceiver{
			get{
				
				if(LastFocusedDocument!=null){
					
					// Get the focused element:
					Element active=LastFocusedDocument.activeElement;
					
					if(active!=null && active.isRooted){
						
						// Dispatch events to the focused element:
						return active;
						
					}
					
				}
				
				// Unhandled set:
				return Unhandled;
				
			}
		}
		
		/// <summary>Scrollwheel deltas are typically either -1 or 1. Those values are multiplied by this.</summary>
		public static float ScrollWheelMultiplier=150f;
		
		/// <summary>Called when a scrollwheel changes by the given delta amount.</summary>
		public static void OnScrollWheel(Vector2 delta){
			
			// Create the event:
			WheelEvent e=new WheelEvent();
			e.deltaX=delta.x * ScrollWheelMultiplier;
			e.deltaY=delta.y * ScrollWheelMultiplier;
			e.SetTrusted();
			
			EventTarget target=ActiveReceiver;
			
			// Dispatch the event to the focused element:
			if(target.dispatchEvent(e)){
				
				if(target is HtmlElement){
					// Run the default:
					(target as HtmlElement).OnWheelEvent(e);
				}
				
			}
			
		}
		
		/// <summary>Tells the UI a key was pressed.</summary>
		/// <param name="down">True if the key is now down.</param>
		/// <param name="keyCode">The keycode of the key</param>
		/// <param name="character">The character entered.</param>
		/// <returns>True if the UI consumed the keypress.</returns>
		public static void OnKeyPress(bool down,char character,int keyCode){
			
			KeyboardEvent e=new KeyboardEvent(keyCode,character,down);
			e.SetTrusted();
			e.SetModifiers();
			e.EventType=down?"keydown":"keyup";
			
			EventTarget target=ActiveReceiver;
			
			// Dispatch the event to the focused element:
			if(target.dispatchEvent(e)){
				
				// Run the tag keypress method:
				if(target is HtmlElement){
					(target as HtmlElement).OnKeyPress(e);
				}
				
				// Handle the defaults now:
				
				if(e.heldDown){
					
					// Get the HTML document:
					HtmlDocument htmlDoc=(target is Node) ? ((target as Node).document) as HtmlDocument : UI.document;
					
					// Grab the keycode:
					KeyCode key=e.unityKeyCode;
					
					if(key==KeyCode.Tab && htmlDoc!=null){
						
						// Tab - hop to next input:
						htmlDoc.TabNext();
						
					}else if(e.ctrlKey){
						
						if(key==KeyCode.V){
							
							// Run the onpaste function.
							
							// Get the pasted text:
							string textToPaste=Clipboard.Paste();
							
							// Fire the event now:
							ClipboardEvent ce=new ClipboardEvent("paste",null);
							ce.clipboardData.setData("text/plain",textToPaste);
							ce.SetTrusted();
							target.dispatchEvent(ce);
							
						}else if(key==KeyCode.C && htmlDoc!=null){
							
							// Run the oncopy function.
							
							// Get the selection:
							string selection=htmlDoc.window.getSelection().ToString();
							
							if(selection==null){
								return;
							}
							
							ClipboardEvent ce=new ClipboardEvent("copy",null);
							ce.clipboardData.setData("text/plain",selection);
							ce.SetTrusted();
							
							if(target.dispatchEvent(ce)){
								
								// Copy to the clipboard now:
								Clipboard.Copy(selection);
								
							}
							
						}
						
					}
					
				}
				
			}
			
		}
		
		#if HANDLE_UNITY_UI
		public static HtmlUIPanel MapToUIPanel(ref Vector2 screenPos){
			
			//Create the PointerEventData with null for the EventSystem
			UnityEngine.EventSystems.PointerEventData ed = new UnityEngine.EventSystems.PointerEventData(null);
			
			// Set pos:
			ed.position=screenPos;
			
			//Create list to receive all results
			List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();
			
			//Raycast it
			UnityUICaster.Raycast(ed,results);
			
			if(results.Count==0){
				return null;
			}
			
			// Look for a hit with a HtmlUIPanel component:
			for(int i=results.Count-1;i>=0;i--){
				
				// Get the result:
				UnityEngine.EventSystems.RaycastResult rayRes=results[i];
				
				// Get the panel:
				HtmlUIPanel panel=rayRes.gameObject.GetComponent<HtmlUIPanel>();
				
				if(panel!=null){
					
					// Got one!
					
					Vector2 localPoint;
					
					RectTransform rectTrans=panel.RectTransform;
					
					UnityEngine.RectTransformUtility.ScreenPointToLocalPointInRectangle(
						rectTrans,
						screenPos,
						UnityUICaster.eventCamera,
						out localPoint
					);
					
					// Map the point (relative to the middle):
					Rect rectangle=rectTrans.rect;
					
					localPoint.x+=rectangle.width/2;
					localPoint.y+=rectangle.height/2;
					
					screenPos.x=localPoint.x;
					screenPos.y=( rectangle.height-1-localPoint.y);
					
					return panel;
					
				}
				
			}
			
			return null;
			
		}
		#endif
		
		/// <summary>True if the given element should accept an input event.
		/// Html and body don't accept input in PowerUI (the event has to pass through)
		/// unless they have a background.</summary>
		public static bool AcceptsInput(Element e){
			
			if( (e is HtmlBodyElement) || (e is HtmlHtmlElement) ){
				
				// Must have a background.
				if((e as IRenderableNode).RenderData.HasBackground){
					
					return true;
					
				}
				
				return false;
				
			}
			
			// All other elements are ok:
			return true;
			
		}
		
		/// <summary>Finds an element from the given screen location.
		/// This fires a ray into the scene if the point isn't on the main UI and caches the hit in the given pointer.
		/// X and Y are updated with the document-relative point if it's on a WorldUI/ on the Unity UI.</summary>
		public static Element ElementFromPoint(ref float x,ref float y){
			return ElementFromPoint(ref x,ref y,null);
		}
		
		/// <summary>Finds an element from the given screen location.
		/// This fires a ray into the scene if the point isn't on the main UI and caches the hit in the given pointer.
		/// X and Y are updated with the document-relative point if it's on a WorldUI/ on the Unity UI.</summary>
		public static Element ElementFromPoint(ref float x,ref float y,InputPointer pointer){
			
			Element result=null;
			
			if(UI.document!=null){
				
				// Test the main UI:
				result=UI.document.elementFromPointOnScreen(x,y);
				
				if(result!=null){
					
					// Is it transparent or not html/body?
					if(AcceptsInput(result)){
						// Great, we're done!
						
						if(pointer!=null){
							pointer.LatestHitSuccess=false;
						}
						
						return result;
					}
					
				}
				
			}
			
			// Handle a Unity UI if needed.
			Vector2 point;
			
			#if HANDLE_UNITY_UI
			
			if(UnityUICaster!=null){
				
				// Try hitting the Unity UI first.
				point=new Vector2(x,ScreenInfo.ScreenY-1-y);
				
				HtmlUIPanel panel=MapToUIPanel(ref point);
				
				if(panel!=null){
					
					// Test for an element there:
					result=panel.document.elementFromPointOnScreen(point.x,point.y);
					
					if(result!=null){
						
						// Is it transparent?
						
						// We're still checking a UI so we might still pass "through" it.
						if(AcceptsInput(result)){
							
							// Got a hit! Update x/y:
							x=point.x;
							y=point.y;
							
							if(pointer!=null){
								pointer.LatestHitSuccess=false;
							}
							
							return result;
							
						}
						
					}
					
				}
			
			}
			
			#endif
			
			// Go after WorldUI's next.
			RaycastHit worldUIHit;
			Camera cam=CameraFor3DInput;
			
			if(cam==null){
				// No camera!
				
				if(pointer!=null){
					pointer.LatestHitSuccess=false;
				}
				
				return null;
			}
			
			// Cast into the scene now:
			if(!Physics.Raycast(cam.ScreenPointToRay(new Vector2(x,y)),out worldUIHit)){
				
				// Nothing hit.
				
				if(pointer!=null){
					pointer.LatestHitSuccess=false;
				}
				
				return null;
				
			}
			
			// Cache it:
			if(pointer!=null){
				pointer.LatestHit=worldUIHit;
				pointer.LatestHitSuccess=false;
			}
			
			// Did it hit a worldUI?
			WorldUI worldUI=WorldUI.Find(worldUIHit);
			
			if(worldUI==null){
				
				// Nope!
				return null;
			}
			
			// Resolve the hit into a -0.5 to +0.5 point:
			// (we're ok to reuse x/y here):
			worldUI.ResolvePoint(worldUIHit,out x,out y);
			
			// Map it from a relative point to a 'real' one:
			point=worldUI.RelativePoint(x,y);
			
			// Apply to x/y:
			x=point.x;
			y=point.y;
			
			// Pull an element from that worldUI:
			return worldUI.document.elementFromPointOnScreen(x,y);
			
		}
		
		/// <summary>Used to resolve GameObjects to an IEventTarget.
		/// This typically happens if you have e.g. some instance that represents
		/// items/ players and you want that object to receive the event.</summary>
		public static TargetResolveDelegate TargetResolver;
		
		/// <summary>Attempts to resolve a Gameobject to an event target. Prefers to use Input.TargetResolver but falls back searching for 
		/// a script on the gameobject which implements it. Will search up the hierarchy too.</summary>
		public static IEventTarget ResolveTarget(GameObject toResolve){
			
			// Got a target resolver?
			IEventTarget result=null;
			
			if(TargetResolver!=null){
				
				// Great - try resolving now:
				bool cancelEvent;
				result=TargetResolver(toResolve,out cancelEvent);
				
				if(cancelEvent || result!=null){
					return result;
				}
				
			}
			
			// Look for a MonoBehaviour on the GameObject which implements IEventTarget instead:
			while(toResolve!=null){
				
				// Try getting it:
				result=toResolve.GetComponent<IEventTarget>();
				
				if(result!=null){
					break;
				}
				
				// Try the parent:
				Transform parent=toResolve.transform.parent;
				
				if(parent==null){
					break;
				}
				
				// Next in the hierarchy:
				toResolve=parent.gameObject;
				
			}
			
			return result;
			
		}
		
		
		/// <summary>True if the pointers are invalid.
		/// The things underneath them all will be recomputed on the next update.</summary>
		internal static bool PointersInvalid=true;
		
		
		/// <summary>Updates mouse overs, touches and the mouse position.</summary>
		public static void Update(){
			
			InputPointer pointer;
			
			// Look out for any 'new' touch events:
			int touchCount=UnityEngine.Input.touchCount;
			
			if(touchCount>0){
				
				// For each one..
				for(int i=0;i<touchCount;i++){
					
					// Get the info:
					Touch touch=UnityEngine.Input.GetTouch(i);
					
					pointer=null;
					
					// Already seen this one?
					// There won't be many touches so a straight linear scan is by far the fastest option here.
					// (It's much better than a dictionary).
					for(int p=InputPointer.PointerCount-1;p>=0;p--){
						
						// Get the pointer:
						pointer=InputPointer.AllRaw[p];
						
						if(pointer.ID==touch.fingerId){
							
							// Got it!
							break;
							
						}
						
					}
					
					TouchPointer tp=null;
					
					if(pointer==null){
						
						// Touch start! A new finger has been seen for the first time.
						#if UNITY_5_3_OR_NEWER
							
							// Could be a stylus:
							if(touch.type==TouchType.Stylus){
								tp=new StylusPointer();
							}else{
								tp=new FingerPointer();
							}
							
						#else
							
							// Can only assume it's a finger here:
							tp=new FingerPointer();
						
						#endif
						
						// Add it to the available set:
						tp.Add();
						
					}else{
						
						// Apply latest info:
						tp=(pointer as TouchPointer);
						
					}
					
					// Mark it as still alive so it doesn't get removed shortly.
					tp.StillAlive=true;
					tp.LatestPosition=touch.position;
					
					#if UNITY_5_3_OR_NEWER
					
					tp.LatestPressure=touch.pressure;
					tp.Radius=touch.radius;
					tp.RadiusVariance=touch.radiusVariance;
					
					// Is it a stylus?
					if(tp is StylusPointer){
						tp.UpdateStylus(touch.azimuthAngle,touch.altitudeAngle);
					}
					
					// Always a pressure of 1 otherwise (which is set by default).
					#endif
					
				}
				
			}
			
			// Update each pointer, invalidating itself only if it has moved:
			bool pointerRemoved=false;
			
			for(int i=InputPointer.PointerCount-1;i>=0;i--){
				
				// Get the pointer:
				pointer=InputPointer.AllRaw[i];
				
				// Update its position and state now:
				bool moved=pointer.Relocate();
				
				if(pointer.Removed){
					// It got removed! (E.g. a finger left the screen).
					pointerRemoved=true;
					continue;
				}
				
				// If the pointer is invalid or they all are:
				if(moved || PointersInvalid){
					
					// Figure out what's under it. This takes its pos on the screen
					// and figures out what's there, as well as converting the position
					// to one which is relative to the document (used by e.g. a WorldUI).
					float documentX=pointer.ScreenX;
					float documentY=pointer.ScreenY;
					Element newActiveOver=ElementFromPoint(ref documentX,ref documentY,pointer);
					
					// Update docX/Y:
					pointer.DocumentX=documentX;
					pointer.DocumentY=documentY;
					
					// Get the old one:
					Element oldActiveOver=pointer.ActiveOver;
					
					// Shared event:
					MouseEvent mouseEvent=new MouseEvent(documentX,documentY,pointer.ButtonID,pointer.IsDown);
					mouseEvent.trigger=pointer;
					mouseEvent.clientX=pointer.DocumentX;
					mouseEvent.clientY=pointer.DocumentY;
					mouseEvent.SetModifiers();
					mouseEvent.SetTrusted();
					
					// If overElement has changed from the previous one..
					if(newActiveOver!=oldActiveOver){
						
						
						if(oldActiveOver!=null){
							
							// Clear active:
							pointer.ActiveOver=null;
							
							// Update the CSS:
							(oldActiveOver as IRenderableNode).ComputedStyle.RefreshLocal(true);
							
							// Trigger a mouseout (bubbles):
							mouseEvent.EventType="mouseout";
							mouseEvent.SetTrusted();
							oldActiveOver.dispatchEvent(mouseEvent);
							
							// And a mouseleave (doesn't bubble).
							// Only triggered if newActiveOver is *not* the parent of oldActiveOver.
							if(oldActiveOver.parentNode_!=newActiveOver){
								mouseEvent.Reset();
								mouseEvent.bubbles=false;
								mouseEvent.EventType="mouseleave";
								oldActiveOver.dispatchEvent(mouseEvent);
							}
							
						}
						
						// Update it:
						pointer.ActiveOver=newActiveOver;
						
						if(newActiveOver!=null){
							
							// Update the CSS (hover)
							(newActiveOver as IRenderableNode).ComputedStyle.RefreshLocal(true);
							
						}
						
						// Trigger a mouseover (bubbles):
						mouseEvent.Reset();
						mouseEvent.bubbles=true;
						mouseEvent.EventType="mouseover";
						
						if(newActiveOver==null){
							
							// Clear the main UI tooltip:
							UI.document.tooltip=null;
							
							// Dispatch to unhandled:
							Unhandled.dispatchEvent(mouseEvent);
							
						}else{
							
							newActiveOver.dispatchEvent(mouseEvent);
							
							// Set the tooltip if we've got one:
							UI.document.tooltip=newActiveOver["title"];
							
							// And a mouseenter (doesn't bubble).
							// Only triggered if newActiveOver is *not* a child of oldActiveOver.
							if(newActiveOver.parentNode_!=oldActiveOver){
								mouseEvent.Reset();
								mouseEvent.bubbles=false;
								mouseEvent.EventType="mouseenter";
								newActiveOver.dispatchEvent(mouseEvent);
							}
							
						}
						
					}
					
					if(moved){
						
						// Trigger a mousemove event:
						mouseEvent.Reset();
						mouseEvent.bubbles=true;
						mouseEvent.EventType="mousemove";
						
						if(newActiveOver==null){
							Unhandled.dispatchEvent(mouseEvent);
						}else{
							newActiveOver.dispatchEvent(mouseEvent);
						}
						
					}
					
				}
				
				// If the pointer requires pressure changes..
				if(pointer.FireTouchEvents){
					
					// Set the pressure (which triggers mousedown/up for us too):
					TouchPointer tp=(pointer as TouchPointer);
					
					if(tp.LatestPressure!=tp.Pressure){
						tp.SetPressure(tp.LatestPressure);
					}
					
					// We test touch move down here because it must happen after we've
					// transferred 'ActiveOver' to 'ActiveDown' which happens inside SetPressure.
					
					// Touch events are triggered on the element that was pressed down on
					// (even if we've moved beyond it's bounds).
					if(moved){
						
						// Trigger a touchmove event too:
						TouchEvent te=new TouchEvent("touchmove");
						te.trigger=pointer;
						te.SetTrusted();
						te.clientX=pointer.DocumentX;
						te.clientY=pointer.DocumentY;
						te.SetModifiers();
						
						
						if(pointer.ActivePressed==null){
							
							// Trigger on unhandled:
							Unhandled.dispatchEvent(te);
							
						}else{
						
							pointer.ActivePressed.dispatchEvent(te);
						
						}
						
					}
					
				}
				
				// Test for dragstart
				if(pointer.IsDown && moved){
					
					// How far has it moved since it went down?
					if(pointer.DragStatus==InputPointer.DRAG_UNKNOWN){
						
						if(pointer.MovedBeyondDragDistance){
							
							// Possibly dragging. Is the element we pressed draggable?
							if(pointer.ActivePressed["draggable"]!=null){
								
								// Try start drag:
								DragEvent de=new DragEvent("dragstart");
								de.trigger=pointer;
								de.SetModifiers();
								de.SetTrusted();
								de.clientX=pointer.DocumentX;
								de.clientY=pointer.DocumentY;
								
								if(pointer.ActivePressed.dispatchEvent(de)){
									
									// We're now dragging!
									pointer.DragStatus=InputPointer.DRAGGING;
									
								}else{
									
									// It didn't allow it. This status prevents it from spamming dragstart.
									pointer.DragStatus=InputPointer.DRAG_NOT_AVAILABLE;
									
								}
								
							}else{
								
								// Selectable?
								ComputedStyle cs=(pointer.ActivePressed as IRenderableNode).ComputedStyle;
								Css.Value userSelect=cs[Css.Properties.UserSelect.GlobalProperty];
								
								if(userSelect!=null && !(userSelect.IsType(typeof(Css.Keywords.None))) && !userSelect.IsAuto){
									
									// Selectable!
									Css.Properties.UserSelect.BeginSelect(pointer,userSelect);
									
									// Set status:
									pointer.DragStatus=InputPointer.SELECTING;
									
								}else{
									
									// This status prevents it from spamming, at least until we release.
									pointer.DragStatus=InputPointer.DRAG_NOT_AVAILABLE;
									
								}
								
							}
							
						}
						
					}else if(pointer.DragStatus==InputPointer.DRAGGING){
						
						// Move the dragged element:
						if(pointer.ActivePressed!=null){
							
							DragEvent de=new DragEvent("drag");
							de.trigger=pointer;
							de.SetModifiers();
							de.SetTrusted();
							de.clientX=pointer.DocumentX;
							de.clientY=pointer.DocumentY;
							
							if(pointer.ActivePressed.dispatchEvent(de)){
								pointer.ActivePressed.OnDrag(de);
							}
							
						}
						
					}else if(pointer.DragStatus==InputPointer.SELECTING){
						
						// Update the selection.
						
						if(pointer.ActivePressed!=null){
							
							DragEvent de=new DragEvent("drag");
							de.trigger=pointer;
							de.SetModifiers();
							de.SetTrusted();
							de.clientX=pointer.DocumentX;
							de.clientY=pointer.DocumentY;
							
							if(pointer.ActivePressed.dispatchEvent(de)){
								
								// Get the current selection:
								Selection s=(pointer.ActivePressed.document as HtmlDocument).getSelection();
								
								// Get the range:
								Range range=s.Ranges[0];
								
								// Get text node:
								HtmlTextNode htn=(range.startContainer as HtmlTextNode);
								
								if(htn!=null){
									
									// Get the new end index:
									int endIndex=htn.LetterIndex(pointer.DocumentX,pointer.DocumentY);
									
									// Update:
									range.endOffset=endIndex;
									
									// Flush:
									s.UpdateSelection(true,range);
									
								}
								
							}
							
						}
						
					}
					
				}
				
			}
			
			if(pointerRemoved){
				// Tidy the removed ones:
				InputPointer.Tidy();
			}
			
			// Clear invalidated state:
			PointersInvalid=false;
			
			#if MOBILE
			// Handle mobile keyboard:
			if(MobileKeyboard!=null){
				
				HandleMobileKeyboardInput();
				
			}
			#endif
			
		}
		
	}
	
}