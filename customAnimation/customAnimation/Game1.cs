using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace customAnimation
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		public GraphicsDeviceManager graphics;
		public SpriteBatch spriteBatch;

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

		String debug;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			graphics.PreferredBackBufferHeight = 720;
			graphics.PreferredBackBufferWidth = 1280;      
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			this.IsMouseVisible = true;
			base.Initialize();
		}

		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// Create and load the level.
			level = new Level(this.Content);

			// Load level.
			level.LoadLevel();
		   
			// Create the Shoes.
			shoes = new Shoes(Content.Load<Texture2D>("Sprites/Shoes32x48"), Character.State.Idle, 0, 32, 48, 0, spriteBatch, graphics.PreferredBackBufferHeight, graphics.PreferredBackBufferWidth, Keys.W, Keys.A, Keys.S, Keys.D, this.Content);

			// Set the initial position of the player.
			shoes.Position = level.getPlayerStartingPosition();

			// Create the Guy.
			guy = new Guy(Content.Load<Texture2D>("Sprites/Guy32x48"), spriteBatch, 0, 0, 32, 48, graphics.PreferredBackBufferHeight, graphics.PreferredBackBufferWidth);

			// Load the debug font. We use this for debugging purposes.
			debugFont = Content.Load<SpriteFont>("debugFont"); 
		}

		protected override void UnloadContent()
		{
			// It's probably a good idea to destory the player, level, etc when the game is over.
		}

		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
			{
				this.Exit();
			}	

			shoes.Update(gameTime, ref guy);
			guy.Update(gameTime, ref shoes, ref level);

			foreach(Air air in Air.allAirs)
			{
				air.Update(gameTime, ref shoes);
			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Gainsboro);

			spriteBatch.Begin();

			foreach (Air air in Air.allAirs)
			{
				air.Draw();
			}

			// Draw the level.
			level.Draw(spriteBatch);

			guy.Draw();
			spriteBatch.Draw(Content.Load<Texture2D>("Sprites/32x48Hitbox"), guy.Position, Color.White);
			spriteBatch.Draw(Content.Load<Texture2D>("Sprites/32x48Hitbox2"), guy.FutureRectangleRect, Color.White);
			spriteBatch.Draw(Content.Load<Texture2D>("Sprites/16x16HitboxUp"), guy.TileCollisionRectangle, Color.White);

			shoes.Draw();
			spriteBatch.Draw(Content.Load<Texture2D>("Sprites/32x48Hitbox"), shoes.PositionRect, Color.White);
			spriteBatch.Draw(Content.Load<Texture2D>("Sprites/32x48Hitbox2"), shoes.FutureRectangleRect, Color.White);
			spriteBatch.Draw(Content.Load<Texture2D>("Sprites/16x16HitboxUp"), shoes.TileCollisionRectangle, Color.White);

			// Draw the debug font.
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
			spriteBatch.DrawString(debugFont, "Shoes Debug: " + shoes.debug.ToString(), new Vector2(0, 240), Color.Black);
			spriteBatch.DrawString(debugFont, "Shoes Debug2: " + shoes.debug2.ToString(), new Vector2(0, 260), Color.Black);
			spriteBatch.DrawString(debugFont, "Character Debug: " + Character.charDebug, new Vector2(0, 280), Color.Black);
			spriteBatch.DrawString(debugFont, "Shoes State: " + shoes.PlayerState.ToString(), new Vector2(0, 300), Color.Black);
			spriteBatch.DrawString(debugFont, "Guy State: " + guy.PlayerState.ToString(), new Vector2(0, 320), Color.Black);
			spriteBatch.DrawString(debugFont, "AnimatedSprite debug: " + AnimatedSprite.debug, new Vector2(0, 340), Color.Black);
			spriteBatch.DrawString(debugFont, "Air debug: " + Air.debug, new Vector2(0, 360), Color.Black);
			spriteBatch.DrawString(debugFont, "Game1 debug: " + debug, new Vector2(0, 380), Color.Black);
			
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}