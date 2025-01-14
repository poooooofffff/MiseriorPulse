using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

public class Register: MonoBehaviour
{
    public SceneChanger sceneChanger;

    public TMP_InputField UsernameInput;
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput1;
    public TMP_InputField PasswordInput2;
    public TMP_Dropdown dropdown;
    public string OrganizationId;
    public TMP_InputField PositionInput;

    public Button registerButton;
    public TMP_Text loginLink;

    public TextMeshProUGUI feedback_user;
    public TextMeshProUGUI feedback_email;
    public TextMeshProUGUI feedback_password;
    public TextMeshProUGUI feedback_password1;
    public TextMeshProUGUI feedback_position;

    public Sprite newInputBg;
    public Sprite oldInputBg;

    // Start is called before the first frame update
    void Start()
    {
        //Replace drop down with Organization Names
        StartCoroutine(ModifyDropdownOptions());

        UsernameInput.onValueChanged.AddListener(EnforceCharacterLimit);
        PasswordInput1.onValueChanged.AddListener(EnforceCharacterMinimum);

        registerButton.onClick.AddListener(() =>
        {
            int num = 0;
            //Check if passwords is empty and if they match
            if (PasswordInput1.text == PasswordInput2.text)
            {
                num++;
                if (PasswordInput1.text != "")
                {
                    num++;
                    if (PasswordInput2.text != "")
                    {
                        num++;
                    } else
                    {
                        errorInput(4);
                        feedback_password.text = "Confirm password.";
                        PasswordInput2.onValueChanged.AddListener(OnInputValueChangedPassword2);
                    }
                }
                else
                {
                    errorInput(4);
                    feedback_password.text = "Please enter a valid password.";
                    PasswordInput1.onValueChanged.AddListener(OnInputValueChangedPassword1);
                }
            } else
            {
                errorInput(4);
                feedback_password.text = "Passwords do not match.";
                PasswordInput2.onValueChanged.AddListener(OnInputValueChangedPassword2);
            }

            //Check if username is empty
            if (UsernameInput.text != "")
            {
                num++;
            } else
            {
                errorInput(1);
                feedback_user.text = "Please enter a username.";
                UsernameInput.onValueChanged.AddListener(OnInputValueChangedUser);
            }

            //Check if email is empty
            if (EmailInput.text != "")
            {
                num++;
            }
            else
            {
                errorInput(2);
                feedback_email.text = "Please enter an email address.";
                EmailInput.onValueChanged.AddListener(OnInputValueChangedEmail);
            }

            //Check if position is empty
            if (PositionInput.text != "")
            {
                num++;
            }
            else
            {
                errorInput(3);
                feedback_position.text = "Please enter your position within the selected organization.";
                PositionInput.onValueChanged.AddListener(OnInputValueChangedPosition);
            }

            OrganizationId = (dropdown.value + 1).ToString();
            Debug.Log(OrganizationId);

            //Check if all fields are met
            if (num == 6 && feedback_password1.text == "" && OrganizationId != "" && OrganizationId != "0")
            {
                StartCoroutine(Main.Instance.WebQueries.Register(UsernameInput.text, EmailInput.text, PasswordInput1.text, OrganizationId, PositionInput.text, OnRegistrationComplete));
            } else
            {
                Debug.Log(num);
            }
        });
    }

    public void OnRegistrationComplete(string result)
    {
        //Tells user that username exists.
        if (result == "Username already exists.")
        {
            errorInput(1);
            feedback_user.text = result;
            UsernameInput.onValueChanged.AddListener(OnInputValueChangedUser);
        }
        
        //Tell user that email already exists.
        else if (result == "Email is taken.")
        {
            errorInput(2);
            feedback_email.text = result;
            EmailInput.onValueChanged.AddListener(OnInputValueChangedEmail);
        }

        else if (result == "Invalid email format.")
        {
            errorInput(2);
            feedback_email.text = result;
            EmailInput.onValueChanged.AddListener(OnInputValueChangedEmail);
        }
        
        else if (result == "Successfully Registered.")
        {
            sceneChanger = FindObjectOfType<SceneChanger>();
            sceneChanger.FadeToPreviousLevel();
        }
    }

    private void setOrgID(string orgID)
    {
        Debug.Log(orgID);
        StartCoroutine(CheckOrganizations(orgID));
    }   

    private void errorInput(int value)
    {
        //change input to red
        switch (value)
        {
            //username field
            case 1:
                Image UsernameInputImage = UsernameInput.GetComponentInChildren<Image>();
                UsernameInputImage.sprite = newInputBg;
                break;

            //email field
            case 2:
                Image EmailInputImage = EmailInput.GetComponentInChildren<Image>();
                EmailInputImage.sprite = newInputBg;
                break;

            //position field
            case 3:
                Image PositionInputImage = PositionInput.GetComponentInChildren<Image>();
                PositionInputImage.sprite = newInputBg;
                break;

            //password
            case 4:
                Image PasswordInput1Image = PasswordInput1.GetComponentInChildren<Image>();
                PasswordInput1Image.sprite = newInputBg;
                Image PasswordInput2Image = PasswordInput2.GetComponentInChildren<Image>();
                PasswordInput2Image.sprite = newInputBg;
                break;
        }
    }

    private void EnforceCharacterLimit(string input)
    {
        int maxLength = 12;
        if (input.Length > maxLength)
        {
            // Truncate the text to the allowed maximum length
            UsernameInput.text = input.Substring(0, maxLength);
        }
    }

    private void EnforceCharacterMinimum(string input)
    {
        int minLength = 6;
        if (input.Length < minLength)
        {
            feedback_password1.text = "Password should be at least 6 characters long.";
        } else
        {
            feedback_password1.text = "";
        }
    }

    private void OnInputValueChangedUser(string value)
    {
        //Declare the bg of input field
        Image UsernameInputImage = UsernameInput.GetComponentInChildren<Image>();
        UsernameInputImage.sprite = newInputBg;

        if (UsernameInputImage != null)
        {
            // Change the image to the old image
            UsernameInputImage.sprite = oldInputBg;
            Debug.Log("Username Input field image updated.");

            //Reset Feedback
            feedback_user.text = "";

            //Remove Listener
            UsernameInput.onValueChanged.RemoveListener(OnInputValueChangedUser);
        }
        else
        {
            Debug.Log("No Image component found on both TMP_InputField.");
        }
    }

    private void OnInputValueChangedEmail(string value)
    {
        //Declare the bg of input field
        Image EmailInputImage = EmailInput.GetComponentInChildren<Image>();
        EmailInputImage.sprite = newInputBg;

        if (EmailInputImage != null)
        {
            // Change the image to the old image
            EmailInputImage.sprite = oldInputBg;
            Debug.Log("Email Input field image updated.");

            //Reset Feedback
            feedback_email.text = "";

            //Remove Listener
            EmailInput.onValueChanged.RemoveListener(OnInputValueChangedEmail);
        }
        else
        {
            Debug.Log("No Image component found on both TMP_InputField.");
        }
    }

    private void OnInputValueChangedPassword1(string value)
    {
        //Declare the bg of input field
        Image PasswordInput1Image = PasswordInput1.GetComponentInChildren<Image>();
        PasswordInput1Image.sprite = newInputBg;
        
        if (PasswordInput1Image != null)
        {
            // Change the image to the old image
            PasswordInput1Image.sprite = oldInputBg;
            Debug.Log("Password 1 Input field image updated.");

            //Reset Feedback
            feedback_password.text = "";

            //Remove Listener
            PasswordInput1.onValueChanged.RemoveListener(OnInputValueChangedPassword1);
        }
        else
        {
            Debug.Log("No Image component found on both TMP_InputField.");
        }
    }

    private void OnInputValueChangedPassword2(string value)
    {
        //Declare the bg of input field
        Image PasswordInput2Image = PasswordInput2.GetComponentInChildren<Image>();
        PasswordInput2Image.sprite = newInputBg;

        if (PasswordInput2Image != null)
        {
            // Change the image to the old image
            PasswordInput2Image.sprite = oldInputBg;
            Debug.Log("Password 2 Input field image updated.");

            //Reset Feedback
            feedback_password.text = "";

            //Remove Listener
            PasswordInput2.onValueChanged.RemoveListener(OnInputValueChangedPassword2);
        }
        else
        {
            Debug.Log("No Image component found on both TMP_InputField.");
        }
    }

    private void OnInputValueChangedPosition(string value)
    {
        //Declare the bg of input field
        Image PositionInputImage = PositionInput.GetComponentInChildren<Image>();
        PositionInputImage.sprite = newInputBg;

        if (PositionInputImage != null)
        {
            // Change the image to the old image
            PositionInputImage.sprite = oldInputBg;
            Debug.Log("Position Input field image updated.");

            //Reset Feedback
            feedback_position.text = "";

            //Remove Listener
            PositionInput.onValueChanged.RemoveListener(OnInputValueChangedPosition);
        }
        else
        {
            Debug.Log("No Image component found on both TMP_InputField.");
        }
    }

    private IEnumerator ModifyDropdownOptions()
    {
        // Clear current options
        dropdown.ClearOptions();

        using (UnityWebRequest webRequest = UnityWebRequest.Get("http://localhost/MiseriorPulse/GetOrganizations.php"))
        {
            // Request and wait for the desired page
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
                    string jsonResponse = webRequest.downloadHandler.text;

                    try
                    {
                        // Parse the JSON array directly using Newtonsoft.Json
                        List<Organization> organizations = JsonConvert.DeserializeObject<List<Organization>>(jsonResponse);

                        // Create a list of strings to add to the dropdown
                        List<string> options = new List<string>();

                        // Ensure the organizations array is not null
                        if (organizations != null)
                        {
                            foreach (var org in organizations)
                            {
                                options.Add(org.name_abbreviation);
                            }

                            // Add the options to the dropdown
                            dropdown.AddOptions(options);
                        }
                        else
                        {
                            Debug.Log("No organizations found in the response.");
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("Error parsing JSON: " + e.Message);
                    }
                    break;
            }
        }
    }


    private IEnumerator CheckOrganizations(string OrgName)
    {
        WWWForm form = new WWWForm();
        form.AddField("OrgName", OrgName);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/MiseriorPulse/CheckOrganization.php", form))
        {
            Debug.Log("Sending UnityWebRequest...");
            yield return www.SendWebRequest();

            Debug.Log("UnityWebRequest completed. Checking result...");
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Request Failed.");
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                OrganizationId = www.downloadHandler.text;
            }
        }
    }

    // Update Organization class and container for the array
    [System.Serializable]
    public class Organization
    {
        public string name_abbreviation;
    }

    // Wrapper class to match the array response
    [System.Serializable]
    public class OrganizationArray
    {
        public Organization[] organizations;
    }
}
