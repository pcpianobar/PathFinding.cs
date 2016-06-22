using System;

namespace Pathfinding
{
	public enum DiagonalMovement
	{
		Null = 0,
		Always = 1,
		Never,
		IfAtMostOneObstacle,
		OnlyWhenNoObstacles
	}
}

