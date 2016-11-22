using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace customAnimation
{
	class Air : AnimatedSprite
	{
		public static List<Air> allAirs = new List<Air>();

		public static string debug;

		/// <summary>
		/// Constructor for the Air class.
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="currentFrame"></param>
		/// <param name="spriteWidth"></param>
		/// <param name="spriteHeight"></param>
		/// <param name="totalFrames"></param>
		/// <param name="spriteBatch"></param>
		/// <param name="position"></param>
		public Air(Texture2D texture, int currentFrame, int spriteWidth, int spriteHeight, int totalFrames, SpriteBatch spriteBatch, Vector2 position)
		{
		    this.Texture = texture;
		    this.CurrentFrame = currentFrame;
		    this.SpriteWidth = spriteWidth;
		    this.SpriteHeight = spriteHeight;
		    this.TotalFrames = totalFrames;
		    this.SpriteBatch = spriteBatch;
			this.Position = position;
			this.PositionRect = new Rectangle((int)position.X, (int)position.Y, spriteWidth, spriteHeight);

			debug = "";
		}

		/// <summary>
		/// Update for the Air class that is called once per frame.
		/// </summary>
		/// <param name="gameTime">Snapshot of the game timing state.</param>
		/// <param name="shoes">A reference to the Shoes.</param>
		public void Update(GameTime gameTime, ref Shoes shoes)
		{
			Animate(gameTime);

			if (PositionRect.Intersects(shoes.PositionRect))
			{
				debug = "it works";
			}
		}
	}
}
