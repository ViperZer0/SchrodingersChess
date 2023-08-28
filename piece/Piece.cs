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
	/// <param name="isWhite">True if this is a white piece, false if black.</param>
	/// <param name="rank">Rank to start the piece at.</param>
	/// <param name="file">File to start the piece at.</param>
	public void Create(IAdjudicator adjudicator, Board board, bool isWhite, uint rank, uint file)
	{
		this.adjudicator = adjudicator;
		this.board = board;

		this.white = isWhite;
		MoveToSquare(rank, file);

		this.pieceTypes = PieceTypeFactory.GetAllPieceTypes().ToList();
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
		if (this.rank == rank && this.file == file)
		{
			MoveToSquare(this.rank, this.file);
			return;
		}

		// If piece makes an invalid move.
		if (!ValidateMove(this.rank, this.file, rank, file))
		{
			MoveToSquare(this.rank, this.file);
			return;
		}
		// If adjudicator says this piece made an invalid move.
		if (!this.adjudicator.ValidateMove(this, this.rank, this.file, rank, file))
		{
			MoveToSquare(this.rank, this.file);
			return;
		}

		// Otherwise, we "resolve" what type of piece this is and make the move.
		FilterPieceTypes(this.rank,this.file, rank, file);

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

	private bool ValidateMove(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
	{
		return this.pieceTypes.Any(p => p.ValidateMove(rankFrom, fileFrom, rankTo, fileTo));
	}

	private void FilterPieceTypes(uint rankFrom, uint fileFrom, uint rankTo, uint fileTo)
	{
		this.pieceTypes = this.pieceTypes.Where(p => p.ValidateMove(rankFrom, fileFrom, rankTo, fileTo)).ToList();
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

