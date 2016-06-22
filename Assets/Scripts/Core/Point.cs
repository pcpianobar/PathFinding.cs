using System;

namespace Pathfinding
{
	public struct Point
	{
		public int x;
		public int y;

		public Point (int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public void Set (int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public Point Displacement (Point other)
		{
			return new Point (Math.Abs (x - other.x), Math.Abs (y - other.y));
		}

		public override string ToString ()
		{
			return string.Format ("x = {0}, y = {1}", x, y);
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public override bool Equals (object obj)
		{
			return obj is Point && this == (Point)obj;
		}

		public Point Offset (int x, int y)
		{
			return this + new Point (x, y);
		}

		#region operation
		public static bool operator == (Point a, Point b)
		{
			return !(a != b);
		}

		public static bool operator != (Point a, Point b)
		{
			return a.x != b.x || a.y != b.y;
		}

		public static Point operator - (Point a, Point b)
		{
			return new Point (a.x - b.x, a.y - b.y);
		}

		public static Point operator + (Point a, Point b)
		{
			return new Point (a.x + b.x, a.y + b.y);
		}
		#endregion
	}
}

