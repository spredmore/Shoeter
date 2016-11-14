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
		KeyboardState currentKeyboardState;
		KeyboardState previousKeyboardState;
		MouseState currentMouseState;
		MouseState previousMouseState;

		public Vector2 currentMousePosition;
		Vector2 previousPosition;	// Used to determine direction of travel.
		int currentScrollWheelValue;
		int previousScrollWheelValue;

		public bool delayCollisionWithShoesAndGuy = true;	// We need to let the Guy travel a little bit before Shoes can catch him.
		Timer delayCollisionWithGuyAndShoesTimer;	// Delay so that the Guy and Shoes don't link back together too quickly.
		Timer launcherTimer;						// Delay before the Guy is launched.
		Timer delayBetweenLaunchesTimer;			// Delay so the Guy doesn't use another launcher too quickly.

		public float angleBetweenGuyAndMouseCursor;
		public float powerOfLauncherBeingUsed = 5f;

		bool useGravity;
		public bool areGuyAndShoesCurrentlyLinked = true;
		public bool isGuyBeingShot = false;
		
		float delta;

		public string debug;
		public string debug2;

		Level currentLevel;
		private int collX;
		private int collY;

		public bool usingLauncher = false;

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

			delayCollisionWithGuyAndShoesTimer = new Timer(0.5f);
			launcherTimer = new Timer(2f);
			delayBetweenLaunchesTimer = new Timer(0.1f);
		}

		/// <summary>
		/// Update method for the Guy that's called once a frame.
		/// </summary>
		/// <param name="gameTime">Snapshot of the game timing state.</param>
		/// <param name="shoes">A reference to the Shoes.</param>
		/// <param name="level">A reference to the current Level.</param>
		public void Update(GameTime gameTime, ref Shoes shoes, ref Level level)
		{
			currentLevel = level;
			handleAnimation(gameTime);
			handleMovement(gameTime, ref shoes);
		}

		/// <summary>
		/// Handles all of the movement for the Guy.
		/// </summary>
		/// <param name="gameTime">Snapshot of the game timing state.</param>
		/// <param name="shoes">A reference to the Shoes.</param>
		private void handleMovement(GameTime gameTime, ref Shoes shoes)
		{
			currentKeyboardState = Keyboard.GetState();
			currentMouseState = Mouse.GetState();
			currentMousePosition.X = currentMouseState.X;
			currentMousePosition.Y = currentMouseState.Y;
			currentScrollWheelValue = currentMouseState.ScrollWheelValue;
			delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
			angleBetweenGuyAndMouseCursor = MathHelper.ToDegrees((float)(Math.Atan2(shoes.Position.Y - currentMousePosition.Y, shoes.Position.X + 26 - currentMousePosition.X)));

			// Handles delaying the collision between the guy and shoes so that they don't link back together too quickly.
			stopDelayingCollisionWithGuyAndShoesIfPossible();

			// Set the position to the player's position so it follows him around.
			setPositionOfGuyToPositionOfShoesIfPossible(shoes.Position);

			// Shoot the Guy if the player clicks the left mouse button.
			shootGuyIfPossible(shoes);

			// ******************************************************
			// Reset the Guy to the Shoes' position.
			// ******************************************************
			if (!(currentMouseState.RightButton == ButtonState.Pressed) && previousMouseState.RightButton == ButtonState.Pressed && !areGuyAndShoesCurrentlyLinked)
			{                
				velocity = new Vector2(0f, 0f);
				isGuyBeingShot = false;
				areGuyAndShoesCurrentlyLinked = true;
				shoes.swapTexture(areGuyAndShoesCurrentlyLinked);
				Position = new Vector2(shoes.Position.X, shoes.Position.Y);
				usingLauncher = false;
				launcherTimer.stopTimer();
				delayBetweenLaunchesTimer.stopTimer();
			}

			// ******************************************************
			// Handle the Guy's movement when he is being shot.
			// ******************************************************
			if (isGuyBeingShot)
			{
				position.X += velocity.X; // SHOES MOVES EACH DIMENSION ONE AT A TIME

				if (useGravity)
				{
					velocity.Y += gravity * delta; // Gravity
				}

				// ******************************************************
				// Determine which direction we're traveling, and check for collisions. 
				// ******************************************************
				if (position.X > previousPosition.X)
				{
					// Traveling right
					updateRectangles(1, 0);
					handleCollisions(State.RunningRight);
					changeState(State.RunningRight);
				}
				if (position.X < previousPosition.X)
				{
					// Traveling left
					updateRectangles(-1, 0);
					handleCollisions(State.RunningLeft);
					changeState(State.RunningLeft);
				}

				position.Y += velocity.Y; // SHOES MOVES EACH DIMENSION ONE AT A TIME

				if (position.Y > previousPosition.Y)
				{
					// Traveling Down
					updateRectangles(0, 1);
					handleCollisions(State.Decending);
					changeState(State.Decending);
				}
				if (position.Y < previousPosition.Y)
				{
					// Traveling Up
					updateRectangles(0, -1);
					handleCollisions(State.Jumping);
					changeState(State.Jumping);
				}

				// ******************************************************
				// Set the Shoes' position to the Guys' upon collision.
				// ******************************************************
				if (PositionRect.Intersects(shoes.PositionRect) && !delayCollisionWithShoesAndGuy && !areGuyAndShoesCurrentlyLinked)
				{
					shoes.Position = new Vector2(Position.X, Position.Y + 40);
					velocity = new Vector2(0f, 0f);
					delayCollisionWithShoesAndGuy = true;
					isGuyBeingShot = false;
					areGuyAndShoesCurrentlyLinked = true;
					shoes.swapTexture(areGuyAndShoesCurrentlyLinked);
				}

				if (usingLauncher)
				{
					prepareMovementDueToLauncherCollision(gameTime);
				}

				if (delayBetweenLaunchesTimer.TimerCompleted)
				{
					delayBetweenLaunchesTimer.stopTimer();
				}
			}
			else
			{
				// ******************************************************
				// The player has reached the goal for the current level.
				// ******************************************************
				if (PositionRect.Intersects(currentLevel.goalRectangle) && areGuyAndShoesCurrentlyLinked)
				{
					currentLevel.LoadLevel();
					shoes.Position = currentLevel.getPlayerStartingPosition();
				}
			}

			// Change angle.
			if (currentScrollWheelValue < previousScrollWheelValue/* && beingShot == false*/) if (powerOfLauncherBeingUsed > 0) powerOfLauncherBeingUsed--;
			if (currentScrollWheelValue > previousScrollWheelValue/* && beingShot == false*/) if (powerOfLauncherBeingUsed < 15f) powerOfLauncherBeingUsed++;
			if ((!currentKeyboardState.IsKeyDown(Keys.Decimal) && previousKeyboardState.IsKeyDown(Keys.Decimal)) || (!currentKeyboardState.IsKeyDown(Keys.Down) && previousKeyboardState.IsKeyDown(Keys.Down))) if (gravity > 0) gravity -= 1f;
			if (!currentKeyboardState.IsKeyDown(Keys.NumPad3) && previousKeyboardState.IsKeyDown(Keys.NumPad3) || (!currentKeyboardState.IsKeyDown(Keys.Up) && previousKeyboardState.IsKeyDown(Keys.Up))) gravity += 1f;

			previousKeyboardState = currentKeyboardState;
			previousMouseState = currentMouseState;
			previousPosition = Position;
			previousScrollWheelValue = currentMouseState.ScrollWheelValue;
			updateRectangles(0, 0);

			// Update timers.
			delayCollisionWithGuyAndShoesTimer.Update(gameTime);
			launcherTimer.Update(gameTime);
			delayBetweenLaunchesTimer.Update(gameTime);
		}

		/// <summary>
		/// Upon collision with a Tile, perform the appropriate action depending on the State of the Guy and what kind of Tile is being collided with.
		/// </summary>
		/// <remarks>Only if there is an actual collision will any of these statements execute.</remarks>
		/// <param name="currentState">The current State of the Guy.</param>
		/// <param name="y">The Y coordinate of the Tile in the level being collided with.</param>
		/// <param name="x">The X coordinate of the Tile in the level being collided with.</param>
		protected override void specializedCollision(State currentState, int y, int x)
		{
			if (!delayBetweenLaunchesTimer.TimerStarted)    // Ensures the Guy doesn't use another Launcher too quickly.
			{
				if (currentState == State.RunningRight)  
				{
					position.X = Level.tiles[y, x].Position.X - spriteWidth;

					if (Level.tiles[y, x].TileRepresentation == 'S')
					{
						prepareMovementDueToSpringCollision(currentState);
					}
					else if (Level.tiles[y, x].IsLauncher)
					{
						usingLauncher = true;
						collY = y;
						collX = x;
					}
					else
					{
						velocity.X = 0f;
						delayCollisionWithShoesAndGuy = false;
					}
				}
				else if (currentState == State.RunningLeft)
				{
					position.X = Level.tiles[y, x].Position.X + Level.tiles[y, x].Texture.Width + 1;

					if (Level.tiles[y, x].TileRepresentation == 'S')
					{
						prepareMovementDueToSpringCollision(currentState);
					}
					else if (Level.tiles[y, x].IsLauncher)
					{
						usingLauncher = true;
						collX = x;
						collY = y;
					}
					else
					{
						velocity.X = 0f;
						delayCollisionWithShoesAndGuy = false;
					}
				}
				else if (currentState == State.Jumping)
				{
					position.Y = Level.tiles[y, x].Position.Y + Level.tiles[y, x].Texture.Height + 2;

					if (Level.tiles[y, x].TileRepresentation == 'S')
					{
						prepareMovementDueToSpringCollision(currentState);
					}
					else if (Level.tiles[y, x].IsLauncher)
					{
						usingLauncher = true;
						collX = x;
						collY = y;
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
						prepareMovementDueToSpringCollision(currentState);
					}
					else if (Level.tiles[y, x].IsLauncher)
					{
						usingLauncher = true;
						collX = x;
						collY = y;
					}
					else
					{
						velocity = new Vector2(0f, 0f); // So we don't fall through.
						useGravity = false;
					}
				}
			}
		}

		/// <summary>
		/// Returns true if there is a tile above the Guy.
		/// </summary>
		/// <returns>Returns true or false depending on if there is a Tile above the Guy or not.</returns>
		public bool tileAbove()
		{
			updateRectangles(0, -1);

			for (int i = 0; i < Level.impassableTileRecs.Count; i++)
			{
				if (FutureRectangleRect.Intersects(Level.impassableTileRecs[i]))
				{
					TileCollisionRectangle = Level.impassableTileRecs[i];
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Sets the Guy's Velocity and Position such that it is bounced off of a Spring.
		/// </summary>
		/// <param name="currentState">The current State of the Guy.</param>
		private void prepareMovementDueToSpringCollision(State currentState)
		{
			if (currentState == State.Decending || currentState == State.Jumping)
			{
				velocity.Y *= -1;			// Flips the velocity to the player bounces up.
				velocity.Y *= 0.55f;		// Decrease the power of the next bounce.
				position.Y += velocity.Y;
			}
			else if (currentState == State.RunningRight || currentState == State.RunningLeft)
			{
				velocity.X *= -1;
				velocity.X *= 0.55f;
				position.X += velocity.X;
			}

			// If the velocity begins to pull the player down, we're falling. 
			if (velocity.Y > 0f)
			{
				isFalling = true;
			}
			else
			{
				isFalling = false;
			}

			// Depending on which direction the player is moving, check the top or bottom.
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

		/// <summary>
		/// Handles setting up the Guy to use a Launcher.
		/// </summary>
		/// <param name="gameTime">Snapshot of the game timing state.</param>
		private void prepareMovementDueToLauncherCollision(GameTime gameTime)
		{
			Vector2 arrayCoordinates = Level.getTilePositionInArray(collX, collY);

			// Set the Guy at the top of the launcher.
			position.Y = Level.tiles[collY, collX].Position.Y - 48;
			position.X = Level.tiles[collY, collX].Position.X - 8;
			velocity = new Vector2(0f, 0f);
			useGravity = false;

			if (launcherTimer.TimerStarted)
			{
				if (launcherTimer.TimerCompleted)
				{
					// Prevent the Guy from using any other Launchers for a time period.
					delayBetweenLaunchesTimer.startTimer();

					// Launch the Guy.
					launcherTimer.stopTimer();
					velocity = Utilities.Vector2FromAngle(MathHelper.ToRadians(Tile.getLauncherAngleInDegrees(Level.tiles[collY, collX]))) * powerOfLauncherBeingUsed;
					velocity *= -1;
					usingLauncher = false;
					useGravity = true;
				}
			}
			else
			{
				launcherTimer.startTimer();
			}
		}

		/// <summary>
		/// Handles delaying the collision between the guy and shoes so that they don't link back together too quickly.
		/// </summary>
		private void stopDelayingCollisionWithGuyAndShoesIfPossible()
		{
			if (delayCollisionWithGuyAndShoesTimer.TimerCompleted)
			{
				delayCollisionWithGuyAndShoesTimer.stopTimer();
				delayCollisionWithShoesAndGuy = false;
			}
		}

		/// <summary>
		/// Set the position to the player's position so it follows him around.
		/// </summary>
		/// <param name="positionOfShoes">The current position of the Shoes.</param>
		private void setPositionOfGuyToPositionOfShoesIfPossible(Vector2 positionOfShoes)
		{
			if (!isGuyBeingShot)
			{
				Position = new Vector2(positionOfShoes.X, positionOfShoes.Y);
				areGuyAndShoesCurrentlyLinked = true;
			}
		}

		/// <summary>
		/// Shoot the Guy if the player clicks the left mouse button.
		/// </summary>
		/// <param name="shoes">A reference to the Shoes.</param>
		private void shootGuyIfPossible(Shoes shoes)
		{
			if (currentMouseState.LeftButton == ButtonState.Pressed && !isGuyBeingShot)
			{
				if (!delayCollisionWithGuyAndShoesTimer.TimerStarted)
				{
					delayCollisionWithGuyAndShoesTimer.startTimer();
				}

				isGuyBeingShot = true;
				useGravity = true;
				delayCollisionWithShoesAndGuy = true;
				velocity = Utilities.Vector2FromAngle(MathHelper.ToRadians(angleBetweenGuyAndMouseCursor)) * powerOfLauncherBeingUsed;
				velocity *= -1;
				areGuyAndShoesCurrentlyLinked = false;
				shoes.swapTexture(areGuyAndShoesCurrentlyLinked); // Changes the texture/size of the shoes because the Guy is being shot.
			}
		}
	}
}