using UnityEngine;

public class Player_Menu_Manager : MonoBehaviour
{
    public static Player_Menu_Manager Instance { get; private set; }

    public bool AttackLeft { get; private set; }
    public bool AttackRight { get; private set; }
    public bool BuyTank { get; private set; }

    [SerializeField] private Player_CoinManager coinManager;

    [SerializeField] private const int attackCost = 5;
    [SerializeField] private const int tankCost = 7;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        AttackLeft = false;
        AttackRight = false;
        BuyTank = false;

        if (coinManager == null)
        {
            coinManager = FindObjectOfType<Player_CoinManager>();
            if (coinManager == null)
            {
                Debug.LogError("Player_CoinManager nu a fost găsit!");
            }
        }
    }

    public void AttackLeftAction()
    {
        if (coinManager.HasEnoughCoins(attackCost))
        {
            coinManager.SpendCoins(attackCost);
            Debug.Log("Attack left");
            AttackLeft = true;
        }
        else
        {
            Debug.Log("Nu ai destule monede pentru Attack Left! (5 necesare)");
        }
    }

    public void AttackRightAction()
    {
        if (coinManager.HasEnoughCoins(attackCost))
        {
            coinManager.SpendCoins(attackCost);
            Debug.Log("Attack right");
            AttackRight = true;
        }
        else
        {
            Debug.Log("Nu ai destule monede pentru Attack Right! (5 necesare)");
        }
    }

    public void BuyTankAction()
    {
        if (coinManager.HasEnoughCoins(tankCost))
        {
            coinManager.SpendCoins(tankCost);
            Debug.Log("Tank cumpărat");
            BuyTank = true;
        }
        else
        {
            Debug.Log("Nu ai destule monede pentru Tank! (7 necesare)");
        }
    }
}
