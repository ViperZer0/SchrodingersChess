using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Piece : Node2D
{
	[Export]
	private uint rank = 0;
	[Export]
	private uint file = 0;

	[Export]
	private Board board;

	[Export]
	private PieceColor pieceColor;

	// How much padding the edge of the piece should have to the edge of the
	// square.
	[Export]
	private float marginScale = 0.9f;

	// Piece is grabbed only when these both are true.
	private bool mouseOver = false;
	private bool pickedUp = false;
	
	private IAdjudicator adjudicator;

    private BoardHistory boardHistory;

	private List<IPieceType> pieceTypes;

	// True if this is a white piece,
	// false if this is a black piece.
	public PieceColor PieceColor
	{
		get => pieceColor;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Area2D area = GetNode("PieceSprite").GetNode<Area2D>("GrabArea");
		area.MouseEntered += OnMouseEnter;
		area.MouseExited += OnMouseExit;
		// Not sure if this check is necessary?
		if(board == null)
		{
			board = GetParent().GetNode<Board>("Board");
			board.BoardResized += OnBoardResized;
		}

		ResizePieceToBoard();
		MoveToSquare(this.rank, this.file);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (this.pickedUp)
		{
			this.Position = GetViewport().GetMousePosition();
		}
	}

	
	/// <summary>
	/// MUST be called when instantiating a new piece:
	/// </summary>
	/// <param name="adjudicator">A reference to the game adjudicator.</param>
    /// <param name="boardHistory"> A reference to the board history.</param>
    /// <param name="pieceColor">What color piece this is</param>
	/// <param name="rank">Rank to start the piece at.</param>
	/// <param name="file">File to start the piece at.</param>
	public void Create(IAdjudicator adjudicator, BoardHistory boardHistory, Board board, PieceColor pieceColor, uint rank, uint file)
	{
		this.adjudicator = adjudicator;
        this.boardHistory = boardHistory;
		this.board = board;

		this.pieceColor = pieceColor;
		MoveToSquare(rank, file);

		this.pieceTypes = PieceTypeFactory.GetAllPieceTypes().ToList();
        // Add pawn type with the color argument.
        this.pieceTypes.Add(new Pawn(pieceColor));
	}

	// Called when an input event happens.
	public override void _Input(InputEvent @event)
	{
		if(@event.IsActionPressed("click") && this.mouseOver)
		{
			this.pickedUp = true;
		}
		if(@event.IsActionReleased("click") && this.pickedUp)
		{
			this.pickedUp = false;
			HandlePieceDropped();
		}
	}

	private void HandlePieceDropped()
	{
		if(!this.board.CoordsOnBoard(this.Position))
		{
			// Move the piece back to where it was.
			MoveToSquare(this.rank, this.file);
			return; 
		}

		// Move to the new space
		(uint rank, uint file) = this.board.GetNearestBoardSpace(this.Position);

		// Check validity of the move.

		// Piece made a non-move (landed on same space)
		if (MovedOntoSameSquare(this.rank, this.file, rank, file))
		{
			MoveToSquare(this.rank, this.file);
			return;
		}

        // Move passes through pieces (if not a knight move)
        if (MovePassesThroughPiece(this.rank, this.file, rank, file))
        {
            MoveToSquare(this.rank, this.file);
            return;
        }
            
        // Is this move a capture? 
        // If so is it a valid capture?
        (bool capture, bool valid) = MoveIsValidCapture(this.rank, this.file, rank, file);
        if(!valid)
        {
            MoveToSquare(this.rank, this.file);
            return;
        }

		// If piece makes an invalid move.
		if (capture == false && !ValidateMove(this.rank, this.file, rank, file))
		{
			MoveToSquare(this.rank, this.file);
			return;
		}

        // If piece makes an invalid capture
        if (capture == true && !ValidateCapture(this.rank, this.file, rank, file))
        {
            MoveToSquare(this.rank, this.file);
            return;
        }

        if(!adjudicator.ValidateMove(this, this.rank, this.file, rank, file))
        {
            MoveToSquare(this.rank, this.file);
            return;
        }
		// Otherwise, we "resolve" what type of piece this is and make the move.
        if (capture == true)
        {
            FilterPieceTypesOnCapture(this.rank, this.file, rank, file);
            boardHistory.CapturePiece(this.rank, this.file, rank, file);
        }
        else
        {
            FilterPieceTypesOnMove(this.rank,this.file, rank, file);
            boardHistory.MovePiece(this.rank, this.file, rank, file);
        }

        adjudicator.EndTurn();
		MoveToSquare(rank, file);
	}

	private void MoveToSquare(uint rank, uint file)
	{
		this.Position = this.board.GetSpaceCoords(rank, file);
		this.rank = rank;
		this.file = file;
	}

	private void ResizePieceToBoard()
	{
		Vector2 spaceSize = this.board.GetSpaceSize();

		float minimumSize = spaceSize[(int)spaceSize.MinAxisIndex()];

		Sprite2D sprite = GetNode<Sprite2D>("PieceSprite");

		// We will assume that the board is perfectly square.
		float scaleFactor = minimumSize / sprite.Texture.GetSize().X * this.marginScale;

		sprite.Scale = new Vector2(scaleFactor, scaleFactor);

		// Reposition it.
		MoveToSquare(this.rank, this.file);
	}

    private bool MovedOntoSameSquare(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
    {
        return (rankFrom == rankTo && fileFrom == fileTo);
    }

    private bool MovePassesThroughPiece(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
    {
        return boardHistory.GetCurrentBoardState().PieceBetweenCoords(rankFrom, fileFrom, rankTo, fileTo);
    }

    /*
     * capture = true, valid = true := move lands on other side's piece
     * capture = true, valud = false := N/A
     * capture = false, valid = true := move does not land on any other piece.
     * capture = false, valid = false := Move lands on own side's pieces.
     */
    private (bool capture, bool valid) MoveIsValidCapture(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
    {
        bool capture = false;
        // We are landing on another piece.
        if (boardHistory.GetCurrentBoardState().PieceAtLocation(rankTo, fileTo))
        {
            Piece piece = boardHistory.GetCurrentBoardState().GetPieceAtCoords(rankTo, fileTo);
            capture = this.pieceColor != piece.PieceColor;
            // Can't capture our own piece!
            if (capture == false)
            {
                return (false, false);
            }
            return (true, true);
        }

        return (false, true);
    }

	private bool ValidateMove(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
	{
		return this.pieceTypes.Any(p => p.ValidateMove(rankFrom, fileFrom, rankTo, fileTo));
	}

    private bool ValidateCapture(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
    {
        return this.pieceTypes.Any(p => p.ValidateCapture(rankFrom, fileFrom, rankTo, fileTo));
    }

	private void FilterPieceTypesOnMove(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
	{
		this.pieceTypes = this.pieceTypes.Where(p => p.ValidateMove(rankFrom, fileFrom, rankTo, fileTo)).ToList();
	}

    private void FilterPieceTypesOnCapture(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
    {
        this.pieceTypes = this.pieceTypes.Where(p => p.ValidateCapture(rankFrom, fileFrom, rankTo, fileTo)).ToList();
    }

	// Event handler calld when a mouse enters the piece area.
	private void OnMouseEnter()
	{
		this.mouseOver = true;
	}

	// Event handler called when a mouse exits the piece area.
	private void OnMouseExit()
	{
		this.mouseOver = false;
	}

	private void OnBoardResized()
	{
		ResizePieceToBoard();
	}
}

