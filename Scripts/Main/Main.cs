using Godot;
using System;

namespace GoblinMines;

public partial class Main : Node2D
{
    // node references
    private Sprite2D _cursorNode;
    private Button _placeBuildingButtonNode;
    private TileMapLayer _highLightTileMapLayerNode;

    // scene references
    private PackedScene _buildingScene;

    // run time constants
     private static readonly int RADIUS = 3;

    // variables
    private Vector2? _hoveredGridCell;               // to denote the active cell over which mouse is being hovered

    public override void _Ready()
    {
        _buildingScene = GD.Load<PackedScene>( "res://Scenes/Building/Building.tscn" );
        _cursorNode = GetNode<Sprite2D>("Cursor");
        _placeBuildingButtonNode = GetNode<Button>("PlaceBuildingButton");
        _highLightTileMapLayerNode = GetNode<TileMapLayer>("HighLightTileMapLayer");
        ConnectSignals();
        _cursorNode.Visible = false;
    
    }

    public override void _Process(double delta)
    {
        Vector2 gridPosition = GetMouseGridCellPosition();        
        _cursorNode.GlobalPosition = gridPosition * Globals.GRID_SIZE;

        if ( _cursorNode.Visible && ( !_hoveredGridCell.HasValue || _hoveredGridCell.Value != gridPosition ) ) 
        {
            _hoveredGridCell = gridPosition;
        }
        UpdateHighLightTileMapLayer();
//        GD.Print( gridPosition );
    }

    private void ConnectSignals() 
    {
        _placeBuildingButtonNode.Pressed += OnButtonPressed;
    }

    public override void _UnhandledInput(InputEvent evt)
    {
        if ( _cursorNode.Visible == true && evt.IsActionPressed( "left_click" ) ) 
        {
            PlaceBuildingAtMousePosition();
            _cursorNode.Visible = false;
        }
    }

    private Vector2 GetMouseGridCellPosition() 
    {
        Vector2 mousePosition = GetGlobalMousePosition();
        Vector2 gridPosition = mousePosition / Globals.GRID_SIZE;
        gridPosition = gridPosition.Floor();
        return gridPosition;
    }

    private void PlaceBuildingAtMousePosition() 
    {
        Node2D building = _buildingScene.Instantiate<Node2D>();
        AddChild( building );

        Vector2 gridPosition = GetMouseGridCellPosition();
        building.GlobalPosition = gridPosition * Globals.GRID_SIZE;

        _hoveredGridCell = null;
        UpdateHighLightTileMapLayer();
    }

    private void OnButtonPressed()
    {
        _cursorNode.Visible = true;
    }

    private void UpdateHighLightTileMapLayer() 
    {
        _highLightTileMapLayerNode.Clear();
        if ( !_hoveredGridCell.HasValue ) 
        {
            return;
        }
        else 
        {
            for ( var x = _hoveredGridCell.Value.X - RADIUS; x <= _hoveredGridCell.Value.X + RADIUS; x++ ) 
            {
                for ( var y = _hoveredGridCell.Value.Y - RADIUS; y <= _hoveredGridCell.Value.Y + RADIUS; y++ ) 
                {
                    _highLightTileMapLayerNode.SetCell(new Vector2I( (int)x, (int)y ), 0, Vector2I.Zero);
                }
            }
        }
    }
}
