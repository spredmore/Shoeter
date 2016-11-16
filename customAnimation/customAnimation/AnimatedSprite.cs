//**************************
// Functionality: 
// The functionality of this class is to simply animate a sprite in a fixed position.  
// You are able to specify a position and a texure associated with an AnimatedSprite.
//**************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace customAnimation
{
	class AnimatedSprite
	{
		SpriteBatch spriteBatch;
		Texture2D spriteTexture;    // The image of our animated sprite.
		float timer = 0f;           // The amount of time it takes before the sprite moves to the next frame.
		float interval = 100f;      // The amount of time a frame is shown on screen.
		int currentFrame = 0;       // The current frame we are drawing.
		int spriteWidth = 64;       // The width of the individual sprite.
		int spriteHeight = 64;      // The height of the individual sprite.
		int totalFrames = 0;        // The total amount of frames in the sprite sheet.
		Rectangle sourceRect;       // The rectangle in which the animated sprite will be drawn.
		Vector2 position;           // The position of the animated sprite.
		Vector2 center;             // The center of the animated sprite.

		/// <summary>
		/// Property for the Position of the AnimatedSprite. 
		/// </summary>
		public Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}

		/// <summary>
		/// Property for the Center of the AnimatedSprite.
		/// </summary>
		public Vector2 Center
		{
			get { return center; }
			set { center = value; }
		}

		/// <summary>
		/// Property for the Texture of the AnimatedSprite.
		/// </summary>
		public Texture2D Texture
		{
			get { return spriteTexture; }
			set { spriteTexture = value; }
		}

		/// <summary>
		/// Property for the Rectangle that is drawn around the AnimatedSprite.
		/// </summary>
		public Rectangle SourceRect
		{
			get { return sourceRect; }
			set { sourceRect = value; }
		}

		/// <summary>
		/// Property for the total amount of frames in the sprite sheet for the AnimatedSprite.
		/// </summary>
		public int TotalFrames
		{
			get { return totalFrames; }
			set { totalFrames = value; }
		}

		/// <summary>
		/// Constructor for the AnimatedSprite.
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="currentFrame"></param>
		/// <param name="spriteWidth"></param>
		/// <param name="spriteHeight"></param>
		/// <param name="totalFrames"></param>
		/// <param name="spriteBatch"></param>
		public AnimatedSprite(Texture2D texture, int currentFrame, int spriteWidth, int spriteHeight, int totalFrames, SpriteBatch spriteBatch)
		{
			// When a new animated sprite is created, these variables must be initialized.

			this.spriteTexture = texture;       // The sprite sheet we will be drawing from.
			this.currentFrame = currentFrame;   // The current frame that we are drawing.
			this.spriteWidth = spriteWidth;     // The width of the individual sprite.
			this.spriteHeight = spriteHeight;   // The height of the individual sprite.
			this.totalFrames = totalFrames;     // The total amount of frames in the sprite sheet.
			this.spriteBatch = spriteBatch;     // The spriteBatch that we will use to draw the AnimatedSprite.
		}

		/// <summary>
		/// Called once a frame (from Update()) to update the AnimatedSprite.
		/// </summary>
		/// <param name="gameTime">Snapshot of the game timing state.</param>
		public void Animate(GameTime gameTime)
		{
			// Get a rectangle around the current frame.
			sourceRect = new Rectangle(currentFrame * spriteWidth, 0, spriteWidth, spriteHeight);

			// Increment the timer to see if the AnimatedSprite can move onto the next frame.
			timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			// Check to see if the amount of time for the current frame has passed.
			if (timer > interval)
			{
				// Check to see if the end of the sprite sheet has been reached or not.
				if (currentFrame < totalFrames)
				{
					// Move to the next frame.
					currentFrame++;
				}
				else
				{
					// The end of the sprite sheet has been reached. Reset to the beginning.
					currentFrame = 0;
				} 

				// Reset the timer.
				timer = 0f;
			}

			// Get the center of the current frame.
			center = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);
		}

		/// <summary>
		/// Draw the sprite
		/// </summary>
		public void Draw()
		{
			spriteBatch.Draw(Texture, Position, SourceRect, Color.White, 0f, Center, 1.0f, SpriteEffects.None, 0); 
		}
	}
}