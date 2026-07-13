using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueMenu : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void setActive()
    {
        gameObject.SetActive(true);
    }
    
    public void restartLevel()
    {
        // Actually load next level per requirements
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
    
    public void loadNextLevel()
    {
        restartLevel();
    }
}