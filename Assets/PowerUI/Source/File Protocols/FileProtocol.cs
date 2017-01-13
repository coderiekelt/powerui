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
using Css;
using Dom;


namespace PowerUI{
	
	/// <summary>A delegate for dealing with event targets.</summary>
	public delegate void InternalEventHandler(EventTarget et);
	
	/// <summary>
	/// Represents a custom protocol:// as used by PowerUI files.
	/// For example, if you wish to deliver content in a custom way to PowerUI, implement a new FileProtocol (e.g. 'cdn')
	/// Then, setup its OnGetGraphic function.
	/// </summary>
	
	public class FileProtocol{
		
		/// <summary>An event called when any request is started.
		/// Useful for debugging networking (and is used by the network inspector).</summary>
		public static InternalEventHandler OnRequestStarted;
		
		/// <summary>Returns all protocol names:// that can be used for this protocol.
		/// e.g. new string[]{"cdn","net"}; => cdn://file.png or net://file.png</summary>
		public virtual string[] GetNames(){
			return null;
		}
		
		/// <summary>Used to determine the content type for the file from the given path.</summary>
		public virtual string ContentType(Location path){
			return path.Filetype;
		}
		
		/// <summary>Does the item at the given location have full access to the code security domain?
		/// Used by Nitro. If it does not have full access, the Nitro security domain is asked about the path instead.
		/// If you're unsure, leave this false! If your game uses simple web browsing, 
		/// this essentially stops somebody writing dangerous Nitro on a remote webpage and directing your game at it.</summary>
		public virtual bool FullAccess(Location path){
			return false;
		}
		
		/// <summary>Get generic binary at the given path using this protocol. Used for e.g. fonts.
		/// Once it's been retrieved, this must call package.GotData(theText) internally.</summary>
		public virtual void OnGetData(ContentPackage package){}
		
		/// <summary>Get the file at the given path as a MovieTexture (Unity Pro only!), Texture2D,
		/// SPA Animation or DynamicTexture using this protocol.
		/// Once it's been retrieved, this must call package.GotGraphic(theObject) internally.</summary>
		public virtual void OnGetGraphic(ImagePackage package){
			
			// Get the data - it'll call package.Done which will handle it for us:
			OnGetData(package);
			
		}
		
		/// <summary>The user clicked on the given link which points to the given path.</summary>
		public virtual void OnFollowLink(HtmlElement linkElement,Location path){
			
			// Apply location:
			linkElement.document.location=path;
			
		}
		
	}
	
}