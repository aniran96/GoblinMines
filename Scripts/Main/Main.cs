using Godot;
using System;

namespace GoblinMines;

public partial class Main : Node2D
{
    // node references
    private Sprite2D _cursorNode;
    private Button _placeBuildingButtonNode;

    // scene references
    private PackedScene _buildingScene;

    public override void _Ready()
    {
        _buildingScene = GD.Load<PackedScene>( "res://Scenes/Building/Building.tscn" );
        _cursorNode = GetNode<Sprite2D>("%Cursor");
        _placeBuildingButtonNode = GetNode<Button>("PlaceBuildingButton");
        ConnectSignals();
        _cursorNode.Visible = false;
    
    }

    public override void _Process(double delta)
    {
        Vector2 gridPosition = GetMouseGridCellPosition();        
        _cursorNode.GlobalPosition = gridPosition * Globals.GRID_SIZE;
//        GD.Print( gridPosition );
    }

    private void ConnectSignals() 
    {
        _placeBuildingButtonNode.Pressed += OnButtonPressed;
    }

    public override void _Input(InputEvent evt)
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
    }

    private void OnButtonPressed()
    {
        _cursorNode.Visible = true;
    }
}
