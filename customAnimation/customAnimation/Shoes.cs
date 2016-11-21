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
		private KeyboardState oldKeyboardState;
		private KeyboardState newKeyboardState;

		private Keys right;
		private Keys left;
		private Keys up;
		private Keys down;

		// These are public so that Game1.draw can see them for debugging.
		public float airMovementSpeed = 600f;
		public float groundMovementSpeed = 300f;
		public float jumpImpulse = 20f;
		public float fallFromTileRate = 25f;

		// These are public so that Game1.draw can see them for debugging.
		public string preset;
		public string debug;
		public string debug2;

		public bool interfaceLinked = true; // Public so that Game1.draw can see them for debugging.
		private bool interfaceEnabled = true;

		private ContentManager content;

		private int bouncingHorizontally = 0;						// Represents which direction the Shoes will move after collision with a Spring. -1 represents left, 0 represents no bouncing, 1 represents right.
		private bool delayMovementAfterSpringCollision = false;		// The player cannot move the Shoes themselves after a Spring has been used.
		private Timer delayMovementAfterSpringCollisionTimer;		// Delays movement of the Shoes from using a Spring too quickly.

		private Timer delayLaunchAfterLauncherCollisionTimer;		// Delays launching the Shoes upon initial collision.
		private int angleInDegreesOfLauncherShoesIsUsing;			// Stores the coordinates of the Launcher Tile from the level. Used to launch the Shoes at the correct angle.
		private bool shoesAreCurrentlyMovingDueToLauncher = false;	// Says whether or not the Shoes are moving due to being launched from a Launcher. 

		private bool isGravityOn = true;							// Flag to use gravity or not.

		int increment = 0;

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

			delayMovementAfterSpringCollisionTimer = new Timer(0.3f);
			delayLaunchAfterLauncherCollisionTimer = new Timer(2f);
			angleInDegreesOfLauncherShoesIsUsing = 0;
		}

		/// <summary>
		/// Update method for the Shoes that's called once a frame.
		/// </summary>
		/// <param name="gameTime">Snapshot of the game timing state.</param>
		/// <param name="guy">A reference to the Guy.</param>
		public void Update(GameTime gameTime, ref Guy guy)
		{
			this.handleAnimation(gameTime);
			handleMovement(gameTime, ref guy);
			doInterface(guy.isGuyBeingShot);

			oldKeyboardState = newKeyboardState; // In Update() so the interface works. Commented out at the bottom of handleMovement.
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
					prepareMovementDueToSpringCollision(currentState);
				}
				else if (Level.tiles[y, x].IsLauncher)
				{
					prepareMovementDueToLauncherCollision(x, y, false);
				}
				else if (Level.tiles[y, x].IsAirCannonSwitch)
				{
					activateAirCannon(Level.tiles[y, x], x, y);
				}
				else
				{
					position.X = Level.tiles[y, x].Position.X - spriteWidth;
					checkIfShoesCollidedWithTileViaSpring();
					checkIfShoesCollidedWithTileViaLauncher();
				}
			}
			else if (currentState == State.RunningLeft)
			{
				if (Level.tiles[y, x].TileRepresentation == 'S')
				{
					position.X = Level.tiles[y, x].Position.X + Level.tiles[y, x].Texture.Width;
					delayMovementAfterSpringCollision = true;
					prepareMovementDueToSpringCollision(currentState);
				}
				else if (Level.tiles[y, x].IsLauncher)
				{
					prepareMovementDueToLauncherCollision(x, y, false);
				}
				else if (Level.tiles[y, x].IsAirCannonSwitch)
				{
					activateAirCannon(Level.tiles[y, x], x, y);
				}
				else
				{
					position.X = Level.tiles[y, x].Position.X + Level.tiles[y, x].Texture.Width;
					checkIfShoesCollidedWithTileViaSpring();
					checkIfShoesCollidedWithTileViaLauncher();
				}
			}
			else if (currentState == State.Jumping)
			{
				if (Level.tiles[y, x].TileRepresentation == 'S')
				{
					prepareMovementDueToSpringCollision(State.Decending); // Why is this passing in Decending?
				}
				else if (Level.tiles[y, x].IsLauncher)
				{
					prepareMovementDueToLauncherCollision(x, y, false);
				}
				else if (Level.tiles[y, x].IsAirCannonSwitch)
				{
					activateAirCannon(Level.tiles[y, x], x, y);
				}
				else
				{
					position.Y = Level.tiles[y, x].Position.Y + Level.tiles[y, x].Texture.Height + 2;
					velocity.Y = -1f;
					isFalling = true;
					checkIfShoesCollidedWithTileViaLauncher();
				}
			}
			else if (currentState == State.Decending)
			{
				if (Level.tiles[y, x].TileRepresentation == 'S')
				{
					position.Y = Level.tiles[y, x].Position.Y - spriteHeight;
					prepareMovementDueToSpringCollision(currentState);
				}
				else if (Level.tiles[y, x].IsLauncher)
				{
					prepareMovementDueToLauncherCollision(x, y, false);
				}
				else if (Level.tiles[y, x].IsAirCannonSwitch)
				{
					activateAirCannon(Level.tiles[y, x], x, y);
				}
				else
				{
					position.Y = Level.tiles[y, x].Position.Y - spriteHeight;
					spriteSpeed = 300f;
					isJumping = false;
					isFalling = false;
				}
			}
		}

		/// <summary>
		/// Displays information on the screen related to physics.
		/// </summary>
		/// <param name="beingShot">Flag that says whether or not the Guy is currently being shot or not.</param>
		private void doInterface(bool isGuyBeingShot)
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
				if (interfaceEnabled)
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
				if (interfaceLinked)
				{
					interfaceLinked = false;
				}
				else
				{
					interfaceLinked = true;
				}
			}

			if (interfaceLinked && interfaceEnabled)
			{
				// Shoes Movement
				if (isGuyBeingShot || shoesAreCurrentlyMovingDueToLauncher)
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
		public void swapTexture(bool areGuyAndShoesCurrentlyLinked)
		{
			if (areGuyAndShoesCurrentlyLinked)
			{
				spriteHeight = 48;
				Texture = content.Load<Texture2D>("Sprites/Shoes32x48"); // Bottom
				position.Y -= 32f;
			}
			else
			{
				spriteHeight = 16;
				Texture = content.Load<Texture2D>("Sprites/Shoes32x48_Top");
				position.Y += 32f;
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
		private void updateTimers(GameTime gameTime)
		{
			delayMovementAfterSpringCollisionTimer.Update(gameTime);
			delayLaunchAfterLauncherCollisionTimer.Update(gameTime);
		}

		// ******************
		// * START MOVEMENT *
		// ******************

		/// <summary>
		/// Handles all of the movement for the Shoes.
		/// </summary>
		/// <param name="gameTime">Snapshot of the game timing state.</param>
		/// <param name="guy">A reference to the Guy.</param>
		private void handleMovement(GameTime gameTime, ref Guy guy)
		{
			float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;	// Represents the amount of time that has passed since the previous frame.
			newKeyboardState = Keyboard.GetState();						// Get the new state of the keyboard.

			// Handles delaying movement after the Shoes have collided with a Spring.
			stopDelayingMovementAfterSpringCollisionIfPossible();

			// Set the horizontal velocity based on if the Shoes are on the ground or in the air.
			setHorizontalVelocity();

			// Check to see if the player wants to jump. If so, set the vertical velocity appropriately.
			checkIfShoesWantToJump(guy.tileAbove());

			// Move the Shoes if the player has pressed the appropriate key.
			moveShoesLeftOrRightIfPossible(delta);

			// Have the Shoes ascend from jumping if they haven't started falling yet.
			haveShoesAscendFromJumpOrFallFromGravity(delta);

			// If the Shoes have collided with a Spring, then apply movement from the Spring over time.
			checkIfShoesCanBounceFromSpring(delta);

			// If the Shoes have collided with a Launcher and are ready to be launched, then apply movement from the Launcher over time.
			checkIfShoesCanLaunch(guy.powerOfLauncherBeingUsed);

			// If the Shoes have fallen to the bottom of the map, reset the Shoes and Guy to the starting position of the level.
			resetShoesAndGuyToLevelStartingPositionIfNecessary(guy);

			// Update timers.
			updateTimers(gameTime);

			// Get the old state of the keyboard.
			//oldKeyboardState = newKeyboardState; // Commented out so the interface works.
		}

		/// <summary>
		/// Set the horizontal velocity based on if the Shoes are jumping or are on the ground.
		/// </summary>
		private void setHorizontalVelocity()
		{
			if (isJumping)
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
		/// <param name="isThereATileAboveTheGuy">Flag that says whether or not there is a tile above the linked Guy/Shoes.</param>
		private void checkIfShoesWantToJump(bool isThereATileAboveTheGuy)
		{
			if (!isJumping
				&& ((newKeyboardState.IsKeyDown(up) && !oldKeyboardState.IsKeyDown(up)) || (newKeyboardState.IsKeyDown(Keys.Space) && !oldKeyboardState.IsKeyDown(Keys.Space)))
				&& standingOnGround()
				&& !underTile()
				&& !isThereATileAboveTheGuy
				&& (!delayLaunchAfterLauncherCollisionTimer.TimerStarted && !delayLaunchAfterLauncherCollisionTimer.TimerCompleted))
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
			// Allow movement if the player has pressed the correct key to move the Shoes, and the Shoes are allowed to move after colliding with a Spring, and the Shoes aren't locked into a Launcher.
			if (newKeyboardState.IsKeyDown(right) && !delayMovementAfterSpringCollision && (!delayLaunchAfterLauncherCollisionTimer.TimerStarted && !delayLaunchAfterLauncherCollisionTimer.TimerCompleted))
			{
				bouncingHorizontally = 0;
				position.X += velocity.X * delta;

				// Allow the player to take over movement of the Shoes if the Shoes are currently being moved due to a Launcher.
				if (shoesAreCurrentlyMovingDueToLauncher)
				{
					shoesAreCurrentlyMovingDueToLauncher = false;
					velocity.Y = 0f;
				}

				// Create the rectangle for the player's future position.
				// Draw a rectangle around the player's position after they move.
				updateRectangles(1, 0);
				handleCollisions(State.RunningRight);
				changeState(State.RunningRight);
			}
			if (newKeyboardState.IsKeyDown(left) && !delayMovementAfterSpringCollision && (!delayLaunchAfterLauncherCollisionTimer.TimerStarted && !delayLaunchAfterLauncherCollisionTimer.TimerCompleted))
			{
				bouncingHorizontally = 0;
				position.X -= velocity.X * delta;

				if (shoesAreCurrentlyMovingDueToLauncher)
				{
					shoesAreCurrentlyMovingDueToLauncher = false;
					velocity.Y = 0f;
				}

				updateRectangles(-1, 0);
				handleCollisions(State.RunningLeft);
				changeState(State.RunningLeft);
			}
		}

		/// <summary>
		/// Have the Shoes ascend due to jumping, or fall due to gravity.
		/// </summary>
		/// <param name="delta">The amount of time that has passed since the previous frame. Used to ensure consitent movement if the framerate drops below 60 FPS.</param>
		private void haveShoesAscendFromJumpOrFallFromGravity(float delta)
		{
			if (isJumping)
			{
				doPlayerJump(delta);
			}
			else if (isGravityOn)
			{
				doGravity(delta); // Handles for when the player walks off the edge of a platform.
			}
		}

		/// <summary>
		/// Have the Shoes ascend if the Shoes are jumping over time.
		/// </summary>
		/// <remarks>This method only runs as the Shoes are jumping. That is, as they are ascending. Once the short hop is over, or the apex of the jump is reached, handleGravity takes over.</remarks>
		/// <param name="delta">The amount of time that has passed since the previous frame. Used to ensure consitent movement if the framerate drops below 60 FPS.</param>
		private void doPlayerJump(float delta)
		{
			// The jump key was down last frame, but in the current frame it's not down. Begin descent. This is for short hops.
			if (!newKeyboardState.IsKeyDown(up) && oldKeyboardState.IsKeyDown(up) && !isFalling && velocity.Y < 0f)
			{
				velocity.Y = 0f;
				isFalling = true;
			}

			position.Y += velocity.Y;       // Ascend the Shoes due to jumping. The vertical velocity was set in checkIfShoesWantToJump. Up -            
			velocity.Y += gravity * delta;  // Slow down the jump due to gravity. Down +   

			// If the velocity begins to pull the player down, the Shoes are falling. 
			if (velocity.Y > 0f)
			{
				isFalling = true;
				isJumping = false;
			}

			// Depending on which direction the Shoes are moving, check the top or bottom.
			if (isFalling)
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
			if (isJumping)
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
				isFalling = true;
			}
			else
			{
				// If the Shoes have fallen onto a Spring, have the Shoes bounce according to the Spring logic. Otherwise, stop falling.
				if (Level.tiles[(int)TileArrayCoordinates.X, (int)TileArrayCoordinates.Y].TileRepresentation == 'S' && velocity.Y > 4f)
				{
					prepareMovementDueToSpringCollision(State.Decending);
				}
				else if (Level.tiles[(int)TileArrayCoordinates.X, (int)TileArrayCoordinates.Y].IsLauncher)
				{
					isFalling = false;
					prepareMovementDueToLauncherCollision((int)TileArrayCoordinates.Y, (int)TileArrayCoordinates.X, true); // I pass the coordinates in backwards because I screwed up when I originally made did Level/Tile creation.
				}
				else
				{
					velocity.Y = 0f;
					isFalling = false;
					shoesAreCurrentlyMovingDueToLauncher = false;
				}
			}

			if (isFalling)
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

		// ****************
		// * END MOVEMENT *
		// ****************

		// ****************
		// * START SPRING *
		// ****************

		/// <summary>
		/// Bounces the Shoes off a Spring. Does not happen over time, just when the Shoes collide with a Spring.
		/// </summary>
		/// <param name="currentState">The current State of the Shoes.</param>
		private void prepareMovementDueToSpringCollision(State currentState)
		{
			if (currentState == State.Decending || currentState == State.Jumping)
			{
				// If the Shoes collided with a Spring (via falling down or raising upwards) due to being launched, let the Spring movement logic take over.
				if (!shoesAreCurrentlyMovingDueToLauncher)
				{
					velocity.Y *= -1;	// Flips the velocity to either bounce the Shoes up or down.
				}
				else
				{
					shoesAreCurrentlyMovingDueToLauncher = false;
					if (angleInDegreesOfLauncherShoesIsUsing >= 90 && angleInDegreesOfLauncherShoesIsUsing <= 180)
					{
						delayMovementAfterSpringCollisionTimer.startTimer();
						delayMovementAfterSpringCollision = true;
						bouncingHorizontally = -1;
					}
					else
					{
						delayMovementAfterSpringCollisionTimer.startTimer();
						delayMovementAfterSpringCollision = true;
						bouncingHorizontally = 1;
					}
				}

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

			// If the Shoes collided with a Spring due to being launched from a Launcher, let the Spring movement logic take over.
			if ((currentState == State.RunningRight || currentState == State.RunningLeft) && shoesAreCurrentlyMovingDueToLauncher)
			{
				velocity.Y *= -1;
				velocity.Y *= 0.55f;
				position.Y += velocity.Y;
				shoesAreCurrentlyMovingDueToLauncher = false;
			}

			// If the velocity begins to pull the player down, the Shoes are falling. Depending on which direction the Shoes are moving, check the top or bottom.
			if (velocity.Y > 0f)
			{
				isFalling = true;
				updateRectangles(0, 1);
				handleCollisions(State.Decending);
				changeState(State.Decending);
			}
			else
			{
				isFalling = false;
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
			float horizontalSpeedFromSpring = 5f;

			if (bouncingHorizontally == 1)
			{
				position.X -= horizontalSpeedFromSpring;

				updateRectangles(-1, 0);
				handleCollisions(State.RunningLeft);
				changeState(State.RunningLeft);
			}
			else
			{
				position.X += horizontalSpeedFromSpring;

				updateRectangles(1, 0);
				handleCollisions(State.RunningRight);
				changeState(State.RunningRight);
			}

			horizontalSpeedFromSpring *= delta;
		}

		/// <summary>
		/// Handles delaying movement after the Shoes have collided with a Spring.
		/// </summary>
		private void stopDelayingMovementAfterSpringCollisionIfPossible()
		{
			if (delayMovementAfterSpringCollisionTimer.TimerCompleted)
			{
				delayMovementAfterSpringCollisionTimer.stopTimer();
				delayMovementAfterSpringCollision = false;
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
		/// Check if the Shoes have collided with a Tile due to movement from a Spring. If so, stop horizontal movement.
		/// </summary>
		private void checkIfShoesCollidedWithTileViaSpring()
		{
			if (bouncingHorizontally != 0)
			{
				velocity.X = 0f;
				bouncingHorizontally = 0;
			}
		}

		// ****************
		// *  END SPRING  *
		// ****************

		// ******************
		// * START LAUNCHER *
		// ******************

		/// <summary>
		/// Sets up the position of the Shoes for the Launcher, and prepares for launch.
		/// </summary>
		/// <param name="xTileCoordinateOfLauncher">The X coordinate of the Launcher that has been collided with. Coordinate is based off of the Level, not actual position.</param>
		/// <param name="yTileCoordinateOfLauncher">The Y coordinate of the Launcher that has been collided with. Coordinate is based off of the Level, not actual position.</param>
		/// <param name="shoesFellOntoLauncher">Flag to denote whether or not the Shoes fell onto a Launcher from the top. or not.</param>
		private void prepareMovementDueToLauncherCollision(int xTileCoordinateOfLauncher, int yTileCoordinateOfLauncher, bool shoesFellOntoLauncher)
		{
			// Store the angle of the Launcher so the Shoes can be launched at the correct angle. Can't pass the angle back through the call stack to the Launcher movement logic without being messy.
			angleInDegreesOfLauncherShoesIsUsing = Tile.getAngleInDegrees(Level.tiles[yTileCoordinateOfLauncher, xTileCoordinateOfLauncher]);

			// If a Launcher is going to shoot the Shoes down, put them at the bottom of the Launcher. Needed so that the Shoes don't have to be launched through a Launcher.
			if (angleInDegreesOfLauncherShoesIsUsing == 315)
			{
				isGravityOn = false;
				position.Y = Level.tiles[yTileCoordinateOfLauncher, xTileCoordinateOfLauncher].Position.Y + 48;
				position.Y -= 32f;
				position.X = Level.tiles[yTileCoordinateOfLauncher, xTileCoordinateOfLauncher].Position.X - 32;
			}
			else if (angleInDegreesOfLauncherShoesIsUsing == 270) 
			{
				isGravityOn = false;
				position.Y = Level.tiles[yTileCoordinateOfLauncher, xTileCoordinateOfLauncher].Position.Y + 48;
				position.Y -= 32f;
			}
			else if (angleInDegreesOfLauncherShoesIsUsing == 225)
			{
				isGravityOn = false;
				position.Y = Level.tiles[yTileCoordinateOfLauncher, xTileCoordinateOfLauncher].Position.Y + 48;
				position.Y -= 32f;
				position.X = Level.tiles[yTileCoordinateOfLauncher, xTileCoordinateOfLauncher].Position.X + 8;
			}
			// Put the Shoes at the top of the Launcher.
			else if (!shoesFellOntoLauncher)
			{
				position.Y = Level.tiles[yTileCoordinateOfLauncher, xTileCoordinateOfLauncher].Position.Y - 48;
				position.Y += 32f;
				position.X = Level.tiles[yTileCoordinateOfLauncher, xTileCoordinateOfLauncher].Position.X - 8;
			}

			// If the Shoes collided with a Launcher due to being launched, stop Launcher movement so that the Shoes can be locked onto the current Launcher to await being launched.
			if (shoesAreCurrentlyMovingDueToLauncher)
			{
				shoesAreCurrentlyMovingDueToLauncher = false;
			}			

			// Stop any movement that was occuring.
			velocity = new Vector2(0f, 0f);

			// The Shoes are no longer jumping (if they were), since they will be snapped to the Launcher upon collision.
			if (isJumping)
			{
				isJumping = false;
			}

			// If the timer hasn't started yet, start it. Otherwise, wait for it to complete.
			if (!delayLaunchAfterLauncherCollisionTimer.TimerStarted)
			{
				delayLaunchAfterLauncherCollisionTimer.startTimer();
			}
		}

		/// <summary>
		/// Handles moving the Shoes over time due to being launched from a Launcher.
		/// </summary>
		/// <param name="power">The power at which the Shoes will be launched from the Launcher.</param>
		private void performHorizontalMovementFromLauncher(float power)
		{
			position -= (Utilities.Vector2FromAngle(MathHelper.ToRadians(angleInDegreesOfLauncherShoesIsUsing)) * power);

			// Check the appropriate side of the Shoes, depending on which way they are being launched.
			if (angleInDegreesOfLauncherShoesIsUsing == 270)
			{
				updateRectangles(0, 1);
				handleCollisions(State.Decending);
				changeState(State.Decending);
			}
			else if (angleInDegreesOfLauncherShoesIsUsing == 225)
			{
				updateRectangles(1, 0);
				handleCollisions(State.RunningRight);
				changeState(State.RunningRight);

				updateRectangles(0, 1);
				handleCollisions(State.Decending);
				changeState(State.Decending);
			}
			else if (angleInDegreesOfLauncherShoesIsUsing == 315) 
			{
				updateRectangles(-1, 0);
				handleCollisions(State.RunningLeft);
				changeState(State.RunningLeft);

				updateRectangles(0, 1);
				handleCollisions(State.Decending);
				changeState(State.Decending);
			}
			else if (angleInDegreesOfLauncherShoesIsUsing >= 90 && angleInDegreesOfLauncherShoesIsUsing <= 180)
			{
				updateRectangles(1, 0);
				handleCollisions(State.RunningRight);
				changeState(State.RunningRight);

				updateRectangles(0, -1);
				handleCollisions(State.Jumping);
				changeState(State.Jumping);
			}
			else
			{
				updateRectangles(-1, 0);
				handleCollisions(State.RunningLeft);
				changeState(State.RunningLeft);

				updateRectangles(0, -1);
				handleCollisions(State.Jumping);
				changeState(State.Jumping);
			}
		}

		/// <summary>
		/// Checks if the Shoes can be launched from a Launcher yet. If so, call the method to perform Launcher movement over time.
		/// </summary>
		/// <param name="power">The power at which the Shoes will be launched from the Launcher.</param>
		private void checkIfShoesCanLaunch(float power)
		{
			if (delayLaunchAfterLauncherCollisionTimer.TimerCompleted)
			{
				delayLaunchAfterLauncherCollisionTimer.resetTimer();
				shoesAreCurrentlyMovingDueToLauncher = true;
				isGravityOn = true;
			}

			if (shoesAreCurrentlyMovingDueToLauncher)
			{				
				performHorizontalMovementFromLauncher(power);

				// Handle collisions with the borders of the screen.
				if (didCharacterCollideWithTopBorderOfScreen)
				{
					shoesAreCurrentlyMovingDueToLauncher = false;
					setFlagsForBorderCollision(false);
				}
			}
		}

		/// <summary>
		/// Check if the Shoes have collided with a Tile due to movement from a Launcher. If so, stop using the Launcher movement logic.
		/// </summary>
		/// <remarks>This area of logic could be improved. Currently, the Shoes just stop on a tile. Should be changed to just stop vertical velocity.</remarks>
		private void checkIfShoesCollidedWithTileViaLauncher()
		{
			if (shoesAreCurrentlyMovingDueToLauncher)
			{
				velocity = new Vector2(0f, 0f);
				shoesAreCurrentlyMovingDueToLauncher = false;
			}
		}

		// ******************
		// *  END LAUNCHER  *
		// ******************

		// ********************
		// * START AIR CANNON *
		// ********************

		/// <summary>
		/// 
		/// </summary>
		private void activateAirCannon(Tile airCannonTileSwitch, int xCoordinateInArray, int yCoordinateInArray)
		{
			airCannonTileSwitch.IsAirCannonSwitchOn = true;

			if (increment < 1)
			{
				Air sprite = new Air(content.Load<Texture2D>("Sprites/AnimatedAir64x48"), 0, 32, 48, 1, spriteBatch);
				//sprite.position.X = 32 * increment;
				//sprite.position.X = 0f;
				sprite.position = airCannonTileSwitch.Position;
				sprite.position.Y -= 32f;
				increment++;

				Air.allAirs.Add(sprite);
			}
			
		}

		// ******************
		// * END AIR CANNON *
		// ******************
	}
}