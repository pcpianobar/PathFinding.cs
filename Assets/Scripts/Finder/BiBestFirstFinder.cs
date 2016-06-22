namespace Pathfinding
{
	public class BiBestFirstFinder : BiAStarFinder
	{
		public BiBestFirstFinder (Option opt)
			: base(opt)
		{
			var orig = this.heuristic;
			this.heuristic = (point) => orig (point) * 1000000;
		}
	}
}