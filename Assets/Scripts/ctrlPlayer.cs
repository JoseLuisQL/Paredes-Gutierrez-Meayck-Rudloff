using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ctrlPlayer : MonoBehaviour
{
    public TextMeshProUGUI txtCoin;
    public TextMeshProUGUI txtPoints;
    public TextMeshProUGUI txtItems;
    public TextMeshProUGUI txtKills;
    public Slider sldVida;
    
    public GameObject continuar;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStatsChanged.AddListener(UpdateHUD);
            UpdateHUD();
        }
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStatsChanged.RemoveListener(UpdateHUD);
        }
    }

    void UpdateHUD()
    {
        if (GameManager.Instance == null) return;
        
        if (txtCoin != null) txtCoin.gameObject.SetActive(false); // Hide the redundant coin text
        if (txtPoints != null) txtPoints.text = $"Puntos: {GameManager.Instance.points}";
        if (txtItems != null) txtItems.text = $"Objetos: {GameManager.Instance.itemsCollected}/{GameManager.Instance.itemsRequired}";
        if (txtKills != null) txtKills.text = $"Bajas: {GameManager.Instance.kills}";
        if (sldVida != null) sldVida.value = GameManager.Instance.health;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Fin")
        {
            if (continuar != null) continuar.SetActive(true);
        }
    }
}
