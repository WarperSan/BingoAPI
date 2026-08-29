using BingoAPI.Models;
using JetBrains.Annotations;

namespace BingoAPI.Goals;

/// <summary>
/// Interface that represents any class that hold instances of <see cref="Goal"/>
/// </summary>
[PublicAPI]
public interface IGoalCollection
{
	/// <summary>
	/// Name of the collection
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Description of the collection
	/// </summary>
	public string? Description { get; }

	/// <summary>
	/// All instances of <see cref="Goal"/> stored in the collection
	/// </summary>
	public IEnumerable<Goal> Goals { get; }
}
