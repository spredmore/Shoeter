#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Shoeter
{
	/// <summary>
	/// The main menu screen is the first thing displayed when the game starts up.
	/// </summary>
	class MainMenuScreen : MenuScreen
	{
		ContentManager content;
		MenuEntry playGameMenuEntry;
		MenuEntry howToPlayMenuEntry;
		MenuEntry bonusLevelsMenuEntry;
		MenuEntry exitMenuEntry;

		#region Initialization

		/// <summary>
		/// Constructor fills in the menu contents.
		/// </summary>
		public MainMenuScreen() : base("MainMenu")
		{
			// Create our menu entries.
			playGameMenuEntry = new MenuEntry("Play Game");
			howToPlayMenuEntry = new MenuEntry("How To Play");
			bonusLevelsMenuEntry = new MenuEntry("Bonus Levels");
			exitMenuEntry = new MenuEntry("Quit Game");

			// Hook up menu event handlers.
			playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
			howToPlayMenuEntry.Selected += HowToPlayMenuEntrySelected;
			bonusLevelsMenuEntry.Selected += BonusLevelsMenuEntrySelected;
			exitMenuEntry.Selected += OnCancel;

			// Add entries to the menu.
			MenuEntries.Add(playGameMenuEntry);
			MenuEntries.Add(howToPlayMenuEntry);
			MenuEntries.Add(bonusLevelsMenuEntry);
			MenuEntries.Add(exitMenuEntry);

			Utilities.movementLockedDueToActivePauseScreen = false;
		}

		public override void LoadContent()
		{
			if (content == null)
			{
				content = new ContentManager(ScreenManager.Game.Services, "Content");
			}

			playGameMenuEntry.prepareToDraw(content, ScreenManager.SpriteBatch, "MainMenu");
			howToPlayMenuEntry.prepareToDraw(content, ScreenManager.SpriteBatch, "MainMenu");
			bonusLevelsMenuEntry.prepareToDraw(content, ScreenManager.SpriteBatch, "MainMenu");
			exitMenuEntry.prepareToDraw(content, ScreenManager.SpriteBatch, "MainMenu");
		}


		#endregion

		#region Handle Input


		/// <summary>
		/// Event handler for when the Play Game menu entry is selected.
		/// </summary>
		void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
		{
			LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, "Hillbilly BBQ", new GameplayScreen(false));
		}

		void HowToPlayMenuEntrySelected(object sender, PlayerIndexEventArgs e)
		{
			LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, "How To Play", new TutorialScreen());
		}


		/// <summary>
		/// Event handler for when the Bouns Level menu entry is selected.
		/// </summary>
		void BonusLevelsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
		{
			LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, "Bonus Level - 1", new GameplayScreen(true));
		}


		/// <summary>
		/// When the user cancels the main menu, ask if they want to exit the sample.
		/// </summary>
		protected override void OnCancel(PlayerIndex playerIndex)
		{
			const string message = "You don't really want to quit do you?";

			MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message, true);

			confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

			ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
		}


		/// <summary>
		/// Event handler for when the user selects ok on the "are you sure
		/// you want to exit" message box.
		/// </summary>
		void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
		{
			ScreenManager.Game.Exit();
		}


		#endregion
	}
}
