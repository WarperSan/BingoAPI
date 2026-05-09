namespace BingoAPI.Models;

public sealed class Square
{
	/// <summary>
	/// Text displayed on this square
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Index of this square
	/// </summary>
	public int Index { get; }

	/// <summary>
	/// Teams currently owning this square
	/// </summary>
	public Team Teams { get; }

	internal Square(
		string name,
		int index,
		Team teams
	)
	{
		Name = name;
		Index = index;
		Teams = teams;
	}
}
