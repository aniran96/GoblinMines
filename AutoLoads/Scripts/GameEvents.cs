using Godot;
using Game.Components;
namespace Game.AutoLoads;

public partial class GameEvents : Node
{
	// signals
	[ Signal ]
	public delegate void BuildingPlacedEventHandler( BuildingComponent buildingComponent );

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

}
