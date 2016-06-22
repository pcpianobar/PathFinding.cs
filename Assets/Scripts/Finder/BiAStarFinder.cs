using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding
{
	public class BiAStarFinder
	{
		public bool allowDiagonal;
		public bool dontCrossCorners;
		public DiagonalMovement? diagonalMovement;
		public Heuristic.Function heuristic;
		public float weight;

		protected PriorityQueue<float, Node> startOpenList;
		protected PriorityQueue<float, Node> endOpenList;

		public BiAStarFinder (Option opt)
		{
			this.allowDiagonal = opt.allowDiagonal;
			this.dontCrossCorners = opt.dontCrossCorners;
			this.heuristic = opt.heuristic ?? Heuristic.Manhattan;;
			this.weight = opt.weight ?? 1;
			this.diagonalMovement = opt.diagonalMovement;

			if (!diagonalMovement.HasValue)
			{
				if (!allowDiagonal)
					diagonalMovement = DiagonalMovement.Never;
				else
					diagonalMovement = dontCrossCorners ? DiagonalMovement.OnlyWhenNoObstacles : DiagonalMovement.IfAtMostOneObstacle;
			}

			// When diagonal movement is allowed the manhattan heuristic is not
			//admissible. It should be octile instead
			if (diagonalMovement == DiagonalMovement.Never)
				heuristic = opt.heuristic ?? Heuristic.Manhattan;
			else
				heuristic = opt.heuristic ?? Heuristic.Octile;

			startOpenList = new PriorityQueue<float, Node> (10, new ComparisonComparer<float>((a, b) => a.CompareTo (b)));
			endOpenList = new PriorityQueue<float, Node> (10, new ComparisonComparer<float>((a, b) => a.CompareTo (b)));
		}


		/// <summary>
		/// 길을 찾아 돌려준다.
		/// </summary>
		public List<Point> FindPath (Point from, Point to, Grid grid)
		{
			startOpenList.Clear ();
			endOpenList.Clear ();

			float sqrt2 = Mathf.Sqrt (2f);

			// openList에 첫번째 노드를 넣는다.
			var startNode = new Node (from, null, 0, 0);
			startOpenList.Enqueue (startNode.f, startNode);
			startNode.opened = true;
			startNode.by = ByType.BY_START;

			var endNode = new Node (to, null, 0, 0);
			endOpenList.Enqueue (endNode.f, endNode);
			endNode.opened = true;
			endNode.by = ByType.BY_END;

			while (startOpenList.Count > 0 && endOpenList.Count > 0)
			{
				// 가장 작은 f값을 가진 노드를 꺼낸다.
				var node = startOpenList.DequeueValue ();
				node.closed = true;

				// 현재노드의 인접한 노드들을 확인한다.
				var neighbors = grid.GetNeighbors (node.point, diagonalMovement.Value);
				foreach (var neighbor in neighbors)
				{
					if (neighbor.closed) continue;

					// 목적지에 도착했다면, 길을 만들어 돌려준다.
					if (neighbor.opened && neighbor.by == ByType.BY_END)
						return Util.BiBacktrace (neighbor, node);

					Point dist = neighbor.point - node.point;
					// 현재 노드와 인접한 노드 사이의 거리를 가져와 "Next G" 점수를 계산한다. 
					float ng = node.g + ((dist.x == 0 || dist.y == 0) ? 1f : sqrt2);

					if (!neighbor.opened || ng < neighbor.g)
					{
						neighbor.g = ng;
						neighbor.h = neighbor.h ?? weight * heuristic (neighbor.point.Displacement (to));
						neighbor.f = neighbor.g + neighbor.h.Value;
						neighbor.parent = node;

						if (!neighbor.opened)
						{
							startOpenList.Enqueue (neighbor.f, neighbor);
							neighbor.opened = true;
							neighbor.by = ByType.BY_START;
						}
						else
							// 이웃한 노드들은 작은 비용으로 도달할수 있다. f값이 업데이트
							// 되었기때문에 openList의 위치를 업데이트 시켜야 한다.
							startOpenList.UpdateItem (neighbor);
					}
				}

				// 가장 작은 f값을 가진 노드를 꺼낸다.
				node = endOpenList.DequeueValue ();
				node.closed = true;

				// 현재노드의 인접한 노드들을 확인한다.
				neighbors = grid.GetNeighbors (node.point, diagonalMovement.Value);
				foreach (var neighbor in neighbors)
				{
					if (neighbor.closed) continue;

					// 목적지에 도착했다면, 길을 만들어 돌려준다.
					if (neighbor.opened && neighbor.by == ByType.BY_START)
						return Util.BiBacktrace (neighbor, node);

					Point dist = neighbor.point - node.point;
					// 현재 노드와 인접한 노드 사이의 거리를 가져와 "Next G" 점수를 계산한다. 
					float ng = node.g + ((dist.x == 0 || dist.y == 0) ? 1f : sqrt2);

					if (!neighbor.opened || ng < neighbor.g)
					{
						neighbor.g = ng;
						neighbor.h = neighbor.h ?? weight * heuristic (neighbor.point.Displacement (to));
						neighbor.f = neighbor.g + neighbor.h.Value;
						neighbor.parent = node;

						if (!neighbor.opened)
						{
							endOpenList.Enqueue (neighbor.f, neighbor);
							neighbor.opened = true;
							neighbor.by = ByType.BY_END;
						}
						else
							// 이웃한 노드들은 작은 비용으로 도달할수 있다. f값이 업데이트
							// 되었기때문에 openList의 위치를 업데이트 시켜야 한다.
							endOpenList.UpdateItem (neighbor);
					}
				}
			}

			return new List<Point> ();
		}
	}
}
