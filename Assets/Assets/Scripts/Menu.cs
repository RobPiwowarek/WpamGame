using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }

    public void Battle()
    {
        SceneManager.LoadScene(2);
    }

    public void Create()
    {
        SceneManager.LoadScene(3);
    }
    
    public void Customize()
    {
        StartCoroutine(fetchDataAndLoad());
    }

    private IEnumerator fetchDataAndLoad()
    {
        
        StartCoroutine(BackendClient.loadTexture(PlayerPrefs.GetString("login"), PlayerPrefs.GetString("password"), Consts.loadTextureAddress));
        StartCoroutine(BackendClient.loadStats(PlayerPrefs.GetString("login"), PlayerPrefs.GetString("password"), Consts.loadStatsAddress));

        yield return new WaitForSeconds(1);
        
        SceneManager.LoadScene(4);
    }
}
