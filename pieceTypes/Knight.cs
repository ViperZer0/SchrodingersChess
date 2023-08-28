using Godot;
using System;

public class Knight: IPieceType
{
    public bool ValidateMove(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
    {
        int changeInRank = Math.Abs((int)rankFrom - (int)rankTo);
        int changeInFile = Math.Abs((int)fileFrom - (int)fileTo);

        if (changeInRank == 2 && changeInFile == 1)
        {
            return true;
        }
        
        if (changeInRank == 1 && changeInFile == 2)
        {
            return true;
        }

        return false;
    }
}
