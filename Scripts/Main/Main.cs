using System;
using GoblinMines.Manager;
using GoblinMines.Resources.Scripts.Building;
using GoblinMines.Scripts.UI;
using Godot;

namespace GoblinMines;

public partial class Main : Node
{
    // references to scenes instantiated as a child
    private GridManager _gridManagerNode;
    // node references
    private Sprite2D _cursorNode;

    private Node2D _ySortRootNode;
    private GameUI _gameUINode;                       
    
    // resources
    private BuildingResource _toPlaceBuildingResource;



    // variables
    private Vector2I? _hoveredGridCell;               

    public override void _Ready()
    {
        InitialiseVariables();
        ConnectSignals();
        _cursorNode.Visible = false;
    
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
        _gameUINode.BuildingResourceSelected += OnBuildingResourceSelected;
        _gridManagerNode.ResourceTilesUpdated += OnResourceTilesUpdated;
    }

    private void InitialiseVariables() 
    {
        _gameUINode = GetNode<GameUI>("GameUI");
        _ySortRootNode = GetNode<Node2D>("YSortRoot");
        _gridManagerNode = GetNode<GridManager>("GridManager");
        _cursorNode = GetNode<Sprite2D>("Cursor");
    }

    public override void _UnhandledInput(InputEvent evt)
    {
        if ( _hoveredGridCell.HasValue && evt.IsActionPressed( "left_click" ) && _gridManagerNode.IsTilePositionBuildable( _hoveredGridCell.Value ) ) 
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
    }

    private void OnBuildingResourceSelected(BuildingResource buildingResource)
    {
        _toPlaceBuildingResource = buildingResource;
         _cursorNode.Visible = true;
        _gridManagerNode.HighLightBuildableTiles();
    }

     private void OnResourceTilesUpdated(int resourceCount)
    {
        GD.Print( resourceCount );     
    }

     
}
