using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadARScene()
    {
        StartCoroutine(LoadSceneAsync(1));
    }

    public void LoadNonARScene()
    {
        StartCoroutine(LoadSceneAsync(2));
    }
    
    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceneIndex);
        while (!loadScene.isDone)
        {
            yield return null;
        }
    }
}
