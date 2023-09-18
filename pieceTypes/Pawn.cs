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
		if(pieceColor == PieceColor.White)
		{
			// Pawn has to move UP the board (increasing rank)
			if(rankTo <= rankFrom)
			{
				return false;
			}

			if (firstMove)
			{
				// Can only move two spaces on the first move
				if (rankTo - rankFrom > 2)
				{
					return false;
				}
			}
			else
			{
				// otherwise can only move one space
				if(rankTo - rankFrom > 1)
				{
					return false;
				}
			}
		}

		if(pieceColor == PieceColor.Black)
		{
			// Pawn has to move DOWN the board (decreasing rank)
			if(rankTo >= rankFrom)
			{
				return false;
			}

			// Can move two spaces on the first move.
			if (firstMove)
			{
				if((rankFrom - rankTo) > 2)
				{
					return false;
				}
			}
			else
			{
				if((rankFrom - rankTo) > 1)
				{
					return false;
				}
			}
		}

		// Pawn cannot change ranks
		if(fileFrom != fileTo)
		{
			return false;
		}
		
		// If we are returning true we want to set first move to be false, since
		// we KNOW this piece WILL be moving.

		this.firstMove = false;
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


