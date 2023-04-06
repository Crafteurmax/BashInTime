using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostItScript : MonoBehaviour
{
    [SerializeField]
    ChefDorchestre chef;

    [SerializeField]
    DialogSystem dialogSystem;

    [SerializeField]
    TextAsset dialog;
    
    public void Interact()
    {
        //new System.Action[] { coucou, coucou2 }
        chef.SwitchSystem(ChefDorchestre.GameSystem.Dialogue);
        dialogSystem.StartDialogue(dialog, null);
        chef.palais.AddMemory(0); //temp
    }

}
