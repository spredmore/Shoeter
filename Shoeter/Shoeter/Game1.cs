using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Shoeter
{
	/// <summary>
	/// Sample showing how to manage different game states, with transitions
	/// between menu screens, a loading screen, the game itself, and a pause
	/// menu. This main game class is extremely simple: all the interesting
	/// stuff happens in the ScreenManager component.
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		#region Fields

		GraphicsDeviceManager graphics;
		ScreenManager screenManager;


		// By preloading any assets used by UI rendering, we avoid framerate glitches
		// when they suddenly need to be loaded in the middle of a menu transition.
		static readonly string[] preloadAssets =
		{
			"Backgrounds/gradient",
		};


		#endregion

		#region Initialization


		/// <summary>
		/// The main game constructor.
		/// </summary>
		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			graphics.PreferredBackBufferWidth = 1280;
			graphics.PreferredBackBufferHeight = 720;

			// Create the screen manager component.
			screenManager = new ScreenManager(this);

			Components.Add(screenManager);

			// Activate the first screens.
			screenManager.AddScreen(new BackgroundScreen(), null);
			screenManager.AddScreen(new MainMenuScreen(), null);
		}


		/// <summary>
		/// Loads graphics content.
		/// </summary>
		protected override void LoadContent()
		{
			foreach (string asset in preloadAssets)
			{
				Content.Load<object>(asset);
			}
		}

		protected override void Initialize()
		{
			this.IsMouseVisible = true;
			base.Initialize();
		}


		#endregion

		#region Draw


		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.Black);

			// The real drawing happens inside the screen manager component.
			base.Draw(gameTime);
		}


		#endregion
	}

}