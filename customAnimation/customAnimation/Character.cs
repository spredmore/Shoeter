using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace customAnimation
{
    public abstract class Character
    {
        protected SpriteBatch spriteBatch;
        protected GameTime gameTime;

        // Animation
        Rectangle sourceRect;   // The rectangle in which the animated sprite will be drawn.
        float timer = 0f;       // The amount of time it takes before the sprite moves to the next frame.
        float interval = 100f;  // The amount of time a frame is shown on screen.
        protected int currentFrame = 0;   // The current frame we are drawing.
        protected int totalFrames = 0;    // Stores the total amount of frames in the sprite sheet.

        // Character information
        protected Texture2D spriteTexture;      // The image of our animated sprite.
        protected Vector2 position;             // The position of the animated sprite.
        Vector2 center;                         // The center of the animated sprite.
        protected Vector2 velocity;             // The velocity that changes the player's position.
        SpriteEffects facingRight;              // Stores if the player is facing right or not.
        protected bool falling = false;         // Stores if the player is falling or not.
        protected bool isJumping = false;       // Stores if we're jumping or not.

        public float gravity;                   // Gravity CHANGE BACK TO PROTECTED LATER

        protected float spriteSpeed = 600f;     // This is how fast the sprite moves.
        protected int spriteWidth;              // The width of the individual sprite.
        protected int spriteHeight;             // The height of the individual sprite.

        // State
        public State state;         // The current state of the player.
        public State oldState;      // The old state of the player.
        public Mode currentMode;    // The current mode of the player.

        // Collision
        Rectangle futurePositionRec;    // The player's future position rectangle.
        Rectangle positionRect;         // The rectangle around the player.
        Rectangle tileCollRect;         // The current tile's position the player is colliding with.
        Vector2 tileArrayCoordinates;   // The current tile's coordinates into the level array.

        // Window Information
        protected int screenHeight;
        protected int screenWidth;

        public static string charDebug;

        // The position of the animated sprite.
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        // The center of the sprite.
        public Vector2 Center
        {
            get { return center; }
            set { center = value; }
        }

        // The image (sprite sheet) of the animated sprite.
        public Texture2D Texture
        {
            get { return spriteTexture; }
            set { spriteTexture = value; }
        }

        // The rectangle we'll draw around the sprite.
        public Rectangle SourceRect
        {
            get { return sourceRect; }
            set { sourceRect = value; }
        }

        public Rectangle PositionRect
        {
            get { return positionRect; }
            set { positionRect = value; }
        }

        public Rectangle TileCollisionRectangle
        {
            get { return tileCollRect; }
            set { tileCollRect = value; }
        }

        public Rectangle FutureRectangleRect
        {
            get { return futurePositionRec; }
            set { futurePositionRec = value; }
        }

        public Vector2 TileArrayCoordinates
        {
            get { return tileArrayCoordinates; }
            set { tileArrayCoordinates = value; }
        }

        // These are the states the player can be in. 
        // Possible states: Idle, RunningRight, RunningLeft, Jumping
        public enum State
        {
            Idle = 0,
            RunningRight = 1,
            RunningLeft = 2,
            Jumping = 3,
            Decending = 4,
        }

        public State PlayerState
        {
            get { return state; }
            set { state = value; }
        }

        // These are the modes the player can be in.
        // The guy moves slow, and can't jump high.
        // Boots can move fast, and can jump high.
        public enum Mode
        {
            Guy = 0,
            Shoes = 1,
        }

        public Mode PlayerMode
        {
            get { return currentMode; }
            set { currentMode = value; }
        }

        // Stores if the sprite is facing right or not.
        public SpriteEffects FacingRight
        {
            get { return facingRight; }
            set { facingRight = value; }
        }

        // Stores the total amount of frames in the current sprite sheet.
        public int TotalFrames
        {
            get { return totalFrames; }
            set { totalFrames = value; }
        }

        public Character() { charDebug = ""; }

        // Animates the character.
        protected void handleAnimation(GameTime gameTime)
        {
            // Get a rectangle around the current frame we're on.
            sourceRect = new Rectangle(currentFrame * spriteWidth, 0, spriteWidth, spriteHeight);

            // Increment the timer to see if we need to move to the next frame.
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Check to see if we need to move to the next frame.
            if (timer > interval)
            {
                // Check to see if we can move to the next frame.
                if (currentFrame < totalFrames)
                {
                    // Move to the next frame.
                    currentFrame++;
                }
                else
                {
                    // We reached the end of the sprite sheet. Reset to the beginning.
                    currentFrame = 0;
                }

                // Reset the timer.
                timer = 0f;
            }

            // Get the center of the current frame.
            // The center will be important, because we will have to do rotations about the center of the sprite.
            center = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);
        }

        // Returns true if there is a tile below us.
        protected bool standingOnGround()
        {
            updateRectangles(0, 1);

            for (int i = 0; i < Level.impassableTileRecs.Count; i++)
            {
                if (futurePositionRec.Intersects(Level.impassableTileRecs[i]))
                {
                    position.Y = Level.impassableTilePos[i].Y - spriteHeight;
                    updateRectangles(0, -1);
                    tileCollRect = Level.impassableTileRecs[i];
                    setTileArrayCoordinates(Level.impassableTilePos[i].X, Level.impassableTilePos[i].Y);

                    charDebug = TileArrayCoordinates.ToString();

                    return true;
                }
            }

            return false;
        }

        // xPosition & yPosition are the coordinates of the tile in the level. 
        // Sets the tile coordinates (between (0-45, 0-80)) of the tile at (xPosition, yPosition)
        protected void setTileArrayCoordinates(float xPosition, float yPosition)
        {
            for (int x = 0; x < Level.numberOfTileColumns; x++)
            {
                for (int y = 0; y < Level.numberOfTilesInRow; y++)
                {
                    if (Level.tiles[x, y].Position.X == xPosition && Level.tiles[x, y].Position.Y == yPosition) TileArrayCoordinates = new Vector2(x, y);
                }
            }
        }

        // Returns true if we are under a tile.
        protected bool underTile()
        {
            updateRectangles(0, -1);

            for (int i = 0; i < Level.impassableTileRecs.Count; i++)
            {
                if (futurePositionRec.Intersects(Level.impassableTileRecs[i]))
                {
                    tileCollRect = Level.impassableTileRecs[i];
                    return true;
                }
            }

            return false;
        }

        // Returns true if there is a tile to the right.
        public bool tileToTheRight()
        {
            updateRectangles(1, 0);

            for (int i = 0; i < Level.impassableTileRecs.Count; i++)
            {
                if (futurePositionRec.Intersects(Level.impassableTileRecs[i]))
                {
                    tileCollRect = Level.impassableTileRecs[i];
                    return true;
                }
            }

            return false;
        }

        // Returns true if there is a tile to the left.
        public bool tileToTheLeft()
        {
            updateRectangles(-1, 0);

            for (int i = 0; i < Level.impassableTileRecs.Count; i++)
            {
                if (futurePositionRec.Intersects(Level.impassableTileRecs[i]))
                {
                    tileCollRect = Level.impassableTileRecs[i];
                    return true;
                }
            }

            return false;
        }

        // Update the player's rectangles depending on where we want to look
        protected void updateRectangles(int xOffset, int yOffset)
        {
            positionRect = new Rectangle((int)position.X, (int)position.Y, spriteWidth, spriteHeight);
            futurePositionRec = new Rectangle((int)position.X + xOffset, (int)position.Y + yOffset, spriteWidth, spriteHeight);
        }

        // Change the state of the player if the state has changed.
        protected void changeState(State newState)
        {
            // Only change the state if it's a new state.
            if (newState != this.state)
            {
                oldState = this.state;
                this.state = newState;
            }
        }

        // This function checks to see if a future move will collide with a tile.
        // Supplies the state, and the coordinates of the collided tile.
        protected void handleCollisions(State potentialState)
        {
            int leftTile = (int)Math.Floor((float)positionRect.Left / Level.impassableTileRecs[0].Width);
            int rightTile = (int)Math.Ceiling(((float)positionRect.Right / Level.impassableTileRecs[0].Width)) - 1;
            int topTile = (int)Math.Floor((float)positionRect.Top / Level.impassableTileRecs[0].Height);
            int bottomTile = (int)Math.Ceiling(((float)positionRect.Bottom / Level.impassableTileRecs[0].Height)) - 1;

            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {   
                    // Keeps the player in bounds of the screen.
                    if (x < 0) 
                    {
                        velocity.X = 0.0f;
                        position.X = 0f;
                    }
                    else if (x > 79)
                    {
                        position.X = screenWidth - spriteWidth;
                        velocity.X = 0f;
                    }
                    else if (y < 0)
                    {
                        position.Y = 0f;
                        velocity.Y = 0f;
                    }
                    else if (y > 44)
                    {
                        //velocity.Y = 0f;
                    }
                    else if (futurePositionRec.Intersects(Level.tiles[y, x].SourceRect) && Level.tiles[y, x].CollProperties == Tile.CollisionProperty.Impassable)
                    {
                        if (potentialState == State.RunningRight)
                        {
                            specializedCollision(potentialState, y, x);
                            updateRectangles(1, 0);
                            tileCollRect = Level.tiles[y, x].SourceRect;
                        }
                        else if (potentialState == State.RunningLeft)
                        {
                            specializedCollision(potentialState, y, x);
                            updateRectangles(-1, 0);
                            tileCollRect = Level.tiles[y, x].SourceRect;
                        }
                        else if (potentialState == State.Jumping)
                        {
                            specializedCollision(potentialState, y, x);
                            updateRectangles(0, 1);
                            tileCollRect = Level.tiles[y, x].SourceRect;
                        }
                        else if (potentialState == State.Decending)
                        {
                            specializedCollision(potentialState, y, x);
                            updateRectangles(0, -1);
                            tileCollRect = Level.tiles[y, x].SourceRect;
                        }
                    }
                }
            }
        }
        
        // Draw the sprite.
        public void Draw()
        {
            spriteBatch.Draw(Texture, Position, SourceRect, Color.White, 0f, new Vector2(0, 0), 1.0f, FacingRight, 0);
        }

        // This function contains specialized collision code for a particular Character.
        // Says what to do in the event of a particular collision.
        protected abstract void specializedCollision(State currentState, int y, int x);
    }
}
