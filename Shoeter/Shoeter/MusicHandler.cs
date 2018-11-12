using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Shoeter
{
	class MusicHandler
	{
		public static void PlayMusic(int currentLevel, ref ContentManager content)
		{
			Song song;
			Random randomizer = new Random();
			int bonusLevelRandomMusic = randomizer.Next(0, 5);

			if (currentLevel == 0 || (Level.bonusLevelsSelected && bonusLevelRandomMusic == 0))
			{
				song = content.Load<Song>("Music/HillbillyWorld");
			}
			else if (currentLevel == 1 || (Level.bonusLevelsSelected && bonusLevelRandomMusic == 1))
			{
				song = content.Load<Song>("Music/HumanZoo");
			}
			else if (currentLevel == 2 || (Level.bonusLevelsSelected && bonusLevelRandomMusic == 2))
			{
				song = content.Load<Song>("Music/SewerWorld");
			}
			else if (currentLevel == 3 || (Level.bonusLevelsSelected && bonusLevelRandomMusic == 3))
			{
				song = content.Load<Song>("Music/HumanSlaughterHouse");
			}
			else if (currentLevel == 4 || (Level.bonusLevelsSelected && bonusLevelRandomMusic == 4))
			{
				song = content.Load<Song>("Music/CarnivalWorld");
			}
			else if ((currentLevel == -1 || currentLevel == 5) || (Level.bonusLevelsSelected && bonusLevelRandomMusic == 5))
			{
				song = content.Load<Song>("Music/MainTheme");
			}
			else
			{
				song = content.Load<Song>("Music/HowToPlay");
			}

			MediaPlayer.Volume = 1.0f;
			MediaPlayer.Play(song);
			MediaPlayer.IsRepeating = true;
		}

		/// <summary>
		/// Handles fading out the music when the player completes a level.
		/// </summary>
		/// <param name="stopPlayerInputDueToLevelCompletion"></param>
		public static void FadeOutMusicIfPossible(Boolean stopPlayerInputDueToLevelCompletion)
		{
			if(stopPlayerInputDueToLevelCompletion)
			{
				MediaPlayer.Volume -= 0.01f;
			}
		}

		/// <summary>
		/// Stops music from being played.
		/// </summary>
		public static void StopMusic()
		{
			MediaPlayer.Stop();
		}
	}
}
