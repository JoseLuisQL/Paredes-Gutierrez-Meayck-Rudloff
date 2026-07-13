using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    public string nextSceneName = "03_Final";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.name == "Player")
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
