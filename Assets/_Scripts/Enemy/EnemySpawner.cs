using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private GameObject _enemyPrefab;

    [Header("Game Events")]
    [SerializeField] private GameEvent _playerDeathEvent;

    [Header("Spawn Params")] [SerializeField]
    private float _spawnRate = 1f;

    [SerializeField] private float _spawnCircleRadius = 4.5f;
    [SerializeField] private float _heightOffset = 0.5f;

    private ObjectPool<Enemy> _enemyPool;

    private void Start()
    {
        _enemyPool = new ObjectPool<Enemy>(OnCreate, OnGet, OnReturn, OnDestroyPool);
    }

    private void OnDestroyPool(Enemy obj)
    {
        Destroy(obj.gameObject);
    }

    private void OnReturn(Enemy obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void OnGet(Enemy obj)
    {
        obj.transform.position = GetRandomPointInCircle();
        obj.gameObject.SetActive(true);
    }

    private Enemy OnCreate()
    {
        Enemy spawnedEnemy = Instantiate(_enemyPrefab, GetRandomPointInCircle(), Quaternion.identity, transform)
            .GetComponent<Enemy>();
        spawnedEnemy.Init(_playerTransform, _enemyPool);
        return spawnedEnemy;
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnRate);
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        _enemyPool.Get();
    }

    private void OnPlayerDeath()
    {
        StopAllCoroutines();
    }

    private void OnEnable()
    {
        StartCoroutine(SpawnLoop());
        _playerDeathEvent.AddListener(OnPlayerDeath);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _playerDeathEvent.RemoveListener(OnPlayerDeath);
    }

    private Vector3 GetRandomPointInCircle()
    {
        float angle = Random.Range(0f, 2 * Mathf.PI);
        return new Vector3(Mathf.Cos(angle) * _spawnCircleRadius, _heightOffset, Mathf.Sin(angle) * _spawnCircleRadius);
    }
}