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
		private KeyboardState currentKeyboardState;
		private KeyboardState previousKeyboardState;
		public MouseState currentMouseState;
		private MouseState previousMouseState;

		private Vector2 currentMousePosition;
		private Vector2 previousPosition;	// Used to determine direction of travel.
		private int currentScrollWheelValue;
		private int previousScrollWheelValue;

		private bool delayCollisionWithShoesAndGuy = true;			// We need to let the Guy travel a little bit before Shoes can catch him.
		private Timer delayCollisionWithGuyAndShoesTimer;			// Delay so that the Guy and Shoes don't link back together too quickly.
		private Timer delayLaunchAfterLauncherCollisionTimer;		// Delays launching the Guy upon initial collision.
		private Timer delayBetweenLaunchesTimer;					// Delay so the Guy doesn't use another launcher too quickly.

		public float angleBetweenGuyAndMouseCursor;
		public float powerOfLauncherBeingUsed = 5f;

		private bool useGravity;
		private bool areGuyAndShoesCurrentlyLinked = true;
		public bool isGuyBeingShot = false;
		
		private float delta;

		private Timer airCannonActivationTimer;	// A timer that keeps track of how long an Air Cannon has been on.
		public List<Air> airsGuyHasCollidedWith = new List<Air>();
		public Queue<Char> airCannonSwitchesCollidedWith;
		public Queue<float> airCannonSwitchesCollidedWithActivationTimes;

		private ContentManager content;

		public string debug;
		public string debug2;
		public string debug3;

		private Level currentLevel;
		private int collX;
		private int collY;

		public bool usingLauncher = false;
		private Boolean idleAnimationLockIsOn = false;

		public static int test;

		public Guy(Texture2D texture, SpriteBatch spriteBatch, int currentFrame, int totalFrames, int spriteWidth, int spriteHeight, int screenHeight, int screenWidth, ContentManager content)
		{
			this.spriteBatch = spriteBatch;
			this.Texture = texture;
			this.currentFrame = currentFrame;
			this.totalFrames = totalFrames;
			this.screenHeight = screenHeight;
			this.screenWidth = screenWidth;
			this.content = content;

			changeSpriteOfTheGuy(AnimatedSprite.AnimationState.Guy_Empty);

			gravity = 10f;
			debug = "";
			debug2 = "";
			collX = 0;
			collY = 0;

			PlayerMode = Mode.Guy;

			delayCollisionWithGuyAndShoesTimer = new Timer(0.5f);
			delayLaunchAfterLauncherCollisionTimer = new Timer(2f);
			delayBetweenLaunchesTimer = new Timer(0.1f);
			airCannonActivationTimer = new Timer(2f);

			airCannonSwitchesCollidedWith = new Queue<Char>();
			airCannonSwitchesCollidedWithActivationTimes = new Queue<float>();
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
			//handleAnimation(gameTime);
			setCurrentAndPreviousCollisionTiles();
			handleMovement(gameTime, ref shoes);
			Sprite.Animate(gameTime);
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
				if (currentState == State.Running_Right)  
				{
					// Allow the Guy to pass through an Air Switch Cannon.
					if (!Level.tiles[y, x].IsAirCannonSwitch)
					{
						position.X = Level.tiles[y, x].Position.X - Hbox.Width;
					}

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
					else if (Level.tiles[y, x].IsAirCannonSwitch)
					{
						Air.activateAirCannons(Level.tiles[y, x], CurrentCollidingTile, content, spriteBatch);
						queueSubsequentAirCannonSwitchCollisions(Level.tiles[y, x].TileRepresentation);

						if (!airCannonActivationTimer.TimerStarted)
						{
							airCannonActivationTimer.startTimer();
						}
					}
					else
					{
						velocity.X = 0f;
						delayCollisionWithShoesAndGuy = false;
					}
				}
				else if (currentState == State.Running_Left)
				{
					if (!Level.tiles[y, x].IsAirCannonSwitch)
					{
						position.X = Level.tiles[y, x].Position.X + Level.tiles[y, x].Texture.Width + 1;
					}

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
					else if (Level.tiles[y, x].IsAirCannonSwitch)
					{
						Air.activateAirCannons(Level.tiles[y, x], CurrentCollidingTile, content, spriteBatch);
						queueSubsequentAirCannonSwitchCollisions(Level.tiles[y, x].TileRepresentation);

						if (!airCannonActivationTimer.TimerStarted)
						{
							airCannonActivationTimer.startTimer();
						}
					}
					else
					{
						velocity.X = 0f;
						delayCollisionWithShoesAndGuy = false;
					}
				}
				else if (currentState == State.Jumping)
				{
					if (!Level.tiles[y, x].IsAirCannonSwitch)
					{
						position.Y = Level.tiles[y, x].Position.Y + Level.tiles[y, x].Texture.Height + 2;
					}

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
					else if (Level.tiles[y, x].IsAirCannonSwitch)
					{
						Air.activateAirCannons(Level.tiles[y, x], CurrentCollidingTile, content, spriteBatch);
						queueSubsequentAirCannonSwitchCollisions(Level.tiles[y, x].TileRepresentation);

						if (!airCannonActivationTimer.TimerStarted)
						{
							airCannonActivationTimer.startTimer();
						}
					}
					else
					{
						velocity.Y = 0f;
					}
				}
				else if (currentState == State.Decending)
				{
					if (!Level.tiles[y, x].IsAirCannonSwitch)
					{
						position.Y = Level.tiles[y, x].Position.Y - Hbox.Width;
					}

					if (Level.tiles[y, x].TileRepresentation == 'S' && velocity.Y > 1f)
					{
						position.Y -= 20f;	// Moves the Guy above the Spring so it doesn't clip through.
						prepareMovementDueToSpringCollision(currentState);
					}
					else if (Level.tiles[y, x].IsLauncher)
					{
						usingLauncher = true;
						collX = x;
						collY = y;
					}
					else if (Level.tiles[y, x].IsAirCannonSwitch)
					{
						position = new Vector2(Level.tiles[y, x].Position.X - 16, Level.tiles[y, x].Position.Y - 32);
						velocity = new Vector2(0f, 0f);

						Air.activateAirCannons(Level.tiles[y, x], CurrentCollidingTile, content, spriteBatch);
						queueSubsequentAirCannonSwitchCollisions(Level.tiles[y, x].TileRepresentation);

						if (!airCannonActivationTimer.TimerStarted)
						{
							airCannonActivationTimer.startTimer();
						}

						if (!idleAnimationLockIsOn)
						{
							changeSpriteOfTheGuy(AnimatedSprite.AnimationState.Guy_Idle_WithoutShoes_Right);
							idleAnimationLockIsOn = true;
						}
					}
					else
					{
						Position = new Vector2(TileCollisionRectangle.X - 8, TileCollisionRectangle.Y - 32);
						velocity = new Vector2(0f, 0f); // So the Guy doesn't fall through.
						useGravity = false;
						changeSpriteOfTheGuy(AnimatedSprite.AnimationState.Guy_Idle_WithoutShoes_Right);
					}
				}
			}
		}

		/// <summary>
		/// Load the next level if the player has reached the end of the level and the Guy and Shoes are linked.
		/// </summary>
		/// <param name="shoes">A reference to the Shoes.</param>
		private void loadNextLevelIfPossible(Shoes shoes)
		{
			if (shoes.Hbox.Intersects(currentLevel.goalRectangle) && areGuyAndShoesCurrentlyLinked)
			{
				currentLevel.LoadLevel();
				shoes.Position = currentLevel.getPlayerStartingPosition();
			}
		}

		/// <summary>
		/// Changes the power of shooting the Guy and of Launchers, based on how much the player scrolls the mouse wheel.
		/// </summary>
		private void changePowerOfShootingAndLaunching()
		{
			if (currentScrollWheelValue < previousScrollWheelValue)
			{
				if (powerOfLauncherBeingUsed > 0)
				{
					powerOfLauncherBeingUsed--;
				}
			}
			if (currentScrollWheelValue > previousScrollWheelValue)
			{
				if (powerOfLauncherBeingUsed < 15f)
				{
					powerOfLauncherBeingUsed++;
				}
			}
		}

		/// <summary>
		/// If the interface is not linked, allows the player to modify the value of gravity.
		/// </summary>
		private void changeGravity()
		{
			if ((!currentKeyboardState.IsKeyDown(Keys.Decimal) && previousKeyboardState.IsKeyDown(Keys.Decimal)) || 
				(!currentKeyboardState.IsKeyDown(Keys.Down) && previousKeyboardState.IsKeyDown(Keys.Down)))
			{
				if (gravity > 0) gravity -= 1f;
			}
			if (!currentKeyboardState.IsKeyDown(Keys.NumPad3) && previousKeyboardState.IsKeyDown(Keys.NumPad3) ||
				(!currentKeyboardState.IsKeyDown(Keys.Up) && previousKeyboardState.IsKeyDown(Keys.Up)))
			{
				gravity += 1f;
			} 
		}

		/// <summary>
		/// Updates a variety of variables used for knowing information about the current frame
		/// </summary>
		/// <remarks>This method name and method summary are terrible. Update them at some point.</remarks>
		private void updateCurrentFrameVariables(GameTime gameTime, Vector2 shoesPosition)
		{
			currentKeyboardState = Keyboard.GetState();
			currentMouseState = Mouse.GetState();
			currentMousePosition.X = currentMouseState.X;
			currentMousePosition.Y = currentMouseState.Y;
			currentScrollWheelValue = currentMouseState.ScrollWheelValue;
			delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
			angleBetweenGuyAndMouseCursor = MathHelper.ToDegrees((float)(Math.Atan2(shoesPosition.Y - currentMousePosition.Y, shoesPosition.X + 26 - currentMousePosition.X)));
		}

		/// <summary>
		/// Updates the variables that are used for storing previous values of frame variables.
		/// </summary>
		/// <remarks>This method name and method summary are terrible. Update them at some point.</remarks>
		private void updatePreviousFrameVariables()
		{
			previousKeyboardState = currentKeyboardState;
			previousMouseState = currentMouseState;
			previousPosition = Position;
			previousScrollWheelValue = currentMouseState.ScrollWheelValue;
			updateRectangles(0, 0);
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
		/// Updates the timers.
		/// </summary>
		/// <param name="gameTime">Snapshot of the game timing state.</param>
		private void updateTimers(GameTime gameTime)
		{
			delayCollisionWithGuyAndShoesTimer.Update(gameTime);
			delayLaunchAfterLauncherCollisionTimer.Update(gameTime);
			delayBetweenLaunchesTimer.Update(gameTime);
			airCannonActivationTimer.Update(gameTime);
		}

		/// <summary>
		/// Returns true if there is a tile above the Guy. Called from Shoes.cs.
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

		// ******************
		// * START MOVEMENT *
		// ******************

		private void debugs()
		{
			debug = "velocity: " + velocity.ToString();
			//debug = "test: " + test.ToString();
			//debug = "airCannonActivationTimer.ElapsedTime: " + airCannonActivationTimer.ElapsedTime.ToString();
			debug2 = "airCannonActivationTimer.TimerStarted: " + airCannonActivationTimer.TimerStarted.ToString();
			debug3 = "airCannonActivationTimer.TimerCompleted: " + airCannonActivationTimer.TimerCompleted.ToString();
			//debug3 = "airCannonTileCollidedWith: " + airCannonSwitchCurrentlyCollidingWith.ToString();
			//debug2 = "airCannonActivationTimer.ElapsedTime: " + airCannonActivationTimer.ElapsedTime.ToString();
			//debug = "areGuyAndShoesCurrentlyLinked: " + areGuyAndShoesCurrentlyLinked.ToString();
			//debug = "currentLevel.goalRectangle: " + currentLevel.goalRectangle.X + ", " + currentLevel.goalRectangle.Y;
		}

		/// <summary>
		/// Handles all of the movement for the Guy.
		/// </summary>
		/// <param name="gameTime">Snapshot of the game timing state.</param>
		/// <param name="shoes">A reference to the Shoes.</param>
		private void handleMovement(GameTime gameTime, ref Shoes shoes)
		{
			debugs();

			// Updates a variety of variables used for knowing information about the current frame.
			updateCurrentFrameVariables(gameTime, shoes.Position);

			// Handles delaying the collision between the guy and shoes so that they don't link back together too quickly.
			stopDelayingCollisionWithGuyAndShoesIfPossible();

			// Set the position to the player's position so it follows him around.
			setPositionOfGuyToPositionOfShoesIfPossible(shoes);

			// Shoot the Guy if the player clicks the left mouse button.
			shootGuyIfPossible(shoes);

			// Reset the Guy to the Shoes' position if the player clicks the right mouse button.
			resetGuyToShoesCurrentPositionIfPossible(shoes);

			if (isGuyBeingShot)
			{
				// Takes care of movement for the Guy while he's being shot.
				handleGuyMovementWhenBeingShot();

				// Set the Guy's position to the Shoes' upon collision.
				setGuyPositionToShoesUponCollisionIfPossible(shoes);

				// Checks to see if the Guy is using a Launcher. If so, then launch the Guy if possible.
				checkIfGuyCanLaunch(gameTime);

				// Stops delaying collisions with the Guy and other Launchers once he's been launched.
				stopDelayingCollisionWithGuyAndLaunchersIfPossible();
			}
			else
			{
				// Load the next level if the player has reached the end of the level and the Guy and Shoes are linked.
				loadNextLevelIfPossible(shoes);
			}

			// Change power of shooting the Guy and of Launchers if the player scrolls the mouse wheel.
			changePowerOfShootingAndLaunching();

			// If the interface is not linked, allows the player to modify the value of gravity.
			changeGravity();

			// If the Guy has turned on a particular set of Air Cannons and has now left that switch, turn the corresponding Air Cannons off.
			checkIfAirCannonsCanBeTurnedOff();

			// Updates the variables that are used for storing the previous values of the current values.
			updatePreviousFrameVariables();

			// Update timers.
			updateTimers(gameTime);

			Sprite.Position = Position;
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

				useGravity = true;
				isGuyBeingShot = true;
				delayCollisionWithShoesAndGuy = true;
				areGuyAndShoesCurrentlyLinked = false;
				velocity = Utilities.Vector2FromAngle(MathHelper.ToRadians(angleBetweenGuyAndMouseCursor)) * powerOfLauncherBeingUsed;
				velocity *= -1;
				//shoes.Position = new Vector2(shoes.Position.X, shoes.Position.Y + 32f);	// Move the shoes to the ground.
				
				if (shoes.CurrentCollidingTile.IsAirCannonSwitch)
				{
					shoes.Position = new Vector2(shoes.CurrentCollidingTile.Position.X, shoes.CurrentCollidingTile.Position.Y);
				}
				else
				{
					shoes.Position = new Vector2(shoes.Position.X, shoes.Position.Y + 32f);	// Move the shoes to the ground.
				}

				setBeingShotAnimationIfPossible(shoes);
			}
		}

		/// <summary>
		/// Handles modifying the Guy's movement while he's being shot.
		/// </summary>
		private void handleGuyMovementWhenBeingShot()
		{
			// Modify the Guy's position by the velocity it's traveling, then check for collisions appropriately.
			position.X += velocity.X;

			// If gravity is active, use it.
			if (useGravity)
			{
				velocity.Y += gravity * delta;
			}

			// Determine which direction the Guy is traveling, then check for collisions. 
			if (position.X > previousPosition.X)
			{
				updateRectangles(1, 0);
				handleCollisions(State.Running_Right);
				changeState(State.Running_Right);
			}
			if (position.X < previousPosition.X)
			{
				updateRectangles(-1, 0);
				handleCollisions(State.Running_Left);
				changeState(State.Running_Left);
			}

			position.Y += velocity.Y;

			if (position.Y > previousPosition.Y)
			{
				updateRectangles(0, 1);
				handleCollisions(State.Decending);
				changeState(State.Decending);
			}
			if (position.Y < previousPosition.Y)
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
			else if (currentState == State.Running_Right || currentState == State.Running_Left)
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

		// ****************
		// *  END SPRING  *
		// ****************

		// ******************
		// * START LAUNCHER *
		// ******************

		/// <summary>
		/// Checks to see if the Guy can Launch or not.
		/// </summary>
		/// <param name="gameTime">Snapshot of the game timing state.</param>
		private void checkIfGuyCanLaunch(GameTime gameTime)
		{
			if (usingLauncher)
			{
				prepareMovementDueToLauncherCollision(gameTime);
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

			if (delayLaunchAfterLauncherCollisionTimer.TimerStarted)
			{
				if (delayLaunchAfterLauncherCollisionTimer.TimerCompleted)
				{
					// Prevent the Guy from using any other Launchers for a time period.
					delayBetweenLaunchesTimer.startTimer();

					// Launch the Guy.
					delayLaunchAfterLauncherCollisionTimer.stopTimer();
					velocity = Utilities.Vector2FromAngle(MathHelper.ToRadians(Tile.getAngleInDegrees(Level.tiles[collY, collX]))) * powerOfLauncherBeingUsed;
					velocity *= -1;
					usingLauncher = false;
					useGravity = true;
				}
			}
			else
			{
				delayLaunchAfterLauncherCollisionTimer.startTimer();
			}
		}

		/// <summary>
		/// Stops delaying collisions with the Guy and other Launchers once he's been launched.
		/// </summary>
		private void stopDelayingCollisionWithGuyAndLaunchersIfPossible()
		{
			if (delayBetweenLaunchesTimer.TimerCompleted)
			{
				delayBetweenLaunchesTimer.stopTimer();
			}
		}

		// ******************
		// *  END LAUNCHER  *
		// ******************

		// ********************
		// * START AIR CANNON *
		// ********************

		/// <summary>
		/// Sets the velocity of the Guy upon collision with an Air.
		/// </summary>
		/// <param name="airCannonRepresentation">The representation of the Air Cannon that the Air came out. Used to determine how to set velocity.</param>
		public void setVelocityUponAirCollision(Char airCannonRepresentation)
		{
			if (airCannonRepresentation == 'Q')
			{
				velocity.X -= 10f;
				velocity.Y -= 10f;
			}
			else if (airCannonRepresentation == 'W')
			{
				velocity.Y -= 10f;
			}
			else if (airCannonRepresentation == 'E')
			{
				velocity.X += 10f;
				velocity.Y -= 10f;
			}
			else if (airCannonRepresentation == 'A')
			{
				velocity.X -= 10f;
			}
			else if (airCannonRepresentation == 'D')
			{
				velocity.X += 10f;
			}
			else if (airCannonRepresentation == 'Z')
			{
				velocity.X -= 10f;
				velocity.Y += 10f;
			}
			else if (airCannonRepresentation == 'X')
			{
				velocity.Y += 10f;
			}
			else if (airCannonRepresentation == 'C')
			{
				velocity.X += 10f;
				velocity.Y += 10f;
			}
		}

		/// <summary>
		/// Turns off any activated Air Cannons if they are on.
		/// </summary>
		private void checkIfAirCannonsCanBeTurnedOff()
		{
			if (airCannonActivationTimer.TimerCompleted)
			{
				airCannonActivationTimer.resetTimer();
				Air.turnOffAirCannonsIfPossible(this, null, airCannonSwitchesCollidedWith.Dequeue());

				// If there are multiple Air Cannon sets activated at the same time, then ensure that the next set to turn off happens at the correct time.
				if (airCannonSwitchesCollidedWith.Count >= 1)
				{
					airCannonActivationTimer.startTimer();
					airCannonActivationTimer.ElapsedTime += airCannonSwitchesCollidedWithActivationTimes.Dequeue();
				}
			}
		}

		/// <summary>
		/// If multiple Air Cannon Switches are activated, ensure that subsequent Air Cannon activations (after the initial one) only stay on for the correct length of time.
		/// </summary>
		/// <param name="tileRepresentation">Denotes which Air Cannon Switch was activated.</param>
		private void queueSubsequentAirCannonSwitchCollisions(Char tileRepresentation)
		{
			// Add the Air Cannon Switch set to the Queue if that set isn't already in the queue.
			if (!airCannonSwitchesCollidedWith.Contains<Char>(tileRepresentation))
			{
				airCannonSwitchesCollidedWith.Enqueue(tileRepresentation);

				// Enqueue the current elapsed time of the timer if there is a Air Cannon set already active.
				// This is needed to ensure that every Air Cannon set only stays active for the correct amount of time.
				if (airCannonActivationTimer.ElapsedTime != 0f)
				{
					airCannonSwitchesCollidedWithActivationTimes.Enqueue(airCannonActivationTimer.ElapsedTime);
				}
			}
		}

		// ******************
		// * END AIR CANNON *
		// ******************

		// **************************
		// * START POSITION SETTING *
		// **************************

		/// <summary>
		/// Set the position to the player's position so it follows him around.
		/// </summary>
		/// <param name="positionOfShoes">The current position of the Shoes.</param>
		private void setPositionOfGuyToPositionOfShoesIfPossible(Shoes shoes)
		{
			if (!isGuyBeingShot)
			{
				Position = new Vector2(shoes.Position.X, shoes.Position.Y);
				areGuyAndShoesCurrentlyLinked = true;
			}
		}

		/// <summary>
		/// Reset the Guy to the Shoes' position if the player has clicked the right mouse button.
		/// </summary>
		/// <param name="shoes">A reference to the Shoes.</param>
		private void resetGuyToShoesCurrentPositionIfPossible(Shoes shoes)
		{
			if (!(currentMouseState.RightButton == ButtonState.Pressed) && previousMouseState.RightButton == ButtonState.Pressed && !areGuyAndShoesCurrentlyLinked && !shoes.underTile())
			{
				isGuyBeingShot = false;
				usingLauncher = false;
				idleAnimationLockIsOn = false;
				areGuyAndShoesCurrentlyLinked = true;
				Position = new Vector2(shoes.Position.X, shoes.Position.Y);
				velocity = new Vector2(0f, 0f);
				delayBetweenLaunchesTimer.stopTimer();
				delayLaunchAfterLauncherCollisionTimer.stopTimer();
				setIdleAnimationIfPossible(shoes, false);
			}
		}

		/// <summary>
		/// Set the Shoes's position to the position of the Guy if the collision delay timer is completed and the Guy and Shoes are not currently linked.
		/// </summary>
		/// <param name="shoes">A reference to the Shoes.</param>
		private void setGuyPositionToShoesUponCollisionIfPossible(Shoes shoes)
		{
			if (Hbox.Intersects(shoes.Hbox) && !delayCollisionWithShoesAndGuy && !areGuyAndShoesCurrentlyLinked)
			{
				velocity = new Vector2(0f, 0f);
				shoes.velocity = new Vector2(0f, 0f);
				Position = new Vector2(shoes.Position.X, shoes.Position.Y);
				isGuyBeingShot = false;
				shoes.stopPlayerInput = true;
				idleAnimationLockIsOn = false;
				delayCollisionWithShoesAndGuy = true;
				areGuyAndShoesCurrentlyLinked = true;
				setIdleAnimationIfPossible(shoes, true);
			}
		}

		// ************************
		// * END POSITION SETTING *
		// ************************

		// *******************
		// * START ANIMATION *
		// *******************

		/// <summary>
		/// Sets the Animated Sprite for the Guy to a new Animated Sprite.
		/// </summary>
		/// <param name="state">The State of the Guy. Used to get the correct Animated Sprite.</param>
		public void changeSpriteOfTheGuy(AnimatedSprite.AnimationState state)
		{
			Sprite = AnimatedSprite.generateAnimatedSpriteBasedOnState(state, content, spriteBatch, (int)Position.X, (int)Position.Y, ref hbox);
		}

		/// <summary>
		/// Sets the Animated Sprite for the Guy to the Being Shot Animation.
		/// </summary>
		private void setBeingShotAnimationIfPossible(Shoes shoes)
		{
			if (shoes.directionShoesAreRunning == State.Running_Left)
			{
				shoes.changeSpriteOfTheShoes(AnimatedSprite.AnimationState.Shoes_Running_Left);
				changeSpriteOfTheGuy(AnimatedSprite.AnimationState.Guy_BeingShot_Left);
			}
			else if (shoes.directionShoesAreRunning == State.Idle_Left)
			{
				shoes.changeSpriteOfTheShoes(AnimatedSprite.AnimationState.Shoes_Idle_Left);
				changeSpriteOfTheGuy(AnimatedSprite.AnimationState.Guy_BeingShot_Left);
			}
			else if (shoes.directionShoesAreRunning == State.Running_Right)
			{
				shoes.changeSpriteOfTheShoes(AnimatedSprite.AnimationState.Shoes_Running_Right);
				changeSpriteOfTheGuy(AnimatedSprite.AnimationState.Guy_BeingShot_Right);
			}
			else if (shoes.directionShoesAreRunning == State.Idle_Right)
			{
				shoes.changeSpriteOfTheShoes(AnimatedSprite.AnimationState.Shoes_Idle_Right);
				changeSpriteOfTheGuy(AnimatedSprite.AnimationState.Guy_BeingShot_Right);
			}
		}

		/// <summary>
		/// Sets the Animated Sprite for the Guy to the Idle With Shoes Animation.
		/// </summary>
		private void setIdleAnimationIfPossible(Shoes shoes, Boolean calledFromMutualCollision)
		{
			if (shoes.directionShoesAreRunning == State.Running_Left && !calledFromMutualCollision)
			{
				shoes.changeSpriteOfTheShoes(AnimatedSprite.AnimationState.Guy_Running_Left);
				changeSpriteOfTheGuy(AnimatedSprite.AnimationState.Shoes_Empty);
			}
			else if ((shoes.directionShoesAreRunning == State.Idle_Left || calledFromMutualCollision) && shoes.directionShoesAreRunning != State.Running_Right)
			{
				shoes.changeSpriteOfTheShoes(AnimatedSprite.AnimationState.Guy_Idle_Left);
				changeSpriteOfTheGuy(AnimatedSprite.AnimationState.Shoes_Empty);
			}
			else if (shoes.directionShoesAreRunning == State.Running_Right && !calledFromMutualCollision)
			{
				shoes.changeSpriteOfTheShoes(AnimatedSprite.AnimationState.Guy_Running_Right);
				changeSpriteOfTheGuy(AnimatedSprite.AnimationState.Shoes_Empty);
			}
			else if ((shoes.directionShoesAreRunning == State.Idle_Right || calledFromMutualCollision) && shoes.directionShoesAreRunning != State.Running_Left)
			{
				shoes.changeSpriteOfTheShoes(AnimatedSprite.AnimationState.Guy_Idle_Right);
				changeSpriteOfTheGuy(AnimatedSprite.AnimationState.Shoes_Empty);
			}
		}

		// *****************
		// * END ANIMATION *
		// *****************
	}
}