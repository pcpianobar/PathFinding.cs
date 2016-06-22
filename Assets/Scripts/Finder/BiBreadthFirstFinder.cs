using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding
{
	public class BiBreadthFirstFinder
	{
		public bool allowDiagonal;
		public bool dontCrossCorners;
		public DiagonalMovement? diagonalMovement;
		public Heuristic.Function heuristic;
		public float weight;

		protected Queue<Node> startOpenList;
		protected Queue<Node> endOpenList;

		public BiBreadthFirstFinder (Option opt)
		{
			this.allowDiagonal = opt.allowDiagonal;
			this.dontCrossCorners = opt.dontCrossCorners;
			this.diagonalMovement = opt.diagonalMovement;

			if (!diagonalMovement.HasValue)
			{
				if (!allowDiagonal)
					diagonalMovement = DiagonalMovement.Never;
				else
					diagonalMovement = dontCrossCorners ? DiagonalMovement.OnlyWhenNoObstacles : DiagonalMovement.IfAtMostOneObstacle;
			}

			startOpenList = new Queue<Node> (10);
			endOpenList = new Queue<Node> (10);
		}


		/// <summary>
		/// 길을 찾아 돌려준다.
		/// </summary>
		public List<Point> FindPath (Point from, Point to, Grid grid)
		{
			startOpenList.Clear ();
			endOpenList.Clear ();

			// openList에 첫번째 노드를 넣는다.
			var startNode = new Node (from, null, 0, 0);
			startOpenList.Enqueue (startNode);
			startNode.opened = true;
			startNode.by = ByType.BY_START;

			var endNode = new Node (to, null, 0, 0);
			endOpenList.Enqueue (endNode);
			endNode.opened = true;
			endNode.by = ByType.BY_END;

			while (startOpenList.Count > 0 && endOpenList.Count > 0)
			{
				// 가장 작은 f값을 가진 노드를 꺼낸다.
				var node = startOpenList.Dequeue ();
				node.closed = true;

				// 현재노드의 인접한 노드들을 확인한다.
				var neighbors = grid.GetNeighbors (node.point, diagonalMovement.Value);
				foreach (var neighbor in neighbors)
				{
					if (neighbor.closed) continue;

					// 목적지에 도착했다면, 길을 만들어 돌려준다.
					if (neighbor.opened)
					{
						if (neighbor.by == ByType.BY_END)
							return Util.BiBacktrace (node, neighbor);
						continue;
					}

					neighbor.parent = node;
					neighbor.opened = true;
					neighbor.by = ByType.BY_START;
					startOpenList.Enqueue (neighbor);
				}

				// 가장 작은 f값을 가진 노드를 꺼낸다.
				node = endOpenList.Dequeue ();
				node.closed = true;

				// 현재노드의 인접한 노드들을 확인한다.
				neighbors = grid.GetNeighbors (node.point, diagonalMovement.Value);
				foreach (var neighbor in neighbors)
				{
					if (neighbor.closed) continue;

					// 목적지에 도착했다면, 길을 만들어 돌려준다.
					if (neighbor.opened)
					{
						if (neighbor.by == ByType.BY_START)
							return Util.BiBacktrace (neighbor, node);
						continue;
					}

					neighbor.parent = node;
					neighbor.opened = true;
					neighbor.by = ByType.BY_END;
					endOpenList.Enqueue (neighbor);
				}
			}

			return new List<Point> ();
		}
	}
}

