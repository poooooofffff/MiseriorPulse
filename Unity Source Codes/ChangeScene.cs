using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class ChangeScene : MonoBehaviour
{
    public SceneChanger sceneChanger;

    public void GoToAttendance()
    {
        sceneChanger = FindObjectOfType<SceneChanger>();
        sceneChanger.FadeToNextLevel();
    }

    public void GoToMain()
    {
        sceneChanger = FindObjectOfType<SceneChanger>();
        sceneChanger.FadeToPreviousLevel();
    }
}