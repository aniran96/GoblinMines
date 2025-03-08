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
	private HashSet<Vector2I> _occupiedCells = new(); // TO prevent buildings from being placed on top of each other
	
	
	 public Vector2I GetMouseGridCellPosition() 
    {
        Vector2 mousePosition = _highLightTileMapLayerNode.GetGlobalMousePosition();
        Vector2 gridPosition = mousePosition / Globals.GRID_SIZE;
        gridPosition = gridPosition.Floor();
		Vector2I gridPositionInt = new Vector2I( (int)gridPosition.X, (int)gridPosition.Y );
        return gridPositionInt;
    }

	public void MarkTileAsOccupied( Vector2I tilePosition ) 
	{
		_occupiedCells.Add( tilePosition );
	}

	public bool IsTilePositionValid( Vector2I tilePosition ) 
	{
		var customData = _baseTerrainTileMapLayerNode.GetCellTileData( tilePosition );

		if ( customData == null ) 
		{
			return false;
		}
		else if ( !(bool)customData.GetCustomData("buildable") ) 
		{
			return false;
		}

		bool isContains = !_occupiedCells.Contains( tilePosition );
		return isContains;
	}

	public void HighLightValidTilesInRadius( Vector2I rootCell, int radius ) 
	{
		ClearHighLightedTiles();
            for ( var x = rootCell.X - radius; x <= rootCell.X + radius; x++ ) 
            {
                for ( var y = rootCell.Y - radius; y <= rootCell.Y + radius; y++ ) 
                {
					var tilePosition = new Vector2I( x, y );

					if ( !IsTilePositionValid( tilePosition ) ) 
					{
						continue;
					}
					else 
					{
						_highLightTileMapLayerNode.SetCell(tilePosition, 0, Vector2I.Zero);
					}
                }
            }
	}

	public void ClearHighLightedTiles() 
	{
		_highLightTileMapLayerNode.Clear();
	}
}
