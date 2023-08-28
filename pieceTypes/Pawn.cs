using Godot;
using System;

public class Pawn: IPieceType
{
	private bool firstMove = true;

	public bool ValidateMove(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
	{
		return false;

	}
}


