using Godot;
using System.Collections.Generic;

namespace Game.Manager;

public partial class GridManager : Node
{
	//node references
	[ Export ] 
	private TileMapLayer _highLightTileMapLayerNode,
						 _baseTerrainTileMapLayerNode;

	// variables
	private HashSet<Vector2> _occupiedCells = new(); // TO prevent buildings from being placed on top of each other
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	 public Vector2 GetMouseGridCellPosition() 
    {
        Vector2 mousePosition = _highLightTileMapLayerNode.GetGlobalMousePosition();
        Vector2 gridPosition = mousePosition / Globals.GRID_SIZE;
        gridPosition = gridPosition.Floor();
        return gridPosition;
    }

	public void MarkTileAsOccupied( Vector2 tilePosition ) 
	{
		_occupiedCells.Add( tilePosition );
	}

	public bool IsTilePositionValid( Vector2 tilePosition ) 
	{
		bool isContains = !_occupiedCells.Contains( tilePosition );
		return isContains;
	}

	public void HighLightValidTilesInRadius( Vector2 rootCell, int radius ) 
	{
		ClearHighLightedTiles();
            for ( var x = rootCell.X - radius; x <= rootCell.X + radius; x++ ) 
            {
                for ( var y = rootCell.Y - radius; y <= rootCell.Y + radius; y++ ) 
                {
					if ( !IsTilePositionValid( new Vector2( x, y) ) ) 
					{
						continue;
					}
					else 
					{
						_highLightTileMapLayerNode.SetCell(new Vector2I( (int)x, (int)y ), 0, Vector2I.Zero);
					}
                }
            }
	}

	public void ClearHighLightedTiles() 
	{
		_highLightTileMapLayerNode.Clear();
	}
}
