using UnityEngine;

public class ButtonSounds : MonoBehaviour
{
    public AudioSource mouseover;
    public AudioSource click;
    
    private void OnMouseEnter()
    {
        mouseover.Play();
    }

    private void OnMouseDown()
    {
        click.Play();
    }
}
