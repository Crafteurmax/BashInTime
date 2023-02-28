using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Typing : MonoBehaviour
{
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

    protected void OnEnable()
    {
        // initialisation des events lors de l'activation
        Keyboard.current.onTextInput += OnTextInput;
        enter.action.canceled += OnEnter;
        del.action.started += OnDel;
        moveCursor.action.started += OnMoveCursor;
    }


    protected void OnDisable()
    {
        // désinitialisation des events si on désactive 
        Keyboard.current.onTextInput -= OnTextInput;
        enter.action.canceled -= OnEnter;
        del.action.started -= OnDel;
        moveCursor.action.started -= OnMoveCursor;
    }

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

        yield return new WaitForSeconds(0.1f);

        if (del.action.IsPressed())
        {
            StartCoroutine(DelChar());
        }
    }

    private void OnEnter(InputAction.CallbackContext obj)
    {
        currentText = "";
        cursor = 0;
        //on envoie la commande
        Debug.Log("enter command");
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

        yield return new WaitForSeconds(0.1f);

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



}

