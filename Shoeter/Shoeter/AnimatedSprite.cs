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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Shoeter
{
	public class AnimatedSprite
	{
		SpriteBatch spriteBatch;
		Texture2D spriteTexture;    // The image of our AnimatedSprite.
		float timer = 0f;           // The amount of time it takes before the sprite moves to the next frame.
		public float interval;		// The amount of time a frame is shown on screen.
		int currentFrame = 0;       // The current frame we are drawing.
		int spriteWidth = 64;       // The width of the individual sprite.
		int spriteHeight = 64;      // The height of the individual sprite.
		int totalFrames = 0;        // The total amount of frames in the sprite sheet.
		Rectangle sourceRect;       // The rectangle in which the AnimatedSprite will be drawn.
		public Vector2 position;    // The position of the AnimatedSprite.
		Rectangle positionRect;
		Vector2 center;             // The center of the AnimatedSprite.
		Single rotation;			// The rotation of the AnimatedSprite.

		RotatedRectangle rotatedRect;

		public enum AnimationState
		{
			Guy_Idle_Left = 0,
			Guy_Idle_Right = 1,
			Guy_Idle_WithoutShoes_Right = 2,
			Guy_Running_Right = 3,
			Guy_Running_Left = 4,
			Guy_Jumping_Right = 5,
			Guy_Jumping_Left = 6,
			Guy_Falling_Right = 7,
			Guy_Falling_Left = 8,
			Guy_BeingShot_Right = 9,
			Guy_BeingShot_Left = 10,
			Guy_Empty = 11,
			Shoes_Idle_Right = 12,
			Shoes_Idle_Left = 13,
			Shoes_Running_Right = 14,
			Shoes_Running_Left = 15,
			Shoes_Empty = 16
		}

		public static String debug;

		/// <summary>
		/// Property for the Position of the AnimatedSprite. 
		/// </summary>
		public Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}

		/// <summary>
		/// Property for the Position Rectangle of the Animated Sprite.
		/// </summary>
		public Rectangle PositionRect
		{
			get { return positionRect; }
			set { positionRect = value; }
		}

		public RotatedRectangle RotatedRect
		{
			get { return rotatedRect; }
			set { rotatedRect = value; }
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
		protected Texture2D Texture
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
		protected int TotalFrames
		{
			get { return totalFrames; }
			set { totalFrames = value; }
		}

		/// <summary>
		/// Property for the current frame in the sprite sheet.
		/// </summary>
		protected int CurrentFrame
		{
			get { return currentFrame; }
			set { currentFrame = value; }
		}
	
		/// <summary>
		/// Property for the SpriteBatch for this class, which enabled a group of sprites to be drawn to the screen using the same settings.
		/// </summary>
		protected SpriteBatch SpriteBatch
		{
			get { return spriteBatch; }
			set { spriteBatch = value; }
		}
		
		/// <summary>
		/// Property for the sprite width of the AnimatedSprite.
		/// </summary>
		protected int SpriteWidth
		{
			get { return spriteWidth; }
			set { spriteWidth = value; }
		}

		/// <summary>
		/// Property for the sprite height of the AnimatedSprite.
		/// </summary>
		protected int SpriteHeight
		{
			get { return spriteHeight; }
			set { spriteHeight = value; }
		}

		/// <summary>
		/// Property for the rotatio of the AnimatedSprite.
		/// </summary>
		public Single Rotation
		{
			get { return rotation; }
			set { rotation = value; }
		}

		/// <summary>
		/// The interval at which to move to the next frame of a Sprite Sheet.
		/// </summary>
		public float Interval
		{
			get { return interval; }
			set { interval = value; }
		}

		/// <summary>
		/// Default constructor for the AnimatedSprite.
		/// </summary>
		public AnimatedSprite()
		{
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
		public AnimatedSprite(Texture2D texture, Vector2 position, int currentFrame, int spriteWidth, int spriteHeight, int totalFrames, SpriteBatch spriteBatch, float interval, Single initialRotation)
		{
			// When a new animated sprite is created, these variables must be initialized.

			this.spriteTexture = texture;       // The sprite sheet we will be drawing from.
			this.currentFrame = currentFrame;   // The current frame that we are drawing.
			this.spriteWidth = spriteWidth;     // The width of the individual sprite.
			this.spriteHeight = spriteHeight;   // The height of the individual sprite.
			this.totalFrames = totalFrames;     // The total amount of frames in the sprite sheet.
			this.spriteBatch = spriteBatch;     // The spriteBatch that we will use to draw the AnimatedSprite.
			this.interval = interval;			// The amount of time it takes before the sprite moves to the next frame.
			this.position = position;
			this.positionRect = new Rectangle((int)Position.X, (int)Position.Y, spriteWidth, spriteHeight);
			this.rotatedRect = new RotatedRectangle(PositionRect, initialRotation);
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
		}

		public static AnimatedSprite generateAnimatedSpriteBasedOnState(String state, ContentManager content, SpriteBatch spriteBatch, Boolean isChangingGuySprite)
		{
			if (isChangingGuySprite)
			{
				if (state == "Idle_Left")
				{
					return new AnimatedSprite(content.Load<Texture2D>("Sprites/Guy Animations/GuyIdleWithShoes_FacingLeft"), new Vector2(50, 650), 0, 45, 48, 50, spriteBatch, 34f, MathHelper.ToRadians(0));
				}
				else if (state == "Idle_Right")
				{
					return new AnimatedSprite(content.Load<Texture2D>("Sprites/Guy Animations/GuyIdleWithShoes_FacingRight"), new Vector2(51, 650), 0, 45, 48, 50, spriteBatch, 34f, MathHelper.ToRadians(0));
				}
				else if (state == "Idle_WithoutShoes_Right")
				{
					return new AnimatedSprite(content.Load<Texture2D>("Sprites/Guy Animations/GuyIdleWithoutShoes_FacingRight"), new Vector2(52, 650), 0, 26, 48, 40, spriteBatch, 33f, MathHelper.ToRadians(0));
				}
				else if (state == "Running_Left")
				{
					return new AnimatedSprite(content.Load<Texture2D>("Sprites/Guy Animations/GuyRunning_RunningLeft"), new Vector2(100, 650), 0, 37, 48, 27, spriteBatch, 34f, MathHelper.ToRadians(0));
				}
				else if (state == "Running_Right")
				{
					return new AnimatedSprite(content.Load<Texture2D>("Sprites/Guy Animations/GuyRunning_RunningRight"), new Vector2(101, 650), 0, 37, 48, 27, spriteBatch, 34f, MathHelper.ToRadians(0));
				}
				else if (state == "BeingShot_Left")
				{
					return new AnimatedSprite(content.Load<Texture2D>("Sprites/Guy Animations/GuyBeingShot_FacingLeft_LessFrames"), new Vector2(150, 650), 0, 68, 70, 18, spriteBatch, 33f, MathHelper.ToRadians(0));
				}
				else if (state == "BeingShot_Right")
				{
					return new AnimatedSprite(content.Load<Texture2D>("Sprites/Guy Animations/GuyBeingShot_FacingRight_LessFrames"), new Vector2(150, 650), 0, 68, 70, 18, spriteBatch, 33f, MathHelper.ToRadians(0));
				}
				else if (state == "Jumping_Right")
				{
					return new AnimatedSprite(content.Load<Texture2D>("Sprites/Guy Animations/GuyJumping_OnlyJump_FacingRight"), new Vector2(175, 650), 0, 25, 48, 20, spriteBatch, 33f, MathHelper.ToRadians(0));
				}
				else if (state == "Jumping_Left")
				{
					return new AnimatedSprite(content.Load<Texture2D>("Sprites/Guy Animations/GuyJumping_OnlyJump_FacingLeft"), new Vector2(175, 650), 0, 25, 48, 20, spriteBatch, 33f, MathHelper.ToRadians(0));
				}
				else if (state == "Falling_Left")
				{
					return new AnimatedSprite(content.Load<Texture2D>("Sprites/Guy Animations/GuyFalling_FacingLeft"), new Vector2(200, 650), 0, 25, 48, 0, spriteBatch, 33f, MathHelper.ToRadians(0));
				}
				else if (state == "Falling_Right")
				{
					return new AnimatedSprite(content.Load<Texture2D>("Sprites/Guy Animations/GuyFalling_FacingRight"), new Vector2(200, 650), 0, 25, 48, 0, spriteBatch, 33f, MathHelper.ToRadians(0));
				}
				else if (state == "Empty")
				{
					return new AnimatedSprite(content.Load<Texture2D>("Sprites/Transparent16x32"), new Vector2(25, 650), 0, 0, 0, 69, spriteBatch, 34f, MathHelper.ToRadians(0));
				}
			}
			else
			{
				if (state == "Idle_Left")
				{
					return new AnimatedSprite(content.Load<Texture2D>("Sprites/Shoes Animations/ShoesIdle_FacingLeft"), new Vector2(10, 650), 0, 23, 16, 10, spriteBatch, 34f, MathHelper.ToRadians(0));
				}
				else if (state == "Idle_Right")
				{
					return new AnimatedSprite(content.Load<Texture2D>("Sprites/Shoes Animations/ShoesIdle_FacingRight"), new Vector2(11, 650), 0, 23, 16, 10, spriteBatch, 34f, MathHelper.ToRadians(0));
				}
				else if (state == "Running_Left")
				{
					return new AnimatedSprite(content.Load<Texture2D>("Sprites/Shoes Animations/ShoesRunning_FacingLeft"), new Vector2(100, 650), 0, 26, 16, 18, spriteBatch, 1f, MathHelper.ToRadians(0));
				}
				else if (state == "Running_Right")
				{
					return new AnimatedSprite(content.Load<Texture2D>("Sprites/Shoes Animations/ShoesRunning_FacingRight"), new Vector2(101, 650), 0, 26, 16, 18, spriteBatch, 1f, MathHelper.ToRadians(0));
				}
				else if (state == "Running_Right")
				{
					return new AnimatedSprite(content.Load<Texture2D>("Sprites/Shoes Animations/ShoesRunning_FacingRight"), new Vector2(102, 650), 0, 26, 16, 18, spriteBatch, 1f, MathHelper.ToRadians(0));
				}
			}


			return null;
		}

		/// <summary>
		/// Draw the sprite
		/// </summary>
		public void Draw()
		{
			PositionRect = new Rectangle((int)Position.X, (int)Position.Y, spriteWidth, spriteHeight);
			RotatedRect = new RotatedRectangle(PositionRect, 0f);

			Rectangle aPositionAdjusted = new Rectangle(rotatedRect.X + (rotatedRect.Width / 2), rotatedRect.Y + (rotatedRect.Height / 2), rotatedRect.Width, rotatedRect.Height);
			spriteBatch.Draw(Texture, aPositionAdjusted, SourceRect, Color.White, Rotation, new Vector2((int)PositionRect.Width / 2, (int)PositionRect.Height / 2), SpriteEffects.None, 0);
		}
	}
}