using System;
using System.Text;
using System.Security.Cryptography;


namespace JavaScript{
	
	/// <summary>
	/// Methods which help caching compiled code.
	/// </summary>
	public static class Cache{

		/// <summary>GEts the MD5 hash for the given input.</summary>
		public static string MD5Hash(string input){
			
			// Create a hash:
			MD5 md5 = MD5.Create();
			
			// Compute the hash now:
			byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
			
			// Create a new Stringbuilder to collect the bytes
			// and create a string.
			StringBuilder sBuilder = new StringBuilder();

			// Loop through each byte of the hashed data 
			// and format each one as a hexadecimal string.
			for(int i = 0; i < data.Length; i++){
				sBuilder.Append(data[i].ToString("x2"));
			}
			
			return sBuilder.ToString();
        }
		
		/// <summary>Gets a unique identifier for the given code string at a given location.</summary>
		public static string GetSeed(string location,string code){
			if(code==null){
				code="";
			}
			
			if(location==null){
				location="";
			}
			return MD5Hash(location+":"+code);
		}
	
	}
	
}