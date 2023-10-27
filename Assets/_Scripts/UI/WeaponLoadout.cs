using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLoadout : MonoBehaviour
{
    [SerializeField] private GameEvent_GunTypeSO _changeWeaponEvent;
    
    public void ChangeWeapon(GunTypeSO chosenWeapon)
    {
        _changeWeaponEvent.TriggerEvent(chosenWeapon);
    }
}
