using Godot;
using System.Collections.Generic;
using GoblinMines.Scripts.Components;
using System.Linq;
using GoblinMines.AutoLoads.Scripts;
using System;


namespace GoblinMines.Scripts.Manager;

public partial class GridManager : Node
{

	// signals
	[ Signal ]
	public delegate void ResourceTilesUpdatedEventHandler( int collectedTiles );

	//node references
	[ Export ] 
	private TileMapLayer _highLightTileMapLayerNode;
	[ Export ]
	private TileMapLayer _baseTerrainTileMapLayerNode;

	// variables
	private HashSet<Vector2I> _validBuildableTiles = new();
	private HashSet<Vector2I> _collectedResourceTiles = new();
	private HashSet<Vector2I> _occupiedTiles = new();
	private List<TileMapLayer> _allTileMapLayers = new();	

    public override void _Ready()
    {
		_allTileMapLayers = GetAllTileMapLayers( _baseTerrainTileMapLayerNode );
		ConnectSignals();
    }

	private void ConnectSignals() 
	{
				GameEvents.Instance.BuildingPlaced += OnBuildingPlaced;
		GameEvents.Instance.BuildingDestroyed += OnBuildingDestroyed;
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
		var validTiles = GetValidTilesInRadius( rootCell, radius ).ToHashSet();
		var expandedTiles = validTiles.Except( _validBuildableTiles ).Except( _occupiedTiles );
		Vector2I atlasCoordinates = new Vector2I( 1, 0 );

		foreach( var tilePosition in expandedTiles ) 
		{
			_highLightTileMapLayerNode.SetCell( tilePosition, 0, atlasCoordinates );
		}
	}

	public void HighLightResourceTiles( Vector2I rootCell, int radius ) 
	{
		var resourceTiles = GetResourceTilesInRadius( rootCell, radius );
		Vector2I atlasCoordinates = new Vector2I( 1, 0 );

		foreach( var tilePosition in resourceTiles ) 
		{
			_highLightTileMapLayerNode.SetCell( tilePosition, 0, atlasCoordinates );
		}
	}



	public bool IsTilePositionBuildable( Vector2I tilePosition ) 
	{
		return _validBuildableTiles.Contains( tilePosition );
	}

	private bool TileHasCustomData( Vector2I tilePosition, string dataName ) 
	{
		foreach ( var tileMap in _allTileMapLayers ) 
		{
			var customData = tileMap.GetCellTileData( tilePosition );
			if ( customData == null ) 
			{
				continue;
			}
			return (bool)customData.GetCustomData( dataName );
		}
		return false;
	}

	public void ClearHighLightedTiles() 
	{
		
		_highLightTileMapLayerNode.Clear();
	}

	private void UpdateValidBuildableTiles( BuildingComponent buildingComponent ) 
	{
		_occupiedTiles.Add( buildingComponent.GetGridCellPosition() );
		var rootCell = buildingComponent.GetGridCellPosition();
		var radius = buildingComponent.BuildingResource.BuildableRadius;

		var validTiles = GetValidTilesInRadius( rootCell, radius );
		_validBuildableTiles.UnionWith( validTiles );

		_validBuildableTiles.ExceptWith( _occupiedTiles );	
	}

	private void RecalculateGrid( BuildingComponent excludeBuildingComponent ) 
	{
		_occupiedTiles.Clear();
		_validBuildableTiles.Clear();
		var buildingComponents = GetTree().GetNodesInGroup( nameof(BuildingComponent) ).Cast<BuildingComponent>()
		.Where( ( buildingComponent ) => buildingComponent != excludeBuildingComponent );

		foreach( var buildingComponent in buildingComponents ) 
		{
			UpdateValidBuildableTiles( buildingComponent );
		}
	}

	private void UpdateCollectedResourceTiles( BuildingComponent buildingComponent ) 
	{
		var rootCell = buildingComponent.GetGridCellPosition();
		var resourceTiles = GetResourceTilesInRadius( rootCell, buildingComponent.BuildingResource.ResourceRadius);
		var oldResourceCount = _collectedResourceTiles.Count;
		_collectedResourceTiles.UnionWith( resourceTiles );
		var newResourceCount = _collectedResourceTiles.Count;
		if ( oldResourceCount != newResourceCount ) 
		{
			EmitSignal( SignalName.ResourceTilesUpdated, _collectedResourceTiles.Count );
		}
	}

	private List<Vector2I> GetTilesInRadius( Vector2I rootCell, int radius, Func<Vector2I, bool> filterfn ) 
	{
		List<Vector2I> result = new();

		for ( var x = rootCell.X - radius; x <= rootCell.X + radius; x++ ) 
            {
                for ( var y = rootCell.Y - radius; y <= rootCell.Y + radius; y++ ) 
                {
					var tilePosition = new Vector2I( x, y );

					if ( !filterfn( tilePosition) ) 
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

	private List<Vector2I> GetValidTilesInRadius( Vector2I rootCell, int radius ) 
	{
		return GetTilesInRadius( rootCell, radius, (tilePosition) => 
		{
			return TileHasCustomData( tilePosition, Globals.IS_BUILDABLE );
		} );
	}

	private List<Vector2I> GetResourceTilesInRadius( Vector2I rootCell, int radius ) 
	{
		return GetTilesInRadius( rootCell, radius, (tilePosition) => 
		{
			return TileHasCustomData( tilePosition, Globals.IS_WOOD );
		} );
	}

	// private IEnumerable<Vector2I> GetOccupiedTiles() 
	// {
	// 	var buildingComponents = GetTree().GetNodesInGroup( nameof( BuildingComponent ) ).Cast<BuildingComponent>();
	// 	var occupiedTiles = buildingComponents.Select( x => x.GetGridCellPosition() );
	// 	return occupiedTiles;
	// }

	private void OnBuildingPlaced( BuildingComponent buildingComponent )
    {
		UpdateValidBuildableTiles( buildingComponent );
		UpdateCollectedResourceTiles( buildingComponent );
    }

	private void OnBuildingDestroyed( BuildingComponent buildingComponent ) 
	{
		RecalculateGrid( buildingComponent );
	}
}
