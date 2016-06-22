using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public abstract class JumpPointFinderBase
	{
		protected Heuristic.Function heuristic;
		protected PriorityQueue<float, Node> openList;
		protected Point endCoord;
		protected Grid grid;

		protected JumpPointFinderBase (Heuristic.Function heuristic)
		{
			this.heuristic = heuristic ?? Heuristic.Manhattan;
			openList = new PriorityQueue<float, Node> (10, new ComparisonComparer<float>((a, b) => a.CompareTo (b)));
		}

		public List<Point> FindPath (Point from, Point to, Grid grid)
		{
			openList.Clear ();

			this.grid = grid;
			this.endCoord = to;

			var startNode = new Node (from, null, 0, 0);

			openList.Enqueue (startNode.f, startNode);
			startNode.opened = true;

			while (openList.Count > 0) 
			{
				Node node = openList.DequeueValue ();
				node.closed = true;

				if (node.point == to) 
				{
					return Util.ExpandPath (Util.Backtrace (node));
				}
				
				IdentifySuccessors (node);
			}

			return null;
		}

		protected void IdentifySuccessors (Node node) 
		{
			var neighbors = FindNeighbors (node);
			foreach (var neighbor in neighbors)
			{
				Point jumpPoint;
				if (Jump (neighbor.point, node.point, out jumpPoint))
				{
					var jumpNode = grid.GetNodeAt (jumpPoint);
					if (jumpNode.closed) continue;

					float d = Heuristic.Octile (jumpPoint.Displacement (node.point));
					float ng = node.g + d;	// next 'g' value

					if (!jumpNode.opened || ng < jumpNode.g)
					{
						jumpNode.g = ng;
						jumpNode.h = jumpNode.h ?? heuristic (jumpPoint.Displacement (endCoord));
						jumpNode.f = ng + jumpNode.h.Value;
						jumpNode.parent = node;

						if (!jumpNode.opened)
						{
							openList.Enqueue (jumpNode.f, jumpNode);
							jumpNode.opened = true;
						}
						else
							openList.UpdateItem (jumpNode);
					}
				}
			}
		}

		protected abstract List<Node> FindNeighbors (Node node);
		protected abstract bool Jump (Point from, Point to, out Point jumpPoint);
	}
}