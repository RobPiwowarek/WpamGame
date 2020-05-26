using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class BackendClient : MonoBehaviour
{
    public static IEnumerator saveTexture(String email, String password, String address, String base64Texture)
    {
        var data = JsonUtility.ToJson(new SaveRequest(email, password, base64Texture));

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
                PlayerPrefs.SetString("texture", base64Texture);
                Debug.Log("Successfully saved your creation");
            }
            else
            {
                Debug.Log(www.error);
                Debug.Log("Failed to save your creation");
            }
        }

        if (www.isNetworkError)
            Debug.Log(www.error);
        else
            Debug.Log("complete");
    }
    
    public static IEnumerator loadTexture(String email, String password, String address)
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
            var response = www.downloadHandler.text;
            
            if (response != null && response != "FAILURE" && response != "UNAUTHORISED")
            {
                PlayerPrefs.SetString("texture", response);
                Debug.Log("Successfully loaded your creation");
            }
            else
            {
                Debug.Log(www.error);
                Debug.Log("Failed to load your texture");
            }
        }

        if (www.isNetworkError)
            Debug.Log(www.error);
        else
            Debug.Log("complete");
    }


    class FetchedTexture
    {
        public String texture;

        public FetchedTexture(string texture)
        {
            this.texture = texture;
        }
    }

    class SaveRequest
    {
        public String email;
        public String password;
        public String texture;

        public SaveRequest(string email, string password, string texture)
        {
            this.email = email;
            this.password = password;
            this.texture = texture;
        }
    }
}