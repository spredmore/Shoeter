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

		#endregion

		#region Initialization


		/// <summary>
		/// Constructor.
		/// </summary>
		public GameplayScreen()
		{
			TransitionOnTime = TimeSpan.FromSeconds(1.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);
			displayInterface = false;
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

			// Load level.
			level.LoadLevel();

			// Create the Shoes.
			shoes = new Shoes(level.getPlayerStartingPosition(), content.Load<Texture2D>("Sprites/Shoes32x48"), Character.State.Idle_Right, 0, 32, 48, 0, spriteBatch, 720, 1280, Keys.W, Keys.A, Keys.S, Keys.D, content);

			// Set the initial position of the player.
			shoes.Position = level.getPlayerStartingPosition();

			// Create the Guy.
			guy = new Guy(content.Load<Texture2D>("Sprites/Guy32x48"), spriteBatch, 0, 0, 32, 48, 720, 1280, content);

			// Load the debug font. We use this for debugging purposes.
			debugFont = content.Load<SpriteFont>("Fonts/debugFont");

			MouseState currentMouseState;
			currentMouseState = Mouse.GetState();
			mouseRect = new Rectangle(currentMouseState.X, currentMouseState.Y, 16, 16);
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

			shoes.Update(gameTime, ref guy);
			guy.Update(gameTime, ref shoes, ref level);

			MouseState currentMouseState;
			currentMouseState = Mouse.GetState();
			mouseRect = new Rectangle(currentMouseState.X, currentMouseState.Y, 16, 16);

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

			oldKeyboardState = newKeyboardState;

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

			//guy.Draw();
			guy.Sprite.Draw();
			//spriteBatch.Draw(content.Load<Texture2D>("Sprites/32x48Hitbox"), new Rectangle(guy.Sprite.RotatedRect.X, guy.Sprite.RotatedRect.Y, guy.Sprite.RotatedRect.Width, guy.Sprite.RotatedRect.Height), Color.White);
			//spriteBatch.Draw(content.Load<Texture2D>("Sprites/32x48Hitbox2"), guy.FutureRectangleRect, Color.White);
			//spriteBatch.Draw(content.Load<Texture2D>("Sprites/16x16HitboxUp"), guy.TileCollisionRectangle, Color.White);

			//shoes.Draw();
			shoes.Sprite.Draw();
			//spriteBatch.Draw(content.Load<Texture2D>("Sprites/32x48Hitbox"), shoes.PositionRect, Color.White);
			//spriteBatch.Draw(content.Load<Texture2D>("Sprites/32x48Hitbox2"), new Rectangle(shoes.Sprite.RotatedRect.X, shoes.Sprite.RotatedRect.Y, shoes.Sprite.RotatedRect.Width, shoes.Sprite.RotatedRect.Height), Color.White);
			//spriteBatch.Draw(content.Load<Texture2D>("Sprites/16x16HitboxUp"), shoes.TileCollisionRectangle, Color.White);

			foreach (Air air in Air.allAirs)
			{
				air.Draw();
			}

			level.drawForeground(spriteBatch, ref content);
			
			// Draw the level.
			level.Draw(spriteBatch, true);

			// Check to see if the player won. If they did, display the level completion picture.
			drawLevelCompleteImageIfPossible();

			// Draw the debug font.
			spriteBatch.DrawString(debugFont, "Angle between mouse and player: " + guy.angleBetweenGuyAndMouseCursor.ToString(), new Vector2(0, 0), Color.LightSlateGray);
			spriteBatch.DrawString(debugFont, "Guy - Power (Scroll Wheel): " + guy.powerOfLauncherBeingUsed.ToString(), new Vector2(0, 20), Color.LightSlateGray);

			debug = mouseRect.X.ToString() + " " + mouseRect.Y.ToString();

			if (displayInterface)
			{
				level.Draw(spriteBatch, false);
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

			spriteBatch.Draw(content.Load<Texture2D>("Sprites/16x16HitboxUp"), mouseRect, Color.White);

			//testAnimatedSprite.Draw();
			//testAnimatedSprite2.Draw();

			spriteBatch.End();

			// If the game is transitioning on or off, fade it out to black.
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
			}
		}


		#endregion
	}
}