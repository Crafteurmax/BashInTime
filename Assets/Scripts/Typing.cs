using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

// lib en fonction de l'OS
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using System.Windows.Forms;
#endif

public class Typing : MonoBehaviour
{
    //################################ variable en commun ################################ 
    private float delayTouche = 0.07f;


    //################################ input du texte ################################  
    [SerializeField]
    private InputActionReference enter;
    [SerializeField]
    private InputActionReference del;
    [SerializeField]
    private InputActionReference moveCursor;
    private int releasedMoveCursorCount;
    [SerializeField]
    private InputActionReference commandHistory;
    private int releasedCommandHistoryCount;

    // emplacement du curseur dans le texte
    private int cursor;

    // texte qui est actuellement entrain d'etre ecrit
    private String currentText = "";

    // historique des commandes
    private List<String> commandHistoryList = new List<string>();

    // emplacement dans l'historique, -1 si c'est la commande actuel
    private int historyPlacement = -1;

    private bool isCommandRunning;

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
            //DisplayText(currentText);
        }

        yield return new WaitForSeconds(delayTouche);

        if (del.action.IsPressed())
        {
            StartCoroutine(DelChar());
        }
    }

    private void OnEnter(InputAction.CallbackContext obj)
    {
        if (!currentText.Trim().Equals("") && !isCommandRunning) Enter();
        else addCommandToFixText("");
    }

    private void Enter()
    {
        addCommandToFixText(currentText);
        commandHistoryList.Add(currentText);
        isCommandRunning = true;
        parser.ExecuteCommand(currentText);
        currentText = "";
        cursor = 0;
        historyPlacement = -1;
    }

    public void PrintOutput(String output)
    {
        fixText += output + "\n";
        //DisplayText(currentText);
        textComponent.text = fixText;
        updateScroll();
        isCommandRunning = false;
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
            //DisplayText(currentText);
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
            //DisplayText(currentText);
        }

        yield return new WaitForSeconds(delayTouche);
        if(releasedMoveCursorCount > 0)releasedMoveCursorCount--;
        else if (moveCursor.action.IsPressed())
        {
            StartCoroutine(MoveCursor(direction));
        }
    }

    // simple outils de debug, permet de representer le curseur au bon endroit
    [UnityEngine.ContextMenu("je dit 'Splay Text'")]
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

    // on lance le debut du changement d'historique
    private void OnCommandHistory(InputAction.CallbackContext obj)
    {
        StartCoroutine(CommandHistory(obj.ReadValue<float>()));
    }

    // si la touche est encore appyé au bout de x temps on re change l'historique
    private IEnumerator CommandHistory(float direction)
    {
        if (commandHistoryList.Count == 0) { }
        else if (historyPlacement == -1 )
        {
            if( direction < 0)
            {
                historyPlacement = commandHistoryList.Count - 1;
                currentText = (String)commandHistoryList[historyPlacement].Clone();
                cursor = currentText.Length;
            }
        }
        else if (!((historyPlacement == commandHistoryList.Count - 1 && direction > 0) || (historyPlacement == 0 && direction < 0))) // si on essaye pas de deplacer le curseur en dehors du texte
        {
            historyPlacement += (int)direction;
            currentText = (String)commandHistoryList[historyPlacement].Clone();
            cursor = currentText.Length;
            //DisplayText(currentText);
        }

        yield return new WaitForSeconds(delayTouche);

        if (releasedCommandHistoryCount > 0) releasedCommandHistoryCount--;
        else if (commandHistory.action.IsPressed())
        {
            StartCoroutine(CommandHistory(direction));
        }
    }

    //################################ affichage du texte ################################  

    [SerializeField]
    private TextMeshProUGUI textComponent;

    [SerializeField]
    private InputActionReference scroll;
    private int releasedScrollCount;

    String fixText = "<color=green>this is the default text and I'm supposed to be green \n"+
                     "if it's not the case, I allow you to scream because if ice cream, you scream \n";

    float textScrollOffset = 100;

    [UnityEngine.ContextMenu("refresh screen")]
    private void refreshScreen()
    {
        textComponent.text = fixText + formatText(currentText);
    }

    private String formatText(String textToFormat)
    {
        String outputText = parser.currentDirectory + " > " ;
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
        fixText += parser.currentDirectory+" > " + currentText + "\n";
    }

    float scrollSpeed = 100.0f;
    private void OnScroll(InputAction.CallbackContext obj)
    {
        if(!NumLockState()) StartCoroutine(Scroll(obj.ReadValue<float>()));
    }

    private IEnumerator Scroll(float direction)
    {
        textComponent.rectTransform.SetPositionAndRotation(
            textComponent.rectTransform.position + new Vector3(0,direction * scrollSpeed * Time.deltaTime,0),
            textComponent.rectTransform.rotation);

        //Debug.Log(NumLockState());

        if (textComponent.rectTransform.position.y < 0)
        {
            textComponent.rectTransform.SetPositionAndRotation(
            new Vector3(0, 0,0),
            textComponent.rectTransform.rotation);
        }
        yield return new WaitForSeconds(delayTouche);

        if (releasedScrollCount > 0 && !NumLockState()) releasedScrollCount--;
        else if (scroll.action.IsPressed() && !NumLockState())
        {
            StartCoroutine(Scroll(direction));
        }
    }

    private bool NumLockState()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        return Control.IsKeyLocked(Keys.NumLock);
#endif
            
#pragma warning disable CS0162 // Unreachable code detected
        return true;
#pragma warning restore CS0162 // Unreachable code detected
    }

    private void updateScroll()
    {
        float canvas_h = textComponent.transform.parent.GetComponent<RectTransform>().rect.height;
        float text_h = textComponent.GetPreferredValues().y;
        //Debug.Log(canvas_h + " - "+ text_h+ " = " + (text_h - canvas_h));
        textComponent.rectTransform.localPosition =
            new Vector3(0, Math.Max(0,text_h - (canvas_h - textScrollOffset)), 0);

    }

    //################################ fonction de unity ################################  
    //Lien avec l'executeur de commandes
    private CommandParser parser;
    private void Awake()
    {
        parser = GetComponent<CommandParser>();
    }
    void Start()
    {
        textComponent.SetText(fixText);
    }

    void Update()
    {
        if(!isCommandRunning) refreshScreen();
    }

    protected void OnEnable()
    {
        // initialisation des events lors de l'activation
        Keyboard.current.onTextInput += OnTextInput;
        enter.action.canceled += OnEnter;
        del.action.started += OnDel;
        moveCursor.action.started += OnMoveCursor;
        moveCursor.action.canceled += _ => { releasedMoveCursorCount++; };
        scroll.action.started += OnScroll;
        scroll.action.canceled += _ => { if(!NumLockState())releasedScrollCount++; };
        commandHistory.action.started += OnCommandHistory;
        commandHistory.action.canceled += _ => { releasedCommandHistoryCount++; };
    }

    protected void OnDisable()
    {
        // désinitialisation des events si on désactive 
        Keyboard.current.onTextInput -= OnTextInput;
        enter.action.canceled -= OnEnter;
        del.action.started -= OnDel;
        moveCursor.action.started -= OnMoveCursor;
        scroll.action.started -= OnScroll;
        commandHistory.action.started -= OnCommandHistory;
    }
}

