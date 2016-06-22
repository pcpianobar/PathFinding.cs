using UnityEngine;
using System.Collections.Generic;


namespace Pathfinding
{
	public class Option
	{
		public bool allowDiagonal;
		public bool dontCrossCorners;
		public Heuristic.Function heuristic;
		public float? weight;
		public DiagonalMovement? diagonalMovement;
		public bool trackRecursion;
		public double timeLimit;
	}

	public class AStarFinder
	{
		public bool allowDiagonal;
		public bool dontCrossCorners;
		public DiagonalMovement? diagonalMovement;
		public Heuristic.Function heuristic;
		public float weight;

		protected PriorityQueue<float, Node> openList;

		public AStarFinder (Option opt)
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
				this.heuristic = opt.heuristic ?? Heuristic.Manhattan;
			else
				this.heuristic = opt.heuristic ?? Heuristic.Octile;

			openList = new PriorityQueue<float, Node> (10, new ComparisonComparer<float>((a, b) => a.CompareTo (b)));
		}


		/// <summary>
		/// 길을 찾아 돌려준다.
		/// </summary>
		public List<Point> FindPath (Point from, Point to, Grid grid)
		{
			openList.Clear ();

			var startNode = new Node (from, null, 0, 0);
			float sqrt2 = Mathf.Sqrt (2f);

			// openList에 첫번째 노드를 넣는다.
			openList.Enqueue (startNode.f, startNode);
			startNode.opened = true;

			while (openList.Count > 0)
			{
				// 가장 작은 f값을 가진 노드를 꺼낸다.
				var node = openList.DequeueValue ();
				node.closed = true;

				// 목적지에 도착했다면, 길을 만들어 돌려준다.
				if (node.point == to) 
					return Util.ExpandPath (Util.Backtrace (node));

				// 현재노드의 인접한 노드들을 확인한다.
				var neighbors = grid.GetNeighbors (node.point, diagonalMovement.Value);
				foreach (var neighbor in neighbors)
				{
					if (neighbor.closed) continue;

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
							openList.Enqueue (neighbor.f, neighbor);
							neighbor.opened = true;
						}
						else
							// 이웃한 노드들은 작은 비용으로 도달할수 있다. f값이 업데이트
							// 되었기때문에 openList의 위치를 업데이트 시켜야 한다.
							openList.UpdateItem (neighbor);
					}
				}
			}

			return new List<Point> ();
		}
	}
}
