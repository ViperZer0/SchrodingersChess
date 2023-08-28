using Godot;
using System;

public partial class BoardInitializer : Node
{
	private IAdjudicator adjudicator;

	[Export]
	private Board board;

	[Export]
	private string piecePath = "res://piece/piece.tscn";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// This should be an immediate child of game.
		Node game = GetParent<Node>();
		adjudicator = GetParent().GetNode<IAdjudicator>("Adjudicator");

		BoardHistory boardHistory = GetParent().GetNode<BoardHistory>("BoardHistory");
		PackedScene pieceScene = GD.Load<PackedScene>(piecePath);

		// White pieces.
		for(uint rank = 0; rank < 2; rank++)
		{
			for(uint file = 0; file < 8; file++)
			{
				Piece piece = pieceScene.Instantiate<Piece>();
				piece.Create(adjudicator, board, PieceColor.White, rank, file);
				boardHistory.AddPieceToBoard(piece, rank, file);
				// Have to wait until scene is finished loading to add children.
				game.CallDeferred(Node.MethodName.AddChild, piece);
			}
		}

		// Black pieces.
		for(uint rank = 6; rank < 8; rank++)
		{
			for(uint file = 0; file < 8; file++)
			{
				Piece piece = pieceScene.Instantiate<Piece>();
				piece.Create(adjudicator, board, PieceColor.Black, rank, file);
				boardHistory.AddPieceToBoard(piece, rank, file);
				game.CallDeferred(Node.MethodName.AddChild, piece);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	/*
	public override void _Process(double delta)
	{
	}
	*/
}
