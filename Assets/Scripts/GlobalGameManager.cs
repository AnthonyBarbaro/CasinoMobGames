using UnityEngine;

public class GlobalGameManager : MonoBehaviour
{
    public static GlobalGameManager Instance { get; private set; }

    private int playerCash = 1000; // Default starting cash

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make this persist across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    public int GetPlayerCash()
    {
        return playerCash;
    }

    public void SetPlayerCash(int amount)
    {
        playerCash = amount;
    }

    public void AdjustPlayerCash(int amount)
    {
        playerCash += amount;
        if (playerCash < 0)
        {
            playerCash = 0; // Prevent negative cash
        }
    }
}
