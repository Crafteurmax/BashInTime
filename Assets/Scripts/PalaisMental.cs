using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PalaisMental : MonoBehaviour
{
    
    [Serializable]
    public class EventDescription
    {
        public string name;
        public string description;
        public int id;
    }

    [SerializeField] private TMPro.TextMeshProUGUI description;
    [SerializeField] private RectTransform buttonParent;
    [SerializeField] private GameObject buttonPrefab;

    [SerializeField] private float intervalleY;
    [SerializeField] private Vector2 firstButtonPosition;

    private int displayedButtonsCount;


    private List<Button> buttons;

    void Start()
    {
        buttons = new List<Button>();
        StartCoroutine(DebugTest());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool isEventDisplayed(EventDescription eventDesc)
    {
        foreach(Button button in buttons){
            if(button.name == eventDesc.name)
            {
                return true;
            }
        }

        return false;
    }



    void AddEventButton(EventDescription eventDesc)
    {
        if (isEventDisplayed(eventDesc)) return;

        GameObject buttonObject = Instantiate(buttonPrefab);

        buttonObject.transform.SetParent(buttonParent);
        
        buttonObject.transform.localPosition = firstButtonPosition + (intervalleY * displayedButtonsCount) * Vector2.down;
        displayedButtonsCount++;

        TMPro.TextMeshProUGUI textMesh = buttonObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        textMesh.text = eventDesc.name;

        Button button = buttonObject.GetComponent<Button>();

        button.name = eventDesc.name;

        buttonParent.sizeDelta += intervalleY * Vector2.up;
    }


    IEnumerator DebugTest()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            EventDescription testEvent = new EventDescription()
            {
                name = "Time debug : " + Time.time,
                description = "desc",
                id = -1
            };

            AddEventButton(testEvent);
        }
       
    }
}
