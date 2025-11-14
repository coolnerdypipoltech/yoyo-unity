using UnityEngine;

public class cancelInterface : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnClick()
    {
        NewScreenManager.instance.ShowLoadingScreen(false);
    }
}
