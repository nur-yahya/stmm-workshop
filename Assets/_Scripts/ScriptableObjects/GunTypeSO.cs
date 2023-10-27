using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Crate New Gun", fileName = "New Gun")]
public class GunTypeSO : ScriptableObject
{
    public float ShootRate;
    public float BulletSpeed;
    public float BulletDuration;
    public float BulletDamage;
}
