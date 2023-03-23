using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    // Start is called before the first frame update
    public float time = 0;
    public float timeSpeed = 4;

    public Condition[] eventConditions = {};

    private int eventCounter;
    public List<Evenement> listeEvenement = new List<Evenement>();

    void Start()
    {
        Evenement e1 = new Evenement(10, new[] { 1}, test);
        Evenement e2 = new Evenement(1, null, null);
        Evenement e3 = new Evenement(121684, null, null);
        Evenement e4 = new Evenement(1982, null, null);
        Evenement e5 = new Evenement(162, null, test2);
        Evenement e6 = new Evenement(651, null, null);
        Evenement e7 = new Evenement(168, null, null);
        Evenement e8 = new Evenement(3255951, null, null);

        listeEvenement.Add(e1);
        listeEvenement.Add(e2);
        listeEvenement.Add(e3);
        listeEvenement.Add(e4);
        listeEvenement.Add(e5);
        listeEvenement.Add(e6);
        listeEvenement.Add(e7);
        listeEvenement.Add(e8);

        listeEvenement.Sort();
    }

    void test()
    {
        Debug.Log("hello");
    }

    void test2()
    {
        Debug.Log("hello 2");
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * timeSpeed;
        if (listeEvenement[eventCounter].IsMature(time))
        {
            if (listeEvenement[eventCounter].action != null && listeEvenement[eventCounter].IsEventPossible(eventConditions)) listeEvenement[eventCounter].action();
            eventCounter++;
        }

        //Debug.Log(eventCounter);
        //int[] temp = ConvertTime(time);
        //Debug.Log(temp[2] + ":" + temp[1] + ":" + temp[0]);
    }

    int[] ConvertTime(float time)
    {
        int timeInt = ((int) time);
        int[] convertedTime = { 0, 0, 0 };
        convertedTime[2] = timeInt / 3600;
        timeInt -= convertedTime[2] * 3600;
        convertedTime[1] = timeInt / 60;
        timeInt -= convertedTime[1] * 60;
        convertedTime[0] = timeInt;
        return convertedTime;

    }
}

public class Evenement : IComparable
{
    public float timeStamp;
    int[] conditionsID;
    public Action action;
    public Evenement(float timeStamp, int[] conditionsID, Action action)
    {
        this.timeStamp = timeStamp;
        this.conditionsID = conditionsID;
        this.action = action;
    }

    public int CompareTo(object otherEvent)
    {
        return timeStamp.CompareTo(((Evenement)otherEvent).timeStamp);
    }
    public bool IsEventPossible(Condition[] conditions)
    {
        if (conditionsID == null) return true;
        for (var i = 0; i < this.conditionsID.Length; i++)  
        {
            if (!conditions[this.conditionsID[i]].state) return false;
        }
        return true;
    }

    public bool IsMature(float actualTime)
    {
        return actualTime > this.timeStamp;
    }

}

[Serializable]
public class Condition 
{
    public string name;
    public bool state;
}
