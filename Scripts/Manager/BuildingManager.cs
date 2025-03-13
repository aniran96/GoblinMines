using System.Linq;
using GoblinMines.AutoLoads.Scripts;
using GoblinMines.Resources.Scripts.Building;
using GoblinMines.Scripts.Building.Sprites;
using GoblinMines.Scripts.Components;
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

	// scene references
	[Export]
	private PackedScene _buildingGhostScene;

	//variables
	private int _currentResourceCount;
	private int _startingResourceCount = 4;
	private int _currentlyUsedResourceCount;
	private int AvailableResourceCount => _startingResourceCount + _currentResourceCount - _currentlyUsedResourceCount;
	private Vector2I _hoveredGridCell;
	private BuildingGhost _buildingGhost;
	private State _currentState;

	public override void _Ready()
	{
		ConnectSignals();
	}
	public override void _Process(double delta)
	{
		Vector2I gridPosition = _gridManagerNode.GetMouseGridCellPosition();

		if (_hoveredGridCell != gridPosition)
		{
			_hoveredGridCell = gridPosition;
			UpdateHoveredGridCell();
		}

		switch (_currentState)
		{
			case State.Normal:
				{
					break;
				}
			case State.PlaceBuilding:
				{
					_buildingGhost.GlobalPosition = gridPosition * Globals.GRID_SIZE;
					break;
				}
		}
	}

	private void UpdateGridDisplay()
	{
		_gridManagerNode.ClearHighLightedTiles();
		_gridManagerNode.HighLightBuildableTiles();
		if (IsBuildingPlaceableAtTile(_hoveredGridCell))
		{
			_gridManagerNode.HighLightExpandedBuildableTiles(_hoveredGridCell, _toPlaceBuildingResource.BuildableRadius);
			_gridManagerNode.HighLightResourceTiles(_hoveredGridCell, _toPlaceBuildingResource.ResourceRadius);
			_buildingGhost.SetValid();
		}
		else
		{
			_buildingGhost.SetInvalid();
		}

	}

	private void ConnectSignals()
	{
		_gridManagerNode.ResourceTilesUpdated += OnResourceTilesUpdated;
		_gameUINode.BuildingResourceSelected += OnBuildingResourceSelected;
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		switch (_currentState)
		{
			case State.Normal:
				{
					if (evt.IsActionPressed(Globals.ACTION_RIGHT_CLICK))
					{
						DestroyBuildingAtHoveredCellPosition();
					}
					break;
				}
			case State.PlaceBuilding:
				{
					if (evt.IsActionPressed(Globals.ACTION_CANCEL))
					{
						ChangeState(State.Normal);
					}
					else if (
						_toPlaceBuildingResource != null &&
						evt.IsActionPressed(Globals.ACTION_LEFT_CLICK) &&
						IsBuildingPlaceableAtTile(_hoveredGridCell)
							)
					{
						PlaceBuildingAtHoveredCellPosition();

					}
					break;
				}

			default:
				{
					break;
				}
		}

	}

	private void PlaceBuildingAtHoveredCellPosition()
	{

		Node2D building = _toPlaceBuildingResource.BuildingScene.Instantiate<Node2D>();
		_ySortRootNode.AddChild(building);
		Vector2 gridPosition = _hoveredGridCell;
		building.GlobalPosition = gridPosition * Globals.GRID_SIZE;
		_currentlyUsedResourceCount += _toPlaceBuildingResource.ResourceCost;
		ChangeState(State.Normal);
	}

	private void DestroyBuildingAtHoveredCellPosition()
	{
		var buildingComponent = GetTree().GetNodesInGroup( nameof( BuildingComponent ) ).Cast<BuildingComponent>()
								.FirstOrDefault( ( buildingComponent ) => buildingComponent.GetGridCellPosition() == _hoveredGridCell );
		
		if ( buildingComponent == null ) { return; } 
	}

	private void ClearBuildingGhost()
	{
		_gridManagerNode.ClearHighLightedTiles();
		if (IsInstanceValid(_buildingGhost))
		{
			_buildingGhost.QueueFree();
		}
		_buildingGhost = null;
	}

	private bool IsBuildingPlaceableAtTile(Vector2I tilePosition)
	{
		return _gridManagerNode.IsTilePositionBuildable(tilePosition) &&
			AvailableResourceCount >= _toPlaceBuildingResource.ResourceCost;
	}

	private void UpdateHoveredGridCell()
	{
		switch (_currentState)
		{
			case State.Normal:
				{
					break;
				}
			case State.PlaceBuilding:
				{
					UpdateGridDisplay();
					break;
				}
			default:
				{
					break;
				}
		}
	}

	private void ChangeState(State toState)
	{
		switch (_currentState)
		{
			case State.Normal:
				{
					break;
				}
			case State.PlaceBuilding:
				{
					ClearBuildingGhost();
					_toPlaceBuildingResource = null;
					break;
				}
		}

		_currentState = toState;

		switch (_currentState)
		{
			case State.Normal:
				{
					break;
				}
			case State.PlaceBuilding:
				{
					_buildingGhost = _buildingGhostScene.Instantiate<BuildingGhost>();
					_ySortRootNode.AddChild(_buildingGhost);
					break;
				}
		}
	}

	private void OnResourceTilesUpdated(int resourceCount)
	{
		_currentResourceCount = resourceCount;
	}

	private void OnBuildingResourceSelected(BuildingResource buildingResource)
	{
		ChangeState(State.PlaceBuilding);
		var buildingSprite = buildingResource.SpriteScene.Instantiate<Sprite2D>();
		_buildingGhost.AddChild(buildingSprite);
		_toPlaceBuildingResource = buildingResource;
		UpdateGridDisplay();
	}
}
