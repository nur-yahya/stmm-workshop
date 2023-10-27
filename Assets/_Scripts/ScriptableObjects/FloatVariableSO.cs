using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Variable/Crate New Float", fileName = "New Float Variable")]
public class FloatVariableSO : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private float _initialValue;
    [HideInInspector] public float RuntimeValue;


    public void OnBeforeSerialize()
    {
        return;
    }

    public void OnAfterDeserialize()
    {
        ResetData();
    }

    private void ResetData()
    {
        RuntimeValue = _initialValue;
    }
}
