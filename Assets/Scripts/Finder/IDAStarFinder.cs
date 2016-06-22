using UnityEngine;
using System.Collections.Generic;
using System;


namespace Pathfinding
{
	public class IDAStarFinder
	{
		bool allowDiagonal;
		bool dontCrossCorners;
		DiagonalMovement? diagonalMovement;
		Heuristic.Function heuristic;
		float weight;
		bool trackRecursion;
		double timeLimit;
		PriorityQueue<float, Node> openList;

		public IDAStarFinder (Option opt)
		{
			allowDiagonal = opt.allowDiagonal;
			dontCrossCorners = opt.dontCrossCorners;
			heuristic = opt.heuristic ?? Heuristic.Manhattan;;
			weight = opt.weight ?? 1;
			diagonalMovement = opt.diagonalMovement;
			trackRecursion = opt.trackRecursion;
			timeLimit = opt.timeLimit;

			if (!diagonalMovement.HasValue)
			{
				if (!allowDiagonal)
					diagonalMovement = DiagonalMovement.Never;
				else
					diagonalMovement = dontCrossCorners ? DiagonalMovement.OnlyWhenNoObstacles : DiagonalMovement.IfAtMostOneObstacle;
			}

			// 대각선 이동이 허용되면 Manhattan은 허용되지 않는다. 
			// 대신에 Octile를 사용해야 한다.
			if (diagonalMovement == DiagonalMovement.Never)
				heuristic = opt.heuristic ?? Heuristic.Manhattan;
			else
				heuristic = opt.heuristic ?? Heuristic.Octile;

			openList = new PriorityQueue<float, Node> (10, new ComparisonComparer<float>((a, b) => a.CompareTo (b)));
		}

		/// <summary>
		/// 시작과 끝 점이 포함된 길을 찾아 돌려준다.
		/// 길이 없거나, 최대 실행시간에 도달할 경우 빈 List를 돌려준다.
		/// </summary>
		public List<Point> FindPath (Point from, Point to, Grid grid)
		{
			openList.Clear ();

			var sqrt2 = Mathf.Sqrt (2f);
			int nodesVisited = 0;
			var startTime = DateTime.Now;
			double t;
			Func<Point, Point, int> h = (a, b) => heuristic (b.Displacement (a));
			Action<Point, Point> cost = (a, b) => (a.x == b.x || a.y == b.y) ? 1 : sqrt2;
			Func<Node, float, int, List<Point>, int, double, bool> search = (node, g, cutoff, route, depth) => 
			{
				nodesVisited++;

				var span = DateTime.Now - startTime;
				if (timeLimit > 0 && span.TotalMilliseconds > timeLimit * 1000)
				{
					t = double.PositiveInfinity;
					return false;
				}

				var f = g + h (node, to) * weight;

				if (f > cutoff)
					return f;

				if (node == to)
				{
					route[depth] = node;
					return node;
				}

				var neighbours = grid.GetNeighbors (node.point, 
			};


			var startNode = new Node (from);
			var endNode = new Node (to);
			var cutOff = h (startNode.point, endNode.point);

			while (openList.Count > 0)
			{
				// 가장 작은 f값을 가진 노드를 꺼낸다.
				var node = openList.DequeueValue ();
				node.closed = true;

				// 목적지에 도착했다면, 길을 만들어 돌려준다.
				if (node.point == to) 
					return Util.ExpandPath (Util.Backtrace (node));

				var route = new List<Point> ();
				while (true)
				{
					route.Clear ();	

					if (search (startNode, 0, cutOff, route, 0))
						return route;

					if (double.IsInfinity (t))
						return new List<Point> ();

					cutOff = t;
				}
			}

			return new List<Point> ();
		}
	}
}
