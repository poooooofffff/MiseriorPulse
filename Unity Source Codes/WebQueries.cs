using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class WebQueries : MonoBehaviour
{
    public TextMeshProUGUI feedback;
    public TMP_InputField inputField1;
    public TMP_InputField inputField2;
    public Sprite newInputBg;
    public Sprite oldInputBg;

    public TextMeshProUGUI feedback_user;
    public TextMeshProUGUI feedback_email;

    public string dateAndTime;

    private XMLCreator xmlCreator;

    public Transform tableParent;
    public GameObject rowPrefab;

    public IEnumerator GetAttendanceData(string orgName)
    {
        WWWForm form = new WWWForm();
        form.AddField("orgName", orgName);

        string apiUrl = "http://localhost/MiseriorPulse/GetAttendanceTableData.php";
        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                PopulateTable(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error fetching data: " + request.error);
            }
        }
    }

    void PopulateTable(string jsonData)
    {
        Debug.Log(jsonData);  // Log the raw JSON data for debugging

        try
        {
            JArray data = JArray.Parse(jsonData);  // Parse the JSON data into a JArray

            foreach (var item in data)
            {
                if (item is JObject jsonObject)
                {
                    CreateTableRow(jsonObject);  // Create a row for each JObject
                }
                else
                {
                    Debug.LogWarning("Unexpected data format: item is not a JObject.");
                }
            }
        }
        catch (JsonException ex)
        {
            Debug.LogError($"JSON parsing error: {ex.Message}");
        }
    }

    void CreateTableRow(JObject jsonObject)
    {
        Debug.Log("Processing row table data");

        GameObject newRow = Instantiate(rowPrefab, tableParent);  // Create a new row
        SetRowData(newRow, "1", jsonObject["student_id"]);
        SetRowData(newRow, "2", jsonObject["username"]);
        SetRowData(newRow, "3", jsonObject["event_name"]);
    }

    void SetRowData(GameObject row, string fieldName, JToken data)
    {
        Transform fieldTransform = row.transform.Find(fieldName);  // Find the field in the row

        if (fieldTransform != null)
        {
            TextMeshProUGUI textComponent = fieldTransform.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = data?.ToString() ?? "N/A";  // Set the text or "N/A" if null
            }
            else
            {
                Debug.LogError($"TextMeshProUGUI component missing on field {fieldName}");
            }
        }
        else
        {
            Debug.LogError($"Field {fieldName} not found in row. Check the rowPrefab hierarchy.");
        }
    }


    public IEnumerator AddAttendance(string userName, string studentId, string orgName, string eventName, System.Action<string> onComplete)
    {

        WWWForm form = new WWWForm();
        form.AddField("user_name", userName);
        form.AddField("student_id", studentId);
        form.AddField("name_abbreviation", orgName);
        form.AddField("event_name", eventName);

        // Send the POST request to the PHP script
        using (UnityWebRequest webRequest = UnityWebRequest.Post("http://localhost/MiseriorPulse/PostAttendance.php", form))
        {
            // Wait for the request to complete
            yield return webRequest.SendWebRequest();

            // Check for errors
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // Process the response from the PHP script
                string response = webRequest.downloadHandler.text;

                // Call the callback function with the response
                onComplete?.Invoke(response);
            }
            else
            {
                // Handle any error that occurred during the request
                Debug.LogError("Request failed: " + webRequest.error);
                onComplete?.Invoke("Oop! An error while trying to save your data. Please try again later.");
            }
        }
    }

    public IEnumerator GetOrganizations()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("http://localhost/MiseriorPulse/GetOrganizations.php"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = ("http://localhost/MiseriorPulse/GetOrganizations.php").Split('/');
            int page = pages.Length - 1;
            
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    private void OnInputValueChanged(string value)
    {
        //change input field back to black
        Image usernameOrEmail_input = inputField1.GetComponentInChildren<Image>();
        Image password_input = inputField2.GetComponentInChildren<Image>();
        if (usernameOrEmail_input != null && password_input != null)
        {
            // Change the image to the new sprite
            usernameOrEmail_input.sprite = oldInputBg;
            Debug.Log("1 - Input field image updated.");

            password_input.sprite = oldInputBg;
            Debug.Log("2 - Input field image updated.");

            //Reset Feedback
            feedback.text = "";

            //Remove Listener
            inputField1.onValueChanged.RemoveListener(OnInputValueChanged);
            inputField2.onValueChanged.RemoveListener(OnInputValueChanged);
        }
        else
        {
            Debug.LogError("No Image component found on both TMP_InputField.");
        }
    }

    public IEnumerator Login(string username, string password, System.Action<string> onComplete)
    {
        Debug.Log("Starting Login Coroutine...");
        WWWForm form = new WWWForm();
        form.AddField("LoginUserOrEmail", username);
        form.AddField("LoginPass", password);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/MiseriorPulse/Login.php", form))
        {
            Debug.Log("Sending UnityWebRequest...");
            yield return www.SendWebRequest();

            ApiResponse response; 

            Debug.Log("UnityWebRequest completed. Checking result...");
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Request Failed.");
                response = JsonConvert.DeserializeObject<ApiResponse>(www.downloadHandler.text);
                //Give feedback to user
                feedback.text = response.message;

                //change input field to red
                Image usernameOrEmail_input = inputField1.GetComponentInChildren<Image>();
                Image password_input = inputField2.GetComponentInChildren<Image>();
                if (usernameOrEmail_input != null && password_input != null)
                {
                    // Change the image to the new sprite
                    usernameOrEmail_input.sprite = newInputBg;
                    Debug.Log("1 - Input field image updated.");

                    password_input.sprite = newInputBg;
                    Debug.Log("2 - Input field image updated.");
                }
                else
                {
                    Debug.LogError("No Image component found on both TMP_InputField.");
                }

                //add lisenter to input fields
                inputField1.onValueChanged.AddListener(OnInputValueChanged);
                inputField2.onValueChanged.AddListener(OnInputValueChanged);
            }
            else
            {
                response = JsonConvert.DeserializeObject<ApiResponse>(www.downloadHandler.text);
                Debug.Log(response.message);
                StartCoroutine(GetUserInfo(response.userId));
            }
        }
    }

    public IEnumerator GetUserInfo(string user_id)
    {
        Debug.Log("Starting to Get user info...");
        WWWForm form = new WWWForm();
        form.AddField("UserID", user_id);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/MiseriorPulse/GetUserInfo.php", form))
        {
            Debug.Log("Sending UnityWebRequest...");
            yield return www.SendWebRequest();

            ApiResponse response;

            Debug.Log("UnityWebRequest completed. Checking result...");
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Request Failed.");
                response = JsonConvert.DeserializeObject<ApiResponse>(www.downloadHandler.text);
            }
            else
            {
                Debug.Log("Server response: " + www.downloadHandler.text);
                response = JsonConvert.DeserializeObject<ApiResponse>(www.downloadHandler.text);
                Debug.Log(response.message);

                //create xml file
                xmlCreator = GetComponent<XMLCreator>();
                xmlCreator.CreateEncryptedXML(response.userId, response.userName, response.email, response.organization, response.position);
            }
        }
    }

    public IEnumerator Register(string username, string email, string password, string orgId, string position, System.Action<string> onComplete)
    {

        WWWForm form = new WWWForm();
        form.AddField("User", username);
        form.AddField("Email", email);
        form.AddField("Pass", password);
        form.AddField("OrgId", orgId);
        form.AddField("Position", position);

        Debug.Log(orgId);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/MiseriorPulse/Register.php", form))
        {
            yield return www.SendWebRequest();
            ApiResponse response;

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                response = JsonConvert.DeserializeObject<ApiResponse>(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                response = JsonConvert.DeserializeObject<ApiResponse>(www.downloadHandler.text);
            }

            Debug.Log(response.message);
            onComplete?.Invoke(response.message);
        }
    }

    public class ApiResponse
    {
        public string message { get; set; }
        public string userId { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string organization { get; set; }
        public string position { get; set; }
    }
}