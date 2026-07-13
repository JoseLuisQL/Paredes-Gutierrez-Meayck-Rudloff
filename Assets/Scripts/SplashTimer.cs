using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashTimer : MonoBehaviour
{
    public float delay = 5f;
    public string nextSceneName = "01_Rampas";

    void Start()
    {
        Invoke("LoadNextScene", delay);
    }

    void LoadNextScene()
    {
        Debug.Log("Cargando nivel: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
}