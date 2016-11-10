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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InfiniText;


namespace PowerUI{

	/// <summary>
	/// Represents a font suitable for displaying text on the screen with.
	/// There is one of these per <see cref="PowerUI.Renderman"/>.
	/// </summary>

	public class DynamicFont{
		
		/// <summary>The default font family. This is just the very first loaded family, or the internal font.</summary>
		public static DynamicFont DefaultFamily;
		/// <summary>The font that will attempt to load entirely if no other font is available. Absolute last resort.</summary>
		public static string InternalFont="DejaVu";
		
		
		/// <summary>Creates a new font by loading the font from resources.</summary>
		/// <param name="name">The name of the font to load.</param>
		/// <returns>A new dynamic font.</returns>
		public static DynamicFont Get(string name){
			
			// Start fonts if it needs it:
			Fonts.Start();
			
			// Create it:
			DynamicFont font=new DynamicFont(name);
			
			// Is a font family available already?
			font.Family=Fonts.Get(name);
			
			if(font.Family==null){
				
				// Try loading the faces from Resources:
				font.LoadFaces();
				
			}
			
			return font;
		}
		
		public static DynamicFont GetDefaultFamily(){
			if(DefaultFamily==null){
				DefaultFamily=Get(InternalFont);
			}
			
			return DefaultFamily;
		}
		
		/// <summary>The font family name. E.g. "Vera".</summary>
		public string Name;
		/// <summary>The underlying InfiniText font family.</summary>
		public FontFamily Family;
		/// <summary>The biggest ascender in this font.</summary>
		public float Ascender=0.8f;
		/// <summary>The biggest descender in this font.</summary>
		public float Descender=0.2f;
		/// <summary>The width of the M character of this font.</summary>
		public float MWidth=1f;
		/// <summary>The size of lines in this font.</summary>
		public float LineHeight=1.2f;
		/// <summary>The height of the x character of this font.</summary>
		public float XHeight=1f;
		/// <summary>The thickness of a strikethrough line.</summary>
		public float StrikeSize=0.1f;
		/// <summary>The offset to a strikethrough line.</summary>
		public float StrikeOffset=0.25f;
		/// <summary>A font to fallback on, if one is specified in the HTML.</summary>
		public DynamicFont Fallback;
		/// <summary>The width of a standard space at 1px.</summary>
		public float SpaceSize=1/3f;
		
		
		/// <summary>Creates a new displayable font.</summary>
		/// <param name="name">The name of the font.</param>
		public DynamicFont(string name){
			Name=name;
		}
		
		/// <summary>True if this font is ready to go.</summary>
		public bool Loaded{
			get{
				return (Family!=null);
			}
		}
		
		/// <summary>Loads this font from the local project files. Should be a folder named after the font family and in it a series of font files.</summary>
		/// <returns>True if at least one font face was loaded.</returns>
		public bool LoadFaces(){
			
			// Load all .ttf/.otf files:
			object[] faces=Resources.LoadAll(Name,typeof(TextAsset));
			
			// For each font face..
			for(int i=0;i<faces.Length;i++){
				
				// Get the raw font face data:
				byte[] faceData=((TextAsset)faces[i]).bytes;
				
				if(faceData==null || faceData.Length==0){
					// It's a folder.
					continue;
				}
				
				// Load the font face - adds itself to the family within it if it's a valid font:
				try{
					
					FontFace loaded;
					
					if(Fonts.DeferLoad){
						loaded=FontLoader.DeferredLoad(faceData);
					}else{
						loaded=FontLoader.Load(faceData);
					}
					
					if(loaded!=null && Family==null){
						// Grab the family:
						Family=loaded.Family;
					}
					
				}catch{
					// Unity probably gave us a copyright file or something like that.
					// Generally the loader will catch this internally and return null.
				}
				
			}
			
			if(Family==null){
				return false;
			}
			
			Load();
			return true;
		}
		
		/// <summary>Loads useful values into this font.</summary>
		public void Load(){
			
			// Get the advance width of space:
			Glyph space=Family.Regular.GetGlyph(' ');
			
			if(space!=null){
				
				// Grab the advance width:
				SpaceSize=space.AdvanceWidth;
				
			}
			
			// Grab the regular face (always will be one):
			FontFace regular=Family.Regular;
			
			// Get some useful values:
			Descender=regular.Descender;
			Ascender=regular.Ascender;
			MWidth=regular.MWidth;
			LineHeight=regular.BaselineToBaseline;
			XHeight=regular.ExHeight;
			StrikeSize=regular.StrikeSize;
			StrikeOffset=regular.StrikeOffset;
			
		}
		
		/// <summary>Gets the max descend at the given font size. This is used to define the baseline.</summary>
		public float GetDescend(float fontSize){
			return fontSize * Descender;
		}
		
		/// <summary>Gets the line height.</summary>
		public float GetLineHeight(float fontSize){
			return fontSize * LineHeight;
		}
		
		/// <summary>Gets the max ascend at the given font size. Positive value.</summary>
		public float GetAscend(float fontSize){
			return fontSize * Ascender;
		}
		
		/// <summary>Gets the height of the font at the given font size, using the x-height of the font.</summary>
		public float GetXHeight(float fontSize){
			return fontSize * XHeight;
		}
		
		/// <summary>Gets the height of the font at the given font size.</summary>
		public float GetHeight(float fontSize){
			return fontSize * MWidth;
		}
		
		/// <summary>Gets the standard size of a space for the given font size.</summary>
		/// <param name="fontSize">The size of the font.</param>
		/// <returns>The space size.</returns>
		public float GetSpaceSize(float fontSize){
			return fontSize * SpaceSize;
		}
		
	}
	
}