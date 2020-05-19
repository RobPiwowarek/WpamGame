using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegistrationSystem : MonoBehaviour
{
    public InputField email;
    public InputField password;
    
    private String loginAddress = "http://localhost:8089/login";
    private String registerAddress = "http://localhost:8089/register";

    public void Login()
    {
        StartCoroutine(sendRequest(email.text, password.text, loginAddress));
    }

    public void Register()
    {
        StartCoroutine(sendRequest(email.text, password.text, registerAddress));
    }
    
    private IEnumerator sendRequest(String email, String password, String address)
    {
        WWWForm form = new WWWForm();

        var data = JsonUtility.ToJson(new User(email, password));
        
        Debug.Log(data);
        
        UnityWebRequest www = UnityWebRequest.Put(address, data);
        www.SetRequestHeader("Content-Type", "application/json");

        Debug.Log(www.uploadHandler.data);
        Debug.Log(Convert.ToBase64String(www.uploadHandler.data));
        
        yield return www.SendWebRequest();

        if (www.isDone)
        {
            Debug.Log(www.downloadHandler.text);
            if (www.downloadHandler.text.Equals("true"))
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                // todo: Display failure
                Debug.Log(www.error);
                Debug.Log("failed to login/register");
            }
        }
        
        if (www.isError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("complete");
        }
    }

    class User
    {
        public String email;
        public String password;

        public User(string email, string password)
        {
            this.email = email;
            this.password = password;
        }
    }
}
