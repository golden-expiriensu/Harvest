using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public Type type;
    protected UIManager UIManager;

    public enum Type
    {
        Back,
        Restart,
        Save,
        JoystickSize, 
        Settings,
        Sensivity,
        MainMenu,
        NewGame,
        JoystickDynamicCheckBox,
        CreateNewGame,
        Size,
        Size_x,
        Size_y,
        Difficult,
        DifficultTuner,
        LoadLastGame,
        _removed_1,
        OpenGlobalMap,
        RecoverByAdd,
        MapLVL
    }

    protected virtual void Awake()
    {
        UIManager = GetComponentInParent<UIManager>();
    }

    public virtual void Press()
    {
        UIManager.ProсessButtonClick(type);
    }
}
