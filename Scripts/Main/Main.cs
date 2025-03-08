using Game.Manager;
using Godot;

namespace GoblinMines;

public partial class Main : Node
{
    // references to scenes instantiated as a child
    private GridManager _gridManagerNode;
    // node references
    private Sprite2D _cursorNode;
    private Button _placeBuildingButtonNode;

    // scene references
    private PackedScene _buildingScene;



    // variables
    private Vector2? _hoveredGridCell;               // to denote the active cell over which mouse is being hovered

    public override void _Ready()
    {
        _gridManagerNode = GetNode<GridManager>("GridManager");
        _buildingScene = GD.Load<PackedScene>( "res://Scenes/Building/Building.tscn" );
        _cursorNode = GetNode<Sprite2D>("Cursor");
        _placeBuildingButtonNode = GetNode<Button>("PlaceBuildingButton");
        ConnectSignals();
        _cursorNode.Visible = false;
    
    }

    public override void _Process(double delta)
    {
        Vector2 gridPosition = _gridManagerNode.GetMouseGridCellPosition();        
        _cursorNode.GlobalPosition = gridPosition * Globals.GRID_SIZE;

        if ( _cursorNode.Visible && ( !_hoveredGridCell.HasValue || _hoveredGridCell.Value != gridPosition ) ) 
        {
            _hoveredGridCell = gridPosition;
            _gridManagerNode.HighLightValidTilesInRadius( _hoveredGridCell.Value, Globals.RADIUS );
        }
       
//        GD.Print( gridPosition );
    }

    private void ConnectSignals() 
    {
        _placeBuildingButtonNode.Pressed += OnButtonPressed;
    }

    public override void _UnhandledInput(InputEvent evt)
    {
        if ( _hoveredGridCell.HasValue && evt.IsActionPressed( "left_click" ) && _gridManagerNode.IsTilePositionValid( _hoveredGridCell.Value ) ) 
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
        Node2D building = _buildingScene.Instantiate<Node2D>();
        AddChild( building );

        Vector2 gridPosition = _hoveredGridCell.Value;
        building.GlobalPosition = gridPosition * Globals.GRID_SIZE;
        _gridManagerNode.MarkTileAsOccupied( _hoveredGridCell.Value );

        _hoveredGridCell = null;
        _gridManagerNode.ClearHighLightedTiles();
    }

    private void OnButtonPressed()
    {
        _cursorNode.Visible = true;
    }

     
}
