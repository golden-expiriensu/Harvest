using UnityEngine;

public class Controls : MonoBehaviour
{
    private Player player;
    public FieldConstructor field;

    public Vector3 currentCell;
    private Vector3 currentTouchDirection;

    public VariableJoystick joystick;
    public RectTransform joystickTransform;

    public float CurrentDeadZone { get; private set; }
    public float CurrentSize { get; private set; }
    public bool CurrentJoystickDynamicStatus { get; private set; }

    private readonly int zoneAroundOpenMenuButton = 10;


    protected void Awake() => player = GetComponent<Player>();

    private void Start()
    {
        
        currentCell = new Vector3(
            player.Tractor.thisTransform.position.x,
            0,
            player.Tractor.thisTransform.position.z);

        CurrentDeadZone = player.DBPlayer.GetJoystickDeadZone();
        joystick.DeadZone = CurrentDeadZone;

        CurrentSize = player.DBPlayer.GetJoystickSize();
        joystickTransform.localScale = new Vector3(CurrentSize, CurrentSize, 0);

        CurrentJoystickDynamicStatus = player.DBPlayer.GetJoystickIsDynamic();
        joystick.SetMode(CurrentJoystickDynamicStatus ? JoystickType.Dynamic : JoystickType.Floating);
    }

    public void SetDeathZone(float value)
    {
        CurrentDeadZone = Mathf.Clamp(value, 0, 1);
        joystick.DeadZone = CurrentDeadZone;
        player.DBPlayer.SetJoystickDeadZone(CurrentDeadZone);
    }

    public void SetJoystickSize(float value)
    {
        CurrentSize = Mathf.Clamp(value, 0, 1);
        joystickTransform.localScale = new Vector3(CurrentSize, CurrentSize, 0);
        player.DBPlayer.SetJoystickSize(CurrentSize);
    }

    public void SetJoystickDynamicStatus(bool status)
    {
        CurrentJoystickDynamicStatus = status;
        joystick.SetMode(status ? JoystickType.Dynamic : JoystickType.Floating);
        player.DBPlayer.SetJoystickIsDynamic(status);
    }

    private void Update()
    {
        if (!player.Tractor.isMoving && player.Tractor.CanMove() && !player.UIManager.menuOpen)
        {
            GetTouchInput();
        }
        if (Input.GetKey(KeyCode.A)) player.AddHarvestScore(10);
    }

    private void GetTouchInput()
    {
        Vector2Int currentJoystickMove = new Vector2Int((int)joystick.Horizontal, (int)joystick.Vertical);

        if (Mathf.Abs(currentJoystickMove.x) == Mathf.Abs(currentJoystickMove.y))
            return;
        else
        {
            currentTouchDirection = new Vector3(currentJoystickMove.x, 0, currentJoystickMove.y);
            SetMove(currentTouchDirection);
        }
    }

    //TODO delete
    //private bool IsPlayerPressOnMenuButton()
    //{
    //    Vector2 cursor = Input.mousePosition;
    //    RectTransform rt = player.UIManager.openMenuButton;
    //    if ((rt.anchoredPosition.x + rt.rect.xMin - zoneAroundOpenMenuButton) < cursor.x &&
    //        (rt.anchoredPosition.x + rt.rect.xMax + zoneAroundOpenMenuButton) > cursor.x &&
    //        (rt.anchoredPosition.y + rt.rect.yMin - zoneAroundOpenMenuButton) < cursor.y &&
    //        (rt.anchoredPosition.y + rt.rect.yMax + zoneAroundOpenMenuButton) > cursor.y)
    //        return true;
    //    else return false;
    //}

    public void SetMove(Vector3 move)
    {
        player.Tractor.SetRotation(move);
        if (field.IsCellAvailable(new Vector2Int((int)(currentCell + move).x, (int)(currentCell + move).z)))
        {
            player.Tractor.isMoving = true;
            player.Tractor.startPosition = currentCell;
            currentCell += move;
            player.Tractor.currentMove = currentCell;
            field.MoveTractorToCell(new Vector2Int((int)currentCell.x, (int)currentCell.z));
        }
    }
}
