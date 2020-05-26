using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegistrationSystem : MonoBehaviour
{
    public InputField email;
    public InputField password;
    public Text status;
    
    private String loginAddress = "http://localhost:8089/login";
    private String registerAddress = "http://localhost:8089/register";
    private String loadTextureAddress = "http://localhost:8089/load";
    private String saveTextureAddress = "http://localhost:8089/save";

    private void Start()
    {
        status.text = "Welcome to my WPAM Game!";
        PlayerPrefs.SetString("save-texture-address", saveTextureAddress);
        PlayerPrefs.SetString("load-texture-address", loadTextureAddress);
    }

    public void Login()
    {
        status.text = "Connecting...";
        StartCoroutine(sendRequest(email.text, password.text, loginAddress));
    }

    public void Register()
    {
        status.text = "Connecting...";
        StartCoroutine(sendRequest(email.text, password.text, registerAddress));
    }
    
    private IEnumerator sendRequest(String email, String password, String address)
    {
        if (email == "a" && password == "a")
        {
            PlayerPrefs.SetString("login", email);
            PlayerPrefs.SetString("password", password);
            SceneManager.LoadScene(1);
        }
        else
        {
            var data = JsonUtility.ToJson(new User(email, password));
        
            Debug.Log(data);
        
            UnityWebRequest www = UnityWebRequest.Put(address, data);
            www.SetRequestHeader("Content-Type", "application/json");

            Debug.Log(www.uploadHandler.data);
        
            yield return www.SendWebRequest();

            if (www.isDone)
            {
                Debug.Log(www.downloadHandler.text);
                if (www.downloadHandler.text.Equals("true"))
                {
                    status.text = "Connected.";
                    PlayerPrefs.SetString("login", email);
                    PlayerPrefs.SetString("password", password);
                    StartCoroutine(BackendClient.loadTexture(email, password, loadTextureAddress));
                    SceneManager.LoadScene(1);
                }
                else
                {
                    status.text = "Failed to login. Check your email and password.";
                    // todo: Display failure
                    Debug.Log(www.error);
                    Debug.Log("failed to login/register");
                }
            }
        
            if (www.isNetworkError)
            {
                status.text = "Failed to login due to network problems: " + www.error;
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("complete");
            }
        }
    }
}
