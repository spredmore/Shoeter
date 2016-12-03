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

		private Char airType;	// Denotes what kind of Air Cannon the Air is coming out of.

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
		public Air(Texture2D texture, int currentFrame, int spriteWidth, int spriteHeight, int totalFrames, SpriteBatch spriteBatch, Vector2 position, Single rotation, Char type)
		{
		    this.Texture = texture;
		    this.CurrentFrame = currentFrame;
		    this.SpriteWidth = spriteWidth;
		    this.SpriteHeight = spriteHeight;
		    this.TotalFrames = totalFrames;
		    this.SpriteBatch = spriteBatch;
			this.Position = position;
			this.PositionRect = new Rectangle((int)position.X, (int)position.Y, spriteWidth, spriteHeight);
			this.Rotation = rotation;
			this.RotatedRect = new RotatedRectangle(this.PositionRect, rotation);
			airType = type;

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
				if (this.RotatedRect.Intersects(new RotatedRectangle(shoes.PositionRect, 0.0f)))
				{
					//MathHelper.Pi / 2
					//shoes.Position = new Vector2(0, 0);
					// Top Left, Top Right, Bottom Left, Bottom Right
					debug = this.RotatedRect.UpperLeftCorner().ToString() + " | " + this.RotatedRect.UpperRightCorner().ToString() + " | " + this.RotatedRect.LowerLeftCorner().ToString() + " | " + this.RotatedRect.LowerRightCorner().ToString();
				}
				else
				{
					debug = "NO AIR COLLISION";
				}
			}

		}

		/// <summary>
		/// Turns on all Q Cannons.
		/// </summary>
		/// <param name="content">Run-time component which loads managed objects from the binary files produced by the design time content pipeline.</param>
		/// <param name="spriteBatch">Enables a group of sprites to be drawn using the same settings.</param>
		public static void turnOnAllQCannons(ContentManager content, SpriteBatch spriteBatch)
		{
			foreach(Tile cannon in allQCannons)
			{
				cannon.IsAirCannonSwitchOn = true;
				Air newAir = new Air(content.Load<Texture2D>("Sprites/AnimatedAir64x48"), 0, 32, 48, 1, spriteBatch, new Vector2(cannon.Position.X - 67, cannon.Position.Y - 52), cannon.Rotation, 'Q');
				Air.allAirs.Add(newAir);
			}
		}

		/// <summary>
		/// Turns on all W Cannons.
		/// </summary>
		/// <param name="content">Run-time component which loads managed objects from the binary files produced by the design time content pipeline.</param>
		/// <param name="spriteBatch">Enables a group of sprites to be drawn using the same settings.</param>
		public static void turnOnAllWCannons(ContentManager content, SpriteBatch spriteBatch)
		{
			foreach (Tile cannon in allWCannons)
			{
				cannon.IsAirCannonSwitchOn = true;
				Air newAir = new Air(content.Load<Texture2D>("Sprites/AnimatedAir64x48"), 0, 32, 48, 1, spriteBatch, new Vector2(cannon.Position.X - 32, cannon.Position.Y - 80), cannon.Rotation, 'W');
				Air.allAirs.Add(newAir);
			}
		}

		/// <summary>
		/// Turns on all E Cannons.
		/// </summary>
		/// <param name="content">Run-time component which loads managed objects from the binary files produced by the design time content pipeline.</param>
		/// <param name="spriteBatch">Enables a group of sprites to be drawn using the same settings.</param>
		public static void turnOnAllECannons(ContentManager content, SpriteBatch spriteBatch)
		{
			foreach (Tile cannon in allECannons)
			{
				cannon.IsAirCannonSwitchOn = true;
				Air newAir = new Air(content.Load<Texture2D>("Sprites/AnimatedAir64x48"), 0, 32, 48, 1, spriteBatch, new Vector2(cannon.Position.X + 12, cannon.Position.Y - 75), cannon.Rotation, 'E');
				Air.allAirs.Add(newAir);
			}
		}

		/// <summary>
		/// Turns on all A Cannons.
		/// </summary>
		/// <param name="content">Run-time component which loads managed objects from the binary files produced by the design time content pipeline.</param>
		/// <param name="spriteBatch">Enables a group of sprites to be drawn using the same settings.</param>
		public static void turnOnAllACannons(ContentManager content, SpriteBatch spriteBatch)
		{
			foreach (Tile cannon in allACannons)
			{
				cannon.IsAirCannonSwitchOn = true;
				Air newAir = new Air(content.Load<Texture2D>("Sprites/AnimatedAir64x48"), 0, 32, 48, 1, spriteBatch, new Vector2(cannon.Position.X - 72, cannon.Position.Y - 8), cannon.Rotation, 'A');
				Air.allAirs.Add(newAir);
			}
		}

		/// <summary>
		/// Turns on all D Cannons.
		/// </summary>
		/// <param name="content">Run-time component which loads managed objects from the binary files produced by the design time content pipeline.</param>
		/// <param name="spriteBatch">Enables a group of sprites to be drawn using the same settings.</param>
		public static void turnOnAllDCannons(ContentManager content, SpriteBatch spriteBatch)
		{
			foreach (Tile cannon in allDCannons)
			{
				cannon.IsAirCannonSwitchOn = true;
				Air newAir = new Air(content.Load<Texture2D>("Sprites/AnimatedAir64x48"), 0, 32, 48, 1, spriteBatch, new Vector2(cannon.Position.X + 40, cannon.Position.Y - 40), cannon.Rotation, 'D');
				Air.allAirs.Add(newAir);
			}
		}

		/// <summary>
		/// Turns on all Z Cannons.
		/// </summary>
		/// <param name="content">Run-time component which loads managed objects from the binary files produced by the design time content pipeline.</param>
		/// <param name="spriteBatch">Enables a group of sprites to be drawn using the same settings.</param>
		public static void turnOnAllZCannons(ContentManager content, SpriteBatch spriteBatch)
		{
			foreach (Tile cannon in allZCannons)
			{
				cannon.IsAirCannonSwitchOn = true;
				Air newAir = new Air(content.Load<Texture2D>("Sprites/AnimatedAir64x48"), 0, 32, 48, 1, spriteBatch, new Vector2(cannon.Position.X - 44, cannon.Position.Y + 26), cannon.Rotation, 'Z');
				Air.allAirs.Add(newAir);
			}
		}

		/// <summary>
		/// Turns on all X Cannons.
		/// </summary>
		/// <param name="content">Run-time component which loads managed objects from the binary files produced by the design time content pipeline.</param>
		/// <param name="spriteBatch">Enables a group of sprites to be drawn using the same settings.</param>
		public static void turnOnAllXCannons(ContentManager content, SpriteBatch spriteBatch)
		{
			foreach (Tile cannon in allXCannons)
			{
				cannon.IsAirCannonSwitchOn = true;
				Air newAir = new Air(content.Load<Texture2D>("Sprites/AnimatedAir64x48"), 0, 32, 48, 1, spriteBatch, new Vector2(cannon.Position.X, cannon.Position.Y + 32), cannon.Rotation, 'X');
				Air.allAirs.Add(newAir);
			}
		}

		/// <summary>
		/// Turns on all C Cannons.
		/// </summary>
		/// <param name="content">Run-time component which loads managed objects from the binary files produced by the design time content pipeline.</param>
		/// <param name="spriteBatch">Enables a group of sprites to be drawn using the same settings.</param>
		public static void turnOnAllCCannons(ContentManager content, SpriteBatch spriteBatch)
		{
			foreach (Tile cannon in allCCannons)
			{
				cannon.IsAirCannonSwitchOn = true;
				Air newAir = new Air(content.Load<Texture2D>("Sprites/AnimatedAir64x48"), 0, 32, 48, 1, spriteBatch, new Vector2(cannon.Position.X + 34, cannon.Position.Y + 4), cannon.Rotation, 'C');
				Air.allAirs.Add(newAir);
			}
		}

		public static void turnOffAllWCannons()
		{
			List<Air> airsToRemove = new List<Air>();

			foreach (Air air in allAirs)
			{
				if(air.airType == 'W')
				{
					airsToRemove.Add(air);
				}
			}

			foreach (Air airToRemove in airsToRemove)
			{
				allAirs.Remove(airToRemove);
			}
		}
	}
}
