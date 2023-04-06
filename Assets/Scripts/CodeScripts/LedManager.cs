using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LedManager : MonoBehaviour
{

    //[SerializeField] private event[] events;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Image img in GetComponentsInChildren<Image>())
        {
            img.color = Color.red;
        }
        
        //GetComponentsInChildren<Image>().color = new Color(1f, 0f, 0f, 1f);
        //1f, 0f, 0f, 1f (couleur rouge)
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PermuteLED (int i)
    {
        Image LEDpicture = transform.GetChild(i).GetComponent<Image>();
        if (LEDpicture.color == Color.red)
            LEDpicture.color = Color.green;
        else
            LEDpicture.color = Color.red;
    }

    private void SwitchLED(int i, bool state)
    {
        //Debug.Log("la LED" + i + "a change d'état");

        transform.GetChild(i).GetComponent<Image>().color = Color.green;
        /*
        GetChild(i).getComponent...
            ou
        GetCompoenentInChildren<...>(i) ....
        */
    }

    public void SwitchLED0()
    {
        PermuteLED(0);
    }
    public void SwitchLED1()
    {
        PermuteLED(1);
    }
    public void SwitchLED2()
    {
        PermuteLED(2);
    }
    public void SwitchLED3()
    {
        PermuteLED(3);
    }
}
