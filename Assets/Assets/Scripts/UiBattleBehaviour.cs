using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiBattleBehaviour : MonoBehaviour
{
    [SerializeField] private BattleSystem battleSystem;

    [SerializeField] private List<Button> buttons;

    // Update is called once per frame
    void Update()
    {
        if (battleSystem.state == BattleState.PLAYERTURN)
        {
            buttons.ForEach(button => { button.interactable = true; });
        }
        else
        {
            buttons.ForEach(button => { button.interactable = false; });
        }
    }
}