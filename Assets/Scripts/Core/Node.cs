using System;
namespace Pathfinding
{
	/// <summary>
	/// 양방향에서 시작된 위치
	/// </summary>
	enum ByType
	{
		BY_NULL,
		BY_START,
		BY_END,
	}

	public class Node : IComparable<Node>
	{
		public Node parent;
		public Point point;
		public float g;
		public float f;
		public float? h;
		public bool opened;
		public bool closed;
		public ByType by;

		public Node (Point point)
		{
			this.point = point;
		}

		public Node (Point point, Node parent, float g, float f, float h = 0)
		{
			this.point = point;
			this.parent = parent;
			this.g = g;
			this.f = f;
			this.h = h;
		}

		public void Set (Node parent, float g, float f, float h = 0)
		{
			this.parent = parent;
			this.g = g;
			this.f = f;
			this.h = h;
		}

		public int CompareTo (Node other)
		{
			return this.f.CompareTo (other.f);
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public override bool Equals (object obj)
		{
			return obj is Node && point == ((Node)obj).point;
		}
	}
}

