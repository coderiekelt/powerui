using Dom;
using System;
using Css;


namespace PowerUI{
	
	/// <summary>
	/// An input pointer.
	/// These trigger touch events and mouse events.
	/// There's one set of pointers for the overall project (InputPointer.All).
	/// If there's a pointer on the screen at all times (i.e. desktops) then the set is always at least 1 in size.
	/// </summary>
	public class InputPointer{
		
		// Various states of InputPointer.DragStatus
		
		/// <summary>Not tested if a pointer is in the dragging state yet.</summary>
		public const int DRAG_UNKNOWN=0;
		/// <summary>Tried to drag something that isn't actually draggable.</summary>
		public const int DRAG_NOT_AVAILABLE=1;
		/// <summary>Dragging something.</summary>
		public const int DRAGGING=2;
		
		/// <summary>To avoid resizing this array repeatedly, we track how many are actually in use.</summary>
		internal static int PointerCount;
		/// <summary>The raw set of all available input pointers. PointerCount is how many indices are actually in use.</summary>
		internal static InputPointer[] AllRaw=new InputPointer[1];
		
		/// <summary>Removes the pointers marked for removal.</summary>
		internal static void Tidy(){
			
			int tidyIndex=0;
			
			for(int i=0;i<PointerCount;i++){
				
				// Get the pointer:
				InputPointer pointer=AllRaw[i];
				
				if(pointer.Removed){
					continue;
				}
				
				// Transfer:
				if(tidyIndex!=i){
					AllRaw[tidyIndex]=pointer;
				}
				
				// Move index up:
				tidyIndex++;
				
			}
			
			// Update count:
			PointerCount=tidyIndex;
			
		}
		
		/// <summary>Doubles the size of the pointer stack, AllRaw.</summary>
		private static void ResizePointerStack(){
			
			InputPointer[] newStack=new InputPointer[AllRaw.Length*2];
			Array.Copy(AllRaw,0,newStack,0,AllRaw.Length);
			AllRaw=newStack;
			
		}
		
		/// <summary>The set of all input pointers that are currently actively doing something.
		/// Note that this array gets built when you request this property;
		/// If you want extra speed, access AllRaw and use PointerCount instead.</summary>
		public static InputPointer[] All{
			get{
				
				// Create:
				InputPointer[] arr=new InputPointer[PointerCount];
				
				// Copy:
				Array.Copy(AllRaw,0,arr,0,PointerCount);
				
				return arr;
				
			}
		}
		
		/// <summary>Used by pointers that don't stick around (fingers and styluses).
		/// They're marked as still alive until they don't show up in the touch set anymore.
		/// It's always unfortunate that we have to do things like this;
		/// the underlying API's are event based but Unity would rather we poll instead.</summary>
		public bool StillAlive=true;
		/// <summary>A unique ID for this pointer.</summary>
		public int ID=-1;
		/// <summary>True if this pointer should fire touch events.</summary>
		public bool FireTouchEvents;
		/// <summary>The current X coordinate on the screen.</summary>
		public float ScreenX;
		/// <summary>The current Y coordinate on the screen.</summary>
		public float ScreenY;
		/// <summary>The current X coordinate on the document that ActiveOver is in.</summary>
		public float DocumentX;
		/// <summary>The current Y coordinate on the document that ActiveOver is in.</summary>
		public float DocumentY;
		/// <summary>The current drag status. See e.g. DRAG_UNKNOWN.</summary>
		public int DragStatus=DRAG_UNKNOWN;
		/// <summary>The X coordinate when the pointer went down (clicked).</summary>
		public float DownDocumentX;
		/// <summary>The Y coordinate when the pointer went down (clicked).</summary>
		public float DownDocumentY;
		/// <summary>This occurs with touch pointers. They get marked as removed when the finger is no longer on the screen.</summary>
		public bool Removed;
		/// <summary>The element that this pointer is currently over.</summary>
		public Element ActiveOver;
		/// <summary>The element that this pointer last pressed/ clicked on.</summary>
		public Element ActivePressed;
		/// <summary>The latest pressure.</summary>
		public float Pressure;
		/// <summary>The latest button that went down.</summary>
		public int ButtonID;
		/// <summary>True if this input is currently down.</summary>
		public bool IsDown{
			get{
				return Pressure!=0f;
			}
		}
		
		/// <summary>The X coordinate when the pointer went down (clicked). Same as DownDocumentX</summary>
		public float startX{
			get{
				return DownDocumentX;
			}
		}
		
		/// <summary>The Y coordinate when the pointer went down (clicked). Same as DownDocumentY</summary>
		public float startY{
			get{
				return DownDocumentY;
			}
		}
		
		/// <summary>Adds this pointer to the available set so it'll get updated.
		public void Add(){
			
			if(PointerCount==AllRaw.Length){
				
				// Resize:
				ResizePointerStack();
				
			}
			
			// Add it:
			AllRaw[PointerCount++]=this;
			
		}
		
		/// <summary>Checks if the delta between DocumentX/Y and DownDocumentX/Y is bigger than our min drag size.</summary>
		public bool MovedBeyondDragDistance{
			get{
				
				if(ActivePressed==null){
					return false;
				}
				
				float d=DownDocumentX-DocumentX;
				
				float distance=ActivePressed.DragStartDistance;
				
				if(distance==0){
					// Default:
					distance=Input.MinimumDragStartDistance;
				}
				
				if(d<=-distance || d>=distance){
					return true;
				}
				
				d=DownDocumentY-DocumentY;
				
				return (d<=-distance || d>=distance);
				
			}
		}
		
		/// <summary>Update ScreenX/ScreenY.</summary>
		/// <returns>True if it moved.</returns>
		public virtual bool Relocate(){
			return false;
		}
		
		/// <summary>Clicks this pointer - same as SetPressure(1).</summary>
		public void Click(int buttonID){
			
			// Set button ID:
			ButtonID=buttonID;
			
			// Always full pressure:
			SetPressure(1f);
			
		}
		
		/// <summary>Releases this pointer - same as SetPressure(0).</summary>
		public void Release(){
			
			// Clear button:
			ButtonID=0;
			
			// Clear pressure:
			SetPressure(0f);
			
		}
		
		/// <summary>Sets the pressure level.</summary>
		public void SetPressure(float v){
			
			// Was it up before?
			bool wasUp=(Pressure==0f);
			
			// Set pressure:
			Pressure=v;
			
			// If it's non-zero then we'll need to grab the clicked object:
			if(v==0f){
				
				if(wasUp){
					// No change.
				}else{
					
					// It's up now. Clear:
					Element oldActivePressed=ActivePressed;
					
					// Clear:
					ActivePressed=null;
						
					if(oldActivePressed!=null){
						// Refresh CSS (active):
						(oldActivePressed as IRenderableNode).ComputedStyle.RefreshPseudoStyle();
					}
					
					// Trigger up event.
					MouseEvent e=new MouseEvent(DocumentX,DocumentY,ButtonID,false);
					e.trigger=this;
					e.SetModifiers();
					e.EventType="mouseup";
					
					if(oldActivePressed==null){
						Input.Unhandled.dispatchEvent(e);
					}else{
						oldActivePressed.dispatchEvent(e);
					}
					
					// Click if needed:
					if(oldActivePressed==ActiveOver){
						
						// Click!
						e.Reset();
						e.trigger=this;
						e.SetModifiers();
						e.EventType="click";
						
						if(oldActivePressed==null){
							Input.Unhandled.dispatchEvent(e);
						}else{
							oldActivePressed.dispatchEvent(e);
						}
						
					}
					
					if(FireTouchEvents){
						
						// Trigger a touchend event too:
						TouchEvent te=new TouchEvent("touchend");
						te.trigger=this;
						te.SetModifiers();
						te.SetTrusted();
						te.clientX=DocumentX;
						te.clientY=DocumentY;
						
						if(oldActivePressed==null){
							Input.Unhandled.dispatchEvent(te);
						}else{
							oldActivePressed.dispatchEvent(te);
						}
						
					}
					
					if(DragStatus==DRAGGING){
						
						// Trigger dragend:
						DragEvent de=new DragEvent("dragend");
						de.trigger=this;
						de.SetModifiers();
						de.SetTrusted();
						de.clientX=ScreenX;
						de.clientY=ScreenY;
						
						if(ActivePressed.dispatchEvent(de)){
							
							// Trigger a drop event next:
							de.Reset();
							de.EventType="drop";
							if(ActiveOver.dispatchEvent(de)){
								
								// Proceed to try and drop it into the dropzone (ActiveOver).
								
							}
							
						}
						
					}
					
					// Always clear drag status:
					DragStatus=0;
					
				}
				
			}else if(wasUp){
				
				// It was up and it's now just gone down.
				
				// Cache position:
				DownDocumentX=DocumentX;
				DownDocumentY=DocumentY;
				
				// Cache down:
				ActivePressed=ActiveOver;
				
				// Trigger down event.
				
				if(ActivePressed!=null){
					
					// Refresh CSS (active):
					(ActivePressed as IRenderableNode).ComputedStyle.RefreshPseudoStyle();
					
				}
			
				// Trigger down event.
				MouseEvent e=new MouseEvent(DocumentX,DocumentY,ButtonID,true);
				e.trigger=this;
				e.EventType="mousedown";
				e.SetModifiers();
				
				if(ActivePressed==null){
					Input.Unhandled.dispatchEvent(e);
				}else{
					ActivePressed.dispatchEvent(e);
				}
				
				if(FireTouchEvents){
					
					// Trigger a touchend event too:
					TouchEvent te=new TouchEvent("touchstart");
					te.trigger=this;
					te.clientX=DocumentX;
					te.clientY=DocumentY;
					te.SetTrusted();
					te.SetModifiers();
					
					if(ActivePressed==null){
						Input.Unhandled.dispatchEvent(te);
					}else{
						ActivePressed.dispatchEvent(te);
					}
					
				}
				
			}
			
		}
		
	}
	
}