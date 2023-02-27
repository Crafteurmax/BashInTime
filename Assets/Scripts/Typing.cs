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

    private int cursor;
    private String currentText;

    protected void OnEnable()
    {
        Keyboard.current.onTextInput += OnTextInput;
        enter.action.canceled += OnEnter;
        del.action.started += OnDel;
    }


    protected void OnDisable()
    {
        Keyboard.current.onTextInput -= OnTextInput;
        enter.action.canceled -= OnEnter;
        del.action.started -= OnDel;
    }
    private void OnDel(InputAction.CallbackContext obj)
    {
        StartCoroutine(DelChar());
    }

    private IEnumerator DelChar()
    {
        if (del.action.IsPressed())
        {
            if (cursor > 0)
            { 
                cursor--;
                currentText = currentText.Remove(cursor, 1);
                Debug.Log(currentText);
            }
            yield return new WaitForSeconds(0.1f);
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
        if(!Char.IsControl(ch))
        { 
            currentText += ch;
            cursor++;
            Debug.Log(currentText);
        }
    }

}

