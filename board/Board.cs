using Godot;
using System;
using System.Collections.Generic;

public partial class Board : Node2D
{
	private uint numRanks = 8;
	private uint numFiles = 8;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		FitBoardToScreen();
		// Change the board size whenever the screen size changes.
		GetViewport().SizeChanged += FitBoardToScreen;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// "Snaps" the x, y coordinates given to the nearest board space.
	/// </summary>
	/// <param name="coords">The coordinates</param>
	/// <returns>A tuple representing a rank (up and down) and file (left to
	/// right) starting at 0 representing A8 (black is at 0), which is weird.</returns>
	public (uint rank, uint file) GetNearestBoardSpace(Vector2 coords)
	{
		// We expect this to be checked beforehand.
		if (!CoordsOnBoard(coords))
		{
			throw new ArgumentOutOfRangeException("Coordinates were outside of board space!");
		}

		// Get coordinates given the top left board corner is (0,0).
		Vector2 relativeCoords = coords - GetTopLeftBoardCorner();
		relativeCoords /= GetSpaceSize();
		Vector2 closestSpace = relativeCoords.Floor();
		// X and Y should both be integers due to floor, and shouldn't be
		// negative due to previous check.
		return ((uint)closestSpace.Y, (uint)closestSpace.X);
	}

	/// <summary>
	/// Given coordinates, determines whether the mouse is on board or not.
	/// If not we want to "invalidate" the move.
	/// </summary>
	/// <param name="coords">Vector2 coords to check against.</param>
	/// <returns>True if the coordinates are on the board, false otherwise.</returns>
	public bool CoordsOnBoard(Vector2 coords)
	{
		if(coords < GetTopLeftBoardCorner())
		{
			return false;
		}
		if(coords > (GetTopLeftBoardCorner() + GetBoardSize()))
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// Returns the coordinates of the center of a square.
	/// </summary>
	/// <param name="rank">The rank of the square (up and down the board)</param>
	/// <param name="file">The file of the square (forward and back)</param>
	/// <returns>The x,y coordinates of the center of the given square.</returns>
	public Vector2 GetSpaceCoords(uint rank, uint file)
	{
		if (rank >= numRanks)
		{
			throw new ArgumentOutOfRangeException($"Rank greater than {numRanks}");
		}
		if (file >= numFiles)
		{
			throw new ArgumentOutOfRangeException($"File greater than ${numFiles}");
		}
		return (new Vector2(file, rank) * GetSpaceSize() + GetCenterOfSpaceOffset());
	}

	/// <summary>
	/// Returns the coordinates of the center of a square given its space name
	/// (i.e "C3", "A2")
	/// </summary>
	/// <param name="spaceName">The name of the space, i.e "A5"</param>
	/// <returns>The coordinates of the center of the square.</returns>
	public Vector2 GetSpaceCoords(string spaceName)
	{
		return new Vector2(0, 0);
	}

	/// <summary>
	/// Converts a rank and file index to a space name.
	/// </summary>
	/// <param name="rank">The index of the rank.</param>
	/// <param name="file">The index of the file.</param>
	/// <returns>The string coordinates (i.e "A3")</returns>
	public string RankAndFileIndexToSpaceName(uint rank, uint file)
	{
		return "A8";
	}

	/// <summary>
	/// Converts a space name to a rank and file index tuple.
	/// </summary>
	/// <param name="spaceName">The string representation of the space name (i.e
	/// "A5")</param>
	/// <returns>A tuple containing the rank and file index.</returns>
	public (uint rank, uint file) SpaceNameToRankAndFile(string spaceName)
	{
		return (0, 0);
	}

	// Call this when the board resizes (to move the pieces)
	[Signal]
	public delegate void BoardResizedEventHandler();


	// Get the size of each space in pixels (global?)
	public Vector2 GetSpaceSize()
	{
		return GetBoardSize() / new Vector2(numFiles, numRanks);
	}

	// Offset to top left space center using global coords?
	private Vector2 GetCenterOfSpaceOffset()
	{
		Vector2 relativeSizeOfSpace = GetSpaceSize();
		Vector2 leftTopCornerOfBoard = GetTopLeftBoardCorner();
		return leftTopCornerOfBoard + relativeSizeOfSpace/2;
	}

	// In number of pixels, but scaled according to the size of the sprite.
	private Vector2 GetBoardSize()
	{
		Sprite2D sprite = GetNode<Sprite2D>("BoardSprite");
		Texture2D texture = sprite.Texture;
		return texture.GetSize() * sprite.Scale;
	}

	private Vector2 GetTextureSize()
	{
		Texture2D texture = GetNode<Sprite2D>("BoardSprite").Texture;
		return texture.GetSize();
	}

	// Global position (in pixels) of TL corner of board.
	private Vector2 GetTopLeftBoardCorner()
	{
		return this.GlobalPosition - GetBoardSize() / 2;
	}

	// Resizes the board to fit in the screen centered.
	private void FitBoardToScreen()
	{		
		//Fit the board to the screen.
		Vector2 size = GetViewport().GetVisibleRect().Size;

		// Get the smallest of the sizes of the viewport.
		float minimumSize = size[(int)size.MinAxisIndex()];

		Sprite2D sprite = GetNode<Sprite2D>("BoardSprite");

		// We're assuming the board is square so x and y are the same.
		float scaleFactor = minimumSize / GetTextureSize().X;

		// Scale the board.
		sprite.Scale = new Vector2(scaleFactor, scaleFactor);
		
		// Then center it?
		this.Position = GetViewport().GetVisibleRect().Position + GetViewport().GetVisibleRect().Size / 2;
		EmitSignal(SignalName.BoardResized);
	}
}
