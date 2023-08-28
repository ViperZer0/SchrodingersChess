using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class BoardHistory : Node
{
	private List<BoardState> boardStates = new List<BoardState>();
	
	public BoardState GetCurrentBoardState()
	{
		return boardStates.Last();
	}

	public void AddPieceToBoard(Piece piece, uint rank, uint file)
	{
		if (boardStates.Count == 0)
		{
			boardStates.Add(new BoardState());
		}

		BoardState boardState = GetCurrentBoardState();

		boardState.SetPieceInPlace(piece, rank, file);
	}

	public void MovePiece(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
	{
		BoardState boardState = GetCurrentBoardState();
		boardStates.Add(boardState.MovePiece(rankFrom, fileFrom, rankTo, fileTo));
	}

	public void CapturePiece(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
	{
		BoardState boardState = GetCurrentBoardState();
		boardStates.Add(boardState.CapturePiece(rankFrom, fileFrom, rankTo, fileTo));
	}
}
