using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour, IPointerClickHandler
{
    public SceneChanger sceneChanger;
    public TMP_InputField UsernameAndEmailInput;
    public TMP_InputField PasswordInput;
    public Button loginButton;
    public TMP_Text registerLink;
    public TMP_Text errorFeedback;

    // Start is called before the first frame update
    void Start()
    {
        loginButton.onClick.AddListener(() =>
        {
            StartCoroutine(Main.Instance.WebQueries.Login(UsernameAndEmailInput.text, PasswordInput.text, OnRegistrationComplete));
        });
    }

    public void OnRegistrationComplete(string result)
    {
        if (result == "Successfully Logged in.")
        {
            errorFeedback.text = result + " Try again later.";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        changeScene("Register");
    }

    public void changeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}