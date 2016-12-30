using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace customAnimation
{
	public class Tile
	{
		// What we need to do:
		// Load a tile from a .txt file into an element of an array
		// Example: level[0][0] = new Tile(parameter, parameter)
		// The tile needs to have the following properties:
		//  - Collision Detection types (Enum)
		//  - Position
		//  - Center or Reference Point
		//  - Texture

		ContentManager content;
		Texture2D texture;
		CollisionProperty collisionProperty;
		char tileRepresentation;
		Rectangle sourceRect;		// The rectangle in which the animated sprite will be drawn.
		Vector2 position;			// Stores the position of a tile.
		Single rotation;			// Stores the current rotation for the tile.
		Vector2 center;             // Stores the center of the tile.
		Boolean isLauncher = false;
		Boolean isAirCannon = false;
		Boolean isAirCannonSwitch = false;
		Boolean isAirCannonSwitchOn = false;
		Vector2 positionInArray;

		public static string debug;

		/// <summary>
		/// Defines a type of Tile.
		/// </summary>
		public enum CollisionProperty
		{
			// A passable tile is one which does not hinder player motion at all.
			Passable = 0,

			// An impassable tile is one which does not allow the player to move through it at all. It is completely solid.
			Impassable = 1,

			// A platform tile is one which behaves like a passable tile except when the player is above it. 
			// A player can jump up through a platform as well as move past it to the left and right, but can not fall down through the top of it.
			Platform = 2,
		}

		/// <summary>
		/// Property representing the Texture of a Tile.
		/// </summary>
		public Texture2D Texture
		{
			get { return texture; }
			set { texture = value; }
		}

		/// <summary>
		/// Property representing the collision property of a Tile.
		/// </summary>
		public CollisionProperty CollProperties
		{
			get { return collisionProperty; }
			set { collisionProperty = value; }
		}

		/// <summary>
		/// Property representing the representation of a Tile in the Level text file.
		/// </summary>
		public char TileRepresentation
		{
			get { return tileRepresentation; }
			set { tileRepresentation = value; }
		}

		/// <summary>
		/// Property representing the actual Position of a Tile in the Level (not array).
		/// </summary>
		public Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}

		/// <summary>
		/// Property representing the Rotation of a Tile.
		/// </summary>
		public Single Rotation
		{
			get { return rotation; }
			set { rotation = value; }
		}

		/// <summary>
		/// Property representing the center of a Tile.
		/// </summary>
		public Vector2 Center
		{
			get { return center; }
			set { center = value; }
		}

		/// <summary>
		/// Property representing a Rectangle around the Tile.
		/// </summary>
		public Rectangle SourceRect
		{
			get { return sourceRect; }
			set { sourceRect = value; }
		}

		/// <summary>
		/// Property representing if the Tile is a Launcher or not.
		/// </summary>
		public Boolean IsLauncher
		{
			get { return isLauncher; }
			set { isLauncher = value; }
		}

		/// <summary>
		/// Property representing if the Tile is an Air Cannon or not.
		/// </summary>
		public Boolean IsAirCannon
		{
			get { return isAirCannon; }
			set { isAirCannon = value; }
		}

		/// <summary>
		/// Property representing if the Tile is an Air Cannon Switch or not.
		/// </summary>
		public Boolean IsAirCannonSwitch
		{
			get { return isAirCannonSwitch; }
			set { isAirCannonSwitch = value; }
		}

		/// <summary>
		/// Property representing if a Tile Switch is on or not.
		/// </summary>
		public Boolean IsAirCannonSwitchOn
		{
			get { return isAirCannonSwitchOn; }
			set { isAirCannonSwitchOn = value; }
		}

		/// <summary>
		/// Property representing the Position of the Tile in the Level array.
		/// </summary>
		public Vector2 PositionInArray
		{
			get { return positionInArray; }
			set { positionInArray = value; }
		}

		/// <summary>
		/// Constructor for the Tile character.
		/// </summary>
		/// <param name="tileRepresentation">Represents what kind of Tile this Tile is.</param>
		/// <param name="content">Run-time component which loads managed objects from the binary files produced by the design time content pipeline.</param>
		public Tile(char tileRepresentation, ref ContentManager content)
		{
			// Set the appropriate tile, dependent on what character is is.
			this.tileRepresentation = tileRepresentation;

			// Set the tile's origin point. Unless the tile is a launcher, the origin is the top left corner.
			this.center.X = 0;
			this.center.Y = 0;

			// Load the Game1's ContentManager, so we can draw outside of Game1.cs.
			this.content = content;

			// Load the new tile.
			LoadTile();
		}

		/// <summary>
		/// Load a new tile and gives it a texture and collision property.
		/// </summary>
		public void LoadTile()
		{        
			// 16 x 16 red block
			if (tileRepresentation == '*')
			{
				// Load the texture. We use Game1's ContentManager here.
				texture = content.Load<Texture2D>("Tiles/test1");

				// Set the collision property of a block.
				collisionProperty = CollisionProperty.Impassable;                
			}

			// Transparent Block.
			else if (tileRepresentation == 't')
			{
				texture = content.Load<Texture2D>("Tiles/TransparentBlock");
				collisionProperty = CollisionProperty.Passable;
			}

			// Player Block. This will be used to get the starting position of the player.
			else if (tileRepresentation == 'P')
			{
				texture = content.Load<Texture2D>("Tiles/TransparentBlock");
				collisionProperty = CollisionProperty.Passable;
			}

			else if (tileRepresentation == 'G')
			{
				texture = content.Load<Texture2D>("Tiles/Goal-16");
				collisionProperty = CollisionProperty.Passable;
			}
			else if (tileRepresentation == 'S')
			{
				texture = content.Load<Texture2D>("Tiles/spring");
				collisionProperty = CollisionProperty.Impassable;
			}
			else if (tileRepresentation == '1' || tileRepresentation == '2' || tileRepresentation == '3' || tileRepresentation == '4' || tileRepresentation == '6' || tileRepresentation == '7' || tileRepresentation == '8' || tileRepresentation == '9')
			{
				texture = content.Load<Texture2D>("Tiles/launcher_0");
				collisionProperty = CollisionProperty.Impassable;
				isLauncher = true;
			}
			else if (tileRepresentation == 'z' || tileRepresentation == 'x' || tileRepresentation == 'c' || tileRepresentation == 'a' || tileRepresentation == 'd' || tileRepresentation == 'q' || tileRepresentation == 'w' || tileRepresentation == 'e')
			{
				texture = content.Load<Texture2D>("Tiles/AirCannon");
				collisionProperty = CollisionProperty.Passable;
				isAirCannon = true;
				putCannonIntoCorrectList(this);
			}
			else if (tileRepresentation == 'Z' || tileRepresentation == 'X' || tileRepresentation == 'C' || tileRepresentation == 'A' || tileRepresentation == 'D' || tileRepresentation == 'Q' || tileRepresentation == 'W' || tileRepresentation == 'E')
			{
				texture = content.Load<Texture2D>("Tiles/AirCannonSwitch");
				collisionProperty = CollisionProperty.Passable;
				isAirCannonSwitch = true;
			}
		}

		/// <summary>
		/// Gets the angle of a Launcher in radians.
		/// </summary>
		/// <param name="tile"></param>
		/// <returns>The angle in radians of the Tile.</returns>
		public static Single getRotationInRadians(Tile tile)
		{
			Single rotationInRadians;
			float rotationInDegrees = getAngleInDegrees(tile) - 90; // It's negative so it points the correct direction.    

			// Now that the center point has been set, set the Tile's rotation to the angle specified by its representation.
			rotationInRadians = MathHelper.ToRadians(rotationInDegrees);

			return rotationInRadians;
		}

		/// <summary>
		/// Gets the angle of a Tile in degrees.
		/// </summary>
		/// <param name="tile"></param>
		/// <returns>The angle at which the Launcher should launch the Guy/Shoes, depending on which Launcher is passed in.</returns>
		public static int getAngleInDegrees(Tile tile)
		{
			if (tile.TileRepresentation == '1' || tile.TileRepresentation == '!' || tile.TileRepresentation == 'z' || tile.TileRepresentation == 'Z') return 315;		// Down Left
			else if (tile.TileRepresentation == '2' || tile.TileRepresentation == '@' || tile.TileRepresentation == 'x' || tile.TileRepresentation == 'X') return 270;	// Down
			else if (tile.TileRepresentation == '3' || tile.TileRepresentation == '#' || tile.TileRepresentation == 'c' || tile.TileRepresentation == 'C') return 225;	// Down Right
			else if (tile.TileRepresentation == '4' || tile.TileRepresentation == '$' || tile.TileRepresentation == 'a' || tile.TileRepresentation == 'A') return 0;	// Left
			else if (tile.TileRepresentation == '6' || tile.TileRepresentation == '^' || tile.TileRepresentation == 'd' || tile.TileRepresentation == 'D') return 180;	// Right
			else if (tile.TileRepresentation == '7' || tile.TileRepresentation == '&' || tile.TileRepresentation == 'q' || tile.TileRepresentation == 'Q') return 45;	// Up Left
			else if (tile.TileRepresentation == '8' || tile.TileRepresentation == '*' || tile.TileRepresentation == 'w' || tile.TileRepresentation == 'W') return 90;	// Up
			else if (tile.TileRepresentation == '9' || tile.TileRepresentation == '(' || tile.TileRepresentation == 'e' || tile.TileRepresentation == 'E') return 135;	// Up Right
			else return -1;
		}

		/// <summary>
		/// When a Cannon is loaded into the game, put it into the correct list so that only the active Cannons need checked against.
		/// </summary>
		/// <param name="cannon">A cannon.</param>
		private static void putCannonIntoCorrectList(Tile cannon)
		{
			if (cannon.TileRepresentation == 'q')
			{
				Air.allQCannons.Add(cannon);
			}
			else if (cannon.TileRepresentation == 'w')
			{
				Air.allWCannons.Add(cannon);
			}
			else if (cannon.TileRepresentation == 'e')
			{
				Air.allECannons.Add(cannon);
			}
			else if (cannon.TileRepresentation == 'a')
			{
				Air.allACannons.Add(cannon);
			}
			else if (cannon.TileRepresentation == 'd')
			{
				Air.allDCannons.Add(cannon);
			}
			else if (cannon.TileRepresentation == 'z')
			{
				Air.allZCannons.Add(cannon);
			}
			else if (cannon.TileRepresentation == 'x')
			{
				Air.allXCannons.Add(cannon);
			}
			else if (cannon.TileRepresentation == 'c')
			{
				Air.allCCannons.Add(cannon);
			}
		}
	}
}
