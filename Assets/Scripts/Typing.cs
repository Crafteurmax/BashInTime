using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Typing : MonoBehaviour
{
    //################################ variable en commun ################################ 

    private float delayTouche = 0.05f;

    //################################ input du texte ################################  
    [SerializeField]
    private InputActionReference enter;
    [SerializeField]
    private InputActionReference del;
    [SerializeField]
    private InputActionReference moveCursor;

    // emplacement du curseur dans le texte
    private int cursor;

    // texte qui est actuellement entrain d'etre ecrit
    private String currentText = "";

    // on lance le debut de suppression des charactères
    private void OnDel(InputAction.CallbackContext obj)
    {
        StartCoroutine(DelChar());
    }

    // si la touche est encore appyé au bout de x temps on re supprime
    private IEnumerator DelChar()
    {
        if (cursor > 0)
        { 
            cursor--;
            currentText = currentText.Remove(cursor, 1);
            DisplayText(currentText);
        }

        yield return new WaitForSeconds(delayTouche);

        if (del.action.IsPressed())
        {
            StartCoroutine(DelChar());
        }
    }

    private void OnEnter(InputAction.CallbackContext obj)
    {
        //Debug.Log("enter command");
        addCommandToFixText(currentText);
        //on envoie la commande
        //on reset la commande
        currentText = "";
        cursor = 0;
    }

    private void OnTextInput(char ch)
    {
        if(!Char.IsControl(ch)) // permet de garder uniquement les charactères "humain"
        {
            if(cursor != currentText.Length) // le insert ne marche pas si on est en bout de ligne
            {
                currentText = currentText.Insert(cursor, "" + ch);
            }
            else
            {
                currentText += ch;
            }
            cursor++;
            DisplayText(currentText);
        }
    }

    // on lance le debut du deplacement du curseur
    private void OnMoveCursor(InputAction.CallbackContext obj)
    {
        StartCoroutine(MoveCursor(obj.ReadValue<float>()));
    }

    // si la touche est encore appyé au bout de x temps on re decale le curseur
    private IEnumerator MoveCursor(float direction)
    {
        if (!((cursor == currentText.Length && direction > 0 ) || (cursor == 0 && direction < 0))) // si on essaye pas de deplacer le curseur en dehors du texte
        {
            cursor += (int) direction;
            DisplayText(currentText);
        }

        yield return new WaitForSeconds(delayTouche);

        if (moveCursor.action.IsPressed())
        {
            StartCoroutine(MoveCursor(direction));
        }
    }

    // simple outils de debug, permet de representer le curseur au bon endroit
    private void DisplayText(String textToDisplay)
    {
        String outputText = "";
        if (cursor != textToDisplay.Length && cursor != 0)
        {
            outputText += textToDisplay.Remove(cursor);
            outputText += "|";
            outputText += textToDisplay.Substring(cursor);
        }
        else if(cursor == 0)
        {
            outputText += "|";
            outputText += textToDisplay;
        }
        else
        {
            outputText += textToDisplay;
            outputText += "|";
        }

        Debug.Log(outputText);
    }

    //################################ affichage du texte ################################  

    [SerializeField]
    private TextMeshProUGUI textComponent;

    [SerializeField]
    private InputActionReference scroll;

    String fixText = "<color=green>this is the default text and I'm supposed to be green \n"+
                     "if it's not the case, I allow you to scream because if ice cream, you scream";
    [ContextMenu("refresh screen")]
    private void refreshScreen()
    {
        textComponent.text = fixText + "\n" + formatText(currentText);
    }

    private String formatText(String textToFormat)
    {
        String outputText = "";
        if (cursor != textToFormat.Length && cursor != 0)
        {
            outputText += textToFormat.Remove(cursor);
            outputText += "|";
            outputText += textToFormat.Substring(cursor);
        }
        else if (cursor == 0)
        {
            outputText += "|";
            outputText += textToFormat;
        }
        else
        {
            outputText += textToFormat;
            outputText += "|";
        }

        return outputText;
    }

    private void addCommandToFixText(String command)
    {
        fixText += "\nCurrent\\Directory\\but i don't know it yet > " + currentText;
    }

    float scrollSpeed = 100.0f;
    private void OnScroll(InputAction.CallbackContext obj)
    {
        StartCoroutine(Scroll(obj.ReadValue<float>()));
    }

    private IEnumerator Scroll(float direction)
    {
        textComponent.rectTransform.SetPositionAndRotation(
            textComponent.rectTransform.position + new Vector3(0,direction * scrollSpeed * Time.deltaTime,0),
            textComponent.rectTransform.rotation);

        if (textComponent.rectTransform.position.y < 0)
        {
            textComponent.rectTransform.SetPositionAndRotation(
            new Vector3(0, 0,0),
            textComponent.rectTransform.rotation);
        }
        yield return new WaitForSeconds(delayTouche);

        if (scroll.action.IsPressed())
        {
            StartCoroutine(Scroll(direction));
        }
    }

    //################################ fonction de unity ################################  
    void Start()
    {
        textComponent.SetText(fixText);
    }

    // Update is called once per frame
    void Update()
    {
        refreshScreen();
    }

    protected void OnEnable()
    {
        // initialisation des events lors de l'activation
        Keyboard.current.onTextInput += OnTextInput;
        enter.action.canceled += OnEnter;
        del.action.started += OnDel;
        moveCursor.action.started += OnMoveCursor;
        scroll.action.started += OnScroll;
    }


    protected void OnDisable()
    {
        // désinitialisation des events si on désactive 
        Keyboard.current.onTextInput -= OnTextInput;
        enter.action.canceled -= OnEnter;
        del.action.started -= OnDel;
        moveCursor.action.started -= OnMoveCursor;
    }
}

