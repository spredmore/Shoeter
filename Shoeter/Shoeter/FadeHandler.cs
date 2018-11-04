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
		public Timer fadeToBlackTimer;
		float alpha;
		Boolean fadingOut;
		Boolean fadingIn;
		SpriteBatch spriteBatch;

		public FadeHandler(float fadeOutTime, SpriteBatch spriteBatch)
		{
			this.fadeToBlackTimer = new Timer(fadeOutTime);
			this.alpha = 0.0f;
			this.fadingIn = false;
			this.fadingOut = false;
			this.spriteBatch = spriteBatch;
		}

		public void fadeToBlack()
		{
			if(!fadeToBlackTimer.TimerStarted)
			{
				fadeToBlackTimer.startTimer();
			}

			fadingOut = true;
		}

		public void resetFadeHandler()
		{
			fadingOut = false;
			fadeToBlackTimer.resetTimer();
			alpha = 0.0f;
		}

		public void Update(GameTime gameTime)
		{
			double elapsedTimeSinceLastDraw = gameTime.ElapsedGameTime.Seconds;

			if (fadingOut)
			{
				alpha += 0.05f;
			}
		}

		public void Draw(Texture2D blankTexture)
		{
			spriteBatch.Draw(blankTexture, new Rectangle(0, 0, 1280, 720), Color.Black * alpha);
		}
	}
}
