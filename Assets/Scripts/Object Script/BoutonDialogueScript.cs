using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoutonDialogueScript : MonoBehaviour
{
    [SerializeField]
    ChefDorchestre chef;

    [SerializeField]
    DialogSystem dialogSystem;

    [SerializeField]
    TextAsset dialogTest;

    
    private void coucou()
    {
        Debug.Log("coucou1");
    }

    private void coucou2()
    {
        Debug.Log("coucou2");
    }
    
    public void Interact()
    {
        //new System.Action[] { coucou, coucou2 }
        chef.SwitchSystem(ChefDorchestre.GameSystem.Dialogue);
        dialogSystem.StartDialogue(dialogTest, new System.Action[] { coucou, coucou2 });
    }

}
