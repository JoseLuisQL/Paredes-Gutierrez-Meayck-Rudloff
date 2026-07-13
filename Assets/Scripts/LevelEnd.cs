using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    public string nextLevelName = "02_Hangar";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.name == "Player")
        {
            SceneManager.LoadScene(nextLevelName);
        }
    }
}