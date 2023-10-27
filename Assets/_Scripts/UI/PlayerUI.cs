using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour
{
    [Header("HP UI")] 
    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private FloatVariableSO _playerHP;

    [Header("End Screen")]
    [SerializeField] private GameObject _endScreen;
    
    [Header("Game Event")]
    [SerializeField] private GameEvent _playerDeathEvent;

    private void Update()
    {
        _hpText.text = _playerHP.RuntimeValue.ToString();
    }

    private void OnPlayerDeath()
    {
        _endScreen.SetActive(true);
    }

    public void RestartScene()
    {
        StartCoroutine(LoadSceneAsync(SceneManager.GetActiveScene().buildIndex));
    }

    public void BackToMenu()
    {
        StartCoroutine(LoadSceneAsync(0));
    }
    
    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceneIndex);
        while (!loadScene.isDone)
        {
            yield return null;
        }
    }
    
    private void OnEnable()
    {
        _playerDeathEvent.AddListener(OnPlayerDeath);
    }

    private void OnDisable()
    {
        _playerDeathEvent.RemoveListener(OnPlayerDeath);
    }
}
