using System.Diagnostics.CodeAnalysis;
using BingoAPI.Models;
using JetBrains.Annotations;

namespace BingoAPI.Goals;

/// <summary>
/// Interface that represents any class that can hold and retrieve <see cref="Goal"/>
/// </summary>
[PublicAPI]
public interface IGoalPool : ICollection<Goal>
{
	/// <summary>
	/// Gets the <see cref="Goal"/> represented by the given <see cref="Square"/>.
	/// </summary>
	public bool TryGetValue(Square square, [NotNullWhen(true)] out Goal? goal);

	/// <summary>
	/// Attempts to add the given <see cref="Goal"/>
	/// </summary>
	public bool TryAdd(Goal goal);
}
