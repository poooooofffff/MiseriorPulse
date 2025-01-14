using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

public class AddToAttendance : MonoBehaviour
{
    public TMP_InputField EventNameInputField;
    public TMP_InputField ID1;
    public TMP_InputField ID2;
    public TMP_InputField ID3;
    public TextMeshProUGUI username;
    public TextMeshProUGUI orgName;
    public TextMeshProUGUI feedback;
    public isAbleToSubmit isAbleToSubmit;

    public void checkFields()
    {
        if (EventNameInputField.text != "" && ID1.text.Length == 4 && ID2.text.Length == 4 && ID3.text.Length == 1)
        {
            string studentId = ID1.text + "-" + ID2.text + "-" + ID3.text;
            StartCoroutine(Main.Instance.WebQueries.AddAttendance(username.text, studentId, orgName.text, EventNameInputField.text, OnRegistrationComplete));
        } else
        {
            feedback.text = "Please fill out the fields above.";
        }
    }

    private void OnRegistrationComplete(string result)
    {
        ApiResponse response;
        Debug.Log(result);
        response = JsonConvert.DeserializeObject<ApiResponse>(result);
        feedback.text = response.message;
        isAbleToSubmit = FindObjectOfType<isAbleToSubmit>();
        isAbleToSubmit.ClearInputFields();
    }

    public class ApiResponse
    {
        public string message { get; set; }
    }
}
