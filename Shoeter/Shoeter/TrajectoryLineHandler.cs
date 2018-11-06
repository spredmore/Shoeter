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
		public static List<Vector2> pointsAlongTrajectory = new List<Vector2>();	// Stores a list of points that will make up the Guy's trajectory.

		/// <summary>
		/// Update method that's called every frame. Calculates the points that the Guy will travel when shot.
		/// </summary>
		/// <param name="guy">A reference to the Guy.</param>
		public static void Update(ref Guy guy)
		{
			pointsAlongTrajectory.Clear();

			Vector2 velocity = Utilities.Vector2FromAngle(MathHelper.ToRadians(guy.angleBetweenGuyAndMouseCursor)) * guy.powerOfLauncherBeingUsed;
			velocity *= -1;

			// Until there have been 200 points collected, create the trajectory line that the Guy would take if shot.
			while (pointsAlongTrajectory.Count < 200)
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

		/// <summary>
		/// Draw method that is called every frame and draws the trajectory line.
		/// </summary>
		/// <param name="spriteBatch">Enables a group of sprites to be drawn using the same settings.</param>
		/// <param name="content">Run-time component which loads managed objects from the binary files produced by the design time content pipeline.</param>
		public static void Draw(SpriteBatch spriteBatch, ref ContentManager content)
		{
			foreach (Vector2 point in pointsAlongTrajectory)
			{
				spriteBatch.Draw(content.Load<Texture2D>("Backgrounds/blank"), new Rectangle((int)point.X, (int)point.Y, 3, 3), Color.GreenYellow);
			}
		}
	}
}
