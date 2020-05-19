using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState
{
    START,
    PLAYERTURN,
    ENEMYTURN,
    ANIMATION,
    WON,
    LOST
}

public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    public Unit playerUnit;
    public Unit enemyUnit;

    public Text dialogueText;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public BattleState state;

    public AnimationPerformer animationPerformer;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerGO.SetActive(false);
        playerUnit = playerGO.GetComponent<Unit>();
        playerUnit.transform.parent = null;

        SpriteRenderer spriteRenderer = playerGO.GetComponent<SpriteRenderer>();

        if (File.Exists("texture.png"))
        {
            Texture2D savedPlayerTexture = new Texture2D(16, 16, TextureFormat.RGBA32, false);

            ImageConversion.LoadImage(savedPlayerTexture, File.ReadAllBytes("texture.png"), false);
            
            savedPlayerTexture.filterMode = FilterMode.Point;
            savedPlayerTexture.Apply();
            
            // pivot is percentage 0.5f 0.5f is center
            Sprite sprite = Sprite.Create(savedPlayerTexture, new Rect(0, 0, savedPlayerTexture.width, savedPlayerTexture.height), new Vector2(0.5f, 0.5f));
            spriteRenderer.sprite = sprite;
        }
        
        playerGO.transform.localScale = new Vector3(16, 16); // totally calculated value
        playerGO.SetActive(true);

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();
        enemyUnit.transform.parent = null;

        dialogueText.text = "A wild " + enemyUnit.unitName + " approaches...";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void PlayerAttack()
    {
        state = BattleState.ANIMATION;
        
        animationPerformer.Attack(
            playerUnit,
            enemyUnit,
            () => { 
                enemyUnit.TakeDamage(playerUnit.damage);
                enemyHUD.SetHP(enemyUnit.currentHP);
                dialogueText.text = "The attack is successful!"; },
            () =>
            {
                if (enemyUnit.isDead())
                {
                    state = BattleState.WON;
                    EndBattle();
                }
                else
                {
                    state = BattleState.ENEMYTURN;
                    StartCoroutine(EnemyTurn());
                }
            }
        );
    }
    
    void EnemyAttack()
    {
        state = BattleState.ANIMATION;
        dialogueText.text = enemyUnit.unitName + " attacks!";
        
        animationPerformer.Attack(
            enemyUnit,
            playerUnit,
            () => { 
                playerUnit.TakeDamage(playerUnit.damage);
                playerHUD.SetHP(playerUnit.currentHP);
                dialogueText.text = "The attack is successful!"; },
            () =>
            {
                if (playerUnit.isDead())
                {
                    state = BattleState.LOST;
                    EndBattle();
                }
                else
                {
                    state = BattleState.PLAYERTURN;
                    PlayerTurn();
                }
            }
        );
    }

    IEnumerator EnemyTurn()
    {
        dialogueText.text = enemyUnit.unitName + " 's turn!";

        yield return new WaitForSeconds(1f);
        
        EnemyAttack();
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueText.text = "You won the battle!";
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You were defeated.";
        }
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
    }

    IEnumerator PlayerHeal()
    {
        state = BattleState.ENEMYTURN;
        playerUnit.Heal(5);

        playerHUD.SetHP(playerUnit.currentHP);
        dialogueText.text = "You feel renewed strength!";

        yield return new WaitForSeconds(2f);

        StartCoroutine(EnemyTurn());
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        PlayerAttack();
    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());
    }
}