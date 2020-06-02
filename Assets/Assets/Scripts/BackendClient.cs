using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class BackendClient : MonoBehaviour
{
    public static IEnumerator saveTexture(String email, String password, String address, String base64Texture)
    {
        var data = JsonUtility.ToJson(new SaveRequest(email, password, base64Texture));

        Debug.Log(data);
        Debug.Log("ADDRESS: " + address);

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
        Debug.Log("ADDRESS: " + address);
        
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
    
    public static IEnumerator saveStats(String email, String password, String address, 
        string name, int level, int damage, int healing, int armor, int energy, int health, int crit, int stun, int evade, int points)
    {
        var data = JsonUtility.ToJson(new StatsSaveRequest(email, password, name, level, damage, healing, armor, energy, health, crit, stun, evade, points));

        Debug.Log(data);
        Debug.Log("ADDRESS: " + address);

        UnityWebRequest www = UnityWebRequest.Put(address, data);
        www.SetRequestHeader("Content-Type", "application/json");

        Debug.Log(www.uploadHandler.data);

        yield return www.SendWebRequest();

        if (www.isDone)
        {
            Debug.Log(www.downloadHandler.text);
            if (www.downloadHandler.text.Equals("true"))
            {
                PlayerPrefs.SetInt("hp", health);
                PlayerPrefs.SetInt("energy", energy);
                PlayerPrefs.SetString("name", name);
                PlayerPrefs.SetInt("armor", armor);
                PlayerPrefs.SetInt("damage", damage);
                PlayerPrefs.SetInt("healing", healing);
                PlayerPrefs.SetInt("crit", crit);
                PlayerPrefs.SetInt("stun", stun);
                PlayerPrefs.SetInt("evasion", evade);
                PlayerPrefs.SetInt("level", level);
                PlayerPrefs.SetInt("points", points);
                Debug.Log("Successfully saved your stats");
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

    public static IEnumerator loadStats(String email, String password, String address)
    {
        var data = JsonUtility.ToJson(new User(email, password));

        Debug.Log(data);
        Debug.Log("ADDRESS: " + address);

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
                FetchedStats stats = JsonUtility.FromJson<FetchedStats>(www.downloadHandler.text);

                PlayerPrefs.SetInt("hp", stats.health);
                PlayerPrefs.SetInt("energy", stats.energy);
                PlayerPrefs.SetString("name", stats.name);
                PlayerPrefs.SetInt("armor", stats.armor);
                PlayerPrefs.SetInt("damage", stats.damage);
                PlayerPrefs.SetInt("healing", stats.healing);
                PlayerPrefs.SetInt("crit", stats.crit);
                PlayerPrefs.SetInt("stun", stats.stun);
                PlayerPrefs.SetInt("evasion", stats.evade);
                PlayerPrefs.SetInt("level", stats.level);
                PlayerPrefs.SetInt("points", stats.points);
                Debug.Log("Successfully loaded your stats");
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

    class FetchedStats
    {
        public String name;
        public int level;
        public int damage;
        public int healing;
        public int armor;
        public int energy;
        public int health;
        public int crit;
        public int stun;
        public int evade;
        public int points;

        public FetchedStats(string name, int level, int damage, int healing, int armor, int energy, int health, int crit, int stun, int evade, int points)
        {
            this.name = name;
            this.level = level;
            this.damage = damage;
            this.healing = healing;
            this.armor = armor;
            this.energy = energy;
            this.health = health;
            this.crit = crit;
            this.stun = stun;
            this.evade = evade;
            this.points = points;
        }
    }

    class StatsSaveRequest
    {
        public String email;
        public String password;
        public String name;
        public int level;
        public int damage;
        public int healing;
        public int armor;
        public int energy;
        public int health;
        public int crit;
        public int stun;
        public int evade;
        public int points;

        public StatsSaveRequest(string email, string password, string name, int level, int damage, int healing, int armor, int energy, int health, int crit, int stun, int evade, int points)
        {
            this.email = email;
            this.password = password;
            this.name = name;
            this.level = level;
            this.damage = damage;
            this.healing = healing;
            this.armor = armor;
            this.energy = energy;
            this.health = health;
            this.crit = crit;
            this.stun = stun;
            this.evade = evade;
            this.points = points;
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