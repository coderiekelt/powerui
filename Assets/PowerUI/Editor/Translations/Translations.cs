//--------------------------------------//               PowerUI////        For documentation or //    if you have any issues, visit//        powerUI.kulestar.com////    Copyright � 2013 Kulestar Ltd//          www.kulestar.com//--------------------------------------using PowerUI.Http;namespace PowerUI{		/// <summary>	/// Used for Automatic whole UI Translations from the Editor.	/// </summary>		public static class Translations{				/// <summary>Performs the translation.</summary>		/// <param name="translation">Information about the translation to perform.</param>		public static void Translate(TranslationInfo translation){						// Start a request:			XMLHttpRequest request=new XMLHttpRequest();						request.open("post","https://translate.kulestar.com/v2/machineTranslate");						request.onload=delegate(UIEvent e){								// Let it know it completed:				translation.Complete(request.responseText);							};						// Send it off:			request.send(translation.Json);					}			}	}