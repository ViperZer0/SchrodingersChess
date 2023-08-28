using Godot;

public partial class Adjudicator : Node, IAdjudicator
{
	[Export]
	private Board board;

	public bool ValidateMove(Piece piece, uint rankFrom, uint rankTo, uint fileFrom, uint fileTo)
	{
		return true;
	}
}

