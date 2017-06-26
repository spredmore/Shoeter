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

		public GraphicsDeviceManager graphics;
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

		// Stores if we are facing right or not.
		SpriteEffects facingRight;

		Rectangle mouseRect;

		AnimatedSprite testAnimatedSprite;
		AnimatedSprite testAnimatedSprite2;

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
			displayInterface = true;
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

			//testAnimatedSprite = new AnimatedSprite(Content.Load<Texture2D>("Sprites/GuyIdleWithShoes"), new Vector2(25, 650), 0, 45, 48, 50, spriteBatch, 34f, MathHelper.ToRadians(0));
			//testAnimatedSprite2 = new AnimatedSprite(Content.Load<Texture2D>("Sprites/GuyRunning"), new Vector2(100, 650), 0, 37, 48, 27, spriteBatch, 34f, MathHelper.ToRadians(0));

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

			//testAnimatedSprite.Animate(gameTime);
			//testAnimatedSprite2.Animate(gameTime);

			base.Update(gameTime, otherScreenHasFocus, false);
		}


		/// <summary>
		/// Lets the game respond to player input. Unlike the Update method,
		/// this will only be called when the gameplay screen is active.
		/// </summary>
		//public override void HandleInput(InputState input)
		//{
		//    if (input == null)
		//    {
		//        throw new ArgumentNullException("input");
		//    }

		//    // Look up inputs for the active player profile.
		//    int playerIndex = (int)ControllingPlayer.Value;

		//    KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
		//    GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

		//    // The game pauses either if the user presses the pause button, or if
		//    // they unplug the active gamepad. This requires us to keep track of
		//    // whether a gamepad was ever plugged in, because we don't want to pause
		//    // on PC if they are playing with a keyboard and have no gamepad at all!
		//    bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

		//    if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
		//    {
		//        ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
		//    }
		//    else
		//    {
		//        // Otherwise move the player position.
		//        Vector2 movement = Vector2.Zero;

		//        if (keyboardState.IsKeyDown(Keys.Left))
		//        {
		//            movement.X--;
		//        }

		//        if (keyboardState.IsKeyDown(Keys.Right))
		//        {
		//            movement.X++;
		//        }

		//        if (keyboardState.IsKeyDown(Keys.Up))
		//        {
		//            movement.Y--;
		//        }

		//        if (keyboardState.IsKeyDown(Keys.Down))
		//        {
		//            movement.Y++;
		//        }

		//        Vector2 thumbstick = gamePadState.ThumbSticks.Left;

		//        movement.X += thumbstick.X;
		//        movement.Y -= thumbstick.Y;

		//        if (movement.Length() > 1)
		//        {
		//            movement.Normalize();
		//        }

		//        playerPosition += movement * 2;
		//    }
		//}


		/// <summary>
		/// Draws the gameplay screen.
		/// </summary>
		public override void Draw(GameTime gameTime)
		{
			ScreenManager.GraphicsDevice.Clear(Color.Gainsboro);

			spriteBatch.Begin();

			foreach (Air air in Air.allAirs)
			{
				air.Draw();
			}

			// Draw the level.
			level.Draw(spriteBatch);

			//guy.Draw();
			guy.Sprite.Draw();
			//spriteBatch.Draw(Content.Load<Texture2D>("Sprites/32x48Hitbox"), new Rectangle(guy.Sprite.RotatedRect.X, guy.Sprite.RotatedRect.Y, guy.Sprite.RotatedRect.Width, guy.Sprite.RotatedRect.Height), Color.White);
			//spriteBatch.Draw(Content.Load<Texture2D>("Sprites/32x48Hitbox2"), guy.FutureRectangleRect, Color.White);
			//spriteBatch.Draw(Content.Load<Texture2D>("Sprites/16x16HitboxUp"), guy.TileCollisionRectangle, Color.White);

			//shoes.Draw();
			shoes.Sprite.Draw();
			//spriteBatch.Draw(Content.Load<Texture2D>("Sprites/32x48Hitbox"), shoes.PositionRect, Color.White);
			//spriteBatch.Draw(Content.Load<Texture2D>("Sprites/32x48Hitbox2"), new Rectangle(shoes.Sprite.RotatedRect.X, shoes.Sprite.RotatedRect.Y, shoes.Sprite.RotatedRect.Width, shoes.Sprite.RotatedRect.Height), Color.White);
			//spriteBatch.Draw(Content.Load<Texture2D>("Sprites/16x16HitboxUp"), shoes.TileCollisionRectangle, Color.White);

			// Draw the debug font.
			spriteBatch.DrawString(debugFont, "Angle between mouse and player: " + guy.angleBetweenGuyAndMouseCursor.ToString(), new Vector2(0, 0), Color.Black);
			spriteBatch.DrawString(debugFont, "Guy - Power (Scroll Wheel): " + guy.powerOfLauncherBeingUsed.ToString(), new Vector2(0, 20), Color.Black);

			debug = mouseRect.X.ToString() + " " + mouseRect.Y.ToString();

			if (displayInterface)
			{
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
		}


		#endregion
	}
}