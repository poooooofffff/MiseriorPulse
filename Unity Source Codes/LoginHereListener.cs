using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class MouseListener : MonoBehaviour, IPointerClickHandler
{
    public SceneChanger sceneChanger;

    public void OnPointerClick(PointerEventData eventData)
    {
        sceneChanger = FindObjectOfType<SceneChanger>();
        sceneChanger.FadeToPreviousLevel();
    }
}
