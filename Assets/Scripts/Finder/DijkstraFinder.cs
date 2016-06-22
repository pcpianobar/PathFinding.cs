namespace Pathfinding
{
	public class DijkstraFinder : AStarFinder
	{
		public DijkstraFinder (Option opt)
			: base(opt)
		{
			heuristic = (point) => 0;
		}
	}
}