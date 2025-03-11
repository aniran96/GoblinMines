using GoblinMines.Resources.Scripts.Building;
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
	[Export]
	private Sprite2D _cursorNode;
	
	
	//variables
	private int _currentResourceCount;
	private int _startingResourceCount = 4;
	private int _currentlyUsedResourceCount;

	private int AvailableResourceCount => _startingResourceCount + _currentResourceCount - _currentlyUsedResourceCount;

	private Vector2I? _hoveredGridCell;      
	
	public override void _Ready()
	{
		ConnectSignals();
	}
	public override void _Process(double delta)
    {
        Vector2I gridPosition = _gridManagerNode.GetMouseGridCellPosition();        
        _cursorNode.GlobalPosition = gridPosition * Globals.GRID_SIZE;

        if ( _toPlaceBuildingResource != null &&_cursorNode.Visible && ( !_hoveredGridCell.HasValue || _hoveredGridCell.Value != gridPosition ) ) 
        {
            _hoveredGridCell = gridPosition;
            _gridManagerNode.ClearHighLightedTiles();
            _gridManagerNode.HighLightExpandedBuildableTiles( _hoveredGridCell.Value, _toPlaceBuildingResource.BuildableRadius );
            _gridManagerNode.HighLightResourceTiles( _hoveredGridCell.Value, _toPlaceBuildingResource.ResourceRadius );

        }       
    }

	private void ConnectSignals() 
	{
		_gridManagerNode.ResourceTilesUpdated += OnResourceTilesUpdated;
		 _gameUINode.BuildingResourceSelected += OnBuildingResourceSelected;
	}

	public override void _UnhandledInput(InputEvent evt)
    {
        if ( _hoveredGridCell.HasValue && 
			 _toPlaceBuildingResource != null &&
		    evt.IsActionPressed( "left_click" ) && 
			_gridManagerNode.IsTilePositionBuildable( _hoveredGridCell.Value ) &&
			AvailableResourceCount >= _toPlaceBuildingResource.ResourceCost ) 
        {
            PlaceBuildingAtHoveredCellPosition();
            _cursorNode.Visible = false;
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
        _hoveredGridCell = null;
        _gridManagerNode.ClearHighLightedTiles();

		_currentlyUsedResourceCount += _toPlaceBuildingResource.ResourceCost;
		GD.Print(AvailableResourceCount);
    }


	private void OnResourceTilesUpdated(int resourceCount) 
	{
		_currentResourceCount = resourceCount;
	}

	 private void OnBuildingResourceSelected(BuildingResource buildingResource)
    {
        _toPlaceBuildingResource = buildingResource;
         _cursorNode.Visible = true;
        _gridManagerNode.HighLightBuildableTiles();
    }
}
