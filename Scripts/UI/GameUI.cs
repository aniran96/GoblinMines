using GoblinMines.Resources.Scripts.Building;
using Godot;

namespace GoblinMines.Scripts.UI;

public partial class GameUI : MarginContainer
{
	// signals 
	[Signal]
	public delegate void BuildingResourceSelectedEventHandler(BuildingResource buildingResource);

	// exported resources
	[Export] private BuildingResource[] _buildingResources;

	// node references
	private HBoxContainer _hBoxContainerNode;

    public override void _Ready()
    {
		InitialiseVariables();
        CreateBuildingButtons();
		ConnectSignals();
    }

    private void ConnectSignals(){}
	private void InitialiseVariables() 
	{
		_hBoxContainerNode = GetNode<HBoxContainer>("HB");
	}

	private void CreateBuildingButtons() 
	{
		foreach ( var buildingResource in _buildingResources ) 
		{
			var buildingButton = new Button();
			buildingButton.Text = $"Place {buildingResource.displayName}";
			_hBoxContainerNode.AddChild(buildingButton);
			buildingButton.Pressed += () => 
			{
				EmitSignal(SignalName.BuildingResourceSelected, buildingResource);
			};
		}
	}
}
