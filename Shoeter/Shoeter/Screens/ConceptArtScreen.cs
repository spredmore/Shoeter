﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Shoeter
{
	class ConceptArtScreen : GameScreen
	{
		#region Fields

		ContentManager content;
		Texture2D backgroundTexture;
		byte currentConceptArtScreen;
		KeyboardState currentKeyboardState;
		KeyboardState previousKeyboardState;

		float pauseAlpha;

		#endregion

		#region Initialization


		/// <summary>
		/// Constructor.
		/// </summary>
		public ConceptArtScreen()
		{
			TransitionOnTime = TimeSpan.FromSeconds(1.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);
			currentConceptArtScreen = 28;
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

			backgroundTexture = content.Load<Texture2D>("Sprites/Concept Art/ConceptArt1");

			// A real game would probably have more content than this sample, so
			// it would take longer to load. We simulate that by delaying for a
			// while, giving you a chance to admire the beautiful loading screen.
			Thread.Sleep(1000);

			// once the load has finished, we use ResetElapsedTime to tell the game's
			// timing mechanism that we have just finished a very long frame, and that
			// it should not try to catch up.
			ScreenManager.Game.ResetElapsedTime();

			// Don't play music on the Concept Art screen.
			MusicHandler.StopMusic();
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

			currentKeyboardState = Keyboard.GetState();

			determineWhichConceptArtToShow();

			// Gradually fade in or out depending on whether we are covered by the pause screen.
			if (coveredByOtherScreen)
			{
				pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
			}

			else
			{
				pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);
			}

			previousKeyboardState = currentKeyboardState;
		}

		private void determineWhichConceptArtToShow()
		{
			if (currentKeyboardState.IsKeyDown(Keys.A) && previousKeyboardState.IsKeyUp(Keys.A))
			{
				if (currentConceptArtScreen == 1)
				{
					currentConceptArtScreen = 28;
				}
				else
				{
					currentConceptArtScreen -= 1;
				}
			}
			else if (currentKeyboardState.IsKeyDown(Keys.D) && previousKeyboardState.IsKeyUp(Keys.D))
			{
				if (currentConceptArtScreen == 28)
				{
					currentConceptArtScreen = 1;
				}
				else
				{
					currentConceptArtScreen += 1;
				}
			}

			backgroundTexture = content.Load<Texture2D>("Sprites/Concept Art/ConceptArt" + currentConceptArtScreen.ToString());
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

			if (input.IsPauseGame(ControllingPlayer))
			{
				ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
			}
		}


		/// <summary>
		/// Draws the gameplay screen.
		/// </summary>
		public override void Draw(GameTime gameTime)
		{
			// This game has a blue background. Why? Because!
			ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

			// Our player and enemy are both actually just text strings.
			SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
			Rectangle fullscreen = new Rectangle(0, 0, 1280, 720);

			spriteBatch.Begin();

			spriteBatch.Draw(backgroundTexture, fullscreen, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

			spriteBatch.End();

			// If the game is transitioning on or off, fade it out to black.
			if (TransitionPosition > 0 || pauseAlpha > 0)
			{
				float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

				ScreenManager.FadeBackBufferToBlack(alpha);
			}
		}


		#endregion
	}
}
