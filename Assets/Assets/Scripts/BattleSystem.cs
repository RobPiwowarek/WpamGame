using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private int lightAttackCost = 1;
    private int heavyAttackCost = 2;
    private int defendCost = 1;

    private float heavyAttackMultiplier = 1.5f;

    public AudioSource lightAttack;
    public AudioSource heavyAttack;
    public AudioSource heal;
    public AudioSource rest;
    public AudioSource defend;
    public AudioSource denied;
    public AudioSource victory;
    public AudioSource defeat;

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

        var texture = PlayerPrefs.GetString("texture");

        if (texture == null)
        {
            Debug.Log("Couldnt load texture or texture not defined");
        }
        else
        {
            Texture2D savedPlayerTexture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            ImageConversion.LoadImage(savedPlayerTexture, Convert.FromBase64String(texture), false);
            savedPlayerTexture.filterMode = FilterMode.Point;
            savedPlayerTexture.Apply();
            // pivot is percentage 0.5f 0.5f is center
            Sprite sprite = Sprite.Create(savedPlayerTexture, new Rect(0, 0, savedPlayerTexture.width, savedPlayerTexture.height), new Vector2(0.5f, 0.5f));
            spriteRenderer.sprite = sprite;
            // totally calculated value
            playerGO.transform.localScale = new Vector3(12, 12); 
        }
        
        playerUnit.damage = PlayerPrefs.GetInt("damage");
        playerUnit.healingPower = PlayerPrefs.GetInt("healing");
        playerUnit.maxHP = PlayerPrefs.GetInt("hp");
        playerUnit.currentHP = PlayerPrefs.GetInt("hp");
        playerUnit.armor = PlayerPrefs.GetInt("armor");
        playerUnit.currentArmor = PlayerPrefs.GetInt("armor");
        playerUnit.criticalChance = PlayerPrefs.GetInt("crit");
        playerUnit.evasionChance = PlayerPrefs.GetInt("evasion");
        playerUnit.maxEnergy = PlayerPrefs.GetInt("energy");
        playerUnit.currentEnergy = PlayerPrefs.GetInt("energy");
        playerUnit.stunChance = PlayerPrefs.GetInt("stun");
        playerUnit.unitName = PlayerPrefs.GetString("name");
        playerUnit.unitLevel = PlayerPrefs.GetInt("level");

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

    void PlayerLightAttack()
    {
        state = BattleState.ANIMATION;

        lightAttack.PlayDelayed(0.75f);
        
        animationPerformer.LightAttack(
            playerUnit,
            enemyUnit,
            () =>
            {
                if (enemyUnit.WillEvade())
                {
                    dialogueText.text = "Dodged!";
                }
                else
                {
                    if (playerUnit.WillCrit())
                    {
                        dialogueText.text = "A critical hit!!!";
                        enemyUnit.TakeDamage(playerUnit.damage * 2);
                    }
                    else
                    {
                        dialogueText.text = "The quick attack is successful!";
                        enemyUnit.TakeDamage(playerUnit.damage);
                    }

                    enemyHUD.SetHP(enemyUnit.currentHP);
                }
                
            },
            () =>
            {
                enemyUnit.GuardDown();

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
    
    void PlayerHeavyAttack()
    {
        state = BattleState.ANIMATION;
        
        heavyAttack.PlayDelayed(0.75f);

        animationPerformer.HeavyAttack(
            playerUnit,
            enemyUnit,
            () =>
            {
                var willCrit = playerUnit.WillCrit();
                var willStun = playerUnit.WillStun();
                var willEvade = enemyUnit.WillEvade();

                if (willEvade)
                {
                    dialogueText.text = "Dodged!";
                }
                else
                {
                    enemyUnit.TakeDamage(heavyAttackDamage());

                    if (willCrit)
                    {
                        dialogueText.text = "A critical hit!!!";
                        enemyUnit.TakeDamage(heavyAttackDamage());
                    }
                    else if (willStun)
                    {
                        dialogueText.text = "A stunning blow!!!";
                        enemyUnit.stunned = true;
                    }
                    else if (willCrit && willStun)
                    {
                        dialogueText.text = "A devastating stun and crit!!!";
                        enemyUnit.stunned = true;
                        enemyUnit.TakeDamage(heavyAttackDamage());

                    }
                    else
                        dialogueText.text = "The heavy attack is successful!";
                    
                    enemyHUD.SetHP(enemyUnit.currentHP);
                }
            },
            () =>
            {
                enemyUnit.GuardDown();

                if (enemyUnit.isDead())
                {
                    state = BattleState.WON;
                    EndBattle();
                }
                else if (enemyUnit.stunned)
                {
                    enemyUnit.stunned = false;
                    state = BattleState.PLAYERTURN;
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
        
        animationPerformer.LightAttack(
            enemyUnit,
            playerUnit,
            () => { 
                playerUnit.TakeDamage(enemyUnit.damage);
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
                    playerUnit.GuardDown();
                    PlayerTurn();
                }
            }
        );
    }

    IEnumerator EnemyTurn()
    {
        playerUnit.GetComponent<SpriteRenderer>().sortingOrder = 10;
        enemyUnit.GetComponent<SpriteRenderer>().sortingOrder = 11;
        
        dialogueText.text = enemyUnit.unitName + " 's turn!";

        yield return new WaitForSeconds(1f);
        
        EnemyAttack();
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueText.text = "You won the battle! 5 points granted!";
            GetComponent<AudioSource>().Stop();
            victory.Play();
            var points = PlayerPrefs.GetInt("points") + 5;
            PlayerPrefs.SetInt("points", points);
            
            StartCoroutine(BackendClient.saveStats(PlayerPrefs.GetString("login"), PlayerPrefs.GetString("password"),
                Consts.saveStatsAddress,
                PlayerPrefs.GetString("name"), PlayerPrefs.GetInt("level"), PlayerPrefs.GetInt("damage"), PlayerPrefs.GetInt("healing"), PlayerPrefs.GetInt("armor"), PlayerPrefs.GetInt("energy"), PlayerPrefs.GetInt("hp"), PlayerPrefs.GetInt("crit"), PlayerPrefs.GetInt("stun"), PlayerPrefs.GetInt("evasion"), points));
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You were defeated.";
            defeat.Play();
        }
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
        playerUnit.GetComponent<SpriteRenderer>().sortingOrder = 11;
        enemyUnit.GetComponent<SpriteRenderer>().sortingOrder = 10;
    }

    IEnumerator PlayerHeal()
    {
        state = BattleState.ANIMATION;
        
        heal.Play();
        playerUnit.Heal();
        playerHUD.SetHP(playerUnit.currentHP);
        
        state = BattleState.ENEMYTURN;

        dialogueText.text = "You feel renewed strength!";

        yield return new WaitForSeconds(2f);

        StartCoroutine(EnemyTurn());
    }
    
    IEnumerator PlayerRest()
    {
        state = BattleState.ANIMATION;
        
        rest.Play();
        playerUnit.setEnergy(playerUnit.maxEnergy);
        playerHUD.SetEnergy(playerUnit.currentEnergy);

        state = BattleState.ENEMYTURN;

        dialogueText.text = "You energy has returned to you!";

        yield return new WaitForSeconds(2f);

        StartCoroutine(EnemyTurn());
    }
    
    IEnumerator PlayerGuard()
    {
        state = BattleState.ANIMATION;

        defend.Play();
        playerUnit.GuardUp();

        state = BattleState.ENEMYTURN;

        dialogueText.text = "You are preparing for the incoming attack!";

        yield return new WaitForSeconds(2f);

        StartCoroutine(EnemyTurn());
    }

    public void OnLightAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        if (playerUnit.currentEnergy < lightAttackCost)
        {
            denied.Play();
            dialogueText.text = "Not enough energy!";
            return;
        }

        playerUnit.currentEnergy -= lightAttackCost;
        playerHUD.SetEnergy(playerUnit.currentEnergy);

        PlayerLightAttack();
    }
    
    public void OnHeavyAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        if (playerUnit.currentEnergy < heavyAttackCost)
        {
            denied.Play();
            dialogueText.text = "Not enough energy!";
            return;
        }

        playerUnit.currentEnergy -= heavyAttackCost;
        playerHUD.SetEnergy(playerUnit.currentEnergy);

        PlayerHeavyAttack();
    }
    
    public void OnDefendButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        if (playerUnit.currentEnergy < defendCost)
        {
            denied.Play();
            dialogueText.text = "Not enough energy!";
            return;
        }

        playerUnit.currentEnergy -= defendCost;
        playerHUD.SetEnergy(playerUnit.currentEnergy);

        StartCoroutine(PlayerGuard());
    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());
    }
    
    public void OnRestButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerRest());
    }

    public void OnRunButton()
    {
        SceneManager.LoadScene(1);
    }

    private int heavyAttackDamage()
    {
        return (int) (playerUnit.damage * heavyAttackMultiplier);
    }
}