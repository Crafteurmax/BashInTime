using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{


    private CodeManager codeScripte;

    // Start is called before the first frame update
    void Start()
    {
        // Récupère le scripte du parent
        codeScripte = transform.parent.GetComponent<CodeManager>();

        // Ajouter des événements de clic aux boutons
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(delegate { codeScripte.AddFigure(button.gameObject.name); });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
