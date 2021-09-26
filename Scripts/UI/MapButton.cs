public class MapButton : MenuButton
{
    public int lvl;

    public override void Press()
    {
        UIManager.currentLvlOnMap = lvl;
        UIManager.ProсessButtonClick(type);
    }
}
