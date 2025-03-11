using GoblinMines.Resources.Scripts.Building;
using Godot;

namespace GoblinMines.Scripts.UI;

public partial class GameUI : MarginContainer
{
	// signals 
	[Signal]
	public delegate void PlaceTowerButtonPressedEventHandler();
	[Signal]
	public delegate void PlaceVillageButtonPressedEventHandler();

	// exported resources
	[Export] private BuildingResource[] _buildingResources;

    public override void _Ready()
    {
        
		ConnectSignals();
    }

    private void ConnectSignals()
    {

    }

	private void CreateBuildingButtons() 
	{
		foreach ( var buildingResource in _buildingResources ) 
		{
			var buildingButton = new Button();
		}
	}

	private void OnPlaceTowerButtonPressed() 
	{
		EmitSignal(SignalName.PlaceTowerButtonPressed);
	}

	private void OnPlaceVillageButtonPressed() 
	{
		EmitSignal(SignalName.PlaceVillageButtonPressed);
	}
}
