using UnityEngine;
using UnityEngine.InputSystem;

public class BattleManager : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] private PlayerData player;
    [SerializeField] private EnemyData enemy;

    [Header("Manager")]
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private CardDisplayManager cardDisplayManager;
    [SerializeField] private CardEffectManager cardEffectManager;
    [SerializeField] private EnemyIntentDisplayManager enemyIntentDisplayManager;
    [SerializeField] private EnemyManager enemyManager;

    [Header("Battle State")]
    //test
    [SerializeField] private CardData selectedCard;
    //

    private BattleState currentState;

    /////Battle Flow/////

    private void Start()
    {
        StartBattle();
    }

    //test
    private void Update()
    {
        if (currentState != BattleState.PlayerTurn)
        {
            return;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("SPACE PRESSED");
            PlaySelectedCard();
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("E PRESSED");
            EndPlayerTurn();
        }
    }
    //

    private void StartBattle()
    {
        currentState = BattleState.Start;

        Debug.Log("Battle Started");

        deckManager.StartBattleSetup();

        cardDisplayManager.RefreshHand();

        PrepareEnemyIntent();

        StartPlayerTurn();
    }

    /////Player/////

    private void StartPlayerTurn()
    {
        SetupPlayerTurn();

        currentState = BattleState.PlayerTurn;

        Debug.Log("Player Turn Started");

        //test
        //PlaySelectedCard();
        //Debug.Log("Energy : "+ player.CurrentEnergy);

    }

    private void SetupPlayerTurn()
    {
        SetupResources();

        SetupCards();

        SetupEffects();

        SetupUI();
    }

    private void SetupResources()
    {
        player.ResetEnergy();
        player.ResetBlock();
    }

    private void SetupCards()
    {
        deckManager.StartPlayerTurnDraw();
    }

    private void SetupEffects()
    {

    }

    private void SetupUI()
    {

    }

    public void SelectCard(CardData card)
    {
        if (currentState != BattleState.PlayerTurn)
        {
            return;
        }

        selectedCard = card;
    }

    public void PlaySelectedCard()
    {
        Debug.Log("PlaySelectedCard Called");

        if (currentState != BattleState.PlayerTurn)
        {
            return;
        }
    
        if (selectedCard == null)
        {
            return;
        }

        if (!deckManager.CanPlayCard(selectedCard))
        {
            Debug.Log("CARD NOT IN HAND");
            return;
        }

        int cost = selectedCard.EnergyCost;
        
        //ถ้าทำสิ่งนี้ไม่สำเร็จให้หยุดทันที
        if (!player.TryUseEnergy(cost))
        {
            Debug.Log("NOT HAVE ENEGY");
            return;
        }
        
        cardEffectManager.Resolve(selectedCard, player, enemy);
        deckManager.PlayCard(selectedCard);

        //test
        Debug.Log("Card Effect Resolved");
        Debug.Log("Energy : "+ player.CurrentEnergy);
        //

        ProcessBattleEnd();

        //RefreshUI();
    }

    private void EndPlayerTurn()
    {
        currentState = BattleState.EnemyTurn;

        Debug.Log("Player Turn Ended");

        StartEnemyTurn();
    }
    
    /////Enemy/////

    private void StartEnemyTurn()
    {
        SetupEnemyTurn();

        currentState = BattleState.EnemyTurn;

        Debug.Log("Enemy Turn Started");

        enemyManager.PerformTurn();

        ProcessBattleEnd();

        if (currentState == BattleState.End)
        {
            return;
        }

        PrepareEnemyIntent();

        EndEnemyTurn();
    }

    private void SetupEnemyTurn()
    {
        enemy.ResetBlock();
    }

    private void PrepareEnemyIntent()
    {
        enemy.DecideAction();

        enemyIntentDisplayManager.ShowIntent(enemy.CurrentAction);
    }

    private void EndEnemyTurn()
    {
        currentState = BattleState.PlayerTurn;

        Debug.Log("Enemy Turn Ended");

        StartPlayerTurn();
    }

    /////Battle Result/////

    /// // TODO: Refactor when battle result has more outcomes
    private void ProcessBattleEnd()
    {
        if (player.IsDead())
        {
            HandlePlayerLose();
            return;
        }

        if (enemy.IsDead())
        {
            HandlePlayerWin();
            return;
        }
    }

    private void HandlePlayerWin()
    {
        Debug.Log("Player Wins!");
        EndBattle();
    }

    private void HandlePlayerLose()
    {
        Debug.Log("Player Loses!");
        EndBattle();
    }

    private void EndBattle()
    {
        currentState = BattleState.End;

        Debug.Log("Battle Ended");
    }
}