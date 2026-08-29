using JetBrains.Annotations;

namespace BingoAPI.Goals;

/// <summary>
/// Interface that represents any class that can store and retrieve a <see cref="IGoalCollection"/>
/// </summary>
[PublicAPI]
public interface IGoalStorage
{
	/// <summary>
	/// Writes the given <see cref="IGoalCollection"/> into the storage
	/// </summary>
	public void Write(IGoalCollection collection);

	/// <summary>
	/// Reads the <see cref="IGoalCollection"/> from the storage
	/// </summary>
	public IGoalCollection Read();
}
