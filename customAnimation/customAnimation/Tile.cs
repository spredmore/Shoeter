using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace customAnimation
{
    class Tile
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
        Rectangle sourceRect;           // The rectangle in which the animated sprite will be drawn.
        Vector2 position;               // Stores the position of a tile.

        public static string debug;

        public enum CollisionProperty
        {
            // A passable tile is one which does not hinder player motion at all.
            Passable = 0,

            // An impassable tile is one which does not allow the player to move through
            // it at all. It is completely solid.
            Impassable = 1,

            // A platform tile is one which behaves like a passable tile except when the
            // player is above it. A player can jump up through a platform as well as move
            // past it to the left and right, but can not fall down through the top of it.
            Platform = 2,
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public CollisionProperty CollProperties
        {
            get { return collisionProperty; }
            set { collisionProperty = value; }
        }

        public char TileRepresentation
        {
            get { return tileRepresentation; }
            set { tileRepresentation = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Rectangle SourceRect
        {
            get { return sourceRect; }
            set { sourceRect = value; }
        }

        public Tile(char tileRepresentation, ref ContentManager content)
        {
            // Set the appropriate tile, dependent on what character is is.
            this.tileRepresentation = tileRepresentation;

            // Load the Game1's ContentManager, so we can draw outside of Game1.cs.
            this.content = content;

            // Load the new tile.
            LoadTile();
        }

        // Load new tiles, and give them the texture and collision property.
        public void LoadTile()
        {        
            // 16 x 16 red block
            if (tileRepresentation == '*')
            {
                // Load the texture. We use Game1's ContentManager here.
                this.texture = content.Load<Texture2D>("Tiles/test1");

                // Set the collision property of a block.
                this.collisionProperty = CollisionProperty.Impassable;                
            }

            // Transparent Block.
            else if (tileRepresentation == 't')
            {
                this.texture = content.Load<Texture2D>("Tiles/TransparentBlock");
                this.collisionProperty = CollisionProperty.Passable;
            }

            // Player Block. This will be used to get the starting position of the player.
            else if (tileRepresentation == 'P')
            {
                this.texture = content.Load<Texture2D>("Tiles/TransparentBlock");
                this.collisionProperty = CollisionProperty.Passable;
            }

            else if (tileRepresentation == 'G')
            {
                this.texture = content.Load<Texture2D>("Tiles/Goal-16");
                this.collisionProperty = CollisionProperty.Passable;
            }
            else if (tileRepresentation == 'S')
            {
                this.texture = content.Load<Texture2D>("Tiles/spring");
                this.collisionProperty = CollisionProperty.Impassable;
            }
        }        
    }
}
