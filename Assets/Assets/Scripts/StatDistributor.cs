using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class StatDistributor : MonoBehaviour
{
	public InputField nameControl;
	public Text damageControl;
	public Text healingControl;
	public Text armorControl;
	public Text energyControl;
	public Text critControl;
	public Text stunControl;
	public Text evasionControl;
	public Text healthControl;
	public Text pointsControl;
	public Image creation;

	public AudioSource denied;

	private int points;

	private void Awake()
	{
		//StartCoroutine(BackendClient.loadTexture(PlayerPrefs.GetString("login"), PlayerPrefs.GetString("password"), PlayerPrefs.GetString("load-texture-address")));
		var texture = PlayerPrefs.GetString("texture");
		Texture2D savedPlayerTexture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
		ImageConversion.LoadImage(savedPlayerTexture, Convert.FromBase64String(texture), false);
		savedPlayerTexture.filterMode = FilterMode.Point;
		savedPlayerTexture.Apply();
		// pivot is percentage 0.5f 0.5f is center
		Sprite sprite = Sprite.Create(savedPlayerTexture, new Rect(0, 0, savedPlayerTexture.width, savedPlayerTexture.height), new Vector2(0.5f, 0.5f));
		creation.sprite = sprite;

		//StartCoroutine(BackendClient.loadStats(PlayerPrefs.GetString("login"), PlayerPrefs.GetString("password"), PlayerPrefs.GetString("load-stats-address")));

		points = PlayerPrefs.GetInt("points");

		pointsControl.text = "Points to spend: " + points;
		Debug.Log("Is awake happening?");
	}
	
	private void Start()
	{
		Debug.Log("Is start happening?");
		damageControl.text = PlayerPrefs.GetInt("damage").ToString();
		healingControl.text = PlayerPrefs.GetInt("healing").ToString();
		healthControl.text = PlayerPrefs.GetInt("hp").ToString();
		armorControl.text = PlayerPrefs.GetInt("armor").ToString();
		critControl.text = PlayerPrefs.GetInt("crit").ToString();
		energyControl.text = PlayerPrefs.GetInt("energy").ToString();
		evasionControl.text = PlayerPrefs.GetInt("evasion").ToString();
		stunControl.text = PlayerPrefs.GetInt("stun").ToString();
		nameControl.text = PlayerPrefs.GetString("name");
	}

	public void increase(Text control)
	{
		if (points == 0)
		{
			denied.Play();
		}
		else
		{
			control.text = (int.Parse(control.text) + 1).ToString();
			points--;
			pointsControl.text = "Points to spend: " + points;
		}
	}
	
	public void decrease(Text control)
	{
		if (int.Parse(control.text) == 0 || (control == healthControl && int.Parse(control.text) == 1))
		{
			denied.Play();
		}
		else
		{
			control.text = (int.Parse(control.text) - 1).ToString();
			points++;
			pointsControl.text = "Points to spend: " + points;
		}
	}

	public void Save()
	{
		var points = int.Parse(pointsControl.text.Substring("Points to spend: ".Length-1));
		var damage = int.Parse(damageControl.text);
		var healing = int.Parse(healingControl.text);
		var stun = int.Parse(stunControl.text);
		var crit = int.Parse(critControl.text);
		var evasion = int.Parse(evasionControl.text);
		var armor = int.Parse(armorControl.text);
		var energy = int.Parse(energyControl.text);
		var health = int.Parse(healthControl.text);
		var name = nameControl.text;

		StartCoroutine(BackendClient.saveStats(PlayerPrefs.GetString("login"), PlayerPrefs.GetString("password"),
			Consts.saveStatsAddress,
			name, PlayerPrefs.GetInt("level"), damage, healing, armor, energy, health, crit, stun, evasion, points));
	}

	public void Quit()
	{
		SceneManager.LoadScene(1);
	}

}
