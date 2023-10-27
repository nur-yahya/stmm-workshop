using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventOneParam<T> : ScriptableObject
{
    private Action<T> _onTrigger;
    public void AddListener(Action<T> listener) => _onTrigger += listener;
    public void RemoveListener(Action<T> listener) => _onTrigger -= listener;
    public void TriggerEvent(T param) => _onTrigger?.Invoke(param);
}
