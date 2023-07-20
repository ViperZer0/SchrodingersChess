using Godot;
using System;

public partial class Piece : Node2D
{
	// Piece is grabbed only when these both are true.
	private bool mouseOver = false;
	private bool pickedUp = false;
	
	[Export]
	private uint rank = 0;
	private uint file = 0;

	[Export]
	private Board board;

	// How much padding the edge of the piece should have to the edge of the
	// square.
	[Export]
	private float marginScale = 0.9f;


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
		}

		board.BoardResized += OnBoardResized;

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
			if(!this.board.CoordsOnBoard(this.Position))
			{
				// Move the piece back to where it was.
				MoveToSquare(this.rank, this.file);
			}
			else
			{
				// Move to the new space
				(uint rank, uint file) = this.board.GetNearestBoardSpace(this.Position);
				MoveToSquare(rank, file);
			}
		}
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

