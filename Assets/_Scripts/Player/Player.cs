using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class Player : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private FloatVariableSO _playerHP;
    [SerializeField] private Animator _animator;
    
    [Header("Game Events")]
    [SerializeField] private GameEvent _playerDeathEvent;
    [SerializeField] private GameEvent_GunTypeSO _changeWeaponEvent;

    [Header("Gun Parameter")] 
    [SerializeField] private Transform _gunPoint;
    [SerializeField] private GunTypeSO _gunTypeSO;
    [SerializeField] private GameObject _bulletPrefab;

    private bool _isShooting = false;
    private float _shootCooldown = 0f;
    private float _lookAngle;
    private Transform _camera;
    private Vector3 _velocityVector = new Vector3();
    private Vector3 _shootVector = new Vector3();
    private Rigidbody _rb;

    private ObjectPool<Bullet> _bulletPool;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _camera = Camera.main.transform;
        _bulletPool = new ObjectPool<Bullet>(OnCreate, OnGet, OnRelease, OnDestroyPool);
    }

    private void OnEnable()
    {
        _changeWeaponEvent.AddListener(ChangeWeapon);
    }

    private void OnDisable()
    {
        _changeWeaponEvent.RemoveListener(ChangeWeapon);
    }

    private void ChangeWeapon(GunTypeSO obj)
    {
        _gunTypeSO = obj;
    }

    #region OBJECT POOL
    private void OnDestroyPool(Bullet obj)
    {
        Destroy(obj.gameObject);
    }

    private void OnRelease(Bullet obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void OnGet(Bullet obj)
    {
        obj.transform.SetPositionAndRotation(_gunPoint.position, _gunPoint.rotation);
        obj.gameObject.SetActive(true);
        obj.Init(_gunTypeSO.BulletDuration, _gunTypeSO.BulletDamage, _gunTypeSO.BulletSpeed, _gunPoint.forward, _bulletPool);
    }

    private Bullet OnCreate()
    {
        Bullet bulletSpawned =
            Instantiate(_bulletPrefab, _gunPoint.position, _gunPoint.rotation).GetComponent<Bullet>();
        bulletSpawned.Init(_gunTypeSO.BulletDuration, _gunTypeSO.BulletDamage, _gunTypeSO.BulletSpeed, _gunPoint.forward, _bulletPool);

        return bulletSpawned;
    }
    #endregion

    private void Update()
    {
        if (_isShooting && _shootCooldown > _gunTypeSO.ShootRate)
        {
            Shoot();
            _shootCooldown = 0f;
        }
        _shootCooldown += Time.deltaTime;
        HandleAnimation();
    }

    private void FixedUpdate()
    {
        _rb.velocity = _velocityVector * _moveSpeed;
    }
    
    private void Shoot()
    {
        _bulletPool.Get();
    }

    private void HandleAnimation()
    {
        Vector3 relativeVelocity = GetVelocityRelativeToLookAngle();
        
        _animator.SetFloat("MoveSpeed", _velocityVector.sqrMagnitude);
        _animator.SetFloat("XInput", relativeVelocity.x);
        _animator.SetFloat("YInput", relativeVelocity.z);
    }

    public void Attacked(float damage)
    {
        _playerHP.RuntimeValue -= damage;
        if (_playerHP.RuntimeValue <= 0f)
        {
            Death();
        }
    }

    private void Death()
    {
        _playerDeathEvent.TriggerEvent();
        Destroy(gameObject);
    }

    private Vector3 NormalizeInputToCamera(Vector2 inputVector)
    {
        Vector3 vecForward = _camera.forward;
        Vector3 vecRight = _camera.right;
        vecForward.y = 0;
        vecRight.y = 0;
        vecForward.Normalize();
        vecRight.Normalize();

        return vecForward * inputVector.y + vecRight * inputVector.x;
    }

    private Vector3 GetVelocityRelativeToLookAngle()
    {
        return Quaternion.AngleAxis(_lookAngle, Vector3.up) * _velocityVector;
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        _velocityVector = NormalizeInputToCamera(input);
    }
    
    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled)
        {
            _isShooting = false;
            return;   
        }

        _isShooting = true;
        Vector2 input = context.ReadValue<Vector2>();
        _shootVector = NormalizeInputToCamera(input);
        _lookAngle = Vector3.SignedAngle(_shootVector, Vector3.forward, Vector3.up);

        transform.rotation = Quaternion.LookRotation(_shootVector, Vector3.up);
    }
}
