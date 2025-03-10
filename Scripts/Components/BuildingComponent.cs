using Game.AutoLoads.Scripts;
using Game.Resources.Scripts.Building;
using Godot;

namespace Game.Scripts.Components;

public partial class BuildingComponent : Node2D
{

	//exported variables
	[Export( PropertyHint.File, "*.tres") ]
	public string _buildingResourcePath;

	// variables/properties
	public BuildingResource BuildingResource { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if ( _buildingResourcePath != null ) 
		{
			BuildingResource = GD.Load<BuildingResource>( _buildingResourcePath );
		}
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
