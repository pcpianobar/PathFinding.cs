using System;
using System.Collections.Generic;

namespace Pathfinding
{
	public class Grid
	{
		public int width;
		public int height;

		public Func<Point, bool> onWalkable;

		protected Node[,] nodes;

		public Grid (int width, int height)
		{
			this.width = width;
			this.height = height;

			BuildNodes ();
		}

		void BuildNodes ()
		{
			nodes = new Node[width, height];
			for (int y=0; y<height; ++y)
				for (int x=0; x<width; ++x)
					nodes[x, y] = new Node (new Point(x, y));
		}

		public bool IsWalkableAt (int x, int y)
		{
			return IsWalkableAt (new Point (x, y));
		}

		public bool IsWalkableAt (Point point) 
		{
			if (onWalkable == null) {
				UnityEngine.Debug.Log ("onWalkable is null");
				return false;
			}

			return IsInside(point) && onWalkable (point);
		}

		public bool IsInside (Point point) 
		{
			return (point.x >= 0 && point.x < width) && (point.y >= 0 && point.y < height);
		}

		/**
		 * Get the neighbors of the given node.
		 *
		 *     offsets      diagonalOffsets:
		 *  +---+---+---+    +---+---+---+
		 *  |   | 0 |   |    | 0 |   | 1 |
		 *  +---+---+---+    +---+---+---+
		 *  | 3 |   | 1 |    |   |   |   |
		 *  +---+---+---+    +---+---+---+
		 *  |   | 2 |   |    | 3 |   | 2 |
		 *  +---+---+---+    +---+---+---+
		 *
		 *  When allowDiagonal is true, if offsets[i] is valid, then
		 *  diagonalOffsets[i] and
		 *  diagonalOffsets[(i + 1) % 4] is valid.
		 * @param {Node} node
		 * @param {DiagonalMovement} diagonalMovement
		 */
		public List<Node> GetNeighbors (Point point, DiagonalMovement diagonalMovement) 
		{
			var neighbors = new List<Node> ();
			bool s0 = false, d0 = false,
			s1 = false, d1 = false,
			s2 = false, d2 = false,
			s3 = false, d3 = false;

			// ↑
			if (IsWalkableAt (point.Offset (0,1))) {
				neighbors.Add (new Node (point.Offset (0,1)));
				s0 = true;
			}
			// →
			if (IsWalkableAt (point.Offset (1,0))) {
				neighbors.Add (new Node (point.Offset (1,0)));
				s1 = true;
			}
			// ↓
			if (IsWalkableAt (point.Offset (0,-1))) {
				neighbors.Add (new Node (point.Offset (0,-1)));
				s2 = true;
			}
			// ←
			if (IsWalkableAt (point.Offset (-1,0))) {
				neighbors.Add (new Node (point.Offset (-1,0)));
				s3 = true;
			}
			
			if (diagonalMovement == DiagonalMovement.Never) {
				return neighbors;
			}
			
			if (diagonalMovement == DiagonalMovement.OnlyWhenNoObstacles) {
				d0 = s3 && s0;
				d1 = s0 && s1;
				d2 = s1 && s2;
				d3 = s2 && s3;
			} else if (diagonalMovement == DiagonalMovement.IfAtMostOneObstacle) {
				d0 = s3 || s0;
				d1 = s0 || s1;
				d2 = s1 || s2;
				d3 = s2 || s3;
			} else if (diagonalMovement == DiagonalMovement.Always) {
				d0 = true;
				d1 = true;
				d2 = true;
				d3 = true;
			} else {
				throw new Exception ("Incorrect value of diagonalMovement");
			}
			
			// ↖
			if (d0 && IsWalkableAt (point.Offset (-1,1))) {
				neighbors.Add (new Node (point.Offset (-1,1)));
			}
			// ↗
			if (d1 && IsWalkableAt (point.Offset (1,1))) {
				neighbors.Add (new Node (point.Offset (1,1)));
			}
			// ↘
			if (d2 && IsWalkableAt (point.Offset (1,-1))) {
				neighbors.Add (new Node (point.Offset (1,-1)));
			}
			// ↙
			if (d3 && IsWalkableAt (point.Offset (-1,-1))) {
				neighbors.Add (new Node (point.Offset (-1,-1)));
			}
			
			return neighbors;
		}

		public Node GetNodeAt (Point point)
		{
			return nodes [point.x, point.y];
		}
	}
}