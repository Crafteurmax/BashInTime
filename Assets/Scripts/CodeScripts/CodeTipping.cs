using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;

[Serializable]
public class Code
{
    public string[] code;
}


public class CodeTipping : MonoBehaviour
{
    //[SerializeField] string[] buttonValues;
    public string[] tippedCode;
    [SerializeField] Code[] rightCodes;
    private int maxLenghCode;
    private int nextInputLoc=0;

    // Start is called before the first frame update
    void Start()
    {
        // il faudra les écrire à la main :( cela pose problème si il y a plusieur utilisation de ce scripte
        /*
        rightCodes = new Code[] 
        
        {
            new  {"B3", "B9"},
            new [] {"B1", "B1", "B0"},
            new [] {"B4", "B4", "B3", "B5"},
            new [] {"B4", "B4", "B3", "B6"}
        };
        */

        foreach (Code tab in rightCodes)
        {
            if (maxLenghCode < tab.code.Length)
                maxLenghCode = tab.code.Length;
        }
        // Création du tableu rotatif
        tippedCode = Enumerable.Repeat("", maxLenghCode).ToArray();
        
        // Ajouter des événements de clic aux boutons
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(delegate { AddFigure(button.gameObject.name); });
        }
        


        /*
        int num = transform.childCount;
        for (int i = 0; i < num; i++)
        {
            transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { AddFigure(transform.GetChild(i).name); });
        }
        */

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddFigure(string tippedValue)
    {

        // Prend en compte l'input
        tippedCode[nextInputLoc] = tippedValue;
        nextInputLoc += 1;
        nextInputLoc %= maxLenghCode;
        //Debug.Log("le bouton " + tippedValue + "a été pressé");

        // Check si un code a été trouvé
        int codeState = CheckCode(tippedCode);
        //Debug.Log("le code n° " + codeState + "a été saisi");
        if (codeState >=0)
        {
            Debug.Log("le code n° " + codeState + " a été saisi");
        }
    }

    void DeleteCode()
    {
        /*
         * RESET la selection du code
         */
        for (int i=0; i<maxLenghCode; i++)
        {
            AddFigure("");
        }
    }
    public int CheckCode (string[] tippedCode)
    {
        /*
         * regarde si l'un des codes posssible a été saisis
         */
        foreach (Code rightCode in rightCodes)
        {
            for (int i=0; i<maxLenghCode; i++)
            {
                int j;
                for (j=0; j<rightCode.code.Length; j++)
                {
                    if (rightCode.code[j % maxLenghCode] != tippedCode[(i + j)%maxLenghCode])
                        break;
                }
                if (j== rightCode.code.Length)
                    return Array.IndexOf(rightCodes, rightCode);
            }

        }
        return -1; // mettre null si ça s'avère plus pratique
    }
}
