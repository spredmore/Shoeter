using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace customAnimation
{
    class Level
    {
        //SpriteBatch spriteBatch;
        ContentManager content;
        
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
        int currentLevel = 0;
        int totalLevels = 3;

        public string debug;

        public Level(ContentManager content)
        {
            this.content = content;
            //lines = new string[numberOfTileColumns];
        }

        public void LoadLevel()
        {
            impassableTileRecs = new List<Rectangle>();
            impassableTilePos = new List<Vector2>();

            if (currentLevel + 1 <= totalLevels) currentLevel++;
            else currentLevel = 1;

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

            debug = numberOfTilesInRow.ToString();

            // The outer loop goes through the columns.
            for (int column = 0; column < numberOfTileColumns; column++)
            {
                // The inner loop goes through the rows.
                for (int row = 0; row < numberOfTilesInRow; row++)
                {
                    // Create a new tile in each position of the grid. We give Tile() the character in the string 'lines'.
                    tiles[column, row] = new Tile(lines[column][row], ref content);

                    // Set the position of a new tile.
                    // The first position of the block is (0, 0), followed by (64, 0), (128, 0), etc.
                    // Going up the y axis works the same way. (0, 0), (0, 64), (0, 128), etc.
                    //tiles[column, row].Position = new Vector2((row * 16), (column * 16));

                    // Draw a rectangle around the tile. We need this for collision detection.
                    tiles[column, row].Position = new Vector2((row * 16), (column * 16));
                    tiles[column, row].SourceRect = new Rectangle((int)tiles[column, row].Position.X, (int)tiles[column, row].Position.Y, 16, 16);
                    
                    // We are going to store all the impassable tiles rectangles in a list. We will use this for collision detection.
                    if (tiles[column, row].CollProperties == Tile.CollisionProperty.Impassable)
                    {
                        impassableTileRecs.Add(tiles[column, row].SourceRect);
                        impassableTilePos.Add(tiles[column, row].Position);
                    }

                    if (tiles[column, row].TileRepresentation == 'G') goalRectangle = tiles[column, row].SourceRect;
                    if (tiles[column, row].TileRepresentation == 'P') playerStartingPosition = tiles[column, row].Position;
                }
            }   
        }

        public void Draw(SpriteBatch sb)
        {   
            // The outer loop goes through the columns.
            for (int column = 0; column < numberOfTileColumns; column++)
            {
                // The inner loop goes through the rows.
                for (int row = 0; row < numberOfTilesInRow; row++)
                {
                    sb.Draw(tiles[column, row].Texture, tiles[column, row].Position, Color.White);
                }
            }
        }

        public void debugFunc(SpriteBatch sb)
        {
            sb.DrawString(Game1.debugFont, numberOfTileColumns.ToString(), new Vector2(0, 80), Color.White);
        }

        public Vector2 getPlayerStartingPosition()
        {
            return playerStartingPosition;
        }
    }
}
