using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriseScript : MonoBehaviour
{
    [SerializeField] private GameObject etincelleSound;

    [SerializeField] private GameObject etincellesParticle;

    [SerializeField] private BoutonOrdiScript ordiScript;

    [SerializeField] private GameObject cableImage;

    public void Interact()
    {
        etincelleSound.SetActive(false);
        etincellesParticle.SetActive(false);
        ordiScript.isPoweredOn = true;
        cableImage.SetActive(true);

        GetComponent<SelectObject>().enabled = false;
    }
}
