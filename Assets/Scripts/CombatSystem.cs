using System.Collections;
using TMPro;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST, BUSY }

public class CombatSystem : MonoBehaviour
{
    [SerializeField] public BattleState state;
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab; // Made SerializeField to assign in Inspector

    private EmotionStats playerStats;
    private EmotionStats enemyStats;

    
    public Text dialogueText;

    public BattleHUD playerHUD;
    

    void Start()
    {
        //enemyPrefab = GameObject.FindWithTag("Enemy");
        

        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        Debug.Log("Battle starts!");
        playerStats = playerPrefab.GetComponent<EmotionStats>();
        enemyStats = enemyPrefab.GetComponent<EmotionStats>();

        dialogueText.text = "A wild " + enemyStats.Name + " approaches...";
        playerHUD.SetHUD(playerStats);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        // Prevent double-actions during animation window
        bool isDead = enemyStats.TakeDamage();

        //dialogueText.text = "The attack is successful!";
        Debug.Log("You attacked");

        yield return new WaitForSeconds(2f);

        if (isDead)
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

    IEnumerator EnemyTurn()
    {
        //dialogueText.text = enemyStats.Name + " attacks!";
        Debug.Log("You were attacked");

        yield return new WaitForSeconds(1f);

        bool isDead = playerStats.TakeDamage();
        playerHUD.SetHP(playerStats.health);

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            Debug.Log("You lose!");
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            Debug.Log("You win!");
            //dialogueText.text = "You won the battle!";
            SceneManager.LoadScene("Test_Room");
            Destroy(enemyPrefab);
        }
        else if (state == BattleState.LOST)
        {
            //dialogueText.text = "You were defeated.";
            Application.Quit(); // Note: This only works in a built game build, not the Unity editor
        }
    }

    void PlayerTurn()
    {
        //dialogueText.text = "Choose an action:";
    }

    IEnumerator PlayerHeal()
    {
        state = BattleState.BUSY; // Lock state
        playerStats.Heal(5);
        playerHUD.SetHP(playerStats.health);

        //dialogueText.text = "You feel renewed strength!";

        yield return new WaitForSeconds(2f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN) return;
        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN) return;
        StartCoroutine(PlayerHeal());
    }
}