using Godot;

namespace GoblinMines.AutoLoads.Scripts;

public partial class Globals : Node
{
	public const int GRID_SIZE = 64;
	public const string IS_BUILDABLE = "is_buildable";
	public const string IS_WOOD = "is_wood";

     public static readonly StringName ACTION_LEFT_CLICK = "left_click";
     public static readonly StringName ACTION_CANCEL = "cancel";
     public static readonly StringName ACTION_RIGHT_CLICK = "right_click";
}
