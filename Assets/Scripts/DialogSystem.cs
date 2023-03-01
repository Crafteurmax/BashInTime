using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
public class DialogueStep
{
    public string characterName;
    public string dialogContent;
}

[System.Serializable]
public class Dialogue
{
    public DialogueStep[] dialogSteps;
}

public class DialogSystem : MonoBehaviour
{

    public float characterHappeningSpeed = 5.0f;
    public TMPro.TextMeshProUGUI dialogText;
    public TMPro.TextMeshProUGUI characterText;

    //Debug--------
    public TextAsset testDialog;
    private void Start()
    {
        StartDialogue(testDialog);
    }
    //-------------

    //Fonction a appeler pour lancer un dialogue avec le fichier json correspondant au dialogue
    public void StartDialogue(TextAsset jsonFile)
    {
        Dialogue dialogue = JsonUtility.FromJson<Dialogue>(jsonFile.text);

        StartCoroutine(DialogMethod(dialogue));
    }


    //Coroutine du dialogue
    IEnumerator DialogMethod(Dialogue dialogue)
    {

        //On deroule les etapes du dialogue
        foreach (DialogueStep step in dialogue.dialogSteps)
        {
            string textToDisplay = step.dialogContent;
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

                DisplayFinalDialog(step.characterName, textToDisplay.Substring(0, realCompletion));

                yield return null;
            }

            DisplayFinalDialog(step.characterName, textToDisplay);
        }

        yield break;
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
    private void DisplayFinalDialog(string characterName, string finalText)
    {
        dialogText.text = finalText;
        characterText.text = characterName;
    }


}
