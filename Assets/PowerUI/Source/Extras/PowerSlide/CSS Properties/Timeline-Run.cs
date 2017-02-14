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
using PowerUI;
using Css;


namespace PowerSlide{
	
	/// <summary>
	/// Handles operating PowerSlide timelines.
	/// </summary>
	
	public partial class Timeline{
		
		/// <summary>The linked list of active timelines.</summary>
		public static Timeline First;
		/// <summary>The linked list of active timelines.</summary>
		public static Timeline Last;
		/// <summary>Used to periodically call UpdateAll.
		/// Similar to a UITimer but instead this runs 
		/// on the Unity main thread and it's syncable with redraws.</summary>
		private static OnUpdateCallback Updater;
		
		
		/// <summary>Gets the current instance for the given element (null if none found).</summary>
		public static Timeline Get(ComputedStyle style){
			
			Timeline current=First;
			
			while(current!=null){
				if(current.Style==style){
					return current;
				}
				
				current=current.After;
			}
			
			return null;
			
		}
		
		/// <summary>Removes all active slides.</summary>
		public static void Clear(){
			Last=First=null;
		}
		
		/// <summary>Called at the UI update rate to progress the currently running slides.</summary>
		public static void UpdateAll(){
			
			// redraw frame?
			if(First==null || !UI.IsRedrawUpdate){
				return;
			}
			
			// Get the frame time:
			float frameTime=UI.CurrentFrameTime;
			
			Timeline current=First;
			
			while(current!=null){
				current.Update(frameTime);
				current=current.After;	
			}
			
		}
		
		/// <summary>The duration of the whole thing. Overrides timeline.duration, if it has one.</summary>
		public float AppliedDuration=float.MinValue;
		/// <summary>The "actual" duration - the one which considers timeline too.</summary>
		public float ActualDuration;
		/// <summary>The current time in seconds that has passed since it started.</summary>
		public float CurrentTime;
		/// <summary>The sampler used when progressing.</summary>
		public Blaze.CurveSampler ProgressSampler;
		/// <summary>Current instances are stored in a linked list. This is the next one in the list.</summary>
		public Timeline After;
		/// <summary>Current instances are stored in a linked list. This is the one before this in the list.</summary>
		public Timeline Before;
		/// <summary>The delay before this plays, in s.</summary>
		public float Delay;
		/// <summary>Currently running backwards?</summary>
		public bool Backwards;
		/// <summary>The amount this should repeat.</summary>
		public int RepeatCount=1;
		/// <summary>The direction to run.</summary>
		public KeyframesAnimationDirection Direction=KeyframesAnimationDirection.Forward;
		/// <summary>Started yet?</summary>
		public bool Started;
		/// <summary>Is it paused?</summary>
		public bool Paused;
		/// <summary>True if this instance is in the update queue.</summary>
		private bool Enqueued;
		/// <summary>The first currently running slide (from any track).</summary>
		private Slide FirstRunning;
		/// <summary>The last currently running slide (from any track).</summary>
		private Slide LastRunning;
		
		
		/// <summary>Starts this instance.</summary>
		public void Start(){
			
			if(Paused || Started || tracks==null){
				// Block start request.
				return;
			}
			
			Started=true;
			
			if(!Enqueued){
				Enqueue();
			}
			
		}
		
		/// <summary>Adds this instance to the update queue.</summary>
		public void Enqueue(){
			
			if(Enqueued){
				return;
			}
			
			Enqueued=true;
			After=null;
			Before=Last;
			
			if(First==null){
				First=Last=this;
				
				// Enqueue in the update system:
				Updater=OnUpdate.Add(UpdateAll);
				
			}else{
				Last.After=this;
				Last=this;
			}
			
		}
		
		/// <summary>Advances this instance by the given amount.</summary>
		public void Update(float deltaTime){
			
			if(!Started || Paused){
				// Awaiting data, usually.
				return;
			}
			
			if(CurrentTime==0f){
				
				if(deltaTime>0.5f){
					// Block slow frames.
					// This is almost always only ever the very first one
					return;
				}
				
				// Clear running:
				FirstRunning=null;
				LastRunning=null;
				
				// Establish duration:
				float rawDuration=AppliedDuration;
				
				if(rawDuration<0f){
					
					if(duration==null){
						rawDuration=1f;
					}else{
						rawDuration=duration.GetDecimal(Style.RenderData,null);
					}
					
				}
				
				ActualDuration=rawDuration;
				
				// Starting backwards?
				Backwards=( ((int)Direction & 1) == 1);
				
				// Update durations for each track:
				for(int i=0;i<tracks.Length;i++){
					Track track=tracks[i];
					
					if(track==null){
						continue;
					}
					
					track.OnStart();
					
					// Set start/duration:
					track.SetStartAndDuration(rawDuration);
					
					// Reset current index:
					if(Backwards){
						track.currentSlide=track.slides.Length;
					}else{
						track.currentSlide=-1;
					}
					
				}
				
				// Dispatch start:
				Dispatch("start");
				
				// Instant?
				if(ActualDuration==0f){
					
					Stop(true);
					
					return;
				}
				
			}
			
			CurrentTime+=deltaTime;
			
			if(!Style.Element.isRooted){
				
				// Immediately stop - the element was removed (don't call the finished event):
				Stop(false);
				
				return;
				
			}
			
			if(CurrentTime >= ActualDuration){
				
				// Done!
				CompletedCycle();
				
				// Stop there:
				return;
				
			}
			
			// Set ActiveValue by sampling from the curve (if there is one):
			float progress=CurrentTime;
			
			if(ProgressSampler!=null){
				
				// Map through the progression curve:
				ProgressSampler.Goto(progress / ActualDuration,true);
				progress=ProgressSampler.CurrentValue * ActualDuration;
				
			}
			
			// Advance all tracks to the frame at 'progress'.
			for(int i=0;i<tracks.Length;i++){
				
				// Get the track:
				Track track=tracks[i];
				
				if(track==null || track.slides==null || track.slides.Length==0){
					continue;
				}
				
				int length=track.slides.Length;
				
				int index=track.currentSlide;
				
				if(Backwards){
					index--;
				}else{
					index++;
				}
				
				while( (Backwards && index>=0) || (!Backwards && index<length) ){
					
					// Get the slide:
					Slide slideToStart=track.slides[index];
					
					// Startable?
					if(Backwards){
						
						if((1f-slideToStart.computedEnd)>=progress){
							// Nope!
							break;
						}
						
					}else if(slideToStart.computedStart>=progress){
						// Nope!
						break;
					}
					
					// Add to queue:
					slideToStart.NextRunning=null;
					slideToStart.PreviousRunning=LastRunning;
					
					if(FirstRunning==null){
						FirstRunning=LastRunning=slideToStart;
					}else{
						LastRunning.NextRunning=slideToStart;
						LastRunning=slideToStart;
					}
					
					// Start it now:
					slideToStart.Start();
					
					// Next:
					track.currentSlide=index;
					
					if(Paused){
						break;
					}
					
					if(Backwards){
						index--;
					}else{
						index++;
					}
					
				}
				
				if(Paused){
					break;
				}
				
			}
			
			// Kill any slides which are now done:
			Slide current=FirstRunning;
			
			while(current!=null){
				
				float end=Backwards ? (1f-current.computedStart) : current.computedEnd;
				
				if(end<=progress){
					
					// Remove from running:
					if(current.NextRunning==null){
						LastRunning=current.PreviousRunning;
					}else{
						current.NextRunning.PreviousRunning=current.PreviousRunning;
					}
					
					if(current.PreviousRunning==null){
						LastRunning=current.NextRunning;
					}else{
						current.PreviousRunning.NextRunning=current.NextRunning;
					}
					
					// Done! This can "pause" to make it wait for a cue.
					current.End();
					
				}
				
				current=current.NextRunning;
			}
			
		}
		
		/// <summary>Called when a cycle is completed.</summary>
		private void CompletedCycle(){
			
			// Can we repeat? -1 is infinite.
			if(RepeatCount!=-1){
				
				RepeatCount--;
				
				// Got to stop?
				if(RepeatCount==0){
					Stop(true);
					return;
				}
				
			}
			
			Dispatch("iteration");
			
			// If alternate, flip the direction:
			if( ((int)Direction & 2) == 2){
				Backwards=!Backwards;
			}else{
				// Start running backwards?
				Backwards=( ((int)Direction & 1) == 1);
			}
			
			// Set the current frame:
			for(int i=0;i<tracks.Length;i++){
				
				Track track=tracks[i];
				
				if(track==null){
					return;
				}
				
				if(Backwards){
					track.currentSlide=track.slides.Length;
				}else{
					track.currentSlide=-1;
				}
				
			}
			
			// Run again:
			CurrentTime=0f;
			
		}
		
		/// <summary>Changes the paused state of this animation.</summary>
		public void SetPause(bool value){
			
			if(Paused==value){
				return;
			}
			
			if(value){
				Dispatch("pause");
			}else{
				Dispatch("play");
			}
			
			Paused=value;
			
			Slide s=FirstRunning;
			
			while(s!=null){
				
				// Pause each slide:
				s.SetPause(value);
				s=s.NextRunning;
				
			}
			
			// Clear cues:
			ClearCues();
			
			if(!value){
				Start();
			}
		}
		
		/// <summary>
		/// Sets a time curve which progresses the overall slides animation.
		/// Almost always linear but change it for advanced effects.</summary>
		public void SetTimingFunction(Css.Value timingFunc){
			
			// The function to use:
			Blaze.VectorPath path=null;
			
			if(timingFunc!=null){
				
				// Get the defined vector path:
				path=timingFunc.GetPath(
					Style.RenderData,
					Css.Properties.SlidesTimingFunction.GlobalProperty
				);
				
			}
			
			if(path==null){
				ProgressSampler=null;
			}else{
				
				if(!(path is Blaze.RasterVectorPath)){
					
					Blaze.RasterVectorPath rvp=new Blaze.RasterVectorPath();
					path.CopyInto(rvp);
					rvp.ToStraightLines();
					path=rvp;
					
				}
				
				ProgressSampler=new Blaze.CurveSampler(path);
				ProgressSampler.Reset();
			}
			
		}
		
		/// <summary>Dispatches an event of the given name.</summary>
		public void Dispatch(string name){
			
			SlideEvent se=new SlideEvent("slides"+name,null);
			se.timeline=this;
			se.SetTrusted(true);
			Style.Element.dispatchEvent(se);
			
		}
		
		/// <summary>Stops this instance, optionally firing an event.</summary>
		public void Stop(bool fireEvent){
			
			if(!Enqueued){
				return;
			}
			
			Enqueued=false;
			
			if(Before==null){
				First=After;
			}else{
				Before.After=After;
			}
			
			if(After==null){
				Last=Before;
			}else{
				After.Before=Before;
			}
			
			// Clear cues:
			ClearCues();
			
			if(First==null && Updater!=null){
				
				// Stop the updater:
				Updater.Stop();
				Updater=null;
				
			}
			
			// Kill any running slides:
			Slide current=FirstRunning;
			
			while(current!=null){
				
				// Done!
				current.End();
				Paused=false;
				
				current=current.NextRunning;
			}
			
			if(fireEvent){
				Dispatch("end");
			}
			
		}
		
	}
	
}