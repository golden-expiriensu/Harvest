public class JoystickTuner : ValueTuner
{
    private Player player;

    private void Start()
    {
        player = GetComponentInParent<Player>();

        if (type == Type.Sensivity)
        {
            text.text = "Joystick dead zone = " + player.Controls.CurrentDeadZone;
            scrollbar.value = player.Controls.CurrentDeadZone; 
        }
        else if(type == Type.JoystickSize)
        {
            text.text = "Joystick size = " + player.Controls.CurrentSize;
            scrollbar.value = player.Controls.CurrentSize;
        }
    }

    public void TuneSensivity()
    {
        player.UIManager.ProсessTunerValue(type, scrollbar, text);
    }
}
