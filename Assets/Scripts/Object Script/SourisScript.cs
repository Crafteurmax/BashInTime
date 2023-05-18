using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourisScript : MonoBehaviour
{
    [SerializeField]
    ChefDorchestre chef;

    [SerializeField]
    DialogSystem dialogSystem;

    [SerializeField]
    TextAsset dialog1;

    [SerializeField]
    TextAsset dialog2;

    [SerializeField]
    ConditionsManager conditionsManager;

    [SerializeField]
    GameObject cheese;

    private bool isAlive = true;
    public void Interact()
    {
        if (isAlive)
        {
            transform.GetComponent<Souris>().isMoving = false;
            chef.SwitchSystem(ChefDorchestre.GameSystem.Dialogue);
            if (conditionsManager.GetConditionState("gotCheese"))
            {
                dialogSystem.StartDialogue(dialog1, new System.Action[] { giveCheese, killMouse , letLive});
            }
            else
            {
                dialogSystem.StartDialogue(dialog2, new System.Action[] { giveCheese, killMouse , letLive });
            }
        }


    }

    private void giveCheese()
    {
        PalaisMental.current.AddMemory(11);
        conditionsManager.SetCondition("gotCheese", false);
        conditionsManager.SetCondition("doorIsUnlock", true);
        cheese.SetActive(false);
        transform.GetComponent<Souris>().isMoving = true;
    }
    private void letLive()
    {
        transform.GetComponent<Souris>().isMoving = true;
    }

    private void killMouse()
    {
        transform.GetComponent<SpriteRenderer>().flipY = true;
        transform.GetComponent<Souris>().isMoving = false;
        isAlive = false;
    }
}
