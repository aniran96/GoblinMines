using Godot;

namespace GoblinMines.Resources.Scripts.Building;

[ GlobalClass ]
public partial class BuildingResource : Resource
{
	[ Export ]
	public int BuildableRadius { get; private set; }
	[ Export ]
	public int ResourceRadius { get; private set; }
	[ Export ]
	public PackedScene BuildingScene { get; private set; }
}
