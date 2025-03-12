using GoblinMines.Resources.Scripts.Building;
using GoblinMines.Scripts.Building.Sprites;
using GoblinMines.Scripts.UI;
using Godot;

namespace GoblinMines.Scripts.Manager;

public partial class BuildingManager : Node
{
	// resources
    private BuildingResource _toPlaceBuildingResource;

	// node references
	[Export]
	private GridManager _gridManagerNode;
	[Export]
	private GameUI _gameUINode;
	[Export]
	private Node2D _ySortRootNode;

	// scene references
	[Export]
	private PackedScene _buildingGhostScene;
	
	
	//variables
	private int _currentResourceCount;
	private int _startingResourceCount = 4;
	private int _currentlyUsedResourceCount;
	private int AvailableResourceCount => _startingResourceCount + _currentResourceCount - _currentlyUsedResourceCount;
	private Vector2I? _hoveredGridCell;     
	private BuildingGhost _buildingGhost; 
	
	public override void _Ready()
	{
		ConnectSignals();
	}
	public override void _Process(double delta)
    {
		if (!IsInstanceValid(_buildingGhost)) {return ;}
        Vector2I gridPosition = _gridManagerNode.GetMouseGridCellPosition();        
        _buildingGhost.GlobalPosition = gridPosition * Globals.GRID_SIZE;

        if ( _toPlaceBuildingResource != null && ( !_hoveredGridCell.HasValue || _hoveredGridCell.Value != gridPosition ) ) 
        {
            _hoveredGridCell = gridPosition;
			UpdateGridDisplay();
        }       
    }

	private void UpdateGridDisplay() 
	{
		if ( _hoveredGridCell == null ) { return; }
		_gridManagerNode.ClearHighLightedTiles();
		_gridManagerNode.HighLightBuildableTiles();
		if ( IsBuildingPlaceableAtTile( _hoveredGridCell.Value ) ) 
		{
			_gridManagerNode.HighLightExpandedBuildableTiles( _hoveredGridCell.Value, _toPlaceBuildingResource.BuildableRadius );
			_gridManagerNode.HighLightResourceTiles( _hoveredGridCell.Value, _toPlaceBuildingResource.ResourceRadius );
			_buildingGhost.SetValid();
		}
		else 
		{
			_buildingGhost.SetInvalid();
		}

	}

	private void ConnectSignals() 
	{
		_gridManagerNode.ResourceTilesUpdated += OnResourceTilesUpdated;
		 _gameUINode.BuildingResourceSelected += OnBuildingResourceSelected;
	}

	public override void _UnhandledInput(InputEvent evt)
    {
		if ( evt.IsActionPressed( Globals.ACTION_CANCEL ) ) 
		{
			ClearBuildingGhost();
		}
		else if ( 
			_hoveredGridCell.HasValue && 
			_toPlaceBuildingResource != null &&
		    evt.IsActionPressed( Globals.ACTION_LEFT_CLICK ) && 
			IsBuildingPlaceableAtTile( _hoveredGridCell.Value )
			)
			
        {
            PlaceBuildingAtHoveredCellPosition();
            
        }
    }

    private void PlaceBuildingAtHoveredCellPosition() 
    {
        if ( !_hoveredGridCell.HasValue ) 
        {
            return;
        }
        Node2D building = _toPlaceBuildingResource.BuildingScene.Instantiate<Node2D>();
        _ySortRootNode.AddChild( building );
        Vector2 gridPosition = _hoveredGridCell.Value;
        building.GlobalPosition = gridPosition * Globals.GRID_SIZE;
		_currentlyUsedResourceCount += _toPlaceBuildingResource.ResourceCost;
		ClearBuildingGhost();
    }

	private void ClearBuildingGhost() 
	{
		_hoveredGridCell = null;
		_gridManagerNode.ClearHighLightedTiles();
		if ( IsInstanceValid( _buildingGhost ) )
		{
			_buildingGhost.QueueFree();
		}
		_buildingGhost = null;
	}

	private bool IsBuildingPlaceableAtTile( Vector2I tilePosition ) 
	{
		return _gridManagerNode.IsTilePositionBuildable( tilePosition ) &&
			AvailableResourceCount >= _toPlaceBuildingResource.ResourceCost  ;
	}


	private void OnResourceTilesUpdated(int resourceCount) 
	{
		_currentResourceCount = resourceCount;
	}

	 private void OnBuildingResourceSelected(BuildingResource buildingResource)
    {
		if ( IsInstanceValid(_buildingGhost) ) 
		{
			_buildingGhost.QueueFree();
		}
		_buildingGhost = _buildingGhostScene.Instantiate<BuildingGhost>();
		_ySortRootNode.AddChild( _buildingGhost );
		var buildingSprite = buildingResource.SpriteScene.Instantiate<Sprite2D>();
		_buildingGhost.AddChild( buildingSprite );
        _toPlaceBuildingResource = buildingResource;
        UpdateGridDisplay();
    }
}
