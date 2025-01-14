using UnityEngine;

public class ExitApp : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}