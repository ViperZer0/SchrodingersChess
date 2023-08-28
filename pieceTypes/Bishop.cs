using Godot;
using System;

public class Bishop: IPieceType
{
    public bool ValidateMove(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
    {
        // Moving a piece to the exact square it came from is a "non" move so
        // it's "illegal". This will probably get handled elsewhere but doesn't
        // turn to check here too.
        if ((rankFrom == rankTo) && (fileFrom == fileTo))
        {
            return false;
        }

        int changeInRank = Math.Abs((int)rankFrom - (int)rankTo);
        int changeInFile = Math.Abs((int)fileFrom - (int)fileTo);

        // Moving diagonally
        if (changeInRank == changeInFile)
        {
            return true;
        }

        return false;
    }

    public bool ValidateCapture(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
    {
        return ValidateMove(rankFrom, fileFrom, rankTo, fileTo);
    }
}
