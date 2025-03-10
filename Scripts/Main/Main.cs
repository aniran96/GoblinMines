using System;
using Game.Manager;
using Godot;

namespace GoblinMines;

public partial class Main : Node
{
    // references to scenes instantiated as a child
    private GridManager _gridManagerNode;
    // node references
    private Sprite2D _cursorNode;
    private Button _placeTowerButtonNode,
                   _placeVillageButtonNode;
    private Node2D _ySortRootNode;

    // scene references
    private PackedScene _towerScene,
                        _villageScene,
                        _toPlaceBuildingScene;



    // variables
    private Vector2I? _hoveredGridCell;               // to denote the active cell over which mouse is being hovered

    public override void _Ready()
    {
        _ySortRootNode = GetNode<Node2D>("YSortRoot");
        _gridManagerNode = GetNode<GridManager>("GridManager");
        _towerScene = GD.Load<PackedScene>( "res://Scenes/Building/Tower.tscn" );   
        _villageScene = GD.Load<PackedScene>( "res://Scenes/Building/Village.tscn" );  
        _cursorNode = GetNode<Sprite2D>("Cursor");
        _placeTowerButtonNode = GetNode<Button>("PlaceTowerButton");
        _placeVillageButtonNode = GetNode<Button>("PlaceVillageButton");
        ConnectSignals();
        _cursorNode.Visible = false;
    
    }

    public override void _Process(double delta)
    {
        Vector2I gridPosition = _gridManagerNode.GetMouseGridCellPosition();        
        _cursorNode.GlobalPosition = gridPosition * Globals.GRID_SIZE;

        if ( _cursorNode.Visible && ( !_hoveredGridCell.HasValue || _hoveredGridCell.Value != gridPosition ) ) 
        {
            _hoveredGridCell = gridPosition;
            _gridManagerNode.HighLightExpandedBuildableTiles( _hoveredGridCell.Value, Globals.RADIUS );
        }
       
//        GD.Print( gridPosition );
    }

    private void ConnectSignals() 
    {
        _placeTowerButtonNode.Pressed += OnTowerButtonPressed;
        _placeVillageButtonNode.Pressed += OnVillageButtonPressed;
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
        Node2D building = _toPlaceBuildingScene.Instantiate<Node2D>();
        _ySortRootNode.AddChild( building );
        Vector2 gridPosition = _hoveredGridCell.Value;
        building.GlobalPosition = gridPosition * Globals.GRID_SIZE;
      //  _gridManagerNode.MarkTileAsOccupied( _hoveredGridCell.Value );

        _hoveredGridCell = null;
        _gridManagerNode.ClearHighLightedTiles();
    }

    private void OnTowerButtonPressed()
    {
        _toPlaceBuildingScene = _towerScene;
        _cursorNode.Visible = true;
        _gridManagerNode.HighLightBuildableTiles();
    }

     private void OnVillageButtonPressed()
    {
        _toPlaceBuildingScene = _villageScene;
        _cursorNode.Visible = true;
        _gridManagerNode.HighLightBuildableTiles();
    }

     
}
