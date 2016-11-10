using UnityEngine;
using Css;
using Dom;


namespace PowerUI{
   
    /// <summary>
    /// This protocol is used by data:meta/type;base64_data
    /// </summary>
   
    public class DataProtocol:FileProtocol{
       
        /// <summary>Returns all protocol names:// that can be used for this protocol.</summary>
        public override string[] GetNames(){
            return new string[]{"data"};
        }
		
		// Raw binary data
		public override void OnGetData(ContentPackage package){
			
			// Content type header is segments[0]:
			string contentType=package.location.Segments[0];
			
			// The data is at location.Segments[1]. We're assuming base64:
			byte[] data=System.Convert.FromBase64String(package.location.Segments[1]);
			
			// Apply headers:
			package.responseHeaders["content-type"]=contentType;
			package.ReceivedHeaders(data.Length);
			
			// Apply data:
			package.ReceivedData(data,0,data.Length);
			
		}
       
    }
   
}