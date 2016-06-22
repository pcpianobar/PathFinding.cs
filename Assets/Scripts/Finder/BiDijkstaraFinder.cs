namespace Pathfinding
{
	public class BiDijkstaraFinder : BiAStarFinder
	{
		public BiDijkstaraFinder (Option opt)
			: base(opt)
		{
			heuristic = (point) => 0;
		}
	}
}