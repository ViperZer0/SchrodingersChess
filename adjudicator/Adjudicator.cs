using Godot;

public partial class Adjudicator : Node, IAdjudicator
{
	[Export]
	private Board board;

	private PieceColor pieceTurn = PieceColor.White;

	public bool ValidateMove(Piece piece, uint rankFrom, uint rankTo, uint fileFrom, uint fileTo)
	{
		GD.Print(pieceTurn.ToString());
		GD.Print(piece.PieceColor.ToString());
		if(piece.PieceColor != pieceTurn)
		{
			return false;
		}
		
		return true;
	}

	// We may want this to check what piece called it?
	public void EndTurn()
	{
		if(pieceTurn == PieceColor.White)
		{
			pieceTurn = PieceColor.Black;
		}
		
		if(pieceTurn == PieceColor.Black)
		{
			pieceTurn = PieceColor.White;
		}
	}
}

