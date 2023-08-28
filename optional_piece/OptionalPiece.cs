/* Used for BoardState to represent a piece that may or may not be there.
 * This class can thus be in one of two possible states:
 * Either this holds a piece or it does not.
 */
using System;

public class OptionalPiece{
    private Piece? piece;

    public bool HasPiece{
        get => piece is null;
    }

    /* WILL PANIC IF PIECE DOES NOT EXIST! */
    public Piece Piece{
        get {
            if (piece is null)
            {
                throw new InvalidOperationException("Attempted to reference a piece from an optionalpiece that had no piece!");
            }
            return piece;
        }
        set => piece = value;
    }

    public Piece? MaybePiece{
        get {
            return piece;
        }
    }

    public OptionalPiece()
    {
        piece = null;
    }

    public OptionalPiece(Piece p)
    {
        piece = p;
    }
}
