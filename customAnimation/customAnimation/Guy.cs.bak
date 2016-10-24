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
    class Guy : Character
    {
        KeyboardState newKeyboardState;
        KeyboardState oldKeyboardState;
        MouseState newMouseState;
        MouseState oldMouseState;

        public Vector2 mousePosition;
        Vector2 oldPosition;    // Used to determine direction of travel.
        int currentScrollValue;
        int oldScrollValue;

        public bool delayCollision = true;  // We need to let the Guy travel a little bit before Shoes can catch him.
        float delayCollisionInterval = 0.5f;  // Amount of time needed to elapse before the Guy and Shoes collide. Measured in milliseconds.
        int counter = 1;
        int limit = 1;
        float currentTime = 0f;

        public float angle = 90;
        public float angleBetweenPlayer;
        public float power = 5f;

        bool useGravity;
        public bool linked = true;
        public bool beingShot = false;
        
        float delta;

        public string debug;
        public string debug2;

        Level currentLevel;
        public int collX;
        public int collY;

        public Guy(Texture2D texture, SpriteBatch spriteBatch, int currentFrame, int totalFrames, int spriteWidth, int spriteHeight, int screenHeight, int screenWidth)
        {
            this.spriteBatch = spriteBatch;
            this.Texture = texture;
            this.currentFrame = currentFrame;
            this.totalFrames = totalFrames;
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;
            this.screenHeight = screenHeight;
            this.screenWidth = screenWidth;

            gravity = 10f;
            debug = "";
            debug2 = "";
            collX = 0;
            collY = 0;

            PlayerMode = Mode.Guy;
        }

        public void Update(GameTime gameTime, ref Shoes shoes, ref Level level)
        {
            currentLevel = level;
            this.handleAnimation(gameTime);
            handleMovement(gameTime, ref shoes);
        }

        private void handleMovement(GameTime gameTime, ref Shoes shoes)
        {
            newKeyboardState = Keyboard.GetState();
            newMouseState = Mouse.GetState();
            mousePosition.X = newMouseState.X;
            mousePosition.Y = newMouseState.Y;
            currentScrollValue = newMouseState.ScrollWheelValue;
            delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            angleBetweenPlayer = MathHelper.ToDegrees((float)(Math.Atan2(shoes.Position.Y - mousePosition.Y, shoes.Position.X + 26 - mousePosition.X))); // Get the angle between the mouse and the player.

            // ******************************************************
            // Handles delaying the collision between the guy and shoes so that they don't link back together too quickly.
            // ******************************************************
            if (currentTime >= delayCollisionInterval)
            {
                counter++;
                currentTime -= delayCollisionInterval; // "use up" the time
            }
            if (counter >= limit)
            {
                counter = 0; //Reset the counter;
                delayCollision = false;
            }

            // ******************************************************
            // Set the position to the player's position so it follows him around.
            // ******************************************************
            if (beingShot == false)
            {
                Position = new Vector2(shoes.Position.X, shoes.Position.Y);
                linked = true;
            }

            // ******************************************************
            // The player clicks the mouse and wants to shoot the guy.
            // ******************************************************
            if (newMouseState.LeftButton == ButtonState.Pressed && beingShot == false)
            {
                currentTime = 0;
                beingShot = true;
                useGravity = true;
                delayCollision = true;
                velocity = Vector2FromAngle(MathHelper.ToRadians(angleBetweenPlayer)) * power;
                velocity *= -1;
                linked = false;
                shoes.swapTexture(linked); // Changes the texture/size of the shoes because the Guy is being shot.
            }

            // ******************************************************
            // Reset the Guy to the Shoes' position.
            // ******************************************************
            if (!(newMouseState.RightButton == ButtonState.Pressed) && oldMouseState.RightButton == ButtonState.Pressed && linked == false)
            {                
                velocity = new Vector2(0f, 0f);
                beingShot = false;
                linked = true;
                shoes.swapTexture(linked);
                Position = new Vector2(shoes.Position.X, shoes.Position.Y);
            }

            // ******************************************************
            // Handle the Guy's movement when he is being shot.
            // ******************************************************
            if (beingShot == true)
            {
                position.X += velocity.X; // SHOES MOVES EACH DIMENSION ONE AT A TIME

                if (useGravity == true) velocity.Y += gravity * delta; // Gravity

                // ******************************************************
                // Determine which direction we're traveling, and check for collisions. 
                // ******************************************************
                if (position.X > oldPosition.X)
                {
                    // Traveling right
                    updateRectangles(1, 0);
                    handleCollisions(State.RunningRight);
                    changeState(State.RunningRight);
                }
                if (position.X < oldPosition.X)
                {
                    // Traveling left
                    updateRectangles(-1, 0);
                    handleCollisions(State.RunningLeft);
                    changeState(State.RunningLeft);
                }

                position.Y += velocity.Y; // SHOES MOVES EACH DIMENSION ONE AT A TIME

                if (position.Y > oldPosition.Y)
                {
                    // Traveling Down
                    updateRectangles(0, 1);
                    handleCollisions(State.Decending);
                    changeState(State.Decending);
                }
                if (position.Y < oldPosition.Y)
                {
                    // Traveling Up
                    updateRectangles(0, -1);
                    handleCollisions(State.Jumping);
                    changeState(State.Jumping);
                }

                // ******************************************************
                // Set the Shoes' position to the Guys' upon collision
                // ******************************************************
                if (PositionRect.Intersects(shoes.PositionRect) && delayCollision == false && linked == false)
                {
                    shoes.Position = new Vector2(Position.X, Position.Y + 40);
                    velocity = new Vector2(0f, 0f);
                    delayCollision = true;
                    beingShot = false;
                    linked = true;
                    shoes.swapTexture(linked);
                }
            }
            else
            {
                // ******************************************************
                // The player has reached the goal for the current level.
                // ******************************************************
                if (PositionRect.Intersects(currentLevel.goalRectangle) && linked == true)
                {
                    currentLevel.LoadLevel();
                    shoes.Position = currentLevel.getPlayerStartingPosition();
                }              
            }

            // Change angle.
            if (currentScrollValue < oldScrollValue && beingShot == false) if (power > 0) power--;
            if (currentScrollValue > oldScrollValue && beingShot == false) if (power < 15f) power++;
            if ((!newKeyboardState.IsKeyDown(Keys.Decimal) && oldKeyboardState.IsKeyDown(Keys.Decimal)) || (!newKeyboardState.IsKeyDown(Keys.Down) && oldKeyboardState.IsKeyDown(Keys.Down))) if (gravity > 0) gravity -= 1f;
            if (!newKeyboardState.IsKeyDown(Keys.NumPad3) && oldKeyboardState.IsKeyDown(Keys.NumPad3) || (!newKeyboardState.IsKeyDown(Keys.Up) && oldKeyboardState.IsKeyDown(Keys.Up))) gravity += 1f;

            oldKeyboardState = newKeyboardState;
            oldMouseState = newMouseState;
            oldPosition = Position;
            oldScrollValue = newMouseState.ScrollWheelValue;
            updateRectangles(0, 0);
        }

        public Vector2 Vector2FromAngle(double angle, bool normalize = true)
        {
            Vector2 vector = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            if (vector != Vector2.Zero && normalize)
                vector.Normalize();
            return vector;
        }

        protected override void specializedCollision(State currentState, int y, int x)
        {
            if (currentState == State.RunningRight)
            {
                position.X = Level.tiles[y, x].Position.X - spriteWidth;

                if (Level.tiles[y, x].TileRepresentation == 'S')
                {
                    doSpring(currentState);
                }
                else
                {
                    velocity.X = 0f;
                    delayCollision = false;
                }
            }
            else if (currentState == State.RunningLeft)
            {
                position.X = Level.tiles[y, x].Position.X + Level.tiles[y, x].Texture.Width + 1;

                if (Level.tiles[y, x].TileRepresentation == 'S')
                {
                    doSpring(currentState);
                }
                else
                {
                    velocity.X = 0f;
                    delayCollision = false;
                }
            }
            else if (currentState == State.Jumping)
            {
                position.Y = Level.tiles[y, x].Position.Y + Level.tiles[y, x].Texture.Height + 2;

                if (Level.tiles[y, x].TileRepresentation == 'S')
                {
                    doSpring(currentState);
                }
                else
                {
                    velocity.Y = 0f;
                }                
            }
            else if (currentState == State.Decending)
            {
                position.Y = Level.tiles[y, x].Position.Y - spriteHeight;

                if (Level.tiles[y, x].TileRepresentation == 'S' && velocity.Y > 1f)
                {
                    doSpring(currentState);
                }
                else
                {
                    velocity = new Vector2(0f, 0f); // So we don't fall through.
                    useGravity = false;
                }
            }
        }

        // Returns true if there is a tile above the Guy.
        public bool tileAbove()
        {
            updateRectangles(0, -1);

            for (int i = 0; i < Level.impassableTileRecs.Count; i++)
            {
                if (this.FutureRectangleRect.Intersects(Level.impassableTileRecs[i]))
                {
                    collX = Level.impassableTileRecs[i].X;
                    collY = Level.impassableTileRecs[i].Y;
                    TileCollisionRectangle = Level.impassableTileRecs[i];
                    return true;
                }
            }

            return false;
        }

        private void doSpring(State currentState)
        {
            // We need to determine which direction the Guy is moving so we can spring him in the correct direction.
            // To do this, we need to compare against the current position and old position.
            // Top of tile: He will be moving left and right, so we need to check the X coordinates.
            // Left of tile: He will be moving up and down, so we need to check the Y coordinates.
            // The reason we pass in the current State is because the Guy's state only switches between Jumping and Descending,
            // because the Y coordinate is updated last when he is getting shot. 
            // In all cases, we need to check the X coordinate because the Guy will always be moving left to right. 
            if (currentState == State.Decending || currentState == State.Jumping)
            {
                velocity.Y *= -1; // Flips the velocity to the player bounces up.
                velocity.Y *= 0.55f; // Decrease the power of the next bounce.
                position.Y += velocity.Y;
            }
            else if (currentState == State.RunningRight || currentState == State.RunningLeft)
            {
                velocity.X *= -1;
                velocity.X *= 0.55f;
                position.X += velocity.X;
            }

            // If the velocity begins to pull the player down, we're falling. 
            if (velocity.Y > 0f) falling = true;
            else falling = false;

            // Depending on which direction the player is moving, we need to check the top or bottom.
            if (falling == true)
            {
                updateRectangles(0, 1);
                handleCollisions(State.Decending);
                changeState(State.Decending);
                debug = "FUCK";
            }
            else
            {
                updateRectangles(0, -1);
                handleCollisions(State.Jumping);
                changeState(State.Jumping);
            }
        }
    }
}