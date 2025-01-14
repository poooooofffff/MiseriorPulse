using System.IO;
using System.Xml;
using System.Security.Cryptography;
using System.Text;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GetUserInformation : MonoBehaviour
{
    public TextMeshProUGUI userName;
    public TextMeshProUGUI email;
    public TextMeshProUGUI organization;
    public TextMeshProUGUI position;

    public string xmlFilePath = "UserInformation.xml"; // Path to your XML file
    private static readonly string encryptionKey = "MySecureKey12345";

    // Start is called before the first frame update
    void Start()
    {
        // Parse and decrypt XML, then display in the UI
        LoadAndDisplayUserInformation();
    }

    void LoadAndDisplayUserInformation()
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "saves");
        string fullPath = Path.Combine(directoryPath, xmlFilePath);

        if (File.Exists(fullPath))
        {
            try
            {
                // Read the encrypted XML file
                string encryptedContent = File.ReadAllText(fullPath);

                // Decrypt the content
                string decryptedContent = Decrypt(encryptedContent, encryptionKey);

                // Load the decrypted XML
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(decryptedContent);

                // Parse the XML
                XmlNode root = xmlDocument.DocumentElement;
                if (root != null)
                {
                    string decryptedUserName = root.SelectSingleNode("username")?.InnerText;
                    string decryptedEmail = root.SelectSingleNode("email")?.InnerText;
                    string decryptedOrganization = root.SelectSingleNode("organization")?.InnerText;
                    string decryptedPosition = root.SelectSingleNode("position")?.InnerText;

                    // Update TextMeshPro fields
                    userName.text = decryptedUserName ?? "N/A";
                    email.text = decryptedEmail ?? "N/A";
                    organization.text = decryptedOrganization ?? "N/A";
                    position.text = decryptedPosition ?? "N/A";

                    Debug.Log(decryptedUserName + ", " + decryptedEmail + ", " + decryptedOrganization + ", " + decryptedPosition);
                    Debug.Log("User information loaded successfully.");

                    if(SceneManager.GetActiveScene().name == "Main")
                    {
                        StartCoroutine(Main.Instance.WebQueries.GetAttendanceData(organization.text));
                    }
                }
                else
                {
                    Debug.LogError("XML root element is null.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to parse XML file: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"XML file not found at path: {fullPath}");
        }
    }

    string Decrypt(string encryptedText, string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] ivBytes = new byte[16];
        byte[] cipherBytes = Convert.FromBase64String(encryptedText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = ivBytes;

            using (var memoryStream = new MemoryStream(cipherBytes))
            {
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (var reader = new StreamReader(cryptoStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}
