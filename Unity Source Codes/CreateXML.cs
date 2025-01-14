using System.Xml;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using UnityEngine;

public class XMLCreator : MonoBehaviour
{
    private static readonly string encryptionKey = "MySecureKey12345";
    public SceneChanger sceneChanger;

    public void CreateEncryptedXML(string userId, string userName, string email, string organization, string position)
    {
        // Define the directory and file paths
        string directoryPath = Path.Combine(Application.persistentDataPath, "saves");
        string filePath = Path.Combine(directoryPath, "UserInformation.xml");

        // Create the directory if it does not exist
        if (!Directory.Exists(directoryPath))
        {
            try
            {
                Directory.CreateDirectory(directoryPath);
                Debug.Log("Directory created at: " + directoryPath);
            }
            catch (IOException ex)
            {
                Debug.LogError("Failed to create directory: " + ex.Message);
                return;
            }
        }

        // Generate XML string and encrypt it
        string xmlContent = GenerateXMLString(userId, userName, email, organization, position);
        string encryptedContent = Encrypt(xmlContent, encryptionKey);

        // Write encrypted content to the file
        try
        {
            File.WriteAllText(filePath, encryptedContent);
            Debug.Log("Encrypted XML file created at: " + filePath);
            sceneChanger = FindObjectOfType<SceneChanger>();
            sceneChanger.Execute(2);
        }
        catch (IOException ex)
        {
            Debug.LogError("IO error while saving encrypted XML file: " + ex.Message);
        }
    }

    private string GenerateXMLString(string userId, string userName, string email, string organization, string position)
    {
        using (var stringWriter = new StringWriter())
        {
            using (var writer = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("User");
                writer.WriteElementString("userId", userId);
                writer.WriteElementString("username", userName);
                writer.WriteElementString("email", email);
                writer.WriteElementString("organization", organization);
                writer.WriteElementString("position", position);
                writer.WriteEndElement();

                writer.WriteEndDocument();
            }

            return stringWriter.ToString();
        }
    }

    private string Encrypt(string plainText, string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] ivBytes = new byte[16];
        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = ivBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (var writer = new StreamWriter(cryptoStream))
                    {
                        writer.Write(plainText);
                    }
                }
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
    }

    public string Decrypt(string encryptedText, string key)
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
