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

    [SerializeField] private GameObject defaultText;
    [SerializeField] private TMPro.TextMeshProUGUI titreText;
    [SerializeField] private TMPro.TextMeshProUGUI descText;

    private int displayedButtonsCount;


    private List<Button> buttons;
    private EventDescription[] eventDescs;

    void Start()
    {
        buttons = new List<Button>();
        eventDescs = new EventDescription[100]; //DEBUG
        StartCoroutine(DebugTest());
    }

    private void OnEnable()
    {
        defaultText.SetActive(true);
        titreText.text = "";
        descText.text = "";
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
        button.onClick.AddListener(delegate { SelectEvent(eventDesc.id); }); //Faire appeler la fonction select event avec l'id

        buttonParent.sizeDelta += intervalleY * Vector2.up;

        eventDescs[eventDesc.id] = eventDesc;
    }


    public void SelectEvent(int id)
    {
        Debug.Log("Event " + id + " selected");

        EventDescription desc = eventDescs[id];

        if (desc == null) return;

        defaultText.SetActive(false);
        titreText.text = desc.name;
        descText.text = desc.description;
    }


    IEnumerator DebugTest()
    {

        for (int i = 0; i<100; i++)
        {
            yield return new WaitForSeconds(1);

            EventDescription testEvent = new EventDescription()
            {
                name = "Time debug : " + Time.time,
                description = "Time debug : " + Time.time + "Time debug : " + Time.time + "Time debug : " + Time.time + "Time debug : " + Time.time + "Time debug : " + Time.time + "Time debug : " + Time.time + "Time debug : " + Time.time + "Time debug : " + Time.time + "Time debug : " + Time.time + "Time debug : " + Time.time + "Time debug : " + Time.time + "Time debug : " + Time.time + "Time debug : " + Time.time + "Time debug : " + Time.time,
                id = i
            };

            AddEventButton(testEvent);
        }
       
    }

    
}
