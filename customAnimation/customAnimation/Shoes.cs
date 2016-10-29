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
	class Shoes : Character
	{
		// The states of the keyboard.
		KeyboardState oldKeyboardState;
		KeyboardState newKeyboardState;

		Keys right;
		Keys left;
		Keys up;
		Keys down;

		public float airMovementSpeed = 600f;
		public float groundMovementSpeed = 300f;
		public float jumpImpulse = 20f;
		public float fallFromTileRate = 25f;

		public string preset;
		public string debug;
		public string debug2;

		public bool interfaceLinked = true;
		private bool interfaceEnabled = true;

		private ContentManager content;

		private int bouncingHorizontally = 0;						// Represents which direction the Shoes will move after collision with a Spring. -1 represents left, 0 represents no bouncing, 1 represents right.
		public bool delayMovementAfterSpringCollision = false;		// The player cannot move the Shoes themselves after a Spring has been used.
		Timer delayMovementAfterSpringCollisionTimer;				// Delays movement of the Shoes from using a Spring too quickly.

		bool turnOffFalling = false;

		public Shoes(Texture2D texture, State state, int currentFrame, int spriteWidth, int spriteHeight, int totalFrames, SpriteBatch spriteBatch, int screenHeight, int screenWidth, Keys up, Keys left, Keys down, Keys right, ContentManager content)
		{
			this.spriteTexture = texture;       // The sprite sheet we will be drawing from.
			this.state = state;                 // The initial state of the player.
			this.currentFrame = currentFrame;   // The current frame that we are drawing.
			this.spriteWidth = spriteWidth;     // The width of the individual sprite.
			this.spriteHeight = spriteHeight;   // The height of the individual sprite.
			this.totalFrames = totalFrames;     // The total frames in the current sprite sheet.
			this.spriteBatch = spriteBatch;     // The spriteBatch we will use to draw the player.
			this.screenHeight = screenHeight;
			this.screenWidth = screenWidth;
			this.right = right;
			this.left = left;
			this.up = up;
			this.down = down;
			this.content = content;

			gravity = 30f;
			debug = "";
			debug2 = "";

			this.delayMovementAfterSpringCollisionTimer = new Timer(0.3f);
		}

		public void Update(GameTime gameTime, ref Guy guy)
		{
			this.handleAnimation(gameTime);
			handleMovement(gameTime, ref guy);
			doInterface(guy.beingShot);

			oldKeyboardState = newKeyboardState; // In Update() so the interface works. Commented out at the bottom of handleMovement.
		}

		private void handleMovement(GameTime gameTime, ref Guy guy)
		{
			debug = "position: " + position.ToString();

			float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;	// Represents the amount of time that has passed since the previous frame.
			newKeyboardState = Keyboard.GetState();						// Get the new state of the keyboard.

			// Handles delaying movement after the Shoes have collided with a Spring.
			if (delayMovementAfterSpringCollisionTimer.TimerCompleted == true)
			{
				delayMovementAfterSpringCollisionTimer.stopTimer();
				delayMovementAfterSpringCollision = false;
			}

			// Set the horizontal velocity based on if the Shoes are on the ground or in the air.
			setHorizontalVelocity();

			// Check to see if the player wants to jump. If so, set the vertical velocity appropriately.
			checkIfShoesWantToJump(guy);

			// Move the Shoes if the player has pressed the appropriate key.
			moveShoesLeftOrRightIfPossible(delta);

			// Have the Shoes ascend from jumping if they haven't started falling yet.
			haveShoesAscendFromJumpOrFallFromGravity(delta);

			// If the Shoes have collided with a Spring, then apply movement from the Spring over time.
			checkIfShoesCanBounceFromSpring(delta);

			// If the Shoes have fallen to the bottom of the map, reset the Shoes and Guy to the starting position of the level.
			resetShoesAndGuyToLevelStartingPositionIfNecessary(guy);

			// Update timers.
			//updateTimers(gameTime);
			delayMovementAfterSpringCollisionTimer.Update(gameTime);

			// Get the old state of the keyboard.
			//oldKeyboardState = newKeyboardState; // Commented out so the interface works.
		}

		/// <summary>
		/// Have the Shoes ascend if the Shoes are jumping over time.
		/// </summary>
		/// <remarks>This method only runs as the Shoes are jumping. That is, as they are ascending. Once the short hop is over, or the apex of the jump is reached, handleGravity takes over.</remarks>
		/// <param name="delta">The amount of time that has passed since the previous frame. Used to ensure consitent movement if the framerate drops below 60 FPS.</param>
		private void doPlayerJump(float delta)
		{
			// The jump key was down last frame, but in the current frame it's not down. Begin descent. This is for short hops.
			if (!newKeyboardState.IsKeyDown(up) && oldKeyboardState.IsKeyDown(up) && falling == false && velocity.Y < 0f)
			{
				velocity.Y = 0f;
				falling = true;
			}

			position.Y += velocity.Y;       // Ascend the Shoes due to jumping. The vertical velocity was set in checkIfShoesWantToJump. Up -            
			velocity.Y += gravity * delta;  // Slow down the jump due to gravity. Down +   

			// If the velocity begins to pull the player down, the Shoes are falling. 
			if (velocity.Y > 0f)
			{
				falling = true;
				isJumping = false;
			}

			// Depending on which direction the Shoes are moving, check the top or bottom.
			if (falling == true)
			{
				updateRectangles(0, 1);
				handleCollisions(State.Decending);
				changeState(State.Decending);
			}
			else
			{
				updateRectangles(0, -1);
				handleCollisions(State.Jumping);
				changeState(State.Jumping);
			}

			// Different speed while in the air.
			if (isJumping == true)
			{
				spriteSpeed = airMovementSpeed;
			}
			else
			{
				spriteSpeed = groundMovementSpeed;
			}
		}

		/// <summary>
		/// Applies gravity over time.
		/// </summary>
		/// <remarks>This method applies gravity over time except when the Shoes are jumping. In that case, doPlayerJump handles gravity.</remarks>
		/// <param name="delta">The amount of time that has passed since the previous frame. Used to ensure consitent movement if the framerate drops below 60 FPS.</param>
		private void doGravity(float delta)
		{
			position.Y += velocity.Y;
			velocity.Y += fallFromTileRate * delta;

			// If the Shoes are not standing on the ground, apply gravity.
			if (!standingOnGround())
			{
				falling = true;
			}
			else
			{
				// If the Shoes have fallen onto a Spring, have the Shoes bounce according to the Spring logic. Otherwise, stop falling.
				setTileArrayCoordinates(position.X, position.Y); // Sets which Tile in the Level (based on the position of the Shoes) that the Shoes are colliding with.
				if (Level.tiles[(int)TileArrayCoordinates.X, (int)TileArrayCoordinates.Y].TileRepresentation == 'S' && velocity.Y > 4f)
				{
					doSpring(State.Decending);
				}
				else
				{
					velocity.Y = 0f;
					falling = false;
				}
			}

			if (falling == true)
			{
				updateRectangles(0, 1);
				handleCollisions(State.Decending);
				changeState(State.Decending);
			}
			else
			{
				updateRectangles(0, -1);
				handleCollisions(State.Jumping);
				changeState(State.Jumping);
			}
		}

		/// <summary>
		/// Upon collision with a Tile, perform the appropriate action depending on the State of the Shoes and which Tile is being collided with.
		/// </summary>
		/// <remarks>Only if there is an actual collision will any of these statments execute.</remarks>
		/// <param name="currentState">The current State of the Shoes.</param>
		/// <param name="y">The Y coordinate of the Tile in the level being collided with.</param>
		/// <param name="x">The X coordinate of the Tile in the level being collided with.</param>
		protected override void specializedCollision(State currentState, int y, int x)
		{
			if (currentState == State.RunningRight)
			{
				if (Level.tiles[y, x].TileRepresentation == 'S')
				{
					position.X = Level.tiles[y, x].Position.X - spriteWidth;
					delayMovementAfterSpringCollision = true;
					doSpring(currentState);
				}
				else
				{
					position.X = Level.tiles[y, x].Position.X - spriteWidth;
				}

			}
			else if (currentState == State.RunningLeft)
			{
				if (Level.tiles[y, x].TileRepresentation == 'S')
				{
					position.X = Level.tiles[y, x].Position.X + Level.tiles[y, x].Texture.Width;
					delayMovementAfterSpringCollision = true;
					doSpring(currentState);
				}
				else
				{
					position.X = Level.tiles[y, x].Position.X + Level.tiles[y, x].Texture.Width;
				}

			}
			else if (currentState == State.Jumping)
			{
				if (Level.tiles[y, x].TileRepresentation == 'S')
				{
					doSpring(State.Decending);
				}
				else
				{
					position.Y = Level.tiles[y, x].Position.Y + Level.tiles[y, x].Texture.Height + 2;
					velocity.Y = -1f;
					falling = true;
				}
			}
			else if (currentState == State.Decending)
			{
				if (Level.tiles[y, x].TileRepresentation == 'S')
				{
					position.Y = Level.tiles[y, x].Position.Y - spriteHeight;
					doSpring(State.Decending);
				}
				else
				{
					position.Y = Level.tiles[y, x].Position.Y - spriteHeight;
					spriteSpeed = 300f;
					isJumping = false;
					falling = false;
				}
			}
		}

		private void doInterface(bool beingShot)
		{
			// Speed Interface
			if (newKeyboardState.IsKeyDown(Keys.NumPad7) || newKeyboardState.IsKeyDown(Keys.D7)) if (airMovementSpeed > 0) airMovementSpeed -= 5f;
			if (newKeyboardState.IsKeyDown(Keys.NumPad8) || newKeyboardState.IsKeyDown(Keys.D8)) if (airMovementSpeed < 1020) airMovementSpeed += 5f;
			if (newKeyboardState.IsKeyDown(Keys.NumPad4) || newKeyboardState.IsKeyDown(Keys.D4)) if (groundMovementSpeed > 0) groundMovementSpeed -= 5f;
			if (newKeyboardState.IsKeyDown(Keys.NumPad5) || newKeyboardState.IsKeyDown(Keys.D5)) groundMovementSpeed += 5f;
			if ((!newKeyboardState.IsKeyDown(Keys.NumPad1) && oldKeyboardState.IsKeyDown(Keys.NumPad1)) || (!newKeyboardState.IsKeyDown(Keys.D1) && oldKeyboardState.IsKeyDown(Keys.D1))) jumpImpulse--;
			if ((!newKeyboardState.IsKeyDown(Keys.NumPad2) && oldKeyboardState.IsKeyDown(Keys.NumPad2)) || (!newKeyboardState.IsKeyDown(Keys.D2) && oldKeyboardState.IsKeyDown(Keys.D2))) jumpImpulse++;
			if ((!newKeyboardState.IsKeyDown(Keys.NumPad6) && oldKeyboardState.IsKeyDown(Keys.NumPad6)) || (!newKeyboardState.IsKeyDown(Keys.D6) && oldKeyboardState.IsKeyDown(Keys.D6))) if (gravity > 0) gravity -= 5f;
			if ((!newKeyboardState.IsKeyDown(Keys.NumPad9) && oldKeyboardState.IsKeyDown(Keys.NumPad9)) || (!newKeyboardState.IsKeyDown(Keys.D9) && oldKeyboardState.IsKeyDown(Keys.D9))) gravity += 5f;
			if ((!newKeyboardState.IsKeyDown(Keys.Divide) && oldKeyboardState.IsKeyDown(Keys.Divide)) || (!newKeyboardState.IsKeyDown(Keys.Left) && oldKeyboardState.IsKeyDown(Keys.Left))) if (fallFromTileRate > 0) fallFromTileRate--;
			if ((!newKeyboardState.IsKeyDown(Keys.Subtract) && oldKeyboardState.IsKeyDown(Keys.Subtract)) || (!newKeyboardState.IsKeyDown(Keys.Right) && oldKeyboardState.IsKeyDown(Keys.Right))) fallFromTileRate++;

			// Presets
			if (newKeyboardState.IsKeyDown(Keys.F1))
			{
				preset = "Average - F1";
				airMovementSpeed = 375f;
				groundMovementSpeed = 355f;
				jumpImpulse = 17f;
				gravity = 30f;
				fallFromTileRate = 25f;
			}
			if (newKeyboardState.IsKeyDown(Keys.F2))
			{
				preset = "Derp - F2";
				airMovementSpeed = 1020f;
				groundMovementSpeed = 90f;
				jumpImpulse = 21f;
				gravity = 530f;
				fallFromTileRate = 80f;
			}
			if (newKeyboardState.IsKeyDown(Keys.F3))
			{
				preset = "Guy - F3";
				airMovementSpeed = 80f;
				groundMovementSpeed = 195f;
				jumpImpulse = 6f;
				gravity = 70f;
				fallFromTileRate = 40f;
			}
			if (newKeyboardState.IsKeyDown(Keys.F4))
			{
				preset = "Shoes - F4";
				airMovementSpeed = 405f;
				groundMovementSpeed = 425f;
				jumpImpulse = 8f;
				gravity = 25f;
				fallFromTileRate = 20f;
			}

			// Toggles the interface on and off
			if (!newKeyboardState.IsKeyDown(Keys.F11) && oldKeyboardState.IsKeyDown(Keys.F11))
			{
				if (interfaceEnabled == true)
				{
					preset = "Interface Disabled";
					interfaceEnabled = false;
				}
				else
				{
					preset = "Interface Enabled";
					interfaceEnabled = true;
				}
			}

			if (!newKeyboardState.IsKeyDown(Keys.F12) && oldKeyboardState.IsKeyDown(Keys.F12))
			{
				// Allows the player to have different speeds when the Guy is being shot or not.
				if (interfaceLinked == true) interfaceLinked = false;
				else interfaceLinked = true;
			}

			if (interfaceLinked == true && interfaceEnabled == true)
			{
				// Shoes Movement
				if (beingShot == true)
				{
					preset = "Shoes - F4";
					airMovementSpeed = 405f;
					groundMovementSpeed = 425f;
					jumpImpulse = 9.0f;
					gravity = 25f;
					fallFromTileRate = 20f;
				}
				else // Guy Movement
				{
					preset = "Guy - F3";
					airMovementSpeed = 80f;
					groundMovementSpeed = 195f;
					jumpImpulse = 6f;
					gravity = 70f;
					fallFromTileRate = 40f;
				}
			}
		}

		/// <summary>
		/// Swaps the texture and dimensions of the Shoes. Used to switch between the Guy being shot and the Guy traveling with the Shoes.
		/// </summary>
		/// <param name="currentLinkedState">Flag that says whether or not the Shoes and Guy are currently linked or not.</param>
		public void swapTexture(bool currentLinkedState)
		{
			if (currentLinkedState == true)
			{
				this.spriteHeight = 48;
				this.Texture = content.Load<Texture2D>("Sprites/Shoes32x48"); // Bottom
				position.Y -= 32f;
			}
			else
			{
				this.spriteHeight = 16;
				this.Texture = content.Load<Texture2D>("Sprites/Shoes32x48_Top");
				position.Y += 32f;
			}
		}

		/// <summary>
		/// Bounces the Shoes off a Spring. Does not happen over time, just when the Shoes collide with a Spring.
		/// </summary>
		/// <param name="currentState">The current State of the Shoes.</param>
		private void doSpring(State currentState)
		{
			if (currentState == State.Decending || currentState == State.Jumping)
			{
				velocity.Y *= -1;       // Flips the velocity to either bounce the Shoes up or down.
				velocity.Y *= 0.55f;    // Decrease the power of the next bounce.
				position.Y += velocity.Y;
			}
			else if (currentState == State.RunningRight)                
			{
				delayMovementAfterSpringCollisionTimer.startTimer();
				delayMovementAfterSpringCollision = true;
				bouncingHorizontally = 1;
			}
			else if (currentState == State.RunningLeft)
			{
				delayMovementAfterSpringCollisionTimer.startTimer();
				delayMovementAfterSpringCollision = true;
				bouncingHorizontally = -1;
			}

			// If the velocity begins to pull the player down, the Shoes are falling. 
			if (velocity.Y > 0f)
			{
				falling = true;
			}
			else
			{
				falling = false;
			}

			// Depending on which direction the Shoes are moving, check the top or bottom.
			if (falling == true)
			{
				updateRectangles(0, 1);
				handleCollisions(State.Decending);
				changeState(State.Decending);
			}
			else
			{
				updateRectangles(0, -1);
				handleCollisions(State.Jumping);
				changeState(State.Jumping);
			}
		}

		/// <summary>
		/// Performs horizontal movement for the Shoes over time. 
		/// </summary>
		/// <param name="delta">The amount of time that has passed since the previous frame. Used to ensure consitent movement if the framerate drops below 60 FPS.</param>
		private void performHorizontalMovementFromSpring(float delta)
		{
			float xSpeed = 5f;

			if (bouncingHorizontally == 1)
			{
				position.X -= xSpeed;

				updateRectangles(-1, 0);
				handleCollisions(State.RunningLeft);
				changeState(State.RunningLeft);
			}
			else
			{
				position.X += xSpeed;

				updateRectangles(1, 0);
				handleCollisions(State.RunningRight);
				changeState(State.RunningRight);
			}

			xSpeed *= delta;
		}

		/// <summary>
		/// Set the horizontal velocity based on if the Shoes are jumping or are on the ground.
		/// </summary>
		private void setHorizontalVelocity()
		{
			if (isJumping == true)
			{
				velocity.X = airMovementSpeed;
			}
			else
			{
				velocity.X = groundMovementSpeed;
			}
		}

		/// <summary>
		/// Check to see if the player wants to jump. If so, set the velocity to a negetive number so that the Shoes will move upwards.
		/// </summary>
		/// <param name="guy">A reference to the Guy. Needed so that a check can be done to ensure that there isn't a tile above the linked Guy/Shoes.</param>
		private void checkIfShoesWantToJump(Guy guy)
		{
			if (isJumping == false
				&& ((newKeyboardState.IsKeyDown(up) && !oldKeyboardState.IsKeyDown(up)) || (newKeyboardState.IsKeyDown(Keys.Space) && !oldKeyboardState.IsKeyDown(Keys.Space)))
				&& standingOnGround() == true
				&& underTile() == false
				&& guy.tileAbove() == false)
			{
				isJumping = true;
				velocity.Y = jumpImpulse * -1;
			}
		}

		/// <summary>
		/// Move the Shoes if the player has pressed the appropriate key.
		/// </summary>
		/// <param name="delta">The amount of time that has passed since the previous frame. Used to ensure consitent movement if the framerate drops below 60 FPS.</param>
		private void moveShoesLeftOrRightIfPossible(float delta)
		{
			if (newKeyboardState.IsKeyDown(right) && delayMovementAfterSpringCollision == false)
			{
				bouncingHorizontally = 0;
				position.X += velocity.X * delta;

				// Create the rectangle for the player's future position.
				// Draw a rectangle around the player's position after they move.
				updateRectangles(1, 0);
				handleCollisions(State.RunningRight);
				changeState(State.RunningRight);
			}
			if (newKeyboardState.IsKeyDown(left) && delayMovementAfterSpringCollision == false)
			{
				bouncingHorizontally = 0;
				position.X -= velocity.X * delta;

				updateRectangles(-1, 0);
				handleCollisions(State.RunningLeft);
				changeState(State.RunningLeft);
			}
			if (newKeyboardState.IsKeyDown(Keys.G))
			{
				if (turnOffFalling == false)
				{
					turnOffFalling = true;
				}
				else
				{
					turnOffFalling = false;
				}
			}
		}

		/// <summary>
		/// Have the Shoes ascend due to jumping, or fall due to gravity.
		/// </summary>
		/// <param name="delta">The amount of time that has passed since the previous frame. Used to ensure consitent movement if the framerate drops below 60 FPS.</param>
		private void haveShoesAscendFromJumpOrFallFromGravity(float delta)
		{
			if (isJumping == true)
			{
				doPlayerJump(delta);
			}
			else
			{
				doGravity(delta); // Handles for when the player walks off the edge of a platform.
			}
		}

		/// <summary>
		/// If the Shoes have collided with a Spring, then apply movement from the Spring over time.
		/// </summary>
		/// <param name="delta">The amount of time that has passed since the previous frame. Used to ensure consitent movement if the framerate drops below 60 FPS.</param>
		private void checkIfShoesCanBounceFromSpring(float delta)
		{
			if (bouncingHorizontally != 0 && !standingOnGround())
			{
				performHorizontalMovementFromSpring(delta);
			}
			else
			{
				bouncingHorizontally = 0; // Stop horizontal movement.
			}
		}

		/// <summary>
		/// If the Shoes have fallen to the bottom of the map, reset the Shoes and Guy to the starting position of the level.
		/// </summary>
		/// <param name="guy">A reference to the Guy. Needed so that a check can be done to ensure that there isn't a tile above the linked Guy/Shoes.</param>
		private void resetShoesAndGuyToLevelStartingPositionIfNecessary(Guy guy)
		{
			if (Position.Y > 704)
			{
				Position = Level.playerStartingPosition;
				guy.Position = Position;
			}
		}

		/// <summary>
		/// Updates the timers.
		/// </summary>
		/// <param name="gametime">Snapshot of the game timing state.</param>
		private void updateTimers(ref GameTime gametime)
		{
			delayMovementAfterSpringCollisionTimer.Update(gameTime);
		}
	}
}