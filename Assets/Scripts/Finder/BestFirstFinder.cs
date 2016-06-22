namespace Pathfinding
{
	public class BestFirstFinder : AStarFinder 
	{
		public BestFirstFinder (Option opt)
			: base(opt)
		{
			var orig = this.heuristic;
			this.heuristic = (point) => orig (point) * 1000000;
		}
	}
}