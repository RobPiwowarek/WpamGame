﻿using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiCreator : MonoBehaviour
{
    private Color nextColor = Color.white;
    private Boolean isMouseButtonPressed = false;
    private GameObject[,] grid = new GameObject[16,16];
    public ColorPicker picker;
    public GameObject panel;

    private Texture2D texture;
    
    private void Awake()
    {
        CreateUi();
    }

    void Start ()
    {
        picker.CurrentColor = Color.white;
        picker.onValueChanged.AddListener(color => { nextColor = color; });
    }

    public void Quit()
    {
        SceneManager.LoadScene(1);
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            isMouseButtonPressed = true;
        if (Input.GetMouseButtonUp(0))
            isMouseButtonPressed = false;
    }

    public void Clear()
    {
        picker.CurrentColor = Color.white;
        nextColor = Color.white;
        foreach (var button in grid)
        {
            button.GetComponent<Image>().color = nextColor;
        }
    }

    public void Save()
    {
        var a = new Texture2D(16, 16, TextureFormat.RGBA32,false);
        a.filterMode = FilterMode.Point;
        
        for (var x = 0; x < 16; x++)
        {
            for (var y = 0; y < 16; y++)
                a.SetPixel(y, 15-x, grid[y, x].GetComponent<Image>().color);
        }

        texture = a;
        texture.Apply();
        // todo: send to backend

        var bytes = ImageConversion.EncodeToPNG(texture);
        File.WriteAllBytes("texture.png", bytes);
    }
    
    private void CreateUi()
    {
        var button = new GameObject("Button");
        button.gameObject.AddComponent<RectTransform>();
        button.gameObject.AddComponent<CanvasRenderer>();
        button.AddComponent<Image>();
        button.gameObject.AddComponent<Button>();
        button.AddComponent<EventTrigger>();

        for (var x = 0; x < 16; x++)
        {
            for (var y = 0; y < 16; y++)
            {
                var newButton = Instantiate(button);
                newButton.GetComponent<RectTransform>().SetParent(panel.transform);
                newButton.GetComponent<Button>().onClick.AddListener(() => newButton.GetComponent<Image>().color = nextColor);
                var trigger = newButton.GetComponent<EventTrigger>();
                var entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener( (eventData) =>
                {
                    if (isMouseButtonPressed)
                        newButton.GetComponent<Image>().color = nextColor;
                });
                trigger.triggers.Add(entry);

                grid[y, x] = newButton;
            }
        }
        Destroy(button);
    }

}