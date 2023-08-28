using Godot;
using System;

public interface IPieceType
{
	// Return true if a move from the given square to the given square is valid.
	// Returns false otherwise.
	public bool ValidateMove(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo);

    // Returns true if a capture from the given square to the given square is
    // valid.
    // Returns false otherwise.
    public bool ValidateCapture(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo);

}
