using UnityEngine.InputSystem;

public class InputManager
{
    private static InputSystem_Actions _actions;

    public static InputSystem_Actions Actions
    {
        get
        {

            if (_actions == null)
            {
                _actions = new InputSystem_Actions();
            }
            return _actions;
        }


    }
    public static void RemoveMap()
    {
        Actions.Disable();
    }
    public static void SwitchMap(InputActionMap mapToActivate)
    {
        Actions.Disable();
        mapToActivate.Enable();
    }
}
