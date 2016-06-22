using UnityEngine;
using System;

namespace Pathfinding
{
	public static class Heuristic
	{
		public static float sqrt2;

		public delegate int Function (Point point);

		static Heuristic ()
		{
			sqrt2 = Mathf.Sqrt (2f);
		}

		public static int Manhattan (Point point)
		{
			return point.x + point.y;
		}

		public static int Euclidean (Point point)
		{
			return Mathf.FloorToInt (Mathf.Sqrt ((float)(point.x * point.x + point.y * point.y)));
		}

		public static int Octile (Point point)
		{
			float F = sqrt2 - 1f;
			return Mathf.FloorToInt ((point.x < point.y) ? F * point.x + point.y : F * point.y + point.x);
		}

		public static int Chebyshev (Point point)
		{
			return Mathf.Max (point.x, point.y);
		}
	}
}

