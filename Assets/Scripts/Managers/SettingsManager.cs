using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
        Application.targetFrameRate = 120;
        Physics.simulationMode = SimulationMode.Script;
        Physics.autoSimulation = false;
    }


}
