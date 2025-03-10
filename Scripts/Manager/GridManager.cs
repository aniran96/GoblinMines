using Godot;
using System.Collections.Generic;
using Game.Components;
using System.Linq;
using Game.AutoLoads;


namespace Game.Manager;

public partial class GridManager : Node
{
	//node references
	[ Export ] 
	private TileMapLayer _highLightTileMapLayerNode,
						 _baseTerrainTileMapLayerNode;

	// variables
	private HashSet<Vector2I> _validBuildableTiles = new();
	private List<TileMapLayer> _allTileMapLayers = new();	

    public override void _Ready()
    {
		_allTileMapLayers = GetAllTileMapLayers( _baseTerrainTileMapLayerNode );
		GameEvents.Instance.BuildingPlaced += OnBuildingPlaced;

	
    }

    public Vector2I GetMouseGridCellPosition() 
    {
        Vector2 mousePosition = _highLightTileMapLayerNode.GetGlobalMousePosition();
        Vector2 gridPosition = mousePosition / Globals.GRID_SIZE;
        gridPosition = gridPosition.Floor();
		Vector2I gridPositionInt = new Vector2I( (int)gridPosition.X, (int)gridPosition.Y );
        return gridPositionInt;
    }

	private List<TileMapLayer> GetAllTileMapLayers( TileMapLayer rootTileMapLayer ) 
	{
		var result = new List<TileMapLayer>();
		var children = rootTileMapLayer.GetChildren();
		children.Reverse();

		foreach ( var child in children ) 
		{
			if ( child is TileMapLayer childTileMapLayer ) 
			{
				result.AddRange( GetAllTileMapLayers( childTileMapLayer ) );
			}
		}
		result.Add( rootTileMapLayer );
		return result;
	}

	public void HighLightBuildableTiles() 
	{  
		
		foreach( var tilePosition in _validBuildableTiles ) 
		{
			_highLightTileMapLayerNode.SetCell( tilePosition, 0, Vector2I.Zero );
		}

	}

	public void HighLightExpandedBuildableTiles( Vector2I rootCell, int radius ) 
	{
		ClearHighLightedTiles();
		HighLightBuildableTiles();
		var validTiles = GetValidTilesInRadius( rootCell, radius ).ToHashSet();
		var expandedTiles = validTiles.Except( _validBuildableTiles ).Except( GetValidTiles() );
		Vector2I atlasCoordinates = new Vector2I( 1, 0 );

		foreach( var tilePosition in expandedTiles ) 
		{
			_highLightTileMapLayerNode.SetCell( tilePosition, 0, atlasCoordinates );
		}
	}



	public bool IsTilePositionBuildable( Vector2I tilePosition ) 
	{
		return _validBuildableTiles.Contains( tilePosition );
	}

	private bool IsTilePositionValid( Vector2I tilePosition ) 
	{
		foreach ( var tileMap in _allTileMapLayers ) 
		{
			var customData = tileMap.GetCellTileData( tilePosition );
			if ( customData == null ) 
			{
				continue;
			}
			return (bool)customData.GetCustomData( "buildable" );
		}
		return false;


		// bool isContains = !_occupiedCells.Contains( tilePosition );
		// return isContains;
	}

	public void ClearHighLightedTiles() 
	{
		
		_highLightTileMapLayerNode.Clear();
	}


	 private void OnBuildingPlaced( BuildingComponent buildingComponent )
    {
		UpdateValidBuildableTiles( buildingComponent );
		
		
		//MarkTileAsOccupied( buildingComponent.GetGridCellPosition() ); 
		       
    }

	private void UpdateValidBuildableTiles( BuildingComponent buildingComponent ) 
	{
		var rootCell = buildingComponent.GetGridCellPosition();
		var radius = buildingComponent.BuildableRadius;

		var validTiles = GetValidTilesInRadius( rootCell, radius );
		_validBuildableTiles.UnionWith( validTiles );

		_validBuildableTiles.ExceptWith( GetValidTiles() );

		// foreach( var existingBuildingComponents in buildingComponents ) 
		// {
		// 	_validBuildableTiles.Remove( existingBuildingComponents.GetGridCellPosition() );
		// }

		
	}

	private List<Vector2I> GetValidTilesInRadius( Vector2I rootCell, int radius ) 
	{
		List<Vector2I> result = new();

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
						result.Add( tilePosition );
					}
                }
            }
			return result;
	}

	private IEnumerable<Vector2I> GetValidTiles() 
	{
		var buildingComponents = GetTree().GetNodesInGroup( nameof( BuildingComponent ) ).Cast<BuildingComponent>();
		var occupiedTiles = buildingComponents.Select( x => x.GetGridCellPosition() );
		return occupiedTiles;
	}

		// public void MarkTileAsOccupied( Vector2I tilePosition ) 
	// {

	// 		_occupiedCells.Add( tilePosition );  
	// }

}
