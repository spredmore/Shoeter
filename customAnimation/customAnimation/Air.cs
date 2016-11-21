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

		public Air(Texture2D texture, int currentFrame, int spriteWidth, int spriteHeight, int totalFrames, SpriteBatch spriteBatch)
		{
		    this.Texture = texture;
		    this.CurrentFrame = currentFrame;
		    this.SpriteWidth = spriteWidth;
		    this.SpriteHeight = spriteHeight;
		    this.TotalFrames = totalFrames;
		    this.SpriteBatch = spriteBatch;
		}
	}
}
