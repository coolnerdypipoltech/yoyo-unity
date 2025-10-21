using UnityEngine;

public class RegisterInputInterface : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject redDot;
    public void OnValueChanged(string value)
    {
        if(value.Length > 0)
        {
            redDot.SetActive(false);
        }
        else
        {
            redDot.SetActive(true);
        }
    }
}
