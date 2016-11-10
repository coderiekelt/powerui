using System;using UnityEngine;using PowerUI;public class PowerBar:DynamicTexture{		/// <summary>A value from 0->1 that represents how full the bar is.</summary>	public float Power=0f;	/// <summary>The number of vertical lines drawn to display the bar.</summary>	public int PixelLines=0;	/// <summary>The colour of a full bar.</summary>	public Color LitColour=Color.blue;	/// <summary>The colour of an empty bar.</summary>	public Color UnlitColour=Color.black;	/// <summary>The y pixel coordinates using the array index as x to draw a quarter circle.	/// The values are essentially mirrored over x to produce a half circle for the ends of the bar.</summary>	public int[] HalfCircleY=new int[]{5,4,4,3,1};		public PowerBar():base(166,11,"powerbar"){		// 166 (w) x 11 (h) px.		// This texture will be accessible with dynamic://powerbar.				PixelLines=166;	}		/// <summary>Increases (or decreases for a negative) the current value of the bar.</summary>	/// <param name="delta">A value from 0->1 representing how much the bar should change by.</param>	public void IncreasePower(float delta){		SetPower(Power+delta);	}		/// <summary>Sets the active value of this bar.</summary>	/// <param name="power">A value from 0->1 indicating how full the bar is.</param>	public void SetPower(float power){		if(power<0f){			power=0f;		}else if(power>1f){			power=1f;		}				if(power==Power){			return;		}		Power=power;		// The bar changed, so request a refresh:		Refresh();	}		/// <summary>Writes the pixels out to the screen.</summary>	public override void Flush(){				// First, clear the graphic:		Clear();				int linesActive=(int)(Power*PixelLines);		if(linesActive<0){			linesActive=0;		}else if(linesActive>PixelLines){			linesActive=PixelLines;		}				// Now draw the ends of the bar:		int left=linesActive-(PixelLines-5);		if(left<0){			left=0;		}		// Left half circle:		DrawLeftHalfCircle(4,5,left);				// Right half circle:		DrawRightHalfCircle(165,5,linesActive);				// And the rectangular section of the bar:		int unlitLineCount=PixelLines-5-linesActive;		if(unlitLineCount<0){			unlitLineCount=0;		}				Color colour=UnlitColour;		int lineCount=0;		for(int x=5;x<=160;x++){			if(lineCount==unlitLineCount){				colour=LitColour;			}			lineCount++;			DrawLine(x,10,x,0,colour);		}	}		/// <summary>Draws the right half of a circle to the image.</summary>	/// <param name="x0">The x location in pixels of the middle of the circle.</param>	/// <param name="y0">The y location in pixels of the middle of the circle.</param>	/// <param name="linesGreen">The number of lines out of 5 that should be coloured green, counting up from the right.</param>	public void DrawRightHalfCircle(int x0,int y0,int linesGreen){		// y^2   = 25 - x^2.		// Radius is always 5. => x^2 + y^2 = 25.		// Must use this as we need to scan from the left to right for linesGreen.				int lineCount=0;		Color colour=LitColour;				for(int i=0;i<5;i++){			if(lineCount==linesGreen){				colour=UnlitColour;			}			lineCount++;			int y=HalfCircleY[4-i];			DrawLine(x0-i,y0-y,x0-i,y0+y,colour);		}	}		/// <summary>Draws the left half of a circle to the image.</summary>	/// <param name="x0">The x location in pixels of the middle of the circle.</param>	/// <param name="y0">The y location in pixels of the middle of the circle.</param>	/// <param name="linesGreen">The number of lines out of 5 that should be coloured green, counting up from the right.</param>	public void DrawLeftHalfCircle(int x0,int y0,int linesGreen){		// y^2   = 25 - x^2.		// Radius is always 5. => x^2 + y^2 = 25.		// Must use this as we need to scan from the left to right for linesGreen.				int lineCount=0;		Color colour=LitColour;				for(int i=0;i<5;i++){			if(lineCount==linesGreen){				colour=UnlitColour;			}			lineCount++;			int y=HalfCircleY[i];			DrawLine(x0-i,y0-y,x0-i,y0+y,colour);		}	}	}