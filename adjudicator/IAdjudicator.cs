public interface IAdjudicator
{
	public bool ValidateMove(Piece piece, uint rankFrom, uint rankTo, uint fileFrom, uint fileTo);
}
