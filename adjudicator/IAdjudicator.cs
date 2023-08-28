public interface IAdjudicator
{
	public bool ValidateMove(Piece piece, uint rankFrom, uint rankTo, uint fileFrom, uint fileTo);

    // End turn and move to next player's turn.
    public void EndTurn();

}
