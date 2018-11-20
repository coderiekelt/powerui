using System;
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace PowerUI{

	public partial class HtmlElement{

		/// <summary>
		/// Searches for the given term within this element.
		/// </summary>
		public List<SearchResult> searchFor(string term){
			List<SearchResult> results = new List<SearchResult>();

			// For each text node.. (using anElement.allText, the convenience iterator)
			foreach(TextNode text in allText){

				// Any of the standard TextNode attribs are available, like TextNode.data:
				var textContent = text.data;

				// Following loop is from https://stackoverflow.com/questions/2641326/finding-all-positions-of-substring-in-a-larger-string-in-c-sharp
				for (int index = 0;; index += term.Length) {
					index = textContent.IndexOf(term, index);
					if (index == -1) {
						 // Aren't any more
						 break;
					}

					// Got a match - add it as a search result:
					results.Add(new SearchResult(text, index));
				}
			}

			return results;
		}

	}
	
	/// <summary>
	/// Represents a search result found by HtmlElement.searchFor.
	/// </summary>
	public class SearchResult{
		public TextNode Node;
		public int Index;

		public SearchResult(TextNode node, int index){
			Node = node;
			Index = index;
		}
	}

}