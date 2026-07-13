using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public GameObject creditsCanvas;
    public Text creditsText;

    void Start()
    {
        if (creditsCanvas != null)
        {
            creditsCanvas.SetActive(false);
        }
        
        if (GameManager.Instance != null) {
            GameManager.Instance.OnHordeDefeated.AddListener(ShowCredits);
        }
    }

    public void ShowCredits()
    {
        if (creditsCanvas != null)
        {
            creditsCanvas.SetActive(true);
            if (creditsText != null) {
                // Placeholder, user must provide real names and NRC
                creditsText.text = "¡Misión Cumplida!\n\nCréditos:\n" +
                                   "Paredes [Nombre] - NRC [XXXX]\n" +
                                   "Gutierrez [Nombre] - NRC [XXXX]\n" +
                                   "Meayck [Nombre] - NRC [XXXX]\n" +
                                   "Rudloff [Nombre] - NRC [XXXX]";
            }
            
            // Pausar el juego
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
