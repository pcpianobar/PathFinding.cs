using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public class JPFNeverMoveDiagonally : JumpPointFinderBase
	{
		public JPFNeverMoveDiagonally (Heuristic.Function heuristic) : base (heuristic)
		{
		}

		protected override bool Jump (Point from, Point to, out Point jumpPoint)
		{
			Point d = from - to;

			if (!grid.IsWalkableAt (from)) 
			{
				jumpPoint = new Point(0,0);
				return false;
			}

			if (from == endCoord) 
			{
				jumpPoint = from;
				return true;
			}
			
			if (d.x != 0) 
			{
				if ((grid.IsWalkableAt (from.Offset (0,-1)) && !grid.IsWalkableAt (from.Offset (-d.x,-1))) ||
				    (grid.IsWalkableAt (from.Offset (0,1)) && !grid.IsWalkableAt (from.Offset (-d.x,1))) ||
				    from.x == endCoord.x) 
				{
					jumpPoint = from;
					return true;
				}
			}
			else if (d.y != 0) 
			{
				if ((grid.IsWalkableAt (from.Offset (-1,0)) && !grid.IsWalkableAt (from.Offset (-1,-d.y))) ||
				    (grid.IsWalkableAt (from.Offset (1,0)) && !grid.IsWalkableAt (from.Offset (1,-d.y))) ||
				    from.y == endCoord.y) 
				{
					jumpPoint = from;
					return true;
				}
				//When moving vertically, must check for horizontal jump points
				if (Jump (from.Offset (1,0), from, out jumpPoint) || Jump (from.Offset (-1,0), from, out jumpPoint)) 
				{
					jumpPoint = from;
					return true;
				}
			}
			else 
			{
				throw new Exception ("Only horizontal and vertical movements are allowed");
			}
			
			return Jump (from + d, from, out jumpPoint);
		}

		protected override List<Node> FindNeighbors (Node node)
		{
			Point n = node.point;
			Node parent = node.parent;


			if (parent != null)
			{
				Point p = parent.point;
				var neighbors = new List<Node> ();
				int dx = (n.x - p.x) / Mathf.Max(Mathf.Abs(n.x - p.x), 1);
				int dy = (n.y - p.y) / Mathf.Max(Mathf.Abs(n.y - p.y), 1);
				
				if (dx != 0) 
				{
					if (grid.IsWalkableAt (n.Offset (0,-1))) 
						neighbors.Add (new Node (n.Offset (0,-1)));

					if (grid.IsWalkableAt (n.Offset (0,1))) 
						neighbors.Add (new Node (n.Offset (0,1)));
					
					if (grid.IsWalkableAt (n.Offset (dx,0))) 
						neighbors.Add (new Node (n.Offset (dx,0)));
				}
				else if (dy != 0) 
				{
					if (grid.IsWalkableAt (n.Offset (-1,0)))
						neighbors.Add (new Node (n.Offset (-1,0)));

					if (grid.IsWalkableAt (n.Offset (1,0))) 
						neighbors.Add (new Node (n.Offset (1,0)));

					if (grid.IsWalkableAt (n.Offset (0,dy)))
						neighbors.Add(new Node (n.Offset (0,dy)));
				}

				return neighbors;
			}
			else 
			{
				return grid.GetNeighbors (node.point, DiagonalMovement.Never);
			}
		}
	}
}