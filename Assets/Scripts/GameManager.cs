using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int health = 100;
    public int points = 0;
    public int itemsCollected = 0;
    public int itemsRequired = 2;
    public int kills = 0;
    public int killsToWin = 5;
    public bool loadCreditsOnWin = false;

    public UnityEvent OnStatsChanged;
    public UnityEvent OnAllItemsCollected;
    public UnityEvent OnHordeDefeated;

    private void Awake()
    {
        if (OnStatsChanged == null) OnStatsChanged = new UnityEvent();
        if (OnAllItemsCollected == null) OnAllItemsCollected = new UnityEvent();
        if (OnHordeDefeated == null) OnHordeDefeated = new UnityEvent();

        if (Instance == null)
        {
            Instance = this;
            // Optionally persist between scenes if needed, though for now a per-scene manager works.
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddHealth(int amount)
    {
        health += amount;
        if (health > 100) health = 100;
        if (health <= 0)
        {
            health = 0;
            if (AudioManager.Instance != null) AudioManager.Instance.PlayGameOverSFX();
            // Trigger death
            var p = GameObject.Find("Player");
            if (p != null) {
                p.SendMessage("perderVida", SendMessageOptions.DontRequireReceiver);
            }
        }
        OnStatsChanged?.Invoke();
    }

    public void TakeDamage(int amount)
    {
        AddHealth(-amount);
    }

    public void AddPoints(int amount)
    {
        points += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddItem()
    {
        itemsCollected++;
        OnStatsChanged?.Invoke();
        // HUD handled by ctrlPlayer listening to OnStatsChanged
        
        if (itemsCollected >= itemsRequired)
        {
            OnAllItemsCollected?.Invoke();
        }
    }

    public void AddKill()
    {
        kills++;
        OnStatsChanged?.Invoke();
        
        if (kills >= killsToWin) {
            OnHordeDefeated?.Invoke();
            if (loadCreditsOnWin) {
                UnityEngine.SceneManagement.SceneManager.LoadScene("04_Creditos");
            }
        }
    }
}
