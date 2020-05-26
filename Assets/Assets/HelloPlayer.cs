using UnityEngine;
using UnityEngine.UI;

public class HelloPlayer : MonoBehaviour
{
   public Text text;

   public void Start()
   {
      text.text = "Hello " + PlayerPrefs.GetString("login") + "!";
   }
}
