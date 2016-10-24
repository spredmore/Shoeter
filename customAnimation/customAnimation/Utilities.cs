using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace customAnimation
{
    class Utilities
    {
        public static Vector2 Vector2FromAngle(double angle, bool normalize = true)
        {
            Vector2 vector = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            if (vector != Vector2.Zero && normalize)
                vector.Normalize();
            return vector;
        }
    }
}
