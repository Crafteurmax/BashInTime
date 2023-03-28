using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class TimeManager : MonoBehaviour
{
    // Start is called before the first frame update
    public float time = 0;
    public float t0 = 55200;
    public float timeSpeed = 4;

    public Condition[] eventConditions = { };

    private int eventCounter;
    public List<Evenement> listeEvenement = new List<Evenement>();
    private Action[] ActionList = { null, test, test2 };
    [SerializeField] TextAsset eventsJSON;

    [SerializeField] private TextMeshProUGUI textComponent;
    void Start()
    {
        EventsInfo eventsInfo = JsonUtility.FromJson<EventsInfo>(eventsJSON.text);

        for(var i = 0; i<eventsInfo.Events.Length; i++) 
        {
            Events _ev = eventsInfo.Events[i];
            listeEvenement.Add(new Evenement(_ev.eventName, _ev.timeStamp, _ev.conditionsID, ActionList[_ev.actionID]));
        }

        listeEvenement.Sort();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * timeSpeed;
        if (listeEvenement[eventCounter].IsMature(time))
        {
            if (listeEvenement[eventCounter].action != null && listeEvenement[eventCounter].IsEventPossible(eventConditions)) listeEvenement[eventCounter].action();
            if (eventCounter +1 < listeEvenement.Count) eventCounter++;
        }
        String temp = formatTime(time);
        textComponent.text = temp;
        Debug.Log(temp);
    
        //Debug.Log(eventCounter);
        //int[] temp = ConvertTime(time);
        //Debug.Log(temp[2] + ":" + temp[1] + ":" + temp[0]);
    }

    private string formatTime(float time)
    {
        String heure = "";
        String minute = "";
        int[] humanTime = ConvertTime(time + t0);
        if (humanTime[2] < 10) heure += "0" + humanTime[2]; else heure += humanTime[2];
        if (humanTime[1] < 10) minute += "0" + humanTime[1]; else minute += humanTime[1];
        if (humanTime[0] % 2 == 0) return heure + " " + minute; else return heure + ":" + minute;

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

    static void test() { Debug.Log("hello"); }

    static void test2() { Debug.Log("hello 2"); }
}

[System.Serializable]
public class Events
{
    public String eventName;
    public float timeStamp;
	public int[] conditionsID;
    public int actionID;
}


[System.Serializable]
public class EventsInfo {
    public Events[] Events; 
}
public class Evenement : IComparable
{
    public String name;
    public float timeStamp;
    int[] conditionsID;
    public Action action;
    public Evenement(String name, float timeStamp, int[] conditionsID, Action action)
    {
        this.name = name;
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
