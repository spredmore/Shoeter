using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace customAnimation
{
	class Utilities
	{
		/// <summary>
		/// Converts an angle in radians to a Vector2.
		/// </summary>
		/// <param name="angle">The angle in radians to convert to a Vector2.</param>
		/// <param name="normalize">Specifies to turn the Vector2 into a unit Vector.</param>
		/// <returns>A Vector2 that represents an angle in radians.</returns>
		public static Vector2 Vector2FromAngle(double angle, bool normalize = true)
		{
			Vector2 vector = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
			if (vector != Vector2.Zero && normalize)
				vector.Normalize();
			return vector;
		}
	}
}
