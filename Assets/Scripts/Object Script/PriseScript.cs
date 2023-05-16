using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriseScript : MonoBehaviour
{
    [SerializeField] private GameObject etincelleSound;

    [SerializeField] private GameObject etincellesParticle;

    [SerializeField] private BoutonOrdiScript ordiScript;

    [SerializeField] private Sprite spritePlugged;

    

    public void Interact()
    {
        etincelleSound.SetActive(false);
        etincellesParticle.SetActive(false);
        ordiScript.isPoweredOn = true;
        GetComponent<SpriteRenderer>().sprite = spritePlugged;

        GetComponent<SelectObject>().outlineColor = new Color(0,0,0,0);
    }
}
