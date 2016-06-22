using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding
{
	public class BreadthFirstFinder
	{
		public bool allowDiagonal;
		public bool dontCrossCorners;
		public DiagonalMovement? diagonalMovement;
		public Heuristic.Function heuristic;
		public float weight;

		protected Queue<Node> openList;

		public BreadthFirstFinder (Option opt)
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

			openList = new Queue<Node> (100);
		}


		/// <summary>
		/// 시작과 끝 점이 포함된 길을 찾아 돌려준다.
		/// </summary>
		public List<Point> FindPath (Point from, Point to, Grid grid)
		{
			openList.Clear ();

			// openList에 첫번째 노드를 넣는다.
			var startNode = new Node (from, null, 0, 0);
			openList.Enqueue (startNode);
			startNode.opened = true;

			while (openList.Count > 0)
			{
				// queue에서 최상위 Node를 꺼낸다
				var node = openList.Dequeue ();
				node.closed = true;

				if (node.point == to)
					return Util.Backtrace (node);
				
				// 현재노드의 인접한 노드들을 확인한다.
				var neighbors = grid.GetNeighbors (node.point, diagonalMovement.Value);
				foreach (var neighbor in neighbors)
				{
					// 이미 확인된 이웃한 Node인 경우 스킵.
					if (neighbor.closed || neighbor.opened) continue;

					neighbor.parent = node;
					neighbor.opened = true;
					openList.Enqueue (neighbor);
				}
			}

			return new List<Point> ();
		}
	}
}

