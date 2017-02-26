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

using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Represents a standard ordered list element.
	/// </summary>
	
	[Dom.TagName("ol")]
	public class HtmlOListElement:HtmlElement{
		
		/// <summary>The starting index.</summray.
		private int Start_;
		/// <summary>True if the numbering is reversed.</summary>
		private bool Reversed_;
		/// <summary>The type. 1, a, A, i, I.</summary>
		private OrdinalType Type_=OrdinalType.Number;
		
		
		/// <summary>The type attribute.</summary>
		public string type{
			get{
				return this["type"];
			}
			set{
				this["type"]=value;
			}
		}
		
		/// <summary>The start attribute.</summary>
		public long start{
			get{
				return Start_;
			}
			set{
				this["start"]=value.ToString();
			}
		}
		
		/// <summary>True if the numbering is reversed.</summary>
		public bool reversed{
			get{
				return Reversed_;
			}
			set{
				SetBoolAttribute("reversed",value);
			}
		}
		
		/// <summary>Figures out the ordinal type for the given type string. 1, a, A, i, I.</summary>
		public static OrdinalType GetOrdinalType(string type){
			
			switch(type){
				case "a":
					return OrdinalType.AlphaLower;
				case "A":
					return OrdinalType.AlphaUpper;
				case "i":
					return OrdinalType.RomanLower;
				case "I":
					return OrdinalType.RomanUpper;
				default:
				case "1":
					return OrdinalType.Number;
			}
			
		}
		
		/// <summary>Gets the ordinal for a given index.</summary>
		public static string GetOrdinal(int index,OrdinalType type){
			
			switch(type){
				case OrdinalType.RomanUpper:
					return ToRomanUpper(index+1);
				case OrdinalType.RomanLower:
					return ToRomanUpper(index+1).ToLower();
				case OrdinalType.AlphaUpper:
					return ToAlphaUpper(index);
				case OrdinalType.AlphaLower:
					return ToAlphaUpper(index).ToLower();
				default:
				case OrdinalType.Number:
					return index.ToString();
			}
			
		}
		
		/// <summary>Gets the ordinal for a given index.</summary>
		public string GetOrdinal(int index){
			
			if(Reversed_){
				index=-index;
			}
			
			return GetOrdinal(Start_+index,Type_);
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.CloseParagraphThenAdd(this);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				lexer.BlockClose("ol");
			}else{
				return false;
			}
			
			return true;
			
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="type"){
				Type_=GetOrdinalType(this["type"]);
			}else if(property=="start"){
				int.TryParse(this["start"],out Start_);
			}else if(property=="reversed"){
				Reversed_=GetBoolAttribute("reversed");
			}else{
				return false;
			}
			
			return true;
		}
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		private static string[][] RomanNumerals = new string[][]{
			new string[]{"", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"}, // ones
			new string[]{"", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC"}, // tens
			new string[]{"", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM"}, // hundreds
			new string[]{"", "M", "MM", "MMM"} // thousands
		};
		
		/// <summary>Converts a number to roman (uppercase).</summary>
		public static string ToRomanUpper(int number){
			
			// Split integer string into array
			char[] intArr = number.ToString().ToCharArray();
			int len = intArr.Length;
			System.Text.StringBuilder romanNumeral = new System.Text.StringBuilder();
			int i = len;
			
			// starting with the highest place (for 3046, it would be the thousands
			// place, or 3), get the roman numeral representation for that place
			// and add it to the final roman numeral string
			while(i-- > 0){
				romanNumeral.Append( RomanNumerals[i][(int)intArr[len-1-i] - (int)'0'] );
			}
			
			return romanNumeral.ToString();
		}
		
		/// <summary>Converts a number to an alpha index (0 is A, 1 is B etc).</summary>
		public static string ToAlphaUpper(int i){
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			int remainder;
			
			while(i > 0){
				remainder = i % 26;
				i = i / 26;
				result.Insert(0,(char)((char)remainder + 'A'));
			}
			
			return result.ToString();
		}
		
	}
	
	/// <summary>An ordinal type as used by the ol element.</summary>
	public enum OrdinalType{
		Number,
		AlphaLower,
		AlphaUpper,
		RomanLower,
		RomanUpper
	}
	
}