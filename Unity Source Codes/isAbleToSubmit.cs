using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class isAbleToSubmit : MonoBehaviour
{
    public TMP_InputField EventNameInputField;
    public TMP_InputField ID1;
    public TMP_InputField ID2;
    public TMP_InputField ID3;
    public TextMeshProUGUI feedback;

    private void Start()
    {
        ID1.onValueChanged.AddListener(delegate { ValidateNumericInput(ID1); });
        ID2.onValueChanged.AddListener(delegate { ValidateNumericInput(ID2); });
        ID3.onValueChanged.AddListener(delegate { ValidateNumericInput(ID3); });

        CheckEventNameInputField();
    }

    public void CheckEventNameInputField()
    {
        if (EventNameInputField.text == "")
        {
            ID1.interactable = false;
            ID2.interactable = false;
            ID3.interactable = false;
            ClearInputFields();
        } else
        {
            ID1.interactable = true;
            ID2.interactable = true;
            ID3.interactable = true;
            feedback.text = "";
        }
    }

    public void ClearInputFields()
    {
        ID1.text = "";
        ID2.text = "";
        ID3.text = "";
    }

    public void LimitID1()
    {
        EnforceCharacterLimit(ID1, 4, ID2);
    }

    public void LimitID2()
    {
        EnforceCharacterLimit(ID2, 4, ID3);
    }

    public void LimitID3()
    {
        EnforceCharacterLimit(ID3, 1);
    }

    private void EnforceCharacterLimit(TMP_InputField inputField, int maxLength, TMP_InputField shiftSelect)
    {
        if (inputField.text.Length >= maxLength)
        {
            // Truncate the text to the allowed maximum length
            inputField.text = inputField.text.Substring(0, maxLength);
            shiftSelect.Select();
        } else if (inputField.text.Length == 1)
        {
            feedback.text = "";
        }
    }

    private void EnforceCharacterLimit(TMP_InputField inputField, int maxLength)
    {
        if (inputField.text.Length >= maxLength)
        {
            // Truncate the text to the allowed maximum length
            inputField.text = inputField.text.Substring(0, maxLength);
        }
    }

    private void ValidateNumericInput(TMP_InputField inputField)
    {
        string newText = "";
        foreach (char c in inputField.text)
        {
            if (char.IsDigit(c))
            {
                newText += c;
            }
        }

        if (inputField.text != newText)
        {
            inputField.text = newText;
        }
    }
}
