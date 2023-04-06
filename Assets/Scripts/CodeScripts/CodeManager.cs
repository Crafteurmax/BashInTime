using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

[Serializable]
public class Code
{
    public string name;
    public string[] code;
    public UnityEvent events;
}


public class CodeManager : MonoBehaviour
{
    /*
     * Permet de gérer un pavé numérique permettant de saisir un code. 
     * Il peut y avoir plusieurs code
     * Cependant il ne faut pas que plusieur code soit inclus l'un dans l'autre où la fin de l'un 
     * correspond à la fin de l'autre
     * 
     * Il peut être interessant d'avoir à terme 3 script :
     * - 1 pour la gestion des bouttons qui envois le boutton pressé à un qui gère la detection des codes 
     * et également leur positionnement, leur nom, couleurs ... (il est dans l'objets Buttons)
     * - 1 qui sert uniquement à détecter les code saisis (il est dans le père suprême) 
     * - 1 qui gère l'affichage des codes qui ont été correctement saisis (il est dans LEDmanager)
     */

    //[SerializeField] string[] buttonValues;
    public string[] tippedCode;
    [SerializeField] Code[] rightCodes;
    private int maxLenghCode;
    private int nextInputLoc=0;

    // Start is called before the first frame update
    void Start()
    {
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
        /*
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(delegate { AddFigure(button.gameObject.name); });
        }
        */
        


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
            //Debug.Log("le code n° " + codeState + " a été saisi");
            rightCodes[codeState].events.Invoke();
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
         * regarde si le dernier bouton appuyer valide un code
         * revois n'indice du code si celui-ci a été saisis
         * -1 si la séquence ne correspond à rien
         */

        int Mod(int a, int n) => (a % n + n) % n;

        foreach (Code rightCode in rightCodes)
        {
            // Itère sur chaque caractère du code
            int indiceCode;
            for (indiceCode = 0; indiceCode< rightCode.code.Length; indiceCode++)
            {
                //Debug.Log("Le code n°" + Array.IndexOf(rightCodes, rightCode) + " a fait vérifier le caractère n°" + Mod(nextInputLoc - rightCode.code.Length + indiceCode, maxLenghCode) + " ("+ tippedCode[Mod(nextInputLoc - rightCode.code.Length + indiceCode, maxLenghCode)] + ") au caractère " + rightCode.code[Mod(indiceCode, maxLenghCode)]);
                if (rightCode.code[indiceCode] != tippedCode[Mod(nextInputLoc - rightCode.code.Length + indiceCode, maxLenghCode)])
                    break;
            }
            if (indiceCode == rightCode.code.Length)
                return Array.IndexOf(rightCodes, rightCode);
        }
        return -1; // mettre null si ça s'avère plus pratique
    }
}
