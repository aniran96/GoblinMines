using Godot;
using GoblinMines.Scripts.Components;

namespace GoblinMines.AutoLoads.Scripts;

public partial class GameEvents : Node
{
	// signals
	[ Signal ]
	public delegate void BuildingPlacedEventHandler( BuildingComponent buildingComponent );
	[ Signal ]
	public delegate void BuildingDestroyedEventHandler( BuildingComponent buildingComponent );

	// class variable
	public static GameEvents Instance { get; private set; }

    public override void _Notification( int what ) 
	{
		if ( what == NotificationSceneInstantiated ) 
		{
			Instance = this;
		}
	}

		public static void EmitBuildingPlaced( BuildingComponent buildingComponent ) 
		{
			Instance.EmitSignal( SignalName.BuildingPlaced, buildingComponent );
		}

		public static void EmitBuildingDestroyed( BuildingComponent buildingComponent ) 
		{
			Instance.EmitSignal( SignalName.BuildingDestroyed, buildingComponent );
		}

}
