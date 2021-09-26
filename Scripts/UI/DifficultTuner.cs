
public class DifficultTuner : ValueTuner
{
    private void Start()
    {
        text.text = "Difficult: " + UIManager.DifficultLevel.ToString();
        scrollbar.value = ((int)UIManager.DifficultLevel - 1) / 2f;
    }

    public void TuneDifficult()
    {
        int difficult = (int)(scrollbar.value * 2) + 1;
        UIManager.SetCurrentDifficultLevel(difficult);
        text.text = "Difficult: " + UIManager.DifficultLevel.ToString();
    }
}
