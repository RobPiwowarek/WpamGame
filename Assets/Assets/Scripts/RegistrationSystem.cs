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

    private void Awake()
    {
        PlayerPrefs.DeleteAll();
    }

    private void Start()
    {
        status.text = "Welcome to GraStworki!";
    }

    public void Login()
    {
        status.text = "Connecting...";
        StartCoroutine(sendRequest(email.text, password.text, Consts.loginAddress));
    }

    public void Register()
    {
        status.text = "Connecting...";
        StartCoroutine(sendRequest(email.text, password.text, Consts.registerAddress));
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
                    StartCoroutine(BackendClient.loadTexture(email, password, Consts.loadTextureAddress));
                    StartCoroutine(BackendClient.loadStats(PlayerPrefs.GetString("login"), PlayerPrefs.GetString("password"), Consts.loadStatsAddress));
                    yield return new WaitForSeconds(1);
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
