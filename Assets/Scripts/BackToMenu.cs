using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("00_Splash");
    }
}
