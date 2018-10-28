using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Shoeter
{
	class Level
	{
		ContentManager contentManager;

		// This stores our level that is read from the text file.
		List<string> level = new List<string>();

		// The two dimensional array that will store our level in a grid.
		//                                  rows, columns
		public static Tile[,] tiles = new Tile[45, 80];

		// Stores the starting position of the player.
		public static Vector2 playerStartingPosition;

		// Store the number of tiles in a column and row.
		public static int numberOfTileColumns = tiles.GetLength(0);
		public static int numberOfTilesInRow = tiles.GetLength(1);

		string[] lines;

		// This will contain a list of impassable tiles.
		public static List<Rectangle> impassableTileRecs;
		public static List<Vector2> impassableTilePos;
		public Rectangle goalRectangle;

		// Starts at 0.
		public int currentLevel = -1;	// -1 is Level 1 (the first level)
		int totalLevels = 4;			// 0 represents Level 1. 6 is Demo 2.

		public string debug;

		/// <summary>
		/// Constructor for the Level class.
		/// </summary>
		/// <param name="content">Run-time component which loads managed objects from the binary files produced by the design time content pipeline.</param>
		public Level(ContentManager content)
		{
			contentManager = content;
		}

		/// <summary>
		/// Loads the next Level.
		/// </summary>
		public void LoadLevel()
		{
			impassableTileRecs = new List<Rectangle>();
			impassableTilePos = new List<Vector2>();
			Air.resetAllAirCannons();

			if (currentLevel + 1 <= totalLevels)
			{
				currentLevel++;
			}
			else
			{
				currentLevel = 0;
			}

			// This array will store a level. Each element in the array will be
			//  a line of tiles.
			// 0 - ****
			// 1 - ****
			// 2 - ****
			// ...

			// The problem is lines is only getting 45
			// Only goes to 65
			lines = System.IO.File.ReadAllLines("Content/Levels/" + currentLevel.ToString() + ".txt");

			int length = lines.Length;

			// The outer loop goes through the columns.
			for (int column = 0; column < numberOfTileColumns; column++)
			{
				// The inner loop goes through the rows.
				for (int row = 0; row < numberOfTilesInRow; row++)
				{
					// The first position of the block is (0, 0), followed by (64, 0), (128, 0), etc.
					// Going up the y axis works the same way. (0, 0), (0, 64), (0, 128), etc.

					// Create a new tile in each position of the grid. We give Tile() the character in the string 'lines'.
					tiles[column, row] = new Tile(lines[column][row], ref contentManager);

					// Set the position of the tile in the Level array.
					tiles[column, row].PositionInArray = new Vector2(column, row);

					// Set the position of a new tile.
					tiles[column, row].Position = new Vector2((row * 16), (column * 16));

					// Draw a rectangle around the tile. We need this for collision detection.
					tiles[column, row].SourceRect = new Rectangle((int)tiles[column, row].Position.X, (int)tiles[column, row].Position.Y, 16, 16);

					// Set the rotation and center for the tile.
					if (tiles[column, row].IsLauncher || tiles[column, row].IsAirCannon || tiles[column, row].IsAirCannonSwitch)
					{
						tiles[column, row].Center = new Vector2(16 / 2, 16 / 2);
						tiles[column, row].Rotation = Tile.getRotationInRadians(tiles[column, row]);
					}
					else
					{
						tiles[column, row].Center = new Vector2(0, 0);
					}

					// We are going to store all the impassable tiles rectangles in a list. We will use this for collision detection.
					if (tiles[column, row].CollProperties == Tile.CollisionProperty.Impassable)
					{
						impassableTileRecs.Add(tiles[column, row].SourceRect);
						impassableTilePos.Add(tiles[column, row].Position);
					}

					if (tiles[column, row].TileRepresentation == 'G' || tiles[column, row].TileRepresentation == 'g')
					{
						goalRectangle = tiles[column, row].SourceRect;
					}
					if (tiles[column, row].TileRepresentation == 'P')
					{
						playerStartingPosition = tiles[column, row].Position;
					}
				}
			}
		}

		/// <summary>
		/// Draws the level.
		/// </summary>
		/// <param name="sb">Enables a group of sprites to be drawn using the same settings.</param>
		public void Draw(SpriteBatch sb, Boolean onlyDrawGoal)
		{
			// The outer loop goes through the columns.
			for (int column = 0; column < numberOfTileColumns; column++)
			{
				// The inner loop goes through the rows.
				for (int row = 0; row < numberOfTilesInRow; row++)
				{
					if (onlyDrawGoal && tiles[column, row].TileRepresentation == 'G')
					{
						sb.Draw(tiles[column, row].Texture, tiles[column, row].Position, null, Color.White, tiles[column, row].Rotation, tiles[column, row].Center, 1f, SpriteEffects.None, 0);
					}
					else if(onlyDrawGoal == false)
					{
						sb.Draw(tiles[column, row].Texture, tiles[column, row].Position, null, Color.White, tiles[column, row].Rotation, tiles[column, row].Center, 1f, SpriteEffects.None, 0);
					}
				}
			}
		}
		public void drawGoal(SpriteBatch sb)
		{
			Tile goalSandwich = new Tile('G', ref contentManager);
			sb.Draw(goalSandwich.Texture, goalSandwich.Position, null, Color.White, goalSandwich.Rotation, goalSandwich.Center, 1f, SpriteEffects.None, 0);
		}

		/// <summary>
		/// Gets the starting position of the Guy/Shoes in the Level.
		/// </summary>
		/// <returns>A Vector2 representing the coordinates of the starting position in the level.</returns>
		public Vector2 getPlayerStartingPosition()
		{
			return playerStartingPosition;
		}

		/// <summary>
		/// Gets the position in the Level array of the Tile that corresponds to the position parameters.
		/// </summary>
		/// <param name="positionX">The X coordinate of the Tile being searched for.</param>
		/// <param name="positionY">The Y coordinate of the Tile being searched for.</param>
		/// <returns></returns>
		public static Vector2 getTilePositionInArray(int positionX, int positionY)
		{
			Vector2 arrayPosition = new Vector2();
			Vector2 tilePosition = new Vector2(positionY, positionX);

			for (int column = 0; column < numberOfTileColumns; column++)
			{
				for (int row = 0; row < numberOfTileColumns; row++)
				{
					if (Level.tiles[column, row].Position == tilePosition)
					{
						arrayPosition = new Vector2(row, column);
					}
				}
			}

			return arrayPosition;
		}

		public void drawBackground(SpriteBatch spriteBatch, ref ContentManager content)
		{
			if (currentLevel == 0)
			{
				spriteBatch.Draw(content.Load<Texture2D>("Levels/HillbillyWorld_Background"), new Vector2(0f, 0f), Color.White);
			}
			else if (currentLevel == 1)
			{
				spriteBatch.Draw(content.Load<Texture2D>("Levels/HumanZoo_Background"), new Vector2(0f, 0f), Color.White);
			}
			else if (currentLevel == 2)
			{
				spriteBatch.Draw(content.Load<Texture2D>("Levels/SewerWorld_Background"), new Vector2(0f, 0f), Color.White);
			}
			else if (currentLevel == 3)
			{
				spriteBatch.Draw(content.Load<Texture2D>("Levels/HumanSlaughterHouse_Background"), new Vector2(0f, 0f), Color.White);
			}
			else if (currentLevel == 4)
			{
				spriteBatch.Draw(content.Load<Texture2D>("Levels/Carnival_Background"), new Vector2(0f, 0f), Color.White);
			}
		}

		public void drawForeground(SpriteBatch spriteBatch, ref ContentManager content)
		{
			if (currentLevel == 0)
			{
				spriteBatch.Draw(content.Load<Texture2D>("Levels/HillbillyWorld_Foreground"), new Vector2(0f, 0f), Color.White);
			}
			else if (currentLevel == 1)
			{
				spriteBatch.Draw(content.Load<Texture2D>("Levels/HumanZoo_Foreground"), new Vector2(0f, 0f), Color.White);
			}
			else if (currentLevel == 2)
			{
				spriteBatch.Draw(content.Load<Texture2D>("Levels/SewerWorld_Foreground"), new Vector2(0f, 0f), Color.White);
			}
			else if (currentLevel == 3)
			{
				spriteBatch.Draw(content.Load<Texture2D>("Levels/HumanSlaughterHouse_Foreground"), new Vector2(0f, 0f), Color.White);
			}
			else if (currentLevel == 4)
			{
				spriteBatch.Draw(content.Load<Texture2D>("Levels/Carnival_Foreground"), new Vector2(0f, 0f), Color.White);
			}
		}
	}
}
