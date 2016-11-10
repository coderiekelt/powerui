#if UNITY_STANDALONE_WIN

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;


namespace PowerUI{
	
	/// <summary>
	/// Loads an NPAPI plugin from a DLL.
	/// Note that PowerUI does not store a list of plugins it has installed - you'll need to manage that yourself.
	/// </summary>
	
	internal class NpApiPlugin : DllLoader{
		
		internal const int NP_ABI_MASK=0;
		
		internal const int NP_VERSION_MAJOR=0;
		internal const int NP_VERSION_MINOR=19;
		
		/* RC_DATA types for version info - required */
		internal const int NP_INFO_ProductVersion=1;
		internal const int NP_INFO_MIMEType=2;
		internal const int NP_INFO_FileOpenName=3;
		internal const int NP_INFO_FileExtents=4;
		
		/* RC_DATA types for version info - used if found */
		internal const int NP_INFO_FileDescription=5;
		internal const int NP_INFO_ProductName=6;
		
		/* RC_DATA types for version info - optional */
		internal const int NP_INFO_CompanyName=7;
		internal const int NP_INFO_FileVersion=8;
		internal const int NP_INFO_InternalName=9;
		internal const int NP_INFO_LegalCopyright=10;
		internal const int NP_INFO_OriginalFilename=11;
		
		/* Error codes */
		internal const short NPERR_NO_ERROR=0;
		internal const short NPERR_GENERIC_ERROR =1;
		internal const short NPERR_INVALID_INSTANCE_ERROR=2;
		internal const short NPERR_INVALID_FUNCTABLE_ERROR=3;
		internal const short NPERR_MODULE_LOAD_FAILED_ERROR=4;
		internal const short NPERR_OUT_OF_MEMORY_ERROR =5;
		internal const short NPERR_INVALID_PLUGIN_ERROR =6;
		internal const short NPERR_INVALID_PLUGIN_DIR_ERROR=7;
		internal const short NPERR_INCOMPATIBLE_VERSION_ERROR=8;
		internal const short NPERR_INVALID_PARAM=9;
		internal const short NPERR_INVALID_URL =10;
		internal const short NPERR_FILE_NOT_FOUND=11;
		internal const short NPERR_NO_DATA= 12;
		internal const short NPERR_STREAM_NOT_SEEKABLE=13;
		
		/// <summary>NP_Initialize function pointer mapped to a delegate.</summary>
		private NP_Initialize_Delegate NP_Initialize;
		
		
		public NpApiPlugin(string dllPath):base(dllPath){
			
			// Get the NPAPI functions:
			NP_Initialize=(NP_Initialize_Delegate)GetFunction("NP_Initialize",typeof(NP_Initialize_Delegate));
			
		}
		
		public void Initialize(){
			NP_Initialize();
		}
		
		#region NPAPI Delegates
		
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void NP_Initialize_Delegate();
		
		#endregion
		
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct NPP{
		
		/// <summary>Plug-in private data</summary>
		IntPtr pdata;
		/// <summary>UA private data</summary>
		IntPtr ndata;
		
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct NPStream{
		
		/// <summary>Plug-in private data</summary>
		IntPtr pdata;
		/// <summary>UA private data</summary>
		IntPtr ndata;
		string url;
		uint end;
		uint lastmodified;
		IntPtr notifyData;
		string headers; // version 0.17+
		
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct NPByteRange{
		/// <summary>Negative offset means from the end.</summary>
		int offset;
		uint length;
		IntPtr next;
	}
	
	internal enum NPPVariable{
		NPPVpluginNameString = 1,
		NPPVpluginDescriptionString,
		NPPVpluginWindowBool,
		NPPVpluginTransparentBool,
		NPPVjavaClass,                /* Not implemented in Mozilla 1.0 */
		NPPVpluginWindowSize,
		NPPVpluginTimerInterval,
		NPPVpluginScriptableInstance = (10 | NpApiPlugin.NP_ABI_MASK),
		NPPVpluginScriptableIID = 11,
		/* Introduced in Mozilla 0.9.9 */
		NPPVjavascriptPushCallerBool = 12,
		/* Introduced in Mozilla 1.0 */
		NPPVpluginKeepLibraryInMemory = 13,
		NPPVpluginNeedsXEmbed         = 14,
		/* Get the NPObject for scripting the plugin. Introduced in Firefox
		* 1.0 (NPAPI minor version 14).
		*/
		NPPVpluginScriptableNPObject  = 15,
		/* Get the plugin value (as \0-terminated UTF-8 string data) for
		* form submission if the plugin is part of a form. Use
		* NPN_MemAlloc() to allocate memory for the string data. Introduced
		* in Mozilla 1.8b2 (NPAPI minor version 15).
		*/
		NPPVformValue = 16
		#if XP_MACOSX
		/* Used for negotiating drawing models */
		, NPPVpluginDrawingModel = 1000
		#endif
	}
	
	/// <summary>
	/// The NPAPI NPN_* functions tbat are provided by the navigator and called by plugins.
	/// </summary>
	public class NavigatorNpApi{
		
		/// <summary>The target window.</summary>
		public Window Window;
		
		
		/*
		public void NPN_Version(ref int plugin_major, ref int plugin_minor,ref int netscape_major, ref int netscape_minor){
			
			plugin_major=NpApiPlugin.NP_VERSION_MAJOR;
			plugin_minor=NpApiPlugin.NP_VERSION_MINOR;
			
			netscape_major=UI.Major;
			netscape_minor=UI.Minor;
			
		}
		
		public short NPN_GetURLNotify(
			NPP instance,
			[MarshalAs(UnmanagedType.LPStr)]ref string url,
			[MarshalAs(UnmanagedType.LPStr)]ref string target,
			IntPtr notifyData
		){
			
			url=Window.document.location.href;
			target="";
			return NPERR_NO_ERROR;
			
		}
		
		public short NPN_GetURL(
			NPP instance,
			[MarshalAs(UnmanagedType.LPStr)]ref string url,
			[MarshalAs(UnmanagedType.LPStr)]ref string target
		){
			
			url=Window.document.location.href;
			target="";
			return NPERR_NO_ERROR;
			
		}
		
		public short NPN_PostURLNotify(
			NPP instance,
			[MarshalAs(UnmanagedType.LPStr)]ref string url,
			[MarshalAs(UnmanagedType.LPStr)]ref string target,
			uint len,
			[MarshalAs(UnmanagedType.LPStr)]ref string buf,
			byte file,
			IntPtr notifyData
		){
			
			url=Window.document.location.href;
			target="";
			return NPERR_NO_ERROR;
			
		}
		
		public short NPN_PostURL(
			NPP instance,
			[MarshalAs(UnmanagedType.LPStr)]ref string url,
			[MarshalAs(UnmanagedType.LPStr)]ref string target,
			uint32 len,
			[MarshalAs(UnmanagedType.LPStr)]ref string buf,
			IntPtr file
		){
			
			url=Window.document.location.href;
			target="";
			buf="";
			return NPERR_NO_ERROR;
			
		}
		
		public short NPN_RequestRead(NPStream stream, NPByteRange rangeList){
			
			return NPERR_NO_ERROR;
			
		}
		
		public short NPN_NewStream(
			NPP instance,
			NPMIMEType type,
			[MarshalAs(UnmanagedType.LPStr)]ref string target,
			ref NPStream stream
		){
			
		}
		
		public int NPN_Write(NPP instance, NPStream stream, int len,void* buffer){
			
		}
		
		public short NPN_DestroyStream(NPP instance, NPStream* stream,NPReason reason){
			
		}
		
		public void NPN_Status(NPP instance, [MarshalAs(UnmanagedType.LPStr)]ref string message){
			
			message="";
			
		}
		
		[MarshalAs(UnmanagedType.LPStr)]
		public string NPN_UserAgent(NPP instance){
			return UI.UserAgent;
		}
		
		public void NPN_ReloadPlugins(byte reloadPages);
		public JRIEnv* NPN_GetJavaEnv(void);
		public jref NPN_GetJavaPeer(NPP instance);
		public short NPN_GetValue(NPP instance, NPNVariable variable, void *value);
		public short NPN_SetValue(NPP instance, NPPVariable variable,void *value);
		public void NPN_InvalidateRect(NPP instance, NPRect *invalidRect);
		public void NPN_InvalidateRegion(NPP instance, NPRegion invalidRegion);
		public void NPN_ForceRedraw(NPP instance);
		*/
		
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct NPNetscapeFuncs{
		ushort size;
		ushort version; // Newer versions may have additional fields added to the end
		IntPtr geturl; // Make a GET request for a URL either to the window or another stream
		IntPtr posturl; // Make a POST request for a URL either to the window or another stream
		IntPtr requestread;
		IntPtr newstream;
		IntPtr write;
		IntPtr destroystream;
		IntPtr status;
		IntPtr uagent;
		IntPtr memalloc; // Allocates memory from the browser's memory space
		IntPtr memfree; // Frees memory from the browser's memory space
		IntPtr memflush;
		IntPtr reloadplugins;
		IntPtr getJavaEnv;
		IntPtr getJavaPeer;
		IntPtr geturlnotify; // Async call to get a URL
		IntPtr posturlnotify; // Async call to post a URL
		IntPtr getvalue; // Get information from the browser
		IntPtr setvalue; // Set information about the plugin that the browser controls
		IntPtr invalidaterect;
		IntPtr invalidateregion;
		IntPtr forceredraw;
		IntPtr getstringidentifier; // Get a NPIdentifier for a given string
		IntPtr getstringidentifiers;
		IntPtr getintidentifier;
		IntPtr identifierisstring;
		IntPtr utf8fromidentifier; // Get a string from a NPIdentifier
		IntPtr intfromidentifier;
		IntPtr createobject; // Create an instance of a NPObject
		IntPtr retainobject; // Increment the reference count of a NPObject
		IntPtr releaseobject; // Decrement the reference count of a NPObject
		IntPtr invoke; // Invoke a method on a NPObject
		IntPtr invokeDefault; // Invoke the default method on a NPObject
		IntPtr evaluate; // Evaluate javascript in the scope of a NPObject
		IntPtr getproperty; // Get a property on a NPObject
		IntPtr setproperty; // Set a property on a NPObject
		IntPtr removeproperty; // Remove a property from a NPObject
		IntPtr hasproperty; // Returns true if the given NPObject has the given property
		IntPtr hasmethod; // Returns true if the given NPObject has the given Method
		IntPtr releasevariantvalue; // Release a MNVariant (free memory)
		IntPtr setexception;
		IntPtr pushpopupsenabledstate;
		IntPtr poppopupsenabledstate;
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct NPPluginFuncs {
		ushort size;
		ushort version;
		IntPtr newp;
		IntPtr destroy;
		IntPtr setwindow;
		IntPtr newstream;
		IntPtr destroystream;
		IntPtr asfile;
		IntPtr writeready;
		IntPtr write;
		IntPtr print;
		IntPtr evt;
		IntPtr urlnotify;
		IntPtr javaClass;
		IntPtr getvalue;
		IntPtr setvalue;
	}
	
	internal enum NPNVariable{
		NPNVxDisplay = 1,
		NPNVxtAppContext,
		NPNVnetscapeWindow,
		NPNVjavascriptEnabledBool,
		NPNVasdEnabledBool,
		NPNVisOfflineBool,
		/* 10 and over are available on Mozilla builds starting with 0.9.4 */
		NPNVserviceManager = (10 | NpApiPlugin.NP_ABI_MASK),
		NPNVDOMElement     = (11 | NpApiPlugin.NP_ABI_MASK),   /* available in Mozilla 1.2 */
		NPNVDOMWindow      = (12 | NpApiPlugin.NP_ABI_MASK),
		NPNVToolkit        = (13 | NpApiPlugin.NP_ABI_MASK),
		NPNVSupportsXEmbedBool = 14,
		/* Get the NPObject wrapper for the browser window. */
		NPNVWindowNPObject = 15,
		/* Get the NPObject wrapper for the plugins DOM element. */
		NPNVPluginElementNPObject = 16,
		NPNVSupportsWindowless = 17
		#if XP_MACOSX
			/* Used for negotiating drawing models */
			, NPNVpluginDrawingModel = 1000
		, NPNVsupportsCoreGraphicsBool = 2001
		#endif
	}
	
	internal enum NPWindowType{
		Window = 1,
		Drawable
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct NPSavedData{
		int len;
		IntPtr buf;
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct NPRect{
		ushort top;
		ushort left;
		ushort bottom;
		ushort right;
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct NPSize{
		int width;
		int height;
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct NPWindow{
		
		/// <summary>The native window handle. hWnd on Windows.</summary>
		IntPtr nativeWindow;
		/// <summary>Position of top left corner relative to the UA.</summary>
		uint x;
		/// <summary>Position of top left corner relative to the UA.</summary>
		uint y;
		/// <summary>Maximum window size</summary>
		uint width;
		/// <summary>Maximum window size</summary>
		uint height;
		/// <summary>Clipping rectangle in port coordinates</summary>
		NPRect clipRect;
		
		#if XP_UNIX && !XP_MACOSX
		/// <summary>Platform-dependent additonal data (old Mac only)</summary>
		IntPtr ws_info;
		#endif
		
		/// <summary>Is this a window or a drawable?</summary>
		NPWindowType type;
		
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct NPFullPrint{
		byte pluginPrinted;/* Set TRUE if plugin handled fullscreen printing */
		byte printOne;		 /* TRUE if plugin should print one copy to default printer */
		IntPtr platformPrint; /* Platform-specific printing info */
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct NPEmbedPrint{
		NPWindow window;
		IntPtr platformPrint; /* Platform-specific printing info */
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct NPPrint_Union{
		NPFullPrint fullPrint;   /* if mode is NP_FULL */
		NPEmbedPrint embedPrint; /* if mode is NP_EMBED */
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct NPPrint{
		ushort mode;               /* NP_FULL or NP_EMBED */
		NPPrint_Union print;
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct NPEvent{
		
		ushort Event; // can also be a uint
		uint WParam;
		uint LParam;
		
	}
	
	/// <summary>
	/// Loads DLLs dynamically. Used by NPAPI to load e.g. Flash.
	/// </summary>
	internal class DllLoader{
		
		/// <summary>A pointer to the DLL itself.</summary>
		private IntPtr Dll;
		
		[DllImport("kernel32.dll")]
		internal static extern IntPtr LoadLibrary(string dllToLoad);
		
		[DllImport("kernel32.dll")]
		internal static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

		[DllImport("kernel32.dll")]
		internal static extern bool FreeLibrary(IntPtr hModule);
		
		/// <summary>Gets a function from the DLL as a C# delegate.</summary>
		internal object GetFunction(string name,Type delegateType){
			
			// Get the pointer:
			IntPtr functionPtr=GetProcAddress(Dll, name);
			
			// Map to a delegate:
			return Marshal.GetDelegateForFunctionPointer(functionPtr, delegateType);
			
		}
		
		/// <summary>Loads the named DLL.</summary>
		public DllLoader(string dllName){
			
			Dll = LoadLibrary(dllName);
			
		}
		
		/// <summary>Frees the DLL.</summary>
		~DllLoader()
		{
			FreeLibrary(Dll);
		}
		
	}
	
}

#endif