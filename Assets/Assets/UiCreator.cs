using UnityEngine;
using UnityEngine.UI;

public class UiCreator : MonoBehaviour
{
    private Color nextColor = Color.white;
    public ColorPicker picker;
    public GameObject panel;

    private void Awake()
    {
        CreateUi();
    }

    void Start ()
    {
        picker.onValueChanged.AddListener(color => { nextColor = color; });
    }
    
    private void CreateUi()
    {
        var button = new GameObject("Button");
        button.gameObject.AddComponent<RectTransform>();
        button.gameObject.AddComponent<CanvasRenderer>();
        button.AddComponent<Image>();
        button.gameObject.AddComponent<Button>();
        //button.transform.localScale = new Vector3(0.1f, 0.1f);
        
        for (var x = 0; x < 16; x++)
        {
            for (var y = 0; y < 16; y++)
            {
                var newButton = Instantiate(button);
                newButton.GetComponent<RectTransform>().SetParent(panel.transform);
                newButton.GetComponent<Button>().onClick.AddListener(() => newButton.GetComponent<Image>().color = nextColor);
            }
        }
    }
    
    // private static Color nextColor(Color current)
    // {
    //     if (current == Color.white) return Color.black;
    //     if (current == Color.black) return Color.red;
    //     if (current == Color.red) return Color.green;
    //     if (current == Color.green) return Color.blue;
    //     if (current == Color.blue) return Color.white;
    //     return Color.white;
    // }
}