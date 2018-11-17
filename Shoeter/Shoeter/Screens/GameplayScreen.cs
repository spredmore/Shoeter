#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Shoeter
{
	/// <summary>
	/// This screen implements the actual game logic. It is just a
	/// placeholder to get the idea across: you'll probably want to
	/// put some more interesting gameplay in here!
	/// </summary>
	class GameplayScreen : GameScreen
	{
		#region Fields

		ContentManager content;
		SpriteFont gameFont;

		Vector2 playerPosition = new Vector2(100, 100);
		Vector2 enemyPosition = new Vector2(100, 100);

		Random random = new Random();

		float pauseAlpha;

		public SpriteBatch spriteBatch;
		private KeyboardState oldKeyboardState;
		private KeyboardState newKeyboardState;
		private MouseState oldMouseState;
		private MouseState newMouseState;
		private Boolean displayInterface;

		// The shoes
		Shoes shoes;

		// The guy.
		Guy guy;

		// The level.
		Level level;

		// The debugging font.
		public static SpriteFont debugFont;

		Rectangle mouseRect;

		String debug;

		Boolean bonusLevelsSelected;
		Boolean hasEndOfGameSoundEffectPlayed;
		Boolean slapEffectLocked;

		#endregion

		#region Initialization


		/// <summary>
		/// Constructor.
		/// </summary>
		public GameplayScreen(Boolean bonusLevelsSelected)
		{
			TransitionOnTime = TimeSpan.FromSeconds(1.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);
			displayInterface = false;
			this.bonusLevelsSelected = bonusLevelsSelected;
			this.hasEndOfGameSoundEffectPlayed = false;
			this.slapEffectLocked = false;

			if (!bonusLevelsSelected)
			{
				Level.currentLevel = -1; // -1
				Level.bonusLevelsSelected = false;
			}
			else
			{
				Level.currentLevel = 5;
				Level.bonusLevelsSelected = true;
			}
		}


		/// <summary>
		/// Load graphics content for the game.
		/// </summary>
		public override void LoadContent()
		{
			if (content == null)
			{
				content = new ContentManager(ScreenManager.Game.Services, "Content");
			}

			gameFont = content.Load<SpriteFont>("Fonts/gamefont");

			// A real game would probably have more content than this sample, so
			// it would take longer to load. We simulate that by delaying for a
			// while, giving you a chance to admire the beautiful loading screen.
			Thread.Sleep(1000);

			// once the load has finished, we use ResetElapsedTime to tell the game's
			// timing mechanism that we have just finished a very long frame, and that
			// it should not try to catch up.
			ScreenManager.Game.ResetElapsedTime();

			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = ScreenManager.SpriteBatch;

			// Create and load the level.
			level = new Level(content);			

			// Load level and music.
			level.LoadLevel();

			// Create the Shoes.
			shoes = new Shoes(level.getPlayerStartingPosition(), content.Load<Texture2D>("Sprites/Shoes32x48"), Character.State.Idle_Right, 0, 32, 48, 0, spriteBatch, 720, 1280, Keys.W, Keys.A, Keys.S, Keys.D, content);

			// Set the initial position of the player.
			shoes.Position = level.getPlayerStartingPosition();

			// Create the Guy.
			guy = new Guy(content.Load<Texture2D>("Sprites/Guy32x48"), spriteBatch, 0, 0, 32, 48, 720, 1280, content);

			// Load the debug font. We use this for debugging purposes.
			debugFont = content.Load<SpriteFont>("Fonts/debugFont");

			// Loads the sound effects that will be used in the game.
			SoundEffectHandler.LoadSoundEffects(content);

			mouseRect = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 16, 16);
		}


		/// <summary>
		/// Unload graphics content used by the game.
		/// </summary>
		public override void UnloadContent()
		{
			content.Unload();
		}


		#endregion

		#region Update and Draw


		/// <summary>
		/// Updates the state of the game. This method checks the GameScreen.IsActive
		/// property, so the game will stop updating when the pause menu is active,
		/// or if you tab away to a different application.
		/// </summary>
		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, false);

			newKeyboardState = Keyboard.GetState();
			newMouseState = Mouse.GetState();

			shoes.Update(gameTime, ref guy);
			guy.Update(gameTime, ref shoes, ref level);

			if (guy.AreGuyAndShoesCurrentlyLinked && newMouseState.LeftButton == ButtonState.Pressed && 
				!Utilities.movementLockedDueToActivePauseScreen)
			{
				TrajectoryLineHandler.Update(ref guy);
			}

			mouseRect = new Rectangle(newMouseState.X, newMouseState.Y, 16, 16);

			// Handles for if the player hits the Guy with the mouse cursor.
			handleSlappingSoundEffect();

			foreach (Air air in Air.allAirs)
			{
				air.Update(gameTime, ref shoes, ref guy);
			}

			// Hide and display the interface.
			if (!newKeyboardState.IsKeyDown(Keys.F10) && oldKeyboardState.IsKeyDown(Keys.F10))
			{
				if (displayInterface)
				{
					displayInterface = false;
				}
				else
				{
					displayInterface = true;
				}
			}

			MusicHandler.FadeOutMusicIfPossible(shoes.stopPlayerInputDueToLevelCompletion);

			// If the player has won the game, play the end of game sound effect.
			playEndOfGameSoundEffectIfPossible();

			// Exit to the main menu if possible.
			exitToMainMenuIfNeeded();

			// If the player presses the 'R' key, reset the player to the beginning of the level.
			restartLevelIfNecessary();

			oldKeyboardState = newKeyboardState;
			oldMouseState = newMouseState;

			// Gradually fade in or out depending on whether we are covered by the pause screen.
			if (coveredByOtherScreen)
			{
				pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
			}

			else
			{
				pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);
			}
		}

		/// <summary>
		/// Draws the gameplay screen.
		/// </summary>
		public override void Draw(GameTime gameTime)
		{
			// This game has a blue background. Why? Because!
			ScreenManager.GraphicsDevice.Clear(Color.Gainsboro);

			spriteBatch.Begin();

			level.drawBackground(spriteBatch, ref content);

			guy.Sprite.Draw();

			shoes.Sprite.Draw();

			foreach (Air air in Air.allAirs)
			{
				air.Draw();
			}

			level.drawForeground(spriteBatch, ref content);
			
			// Draw the level.
			level.Draw(spriteBatch, true);

			if(Level.bonusLevelsSelected || displayInterface)
			{
				level.Draw(spriteBatch, false);
			}

			if (displayInterface)
			{
				spriteBatch.DrawString(debugFont, "Angle between mouse and player: " + guy.angleBetweenGuyAndMouseCursor.ToString(), new Vector2(0, 0), Color.Black);
				spriteBatch.DrawString(debugFont, "Guy - Power (Scroll Wheel): " + guy.powerOfLauncherBeingUsed.ToString(), new Vector2(0, 20), Color.Black);
				spriteBatch.DrawString(debugFont, "Guy - Gravity (./3): " + guy.gravity.ToString(), new Vector2(0, 40), Color.Black);
				spriteBatch.DrawString(debugFont, "Shoes - Air Movement (7/8): " + shoes.airMovementSpeed.ToString(), new Vector2(0, 60), Color.Black);
				spriteBatch.DrawString(debugFont, "Shoes - Ground Movement (4/5): " + shoes.groundMovementSpeed.ToString(), new Vector2(0, 80), Color.Black);
				spriteBatch.DrawString(debugFont, "Shoes - Jump Impulse (1/2): " + shoes.jumpImpulse.ToString(), new Vector2(0, 100), Color.Black);
				spriteBatch.DrawString(debugFont, "Shoes - Gravity (9/6): " + shoes.gravity.ToString(), new Vector2(0, 120), Color.Black);
				spriteBatch.DrawString(debugFont, "Shoes - Fall From Tile Rate (/ / -): " + shoes.fallFromTileRate.ToString(), new Vector2(0, 140), Color.Black);
				spriteBatch.DrawString(debugFont, "Shoes - Preset (F Keys): " + shoes.preset, new Vector2(0, 160), Color.Black);
				spriteBatch.DrawString(debugFont, "Interface Linked (F12): " + shoes.interfaceLinked.ToString(), new Vector2(0, 180), Color.Black);
				spriteBatch.DrawString(debugFont, "Guy Debug: " + guy.debug, new Vector2(0, 200), Color.Black);
				spriteBatch.DrawString(debugFont, "Guy Debug2: " + guy.debug2, new Vector2(0, 220), Color.Black);
				spriteBatch.DrawString(debugFont, "Guy Debug3: " + guy.debug3, new Vector2(0, 240), Color.Black);
				spriteBatch.DrawString(debugFont, "Shoes Debug: " + shoes.debug.ToString(), new Vector2(0, 260), Color.Black);
				spriteBatch.DrawString(debugFont, "Shoes Debug2: " + shoes.debug2.ToString(), new Vector2(0, 280), Color.Black);
				spriteBatch.DrawString(debugFont, "Shoes Debug3: " + shoes.debug3.ToString(), new Vector2(0, 300), Color.Black);
				spriteBatch.DrawString(debugFont, "Character Debug: " + Character.charDebug, new Vector2(0, 320), Color.Black);
				spriteBatch.DrawString(debugFont, "Shoes State: " + shoes.CurrentState.ToString(), new Vector2(0, 340), Color.Black);
				spriteBatch.DrawString(debugFont, "Guy State: " + guy.CurrentState.ToString(), new Vector2(0, 360), Color.Black);
				spriteBatch.DrawString(debugFont, "AnimatedSprite Debug: " + AnimatedSprite.debug, new Vector2(0, 380), Color.Black);
				spriteBatch.DrawString(debugFont, "Air Debug: " + Air.debug, new Vector2(0, 400), Color.Black);
				spriteBatch.DrawString(debugFont, "Air Debug2: " + Air.debug2, new Vector2(0, 420), Color.Black);
				spriteBatch.DrawString(debugFont, "Game1 Debug: " + debug, new Vector2(0, 440), Color.Black);
			}

			// Draw the trajectory line while the left mouse button is being held.
			if (guy.AreGuyAndShoesCurrentlyLinked && newMouseState.LeftButton == ButtonState.Pressed)
			{
				TrajectoryLineHandler.Draw(spriteBatch, ref content);
			}

			// Write the power level below the Shoes or Guy when using a Launcher.
			displayPowerLevelIfPossible(ref guy, ref shoes);

			// Check to see if the player won. If they did, display the level completion picture.
			drawLevelCompleteImageIfPossible();
			
			// Draw the faded out screen if possible.
			guy.fadeHandler.Draw(content.Load<Texture2D>("Backgrounds/blank"));

			// Draw the level name while the screen is faded out.
			if (guy.fadeHandler.HoldingWhileFaded)
			{
				spriteBatch.DrawString(ScreenManager.Font, level.getCurrentLevelName(), getLevelTransitionTextLocation(), Color.White);
			}
			
			spriteBatch.End();

			// If the game is transitioning on or off, fade it out to black.
			// *** Only for transitioning from the Main Menu to Level 1. ***
			if (TransitionPosition > 0 || pauseAlpha > 0)
			{
				float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

				ScreenManager.FadeBackBufferToBlack(alpha);
			}
		}

		/// <summary>
		/// Check to see if the player won. If they did, display the level completion picture.
		/// </summary>
		private void drawLevelCompleteImageIfPossible()
		{
			if (shoes.stopPlayerInputDueToLevelCompletion)
			{
				if(Level.currentLevel == 0)
				{
					spriteBatch.Draw(content.Load<Texture2D>("Sprites/EndOfLevelPictures/HillbillyBBQ"), new Vector2(0, 0), Color.White);
				}
				else if (Level.currentLevel == 1)
				{
					spriteBatch.Draw(content.Load<Texture2D>("Sprites/EndOfLevelPictures/HumanZoo"), new Vector2(0, 0), Color.White);
				}
				else if (Level.currentLevel == 2)
				{
					spriteBatch.Draw(content.Load<Texture2D>("Sprites/EndOfLevelPictures/SewerPersuer"), new Vector2(0, 0), Color.White);
				}
				else if (Level.currentLevel == 3)
				{
					spriteBatch.Draw(content.Load<Texture2D>("Sprites/EndOfLevelPictures/HumanSlaughterHouse"), new Vector2(0, 0), Color.White);
				}
				else if (Level.currentLevel == 4)
				{
					spriteBatch.Draw(content.Load<Texture2D>("Sprites/EndOfLevelPictures/CarnivaloftheWood"), new Vector2(0, 0), Color.White);
				}
				else if (Level.currentLevel == 5)
				{
					spriteBatch.Draw(content.Load<Texture2D>("Sprites/EndOfLevelPictures/ShoeterComplete"), new Vector2(0, 0), Color.White);
				}
				else
				{
					spriteBatch.Draw(content.Load<Texture2D>("Sprites/EndOfLevelPictures/BonusLevel"), new Vector2(0, 0), Color.White);
				}
			}
		}

		/// <summary>
		/// Displays the power level under the Guy and Shoes while they are using a Launcher.
		/// </summary>
		/// <param name="guy">A reference to the Guy.</param>
		/// <param name="shoes">A reference to the Shoes.</param>
		private void displayPowerLevelIfPossible(ref Guy guy, ref Shoes shoes)
		{
			// Display under only the Guy.
			if (guy.usingLauncher && !guy.AreGuyAndShoesCurrentlyLinked)
			{
				spriteBatch.DrawString(ScreenManager.FontSmall, guy.powerOfLauncherBeingUsed.ToString(), new Vector2(guy.Position.X, guy.Position.Y + 60), Color.LimeGreen);
			}
			// Display under the Shoes.
			else if (shoes.delayLaunchAfterLauncherCollisionTimer.TimerStarted &&
				!shoes.delayLaunchAfterLauncherCollisionTimer.TimerCompleted)
			{
				spriteBatch.DrawString(ScreenManager.FontSmall, guy.powerOfLauncherBeingUsed.ToString(), new Vector2(shoes.Position.X + 15, shoes.Position.Y + 60), Color.LimeGreen);
			}
		}

		/// <summary>
		/// Lets the game respond to player input. Unlike the Update method,
		/// this will only be called when the gameplay screen is active.
		/// </summary>
		public override void HandleInput(InputState input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}

			// Look up inputs for the active player profile.
			int playerIndex = (int)ControllingPlayer.Value;

			KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];

			if (input.IsPauseGame(ControllingPlayer))
			{
				ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);

				// Lock Guy and Shoes from moving while the pause screen is up
				Utilities.movementLockedDueToActivePauseScreen = true;
			}
		}

		/// <summary>
		/// Calculates and returns the location to draw the next level text.
		/// </summary>
		/// <returns>Returns the location to draw the next level text</returns>
		private Vector2 getLevelTransitionTextLocation()
		{
			Vector2 windowSize = new Vector2(1280, 720);
			Vector2 textSize = ScreenManager.Font.MeasureString(level.getCurrentLevelName());
			
			return (windowSize - textSize) / 2;
		}

		/// <summary>
		/// Watches to see if Level.exitGame is true. If it is, exit to the main menu upon level completion.
		/// </summary>
		private void exitToMainMenuIfNeeded()
		{
			if (Level.exitGame)
			{
				Level.exitGame = false;
				LoadingScreen.Load(ScreenManager, false, null, "Main Menu", new BackgroundScreen(), new MainMenuScreen());
			}
		}

		/// <summary>
		/// Plays the end of game sound effect if possible.
		/// </summary>
		private void playEndOfGameSoundEffectIfPossible()
		{
			if (shoes.stopPlayerInputDueToLevelCompletion && 
				Level.currentLevel == 5 &&
				!hasEndOfGameSoundEffectPlayed)
			{
				hasEndOfGameSoundEffectPlayed = true;
				SoundEffectHandler.playEndOfGameCompleteSoundEffect();
			}
			
		}

		/// <summary>
		/// Plays one of the slapping sound effects based on a random number and if the player has gone for a new slap.
		/// </summary>
		private void handleSlappingSoundEffect()
		{
			if (mouseRect.Intersects(guy.PositionRect) &&
					!slapEffectLocked)
			{
				Random randomizer = new Random();
				int randomNumber = randomizer.Next(0, 2);
				slapEffectLocked = true;

				if (randomNumber == 0)
				{
					SoundEffectHandler.playSlap1SoundEffect();
				}
				else
				{
					SoundEffectHandler.playSlap2SoundEffect();
				}
			}
			else if(!mouseRect.Intersects(guy.PositionRect) && 
				slapEffectLocked)
			{
				slapEffectLocked = false;
			}
		}

		/// <summary>
		/// Resets the Guy and Shoes to the beginning of the level if the player presses the 'R' key.
		/// </summary>
		private void restartLevelIfNecessary()
		{
			if (!newKeyboardState.IsKeyDown(Keys.R) && oldKeyboardState.IsKeyDown(Keys.R))
			{
				guy.resetShoesAndGuyToLevelStartingPositionIfNecessary(ref shoes, true);
			}
		}

		#endregion
	}
}