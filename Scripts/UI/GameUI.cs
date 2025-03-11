using Godot;

namespace GoblinMines.Scripts.UI;

public partial class GameUI : MarginContainer
{
	// signals 
	[Signal]
	public delegate void PlaceTowerButtonPressedEventHandler();
	[Signal]
	public delegate void PlaceVillageButtonPressedEventHandler();

	// node references
	private Button _placeTowerButtonNode;
	private Button _placeVillageButtonNode;

    public override void _Ready()
    {
        _placeTowerButtonNode = GetNode<Button>("%PlaceTowerButton");
		_placeVillageButtonNode = GetNode<Button>("%PlaceVillageButton");
		ConnectSignals();
    }

    private void ConnectSignals()
    {
		_placeTowerButtonNode.Pressed += OnPlaceTowerButtonPressed;
		_placeVillageButtonNode.Pressed += OnPlaceVillageButtonPressed;
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
