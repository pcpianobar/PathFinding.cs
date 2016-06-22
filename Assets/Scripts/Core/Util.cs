using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public static class Util
	{
		public static List<Point> Backtrace (Node node) {
			List<Point> path = new List<Point> ();
			while (node != null) {
				path.Add (node.point);
				node = node.parent;
			}
			path.Reverse ();
			return path;
		}

		public static List<Point> BiBacktrace (Node nodeA, Node nodeB) 
		{
			List<Point> pathA = Backtrace (nodeA);
			List<Point> pathB = Backtrace (nodeB);
			pathB.Reverse ();
			pathA.AddRange (pathB);
			return pathA;
		}

		public static int PathLength (List<Point> path) 
		{
			int sum = 0;
			for (int i = 1; i < path.Count; ++i) {
				Point a = path[i - 1];
				Point b = path[i];
				int dx = a.x - b.x;
				int dy = a.y - b.y;
				sum += Mathf.FloorToInt (Mathf.Sqrt (dx * dx + dy * dy));
			}

			return sum;
		}

		public static List<Point> Interpolate (Point start, Point end)
		{
			return Interpolate (start.x, start.y, end.x, end.y);
		}

		public static List<Point> Interpolate (int x0, int y0, int x1, int y1)
		{
			List<Point> line = new List<Point> ();

			int dx = Mathf.Abs (x1 - x0);
			int dy = Mathf.Abs (y1 - y0);
			
			int sx = (x0 < x1) ? 1 : -1;
			int sy = (y0 < y1) ? 1 : -1;
			
			int err = dx - dy;
			
			while (true) {
				line.Add (new Point (x0, y0));
				
				if (x0 == x1 && y0 == y1) {
					break;
				}
				
				int e2 = 2 * err;
				if (e2 > -dy) {
					err = err - dy;
					x0 = x0 + sx;
				}
				if (e2 < dx) {
					err = err + dx;
					y0 = y0 + sy;
				}
			}
			
			return line;
		}

		public static List<Point> ExpandPath (List<Point> path) {
			List<Point> expanded = new List<Point> ();
			int len = path.Count;

			if (len < 2) {
				return expanded;
			}
			
			for (int i = 0; i < len - 1; ++i) {
				Point coord0 = path[i];
				Point coord1 = path[i + 1];
				
				List<Point> interpolated = Interpolate (coord0, coord1);
				int interpolatedLen = interpolated.Count;
				for (int j = 0; j < interpolatedLen - 1; ++j) {
					expanded.Add (interpolated[j]);
				}
			}
			expanded.Add (path[len - 1]);
			
			return expanded;
		}

		public static List<Point> SmoothenPath (Grid grid, List<Point> path) 
		{
			int len = path.Count;
			Point startCoord = path[0];	// current start coordinate

			List<Point> newPath = new List<Point> ();
			newPath.Add (startCoord);

			for (int i = 2; i < len; ++i) {
				Point endCoord = path[i]; // current end coordinate
				List<Point> line = Interpolate (startCoord, endCoord);
				
				bool blocked = false;
				for (int j = 1; j < line.Count; ++j) {
					Point testCoord = line[j];
					
					if (!grid.IsWalkableAt(testCoord)) {
						blocked = true;
						break;
					}
				}
				if (blocked) {
					Point lastValidCoord = path[i - 1];
					newPath.Add(lastValidCoord);
					startCoord = lastValidCoord;
				}
			}
			newPath.Add (path[len - 1]);
			
			return newPath;
		}

		public static List<Point> CompressPath (List<Point> path) 
		{	
			// nothing to compress
			if(path.Count < 3) {
				return path;
			}
			
			List<Point> compressed = new List<Point> ();
			Point start = path[0];	// start
			Point second = path[1];	// second point
//			sx = path[0][0], // start x
//			sy = path[0][1], // start y
//			px = path[1][0], // second point x
//			py = path[1][1], // second point y
			int dx = second.x - start.x; // direction between the two points
			int dy = second.y - start.y; // direction between the two points

			// normalize the direction
			float sq = Mathf.Sqrt (dx*dx + dy*dy);
			dx = Mathf.FloorToInt ((float)dx / sq);
			dy = Mathf.FloorToInt ((float)dy / sq);
			
			// start the new path
			compressed.Add (start);
			
			for(int i = 2; i < path.Count; i++) {
				
				// store the last point
				Point last = second;

				// store the last direction
				int ldx = dx;
				int ldy = dy;
				
				// next point
				second = path[i];
				
				// next direction
				dx = second.x - last.x;
				dy = second.y - last.y;
				
				// normalize
				sq = Mathf.Sqrt (dx*dx + dy*dy);
				dx = Mathf.FloorToInt ((float)dx / sq);
				dy = Mathf.FloorToInt ((float)dy / sq);
				
				// if the direction has changed, store the point
				if ( dx != ldx || dy != ldy ) {
					compressed.Add (last);
				}
			}
			
			// store the last point
			compressed.Add (second);
			
			return compressed;
		}

		public static bool IsTurnLine (Point point, Node parent)
		{
			if (parent != null && parent.parent != null)
			{
				Point parentCoord = parent.parent.point;
				return !(parentCoord.x != point.x && parentCoord.y != point.y);
			}
			return true;
		}
	}
}