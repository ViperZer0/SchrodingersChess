using Godot;
using System;

public class Rook: IPieceType
{
    public bool ValidateMove(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
    {
        if ((rankFrom == rankTo) && (fileFrom == fileTo))
        {
            return false;
        }

        // moving horizontinally
        if (rankFrom == rankTo)
        {
            return true;
        }

        // moving vertically
        if (fileFrom == fileTo)
        {
            return true;
        }

        return false;
    }
}

