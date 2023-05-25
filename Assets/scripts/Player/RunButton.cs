using UnityEngine;
using UnityEngine.UI;

public class RunButton : MonoBehaviour
{
    public Button button;
    public PlayerController playerController;

    private bool isRunning;

    private void Start()
    {
        button.onClick.AddListener(ToggleRunning);
    }

    private void ToggleRunning()
    {
        isRunning = !isRunning;
        playerController.SetRunning(isRunning);
    }
}