using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Shoeter
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

		private Char airCannonRepresentation;	// Denotes what kind of Air Cannon the Air is coming out of.

		public static string debug;
		public static string debug2;

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
			this.interval = 100f;
			airCannonRepresentation = type;

			debug = "";
			debug2 = "";
		}

		/// <summary>
		/// Update for the Air class that is called once per frame.
		/// </summary>
		/// <param name="gameTime">Snapshot of the game timing state.</param>
		/// <param name="shoes">A reference to the Shoes.</param>
		/// <param name="guy">A reference to the Guy.</param>
		public void Update(GameTime gameTime, ref Shoes shoes, ref Guy guy)
		{
			Animate(gameTime);

			foreach (Air air in Air.allAirs)
			{
				// Shoes Collision
				if (air.RotatedRect.Intersects(new RotatedRectangle(shoes.PositionRect, 0.0f)) && !shoes.airsShoesHasCollidedWith.Contains(air))
				{
					shoes.airsShoesHasCollidedWith.Add(air);
					shoes.setVelocityUponAirCollision(air.airCannonRepresentation);
				}

				// Guy Collision
				if (air.RotatedRect.Intersects(new RotatedRectangle(guy.PositionRect, 0.0f)) && !guy.airsGuyHasCollidedWith.Contains(air))
				{
					guy.airsGuyHasCollidedWith.Add(air);
					guy.setVelocityUponAirCollision(air.airCannonRepresentation);
				}
			}

			shoes.clearAirsThatShoesCollidedWithIfPossible();
		}

		/// <summary>
		/// Activates all Air Cannons for the Switch that the Shoes collided with.
		/// </summary>
		/// <param name="airCannonSwitch">The Air Cannon Switch that the Shoes has collided with.</param>
		/// <param name="tileCharacterCurrentlyCollidingWith">The tile that the Character is currently colliding with.</param>
		/// <param name="content">Run-time component which loads managed objects from the binary files produced by the design time content pipeline.</param>
		/// <param name="spriteBatch">Enables a group of sprites to be drawn using the same settings.</param>
		public static void activateAirCannons(Tile airCannonSwitch, Tile tileCharacterCurrentlyCollidingWith, ContentManager content, SpriteBatch spriteBatch)
		{
			if (airCannonSwitch.TileRepresentation == 'Q')
			{
				if (!Air.areQCannonsOn && tileCharacterCurrentlyCollidingWith == airCannonSwitch)
				{
					Air.areQCannonsOn = true;
					Air.turnOnAllQCannons(content, spriteBatch);
				}
			}
			else if (airCannonSwitch.TileRepresentation == 'W')
			{
				if (!Air.areWCannonsOn && tileCharacterCurrentlyCollidingWith == airCannonSwitch)
				{
					Air.areWCannonsOn = true;
					Air.turnOnAllWCannons(content, spriteBatch);
				}
			}
			else if (airCannonSwitch.TileRepresentation == 'E')
			{
				if (!Air.areECannonsOn && tileCharacterCurrentlyCollidingWith == airCannonSwitch)
				{
					Air.areECannonsOn = true;
					Air.turnOnAllECannons(content, spriteBatch);
				}
			}
			else if (airCannonSwitch.TileRepresentation == 'A')
			{
				if (!Air.areACannonsOn && tileCharacterCurrentlyCollidingWith == airCannonSwitch)
				{
					Air.areACannonsOn = true;
					Air.turnOnAllACannons(content, spriteBatch);
				}
			}
			else if (airCannonSwitch.TileRepresentation == 'D')
			{
				if (!Air.areDCannonsOn && tileCharacterCurrentlyCollidingWith == airCannonSwitch)
				{
					Air.areDCannonsOn = true;
					Air.turnOnAllDCannons(content, spriteBatch);
				}
			}
			else if (airCannonSwitch.TileRepresentation == 'Z')
			{
				if (!Air.areZCannonsOn && tileCharacterCurrentlyCollidingWith == airCannonSwitch)
				{
					Air.areZCannonsOn = true;
					Air.turnOnAllZCannons(content, spriteBatch);
				}
			}
			else if (airCannonSwitch.TileRepresentation == 'X')
			{
				if (!Air.areXCannonsOn && tileCharacterCurrentlyCollidingWith == airCannonSwitch)
				{
					Air.areXCannonsOn = true;
					Air.turnOnAllXCannons(content, spriteBatch);
				}
			}
			else if (airCannonSwitch.TileRepresentation == 'C')
			{
				if (!Air.areCCannonsOn && tileCharacterCurrentlyCollidingWith == airCannonSwitch)
				{
					Air.areCCannonsOn = true;
					Air.turnOnAllCCannons(content, spriteBatch);
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
			foreach (Tile cannon in allQCannons)
			{
				cannon.IsAirCannonSwitchOn = true;
				Air newAir = new Air(content.Load<Texture2D>("Sprites/AnimatedAir64x48_2"), 0, 32, 48, 1, spriteBatch, new Vector2(cannon.Position.X - 39, cannon.Position.Y - 46), cannon.Rotation, 'Q');
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
				Air newAir = new Air(content.Load<Texture2D>("Sprites/AnimatedAir64x48_2"), 0, 32, 48, 1, spriteBatch, new Vector2(cannon.Position.X - 16, cannon.Position.Y - 56), cannon.Rotation, 'W');
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
				Air newAir = new Air(content.Load<Texture2D>("Sprites/AnimatedAir64x48_2"), 0, 32, 48, 1, spriteBatch, new Vector2(cannon.Position.X + 2, cannon.Position.Y - 44), cannon.Rotation, 'E');
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
				Air newAir = new Air(content.Load<Texture2D>("Sprites/AnimatedAir64x48_2"), 0, 32, 48, 1, spriteBatch, new Vector2(cannon.Position.X - 48, cannon.Position.Y - 24), cannon.Rotation, 'A');
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
				Air newAir = new Air(content.Load<Texture2D>("Sprites/AnimatedAir64x48_2"), 0, 32, 48, 1, spriteBatch, new Vector2(cannon.Position.X + 16, cannon.Position.Y - 24), cannon.Rotation, 'D');
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
				Air newAir = new Air(content.Load<Texture2D>("Sprites/AnimatedAir64x48_2"), 0, 32, 48, 1, spriteBatch, new Vector2(cannon.Position.X - 38, cannon.Position.Y - 2), cannon.Rotation, 'Z');
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
				Air newAir = new Air(content.Load<Texture2D>("Sprites/AnimatedAir64x48_2"), 0, 32, 48, 1, spriteBatch, new Vector2(cannon.Position.X - 16, cannon.Position.Y + 8), cannon.Rotation, 'X');
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
				Air newAir = new Air(content.Load<Texture2D>("Sprites/AnimatedAir64x48_2"), 0, 32, 48, 1, spriteBatch, new Vector2(cannon.Position.X + 6, cannon.Position.Y - 2), cannon.Rotation, 'C');
				Air.allAirs.Add(newAir);
			}
		}

		/// <summary>
		/// Turns off a specific set of Air Cannons, depending on which set is on.
		/// </summary>
		/// <param name="tileCharacterCurrentlyCollidingWith">The tile that the Character is currently colliding with.</param>
		/// <param name="tileCharacterPreviouslyCollidedWith">The tile that the Character previously collided with.</param>
		public static void turnOffAirCannonsIfPossible(Tile tileCharacterCurrentlyCollidingWith, Tile tileCharacterPreviouslyCollidedWith, Guy guy, Shoes shoes)
		{
			if ((tileCharacterCurrentlyCollidingWith != null && tileCharacterPreviouslyCollidedWith != null) && !tileCharacterCurrentlyCollidingWith.IsAirCannonSwitch && tileCharacterPreviouslyCollidedWith.IsAirCannonSwitch)
			{
				if (guy != null && shoes == null)
				{
					guy.airsGuyHasCollidedWith.Clear();
				}
				else if (shoes != null && guy == null)
				{
					shoes.airsShoesHasCollidedWith.Clear();
				}

				if (Air.areQCannonsOn)
				{
					Air.areQCannonsOn = false;
					Air.turnOffSpecificSetOfCannons('Q');
				}

				if (Air.areWCannonsOn)
				{
					Air.areWCannonsOn = false;
					Air.turnOffSpecificSetOfCannons('W');
				}

				if (Air.areECannonsOn)
				{
					Air.areECannonsOn = false;
					Air.turnOffSpecificSetOfCannons('E');
				}

				if (Air.areACannonsOn)
				{
					Air.areACannonsOn = false;
					Air.turnOffSpecificSetOfCannons('A');
				}

				if (Air.areDCannonsOn)
				{
					Air.areDCannonsOn = false;
					Air.turnOffSpecificSetOfCannons('D');
				}

				if (Air.areZCannonsOn)
				{
					Air.areZCannonsOn = false;
					Air.turnOffSpecificSetOfCannons('Z');
				}

				if (Air.areXCannonsOn)
				{
					Air.areXCannonsOn = false;
					Air.turnOffSpecificSetOfCannons('X');
				}

				if (Air.areCCannonsOn)
				{
					Air.areCCannonsOn = false;
					Air.turnOffSpecificSetOfCannons('C');
				}
			}
		}

		/// <summary>
		/// Turns off a specific set of Air Cannons, depending on the passed in Air Cannon representation.
		/// </summary>
		/// <param name="cannonRepresentation">A Char representing an Air Cannon.</param>
		public static void turnOffSpecificSetOfCannons(Char cannonRepresentation)
		{
			List<Air> airsToRemove = new List<Air>();

			// Find the Airs to remove.
			foreach (Air air in allAirs)
			{
				if (air.airCannonRepresentation == cannonRepresentation)
				{
					airsToRemove.Add(air);
				}
			}

			// Remove the Airs that were found.
			foreach (Air airToRemove in airsToRemove)
			{
				allAirs.Remove(airToRemove);
			}
		}

		/// <summary>
		/// Resets all of the Air Cannons.
		/// </summary>
		public static void resetAllAirCannons()
		{
			Air.allAirs.Clear();
			Air.allQCannons.Clear();
			Air.allWCannons.Clear();
			Air.allECannons.Clear();
			Air.allACannons.Clear();
			Air.allDCannons.Clear();
			Air.allZCannons.Clear();
			Air.allXCannons.Clear();
			Air.allCCannons.Clear();
			Air.areQCannonsOn = false;
			Air.areWCannonsOn = false;
			Air.areECannonsOn = false;
			Air.areACannonsOn = false;
			Air.areDCannonsOn = false;
			Air.areZCannonsOn = false;
			Air.areXCannonsOn = false;
			Air.areCCannonsOn = false;
		}
	}
}
