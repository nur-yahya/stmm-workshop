using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _maxHP = 3f;
    [SerializeField] private float _speed = 4f;

    [Header("Game Event")]
    [SerializeField] private GameEvent _playerDeathEvent;

    private float _currentHP;
    private Rigidbody _rb;
    private Transform _playerTransform;
    private ObjectPool<Enemy> _enemyPool;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _currentHP = _maxHP;
    }

    public void Init(Transform playerTransform, ObjectPool<Enemy> enemyPool)
    {
        _playerTransform = playerTransform;
        _enemyPool = enemyPool;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(_playerTransform, Vector3.up);
    }

    private void FixedUpdate()
    {
        _rb.velocity = transform.forward * _speed;
    }

    public void Damaged(float damage)
    {
        _currentHP -= damage;
        if (_currentHP <= 0f)
        {
            Death();
        }
    }

    private void Death()
    {
        if(!gameObject.activeSelf) return;
        _enemyPool.Release(this);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.TryGetComponent(out Player player))
        {
            player.Attacked(1f);
            Death();
        }
    }

    private void OnPlayerDeath()
    {
        Death();
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
