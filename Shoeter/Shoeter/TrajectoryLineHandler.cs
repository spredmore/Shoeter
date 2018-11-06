using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Shoeter
{
	class TrajectoryLineHandler
	{
		public static List<Vector2> pointsAlongTrajectory = new List<Vector2>();

		public static void Update(ref Guy guy)
		{
			pointsAlongTrajectory.Clear();

			Vector2 velocity = Utilities.Vector2FromAngle(MathHelper.ToRadians(guy.angleBetweenGuyAndMouseCursor)) * guy.powerOfLauncherBeingUsed;
			velocity *= -1;

			while (/*tipOfLine.X < 1280 && tipOfLine.Y < 720*/pointsAlongTrajectory.Count < 500)
			{
				Vector2 tipOfLine = new Vector2();

				if (pointsAlongTrajectory.Count == 0)
				{
					tipOfLine = guy.Position;
				}
				else
				{
					velocity.Y += guy.gravity * guy.delta;
					tipOfLine = pointsAlongTrajectory[pointsAlongTrajectory.Count - 1] + velocity;
				}

				pointsAlongTrajectory.Add(tipOfLine);
			}
		}

		public static void Draw(SpriteBatch spriteBatch, ref ContentManager content)
		{
			foreach (Vector2 point in pointsAlongTrajectory)
			{
				spriteBatch.Draw(content.Load<Texture2D>("Backgrounds/blank"), new Rectangle((int)point.X, (int)point.Y, 4, 4), Color.Green);
			}
		}
	}
}
