using Godot;
using System;

public class Pawn: IPieceType
{
	private bool firstMove = true;
    private PieceColor pieceColor;

    public Pawn(PieceColor pieceColor)
    {
        this.pieceColor = pieceColor;
    }

	public bool ValidateMove(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
	{
        // Pawn has to move UP the board (increasing rank)
        if(pieceColor == PieceColor.White)
        {
            if(rankTo >= rankFrom)
            {
                return false;
            }

            // Can move two spaces on the first move
            if (firstMove && (rankTo - rankFrom > 2))
            {
                return false;
            }
            // otherwise can only move one space
            else if(rankTo - rankFrom > 1)
            {
                return false;
            }
        }

        // Pawn has to move DOWN the board (decreasing rank)
        if(pieceColor == PieceColor.Black)
        {
            if(rankTo <= rankFrom)
            {
                return false;
            }

            // Can move two spaces on the first move.
            if (firstMove && (rankFrom - rankTo > 2))
            {
                return false;
            }
            // otherwise can only move one space.
            else if(rankFrom - rankTo > 1)
            {
                return false;
            }
        }

        // Pawn cannot change ranks

        if(fileFrom != fileTo)
        {
            return false;
        }
        
        return true;
	}

    public bool ValidateCapture(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
    {
        // Piece can only move one space left or right.
        if(!(fileTo == fileFrom - 1) && !(fileTo == fileFrom + 1))
        {
            return false;
        }

        // Piece can only move one space fowards.
        if(pieceColor == PieceColor.White)
        {
            if(!(rankTo == rankFrom + 1))
            {
                return false;
            }
        }

        // Piece can only move one space back (down rank)
        if(pieceColor == PieceColor.Black)
        {
            if(!(rankTo == rankFrom - 1))
            {
                return false;
            }
        }

        return true;
    }
}


