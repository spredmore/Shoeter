using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace customAnimation
{
	class Air : AnimatedSprite
	{
		public static List<Air> allAirs = new List<Air>();
		public static List<Tile> allQCannons = new List<Tile>();
		public static List<Tile> allWCannons = new List<Tile>();
		public static List<Tile> allECannons = new List<Tile>();
		public static List<Tile> allACannons = new List<Tile>();
		public static List<Tile> allDCannons = new List<Tile>();
		public static List<Tile> allZCannons = new List<Tile>();
		public static List<Tile> allXCannons = new List<Tile>();
		public static List<Tile> allCCannons = new List<Tile>();

		public static Boolean areQCannonsOn = false;
		public static Boolean areWCannonsOn = false;
		public static Boolean areECannonsOn = false;
		public static Boolean areACannonsOn = false;
		public static Boolean areDCannonsOn = false;
		public static Boolean areZCannonsOn = false;
		public static Boolean areXCannonsOn = false;
		public static Boolean areCCannonsOn = false;

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

			foreach(Air air in Air.allAirs)
			{
				if (PositionRect.Intersects(shoes.PositionRect))
				{
					// This works. Set position to new (0, 0) to confirm/see again.
				}
			}

		}

		public static void turnOnAllWCannons(ContentManager content, SpriteBatch spriteBatch)
		{
			foreach(Tile cannon in allWCannons)
			{
				cannon.IsAirCannonSwitchOn = true;
				Air newAir = new Air(content.Load<Texture2D>("Sprites/AnimatedAir64x48"), 0, 32, 48, 1, spriteBatch, new Vector2(cannon.Position.X - 16, cannon.Position.Y - 56f));
				Air.allAirs.Add(newAir);
			}
		}
	}
}
