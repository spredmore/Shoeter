using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shoeter
{
	class FadeHandler
	{
		public Timer fadeToBlackTimer;		// Timer to denote how long to fade to black.
		public Timer holdWhileFadedTimer;	// Timer to denote how long the screen should stay faded out.
		public Timer fadeFromBlackTimer;	// Timer to denote how long to fade from black.
		float alpha;						// The value used to modify the color value in Draw in order to fade in/out.
		Boolean fadingOut;					// Specifies if the FadeHandler is fading out or not.
		Boolean holdingWhileFaded;			// Specifies if the FadeHandler is holding the screen faded out or not.
		Boolean fadingIn;					// Specifies if the FadeHandler is fading in or not.
		SpriteBatch spriteBatch;			// SpriteBatch used to draw the fade.

		/// <summary>
		/// Constructor for a FadeHandler.
		/// </summary>
		/// <param name="fadeOutTime">The amount of time it takes to fade out.</param>
		/// <param name="fadeInTime">The amount of time it takes to fade in.</param>
		/// <param name="spriteBatch">Used to draw the fade.</param>
		public FadeHandler(float fadeOutTime, float fadeHoldTime, float fadeInTime, SpriteBatch spriteBatch)
		{
			this.fadeToBlackTimer = new Timer(fadeOutTime);
			this.fadeFromBlackTimer = new Timer(fadeInTime);
			this.holdWhileFadedTimer = new Timer(fadeHoldTime);
			this.alpha = 0.0f;
			this.fadingIn = false;
			this.holdingWhileFaded = false;
			this.fadingOut = false;
			this.spriteBatch = spriteBatch;
		}

		/// <summary>
		/// Fades to black.
		/// </summary>
		public void fadeToBlack()
		{
			if(!fadeToBlackTimer.TimerStarted && !fadeFromBlackTimer.TimerStarted)
			{
				fadeToBlackTimer.startTimer();
				fadingOut = true;
			}			
		}

		/// <summary>
		/// Holds the screen faded out.
		/// </summary>
		public void holdFadeOut()
		{
			if(!holdWhileFadedTimer.TimerStarted && fadeToBlackTimer.TimerCompleted)
			{
				fadingOut = false;
				holdingWhileFaded = true;
				holdWhileFadedTimer.startTimer();
				fadeToBlackTimer.resetTimer();
			}
		}

		/// <summary>
		/// Fades from black.
		/// </summary>
		public void fadeFromBlack()
		{
			if(!fadeFromBlackTimer.TimerStarted && holdWhileFadedTimer.TimerCompleted)
			{
				fadeFromBlackTimer.startTimer();
				holdWhileFadedTimer.resetTimer();
				holdingWhileFaded = false;
				fadingIn = true;
				alpha = 1.0f;
			}
		}

		/// <summary>
		/// Resets all variables in a FadeHandler.
		/// </summary>
		public void resetFadeHandler()
		{
			fadeToBlackTimer.resetTimer();
			holdWhileFadedTimer.resetTimer();
			fadeFromBlackTimer.resetTimer();
			fadingOut = false;
			holdingWhileFaded = false;
			fadingIn = false;
			alpha = 0.0f;
		}

		/// <summary>
		/// Update that is called every frame if fading in/out can occur.
		/// </summary>
		/// <param name="gameTime"></param>
		public void Update(GameTime gameTime)
		{
			if (fadingOut)
			{
				alpha = (fadeToBlackTimer.ElapsedTime / fadeToBlackTimer.TimerEndTime); // Used as a percentage.
				fadeToBlackTimer.Update(gameTime);
			}
			else if (holdingWhileFaded)
			{
				holdWhileFadedTimer.Update(gameTime);
			}
			else if (fadingIn)
			{
				alpha = 1.0f - (fadeFromBlackTimer.ElapsedTime / fadeFromBlackTimer.TimerEndTime);
				fadeFromBlackTimer.Update(gameTime);
			}
		}

		/// <summary>
		/// Draw method called every frame to draw the fade to the screen.
		/// </summary>
		/// <param name="blankTexture"></param>
		public void Draw(Texture2D blankTexture)
		{
			spriteBatch.Draw(blankTexture, new Rectangle(0, 0, 1280, 720), Color.Black * alpha);
		}
	}
}
