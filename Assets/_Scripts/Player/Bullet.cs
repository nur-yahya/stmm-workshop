using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    private float _damage;
    private Rigidbody _rb;
    
    private ObjectPool<Bullet> _bulletPool;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Init(float duration, float damage, float speed, Vector3 direction, ObjectPool<Bullet> bulletPool)
    {
        _damage = damage;
        _bulletPool = bulletPool;

        _rb.velocity = direction * speed;
        StartCoroutine(WaitThenDisable(duration));
    }

    IEnumerator WaitThenDisable(float duration)
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
        _bulletPool.Release(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            return;
        }
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.Damaged(_damage);
        }
        
        gameObject.SetActive(false);
        _bulletPool.Release(this);
    }
}
