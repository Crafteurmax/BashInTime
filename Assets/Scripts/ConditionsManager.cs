using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsManager : MonoBehaviour
{
    public Condition[] eventConditions = { };
    public Dictionary<String, int> dicoEvent = new Dictionary<string, int>();
    public void Start()
    {
        for (var i =0; i < eventConditions.Length; i++)
        {
            dicoEvent.Add(eventConditions[i].name, i);
        }
    }

    public void SetCondition(int ID, bool state)
    {
        eventConditions[ID].state = state;
    }

    public void SetCondition(string condName, bool state)
    {
        SetCondition(dicoEvent[condName], state);
    }

    public bool GetConditionState(int ID)
    {
        return eventConditions[ID].state;
    }
    public bool GetConditionState(String condName)
    {
        return GetConditionState(dicoEvent[condName]);
    }
}

[Serializable]
public class Condition
{
    public string name;
    public bool state;
}