/* Tracks the pieces as they move around the board.
 * A list of board states can be used to recreate
 * the history of the game.
 */
using System;
using System.Collections.Generic;

public class BoardState 
{   
    protected uint numRanks = 8;
    protected uint numFiles = 8;

    /* Rows come first in arrays (i.e RANK)
     * Columns come second (i.e FILE)
     */
    protected OptionalPiece[,] pieces;

    protected List<Piece> capturedPieces;

    public BoardState()
    {
        numRanks = 8;
        numFiles = 8;
        pieces = new OptionalPiece[numRanks, numFiles];
        capturedPieces = new List<Piece>();
    }

    public BoardState(uint ranks, uint files)
    {
        numRanks = ranks;
        numFiles = files;
        pieces = new OptionalPiece[ranks, files];
        capturedPieces = new List<Piece>();
    }

    public BoardState Copy()
    {
        NewBoardState newBoardState = new NewBoardState();
        newBoardState.SetNumRanks(numRanks);
        newBoardState.SetNumFiles(numFiles);
        // This makes an independent copy of the array, but the items themselves
        // are still referenced shallow-ly (that's what we want.
        newBoardState.SetPieces((OptionalPiece[,])pieces.Clone());
        newBoardState.SetCapturedPieces(new List<Piece>(capturedPieces));

        return newBoardState;
    }

    /// <summary>
    /// Sets the piece in a given state without returning a new board state (i.e
    /// a "move")
    /// </summary>
    /// <param name="piece">The piece that is being added to a square.</param>
    /// <param name="rank">The rank of the piece.</param>
    /// <param name="file">The file of the piece.</param>
    public void SetPieceInPlace(Piece piece, uint rank, uint file)
    {
        CheckBounds(rank, file);
        pieces[rank, file].Piece = piece;
    }

    /* Queries that can be run against the BoardState (i.e to check the validity
     * of a move)
     */

    /// <summary>
    /// Checks if there is a piece at a location on the board.
    /// </summary>
    /// <param name="rank">The rank to check</param>
    /// <param name="file">The file to check</param>
    /// <returns>True if a piece is there, false if not.</returns>
    public bool PieceAtLocation(uint rank, uint file)
    {
        CheckBounds(rank, file);
        return pieces[rank, file].HasPiece;
    }

    /// <summary>
    /// Checks if a piece is found between two points.
    /// The two points must be in a straight line (horizontinal, vertical, or
    /// diagonal) since otherwise there are multiple possible paths.
    /// This is used to check if a piece's move is "blocked" and doesn't apply
    /// to knights.
    /// If the two points are NOT in a straight line, this function returns
    /// false (we assume the path is not blocked)
    /// </summary>
    /// <param name="startRank">The starting rank (exclusive: because this
    /// should be the square that a given piece STARTS on)</param>
    /// <param name="startFile">The starting file (exclusive: because this
    /// should be the square that a given piece STARTS on)</param>
    /// <param name="endRank">The ending rank (also exclusive: because if a
    /// piece IS on this square, it may be capturable)</param>
    /// <param name="endFile">The ending file (also exclusive: because if a
    /// piece IS on this square, it may be capturable)</param>
    /// <returns>[TODO:description]</returns>
    public bool PieceBetweenCoords(uint startRank, uint startFile, uint endRank, uint endFile)
    {
        CheckBounds(startRank, startFile);
        CheckBounds(endRank, endFile);

        // Same square move?
        if (startRank == endRank && startFile == endFile)
        {
            return false;
        }

        // Moving vertically
        if (startRank == endRank)
        {
            uint lowerFile, higherFile;
            if(startFile < endFile)
            {
                lowerFile = startFile;
                higherFile = endFile;
            }
            else
            {
                lowerFile = endFile;
                higherFile = startFile;
            }
            for(uint file = lowerFile+1; file < higherFile; file++)
            {
                if(PieceAtLocation(startRank, file))
                    return true;
            }

            // No piece found in line.
            return false;
        }

        // Moving horizontinally
        if (startFile == endFile)
        {
            uint lowerRank, higherRank;
            if(startRank < endRank)
            {
                lowerRank = startRank;
                higherRank = endRank;

            }
            else
            {
                lowerRank = endRank;
                higherRank = startRank;
            }

            for(uint rank = lowerRank+1; rank < higherRank; rank++)
            {
                if(PieceAtLocation(startRank, rank))
                    return true;
            }

            // No piece found in line.
            return false;
        }

        // Moving diagonally?
        int changeInRank = Math.Abs((int)startRank - (int)endRank);
        int changeInFile = Math.Abs((int)startFile - (int)endFile);

        if (changeInRank == changeInFile)
        {
            // Four possible directions from here.
            uint lowerFile, lowerRank;
            if (startRank < endRank)
            {
                lowerRank = startRank;
            }
            else
            {
                lowerRank = endRank;
            }

            if (startFile < endFile)
            {
                lowerFile = startFile;
            }
            else
            {
                lowerFile = endFile;
            }

            for(uint offset=1; offset < changeInFile; offset++)
            {
                if(PieceAtLocation(lowerRank + offset, lowerFile + offset))
                    return true;
            }
            return false;
        }

        // Coordinates are not in a line or diagonal.
        return false;
    }

    /// <summary>
    /// Get piece at coordinates. This will throw an exception
    /// in the underlying operation if no piece is there.
    /// For a softer operation use GetMaybePieceAtCoords()
    /// </summary>
    /// <param name="rank">The piece's rank</param>
    /// <param name="file">The piece's file</param>
    /// <returns>The piece at the coordinates specified.</returns>
    public Piece GetPieceAtCoords(uint rank, uint file)
    {
        CheckBounds(rank, file);
        return pieces[rank, file].Piece;
    }

    /// <summary>
    /// Get a piece that may or may not exist at the coordinates
    /// specified.
    /// </summary>
    /// <param name="rank">The piece's rank</param>
    /// <param name="file">The piece's file</param>
    /// <returns>A piece if one is found or null if no piece is there.</returns>
    public Piece? GetMaybePieceAtCoords(uint rank, uint file)
    {
        CheckBounds(rank, file);
        return pieces[rank, file].MaybePiece;
    }

    /* BoardState OPERATIONS*/
    /// <summary>
    /// Moves a piece from one square to another. The starting square is allowed
    /// to be empty. The destination square will be overwritten with the
    /// starting square piece (whether or not it exists)
    /// </summary>
    /// <param name="rankFrom">The starting square's rank</param>
    /// <param name="fileFrom">The starting square's file</param>
    /// <param name="rankTo">The ending square's rank</param>
    /// <param name="fileTo">The ending square's file</param>
    public BoardState MovePiece(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
    {
        BoardState newBoard = Copy();
        newBoard.MovePieceInPlace(rankFrom, fileFrom, rankTo, fileTo);
        return newBoard;
    }

    /// <summary>
    /// Captures a piece and moves the capturing piece to that square.
    /// Will throw an exception if the piece being captured does not exist.
    /// Doesn't care if the capturing piece really exists.
    /// </summary>
    /// <param name="rankFrom">The capturing piece's starting rank.</param>
    /// <param name="fileFrom">The capturing piece's starting file.</param>
    /// <param name="rankTo">The captured piece's rank.</param>
    /// <param name="fileTo">The captured piece's file.</param>
    public BoardState CapturePiece(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
    {
        BoardState newBoard = Copy();
        newBoard.CapturePieceInPlace(rankFrom, fileFrom, rankTo, fileTo);
        return newBoard;
    }

    /* In-place BoardState operations */
    /// <summary>
    /// Moves a piece from one square to another. The starting square is allowed
    /// to be empty. The destination square will be overwritten with the
    /// starting square piece (whether or not it exists)
    /// </summary>
    /// <param name="rankFrom">The starting square's rank</param>
    /// <param name="fileFrom">The starting square's file</param>
    /// <param name="rankTo">The ending square's rank</param>
    /// <param name="fileTo">The ending square's file</param>
    private void MovePieceInPlace(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
    {
        CheckBounds(rankFrom, fileFrom);
        CheckBounds(rankTo, fileTo);
        OptionalPiece piece = pieces[rankFrom, fileFrom];
        pieces[rankTo, fileTo] = piece;
        pieces[rankFrom, fileFrom] = new OptionalPiece();
    }

    /// <summary>
    /// Captures a piece and moves the capturing piece to that square.
    /// Will throw an exception if the piece being captured does not exist.
    /// Doesn't care if the capturing piece really exists.
    /// </summary>
    /// <param name="rankFrom">The capturing piece's starting rank.</param>
    /// <param name="fileFrom">The capturing piece's starting file.</param>
    /// <param name="rankTo">The captured piece's rank.</param>
    /// <param name="fileTo">The captured piece's file.</param>
    private void CapturePieceInPlace(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
    {
        CheckBounds(rankFrom, fileFrom);
        CheckBounds(rankTo, fileTo);
        // Assert piece can be found at destination and try to "capture" it.
        Piece capturedPiece = pieces[rankTo, fileTo].Piece;
        capturedPieces.Add(capturedPiece);
        // Move starting piece to new square.
        OptionalPiece piece = pieces[rankFrom, fileFrom];
        pieces[rankTo, fileTo] = piece;
        pieces[rankFrom, fileFrom] = new OptionalPiece();
    }

    private void CheckBounds(uint rank, uint file)
    {
        if (rank >= numRanks || file >= numFiles)
        {
            throw new ArgumentOutOfRangeException("Rank or file was greater than board size!");
        }
    }

    /// <summary>
    /// When a board state copies itself,
    /// it needs to be able to set the new board state's array of
    /// OptionalPieces; a private field.
    /// This class is used to expose those details of that inner class.
    /// </summary>
    private class NewBoardState: BoardState
    {
        public void SetNumRanks(uint numRanks)
        {
            this.numRanks = numRanks;
        }

        public void SetNumFiles(uint numFiles)
        {
            this.numFiles = numFiles;
        }

        public void SetPieces(OptionalPiece[,] pieces)
        {
            this.pieces = pieces;
        }

        public void SetCapturedPieces(List<Piece> capturedPieces)
        {
            this.capturedPieces = capturedPieces;
        }
    }

}

