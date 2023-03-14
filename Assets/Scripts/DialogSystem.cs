using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Runtime;
using System;

//Structures de données pour représenter les dialogues

[System.Serializable]
public class DialogueChoice
{
    public string name;
    public int nextAction;
    public int nextDialog;
}


[System.Serializable]
public class DialogueStep
{
    public string characterName;
    public string dialogContent;
    public DialogueChoice[] dialogChoices;
}

[System.Serializable]
public class Dialogue
{
    public DialogueStep[] dialogSteps;
}

public class DialogSystem : MonoBehaviour
{

    [SerializeField] private InputActionReference dialogButton; 

    [SerializeField] private float characterHappeningSpeed = 5.0f;
    [SerializeField] private TMPro.TextMeshProUGUI dialogText;
    [SerializeField] private TMPro.TextMeshProUGUI characterText;
    [SerializeField] private GameObject dialogUI;

    [SerializeField] private TMPro.TextMeshProUGUI[] choicesText;
    [SerializeField] private GameObject[] choicesBox;


    //Fonction a appeler pour lancer un dialogue avec le fichier json correspondant au dialogue
    public void StartDialogue(TextAsset jsonFile, Action[] actions)
    {
        Dialogue dialogue = JsonUtility.FromJson<Dialogue>(jsonFile.text);


        dialogUI.SetActive(true);
        StartCoroutine(DialogMethod(dialogue, actions));
    }


    private bool pressedPreviously = false;
    //Coroutine du dialogue
    private IEnumerator DialogMethod(Dialogue dialogue, Action[] actions)
    {
        //On deroule les etapes du dialogue
        for (int i = 0; i < dialogue.dialogSteps.Length; i++)
        {
            DialogueStep step = dialogue.dialogSteps[i];

            //Ecriture progressive et on attend
            endProgressiveWriting = false;
            Coroutine courout = StartCoroutine(
                        ProgressiveWriting(step.dialogContent, step.characterName));

            while (!endProgressiveWriting)
            {
                yield return null;
            }


            DisplayFinalDialog(step.characterName, step.dialogContent, step.dialogChoices);

            //On bloque le dialogue tant que le jouer n'a pas appuye sur E si on attend un choix
            if(step.dialogChoices.Length == 0)
            {
                while (dialogButton.action.IsPressed()) yield return null;
                while (!dialogButton.action.IsPressed()) yield return null;
            }
            //On attend pour les choix
            else
            {
                while (currentChoice < 0) yield return null;

                ManageChoice(ref i, actions, step.dialogChoices[currentChoice]);
            }

            pressedPreviously = true;
        }

        dialogUI.SetActive(false);
        yield break;
    }

    //On gère l'execution des choix
    private void ManageChoice(ref int i, Action[] actions, DialogueChoice choice)
    {
        if (choice.nextDialog >= 0)
        {
            i = choice.nextDialog - 1; //On anticipe le i++
        }

        if (actions != null)
        {
            if (choice.nextAction >= 0 && choice.nextAction < actions.Length)
            {
                actions[choice.nextAction]();
            }
        }
    }

    //Permet d'écrire progressivement le dialogue
    private bool endProgressiveWriting = false;
    private IEnumerator ProgressiveWriting(string textToDisplay, string characterName)
    {
        float beginningTime = Time.time;
        int length = textToDisplay.Length;
        int showLength = 0;
        bool[] isTextTag = FindTMTags(length, textToDisplay, ref showLength);

        int previousCompletion = 0;
        int previousRealCompletion = 0;


        //A chaque frame, tant que la boite de dialogue doit continuer de s'executer
        while (Time.time < beginningTime + showLength / characterHappeningSpeed)
        {
            int realCompletion = CalculateRealCompletion(beginningTime, showLength, length, isTextTag, ref previousCompletion, ref previousRealCompletion);

            DisplayFinalDialog(characterName, textToDisplay.Substring(0, realCompletion), null);

            //Permet de skip le dialogue
            if (dialogButton.action.IsPressed())
            {
                if (!pressedPreviously) break;
            }
            else pressedPreviously = false;

            yield return null;
        }
        endProgressiveWriting = true;
    }

    //Permet d'indiquer ou sont les balises, permet afficher le texte colore correctement
    private bool[] FindTMTags(int length, string text, ref int showLength)
    {
        bool[] isTextTag = new bool[length];

        for (int i = 0; i < length; i++)
        {
            if (text[i] == '<')
            {
                isTextTag[i] = true;
            }

            if (i < 1) continue;
            else if (isTextTag[i]) showLength++;


            if (isTextTag[i - 1]) isTextTag[i] = true;

            if (text[i - 1] == '>') isTextTag[i] = false;

            if (!isTextTag[i]) showLength++;
        }

        return isTextTag;
    }

    //Calcule le nombre de caracteres a afficher en prenant soin d'afficher les balises instantanement 
        //(les references avec les anciennes completions permettent d'eviter de recalculer a chaque frame)
    private int CalculateRealCompletion(float beginningTime, int showLength, int length, bool[] isTextTag, ref int previousCompletion, ref int previousRealCompletion)
    {
        int completion = (int)Mathf.Clamp((Time.time - beginningTime) * characterHappeningSpeed, 0, showLength);

        int realCompletion = previousRealCompletion;
        for (int i = previousCompletion; i < completion; i++)
        {
            while (realCompletion < length && isTextTag[realCompletion])
            {
                realCompletion++;
            }
            realCompletion++;
        }

        return (int)Mathf.Clamp(realCompletion, 0, length);
    }

    //On affiche les textes sur la boite de dialogue
    private void DisplayFinalDialog(string characterName, string finalText, DialogueChoice[] dialogChoices)
    {
        dialogText.text = finalText;
        characterText.text = characterName;

        //On clear les choix s'il y en a pas
        if (dialogChoices == null)
        {
            for (int i = 0; i < choicesBox.Length; i++)
            {
                choicesBox[i].SetActive(false);
            }

            return;
        }
            
            
        //On affiche les choix
        for(int i = 0; i < Mathf.Min(dialogChoices.Length, choicesBox.Length); i++)
        {
            choicesBox[i].SetActive(true);
            choicesText[i].text = dialogChoices[i].name;
        }
    }

    //Fonction appelée quand on sélectionne le choix en cliquant sur le bouton
    private int currentChoice = -1;
    public void ChoiceSelect(int choiceId)
    {
        currentChoice = choiceId;
    }




    //Debug----------------------------------------
    /*
    [SerializeField] private TextAsset testDialog;

    private void coucou()
    {
        Debug.Log("coucou1");
    }

    private void coucou2()
    {
        Debug.Log("coucou2");
    }

    private void Start()
    {
        StartDialogue(testDialog, new Action[] { coucou, coucou2 });
    }
    */
    //----------------------------------------------

}
