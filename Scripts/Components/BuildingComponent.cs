using Game.AutoLoads;
using Godot;

namespace Game.Components;

public partial class BuildingComponent : Node2D
{
	// properties
	[ Export ]
	public int BuildableRadius { get; private set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddToGroup( nameof( BuildingComponent ) );
		Callable.From( () => GameEvents.EmitBuildingPlaced( this )).CallDeferred();
	}

	public Vector2I GetGridCellPosition() 
	{
		 Vector2 position = GlobalPosition;
        Vector2 gridPosition = position / Globals.GRID_SIZE;
        gridPosition = gridPosition.Floor();
		Vector2I gridPositionInt = new Vector2I( (int)gridPosition.X, (int)gridPosition.Y );
        return gridPositionInt;
	}
}
