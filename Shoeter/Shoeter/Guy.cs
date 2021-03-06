﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Shoeter
{
	class Guy : Character
	{
		private KeyboardState currentKeyboardState;
		private KeyboardState previousKeyboardState;
		private MouseState currentMouseState;
		private MouseState previousMouseState;

		private Vector2 currentMousePosition;
		private Vector2 previousPosition;	// Used to determine direction of travel.
		private int currentScrollWheelValue;
		private int previousScrollWheelValue;

		private bool delayCollisionWithShoesAndGuy = true;			// We need to let the Guy travel a little bit before Shoes can catch him.
		private Timer delayCollisionWithGuyAndShoesTimer;			// Delay so that the Guy and Shoes don't link back together too quickly.
		public Timer delayLaunchAfterLauncherCollisionTimer;		// Delays launching the Guy upon initial collision.
		public Timer delayBetweenLaunchesTimer;					// Delay so the Guy doesn't use another launcher too quickly.

		public float angleBetweenGuyAndMouseCursor;
		public float powerOfLauncherBeingUsed = 5f;

		private bool useGravity;
		private bool areGuyAndShoesCurrentlyLinked = true;
		public bool isGuyBeingShot = false;

		public float delta;

		public List<Air> airsGuyHasCollidedWith = new List<Air>();

		private ContentManager content;

		public string debug;
		public string debug2;
		public string debug3;

		private Level currentLevel;
		private int collX;
		private int collY;

		public bool usingLauncher = false;
		private Boolean idleAnimationLockIsOn = false;
		public FadeHandler fadeHandler;

		public Boolean AreGuyAndShoesCurrentlyLinked
		{
			get { return areGuyAndShoesCurrentlyLinked; }
		}

		public Guy(Texture2D texture, SpriteBatch spriteBatch, int currentFrame, int totalFrames, int spriteWidth, int spriteHeight, int screenHeight, int screenWidth, ContentManager content)
		{
			this.spriteBatch = spriteBatch;
			this.Texture = texture;
			this.currentFrame = currentFrame;
			this.totalFrames = totalFrames;
			this.spriteWidth = spriteWidth;
			this.spriteHeight = spriteHeight;
			this.screenHeight = screenHeight;
			this.screenWidth = screenWidth;
			this.content = content;

			changeSpriteOfTheGuy("Empty");

			gravity = 10f;
			debug = "";
			debug2 = "";
			collX = 0;
			collY = 0;

			PlayerMode = Mode.Guy;

			delayCollisionWithGuyAndShoesTimer = new Timer(0.5f);
			delayLaunchAfterLauncherCollisionTimer = new Timer(2f);
			delayBetweenLaunchesTimer = new Timer(0.1f);
			fadeHandler = new FadeHandler(1f, 2f, 1f, spriteBatch);
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
			fadeHandler.Update(gameTime);
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
						position.X = Level.tiles[y, x].Position.X - spriteWidth;
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
						position.Y = Level.tiles[y, x].Position.Y - spriteHeight;
					}

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
					else if (Level.tiles[y, x].IsAirCannonSwitch)
					{
						Air.activateAirCannons(Level.tiles[y, x], CurrentCollidingTile, content, spriteBatch);
						position = new Vector2(Level.tiles[y, x].Position.X - 16, Level.tiles[y, x].Position.Y - 32);
						velocity = new Vector2(0f, 0f);

						if (!idleAnimationLockIsOn)
						{
							changeSpriteOfTheGuy("Idle_WithoutShoes_Right");
							idleAnimationLockIsOn = true;
						}
					}
					else
					{
						velocity = new Vector2(0f, 0f); // So the Guy doesn't fall through.
						useGravity = false;
						changeSpriteOfTheGuy("Idle_WithoutShoes_Right");
						SoundEffectHandler.playGuyLandingSoundEffect();
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
			if (PositionRect.Intersects(currentLevel.goalRectangle) && areGuyAndShoesCurrentlyLinked && 
				(currentMouseState.RightButton != ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released))
			{
				shoes.stopPlayerInputDueToLevelCompletion = true;
			}

			// The victory screen will show here. Prompt player to continue to the next level.
			if (currentKeyboardState.IsKeyUp(Keys.Enter) && previousKeyboardState.IsKeyDown(Keys.Enter) && 
				shoes.stopPlayerInputDueToLevelCompletion &&
				!fadeHandler.FadingOut &&
				!fadeHandler.HoldingWhileFaded &&
				!fadeHandler.FadingIn)
			{
				// If the player hasn't completed all of the Main Game levels or the Bonus Levels, keep loading them. Otherwise, exit to main menu.
				if ((!Level.bonusLevelsSelected && Level.currentLevel + 1 <= 5) || (Level.bonusLevelsSelected && Level.currentLevel + 1 <= 12)) 
				{
					// Begin fading out the sceen.
					fadeHandler.fadeToBlack();
				}
				else
				{
					// Exit to main menu.
					Level.exitGame = true;
				}
			}

			// Once the screen is completely faded out, hold the fade for the specified time.
			if(fadeHandler.fadeToBlackTimer.TimerCompleted)
			{
				fadeHandler.holdFadeOut();
			}

			// Once the amount of time to hold the screen has passed, load the next level.
			if (fadeHandler.holdWhileFadedTimer.TimerCompleted)
			{
				fadeHandler.fadeFromBlack();
				currentLevel.LoadLevel();
				shoes.Position = currentLevel.getPlayerStartingPosition();
				shoes.stopPlayerInputDueToLevelCompletion = false;
			}

			// Once the screen is done fading in, reset the FadeHandler.
			if (fadeHandler.fadeFromBlackTimer.TimerCompleted) 
			{
				fadeHandler.resetFadeHandler();
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

		public void changeSpriteOfTheGuy(String state)
		{
			Sprite = AnimatedSprite.generateAnimatedSpriteBasedOnState(state, content, spriteBatch, true);
		}

		// ******************
		// * START MOVEMENT *
		// ******************

		/// <summary>
		/// Handles all of the movement for the Guy.
		/// </summary>
		/// <param name="gameTime">Snapshot of the game timing state.</param>
		/// <param name="shoes">A reference to the Shoes.</param>
		private void handleMovement(GameTime gameTime, ref Shoes shoes)
		{
			// Updates a variety of variables used for knowing information about the current frame.
			updateCurrentFrameVariables(gameTime, shoes.Position);

			// Handles delaying the collision between the guy and shoes so that they don't link back together too quickly.
			stopDelayingCollisionWithGuyAndShoesIfPossible();

			// Set the position to the player's position so it follows him around.
			setPositionOfGuyToPositionOfShoesIfPossible(shoes);

			// Shoot the Guy if the player clicks the left mouse button.
			shootGuyIfPossible(shoes);

			// Reset the Guy to the Shoes' position if the player clicks the right mouse button.
			// This should be disabled in game.
			//resetGuyToShoesCurrentPositionIfPossible(shoes);

			if (isGuyBeingShot)
			{
				// Takes care of movement for the Guy while he's being shot.
				handleGuyMovementWhenBeingShot();

				// Set the Shoes' position to the Guys' upon collision.
				setShoesPositionToGuyUponCollisionIfPossible(shoes);

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
			Air.turnOffAirCannonsIfPossible(CurrentCollidingTile, PreviousCollidingTile, this, null);

			// Reset the Guy and Shoes to the beginning of the level if they fall off.
			resetShoesAndGuyToLevelStartingPositionIfNecessary(ref shoes, false);

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
			if (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed && !isGuyBeingShot && !shoes.stopPlayerInputDueToLevelCompletion && !Utilities.movementLockedDueToActivePauseScreen)
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

				if (shoes.directionShoesAreRunning == State.Running_Left)
				{
					shoes.changeSpriteOfTheShoes("Running_Left", false);
					changeSpriteOfTheGuy("BeingShot_Left");
				}
				else if (shoes.directionShoesAreRunning == State.Idle_Left)
				{
					shoes.changeSpriteOfTheShoes("Idle_Left", false);
					changeSpriteOfTheGuy("BeingShot_Left");
				}
				else if (shoes.directionShoesAreRunning == State.Running_Right)
				{
					shoes.changeSpriteOfTheShoes("Running_Right", false);
					changeSpriteOfTheGuy("BeingShot_Right");
				}
				else if (shoes.directionShoesAreRunning == State.Idle_Right)
				{
					shoes.changeSpriteOfTheShoes("Idle_Right", false);
					changeSpriteOfTheGuy("BeingShot_Right");
				}
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
			if (!(currentMouseState.RightButton == ButtonState.Pressed) && 
				previousMouseState.RightButton == ButtonState.Pressed && 
				!areGuyAndShoesCurrentlyLinked && 
				!Utilities.movementLockedDueToActivePauseScreen)
			{
				isGuyBeingShot = false;
				usingLauncher = false;
				idleAnimationLockIsOn = false;
				areGuyAndShoesCurrentlyLinked = true;
				shoes.swapTexture(areGuyAndShoesCurrentlyLinked);
				Position = new Vector2(shoes.Position.X, shoes.Position.Y);
				velocity = new Vector2(0f, 0f);
				delayLaunchAfterLauncherCollisionTimer.stopTimer();
				delayBetweenLaunchesTimer.stopTimer();

				if (shoes.directionShoesAreRunning == State.Running_Left || shoes.directionShoesAreRunning == State.Idle_Left)
				{
					shoes.changeSpriteOfTheShoes("Idle_Left", true);
					changeSpriteOfTheGuy("Empty");
				}
				else if (shoes.directionShoesAreRunning == State.Running_Right || shoes.directionShoesAreRunning == State.Idle_Right)
				{
					shoes.changeSpriteOfTheShoes("Idle_Right", true);
					changeSpriteOfTheGuy("Empty");
				}
			}
		}

		/// <summary>
		/// Set the Shoes's position to the position of the Guy if the collision delay timer is completed and the Guy and Shoes are not currently linked.
		/// </summary>
		/// <param name="shoes">A reference to the Shoes.</param>
		private void setShoesPositionToGuyUponCollisionIfPossible(Shoes shoes)
		{
			if (PositionRect.Intersects(shoes.PositionRect) && !delayCollisionWithShoesAndGuy && !areGuyAndShoesCurrentlyLinked)
			{
				velocity = new Vector2(0f, 0f);
				shoes.velocity = new Vector2(0f, 0f);
				shoes.Position = new Vector2(Position.X, Position.Y + 40);
				isGuyBeingShot = false;
				shoes.stopPlayerInput = true;
				idleAnimationLockIsOn = false;
				delayCollisionWithShoesAndGuy = true;
				areGuyAndShoesCurrentlyLinked = true;
				shoes.swapTexture(areGuyAndShoesCurrentlyLinked);
				SoundEffectHandler.stopShoesRunningEffect();

				if (shoes.directionShoesAreRunning == State.Running_Left || shoes.directionShoesAreRunning == State.Idle_Left)
				{
					shoes.changeSpriteOfTheShoes("Idle_Left", true);
					changeSpriteOfTheGuy("Empty");
				}
				else if (shoes.directionShoesAreRunning == State.Running_Right || shoes.directionShoesAreRunning == State.Idle_Right)
				{
					shoes.changeSpriteOfTheShoes("Idle_Right", true);
					changeSpriteOfTheGuy("Empty");
				}
			}
		}

		/// <summary>
		/// Resets the Shoes and Guy to the current level's starting position if the Guy falls below the screen.
		/// </summary>
		/// <param name="shoes"></param>
		public void resetShoesAndGuyToLevelStartingPositionIfNecessary(ref Shoes shoes, Boolean playerRestartLevel)
		{
			if (Position.Y > 720 || playerRestartLevel)
			{
				shoes.Position = Level.playerStartingPosition;
				Position = shoes.Position;
				delayLaunchAfterLauncherCollisionTimer.resetTimer();
				delayBetweenLaunchesTimer.resetTimer();
				usingLauncher = false;
			}
		}

		// ************************
		// * END POSITION SETTING *
		// ************************
	}
}