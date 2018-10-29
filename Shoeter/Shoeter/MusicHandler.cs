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

			if (currentLevel == 0)
			{
				song = content.Load<Song>("Music/HillbillyWorld");
			}
			else if (currentLevel == 1)
			{
				song = content.Load<Song>("Music/HumanZoo");
			}
			else if (currentLevel == 2)
			{
				song = content.Load<Song>("Music/SewerWorld");
			}
			else if (currentLevel == 3)
			{
				song = content.Load<Song>("Music/HumanSlaughterHouse");
			}
			else if (currentLevel == 4)
			{
				song = content.Load<Song>("Music/CarnivalWorld");
			}
			else if (currentLevel == -1)
			{
				song = content.Load<Song>("Music/MainTheme");
			}
			else 
			{
				song = content.Load<Song>("Music/HowToPlay");
			}

			MediaPlayer.Play(song);
		}
	}
}
