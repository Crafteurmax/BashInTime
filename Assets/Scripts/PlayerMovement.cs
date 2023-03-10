using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private int propOfNoMoveScreen = 33;
    [SerializeField] private float caracterSpeed = 7;
    [SerializeField] private float roomWidth = 20;
    private float screenWidth;
    private float dmax;

    // Start is called before the first frame update
    void Start()
    {
        // A priori la taille de l'écant ne change pas pendant la session
        screenWidth = 2 * Camera.main.orthographicSize * Screen.width / Screen.height;
        dmax = screenWidth / 2 * propOfNoMoveScreen / 100;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;
        //Debug.Log("La position du sprite est : " + posX);
        //Debug.Log("La largeur de l'écran est : " + screenWidth);

        // Calcule le déplacmeent gauche/droite de l'objet auquel ce scripte est attaché
        position.x += playerInput.actions["Move"].ReadValue<float>() * caracterSpeed * Time.deltaTime;
        // On ne dépasse pas les bords de la pièce ...
        position.x = ecrete(position.x, roomWidth / 2);
        transform.position = position;

        // Calcule la position de la camera et la met à jour en consequence
        Vector3 posCamGirl = mainCamera.transform.position;
        if (position.x > posCamGirl.x + dmax)
            posCamGirl.x = position.x - dmax;
        if (position.x < posCamGirl.x - dmax)
            posCamGirl.x = position.x + dmax;
        mainCamera.transform.position = posCamGirl;
        //Debug.Log("posX " + position.x + " | taille ecran " + screenWidth + " | max écartement " + screenWidth/2 * propOfNoMoveScreen/100 ); 
        //Debug.Log("la pos de la camera est : " + posCamGirl);

    }

    float seuil (float value, float minValue)
    {
        /*
         * Applique un seuil à value
         */

        if (Mathf.Abs(value) <= minValue)
            value = 0;
        return value;
    }


    float ecrete (float value, float maxVal)
    {
        /*
         * Ecrète les valeur à hauteur de maxVal et conserve le signe de value
         * Applique la fonction Signe(value) * Min(abs(value), maxVal)
         */

        value = Mathf.Max(-maxVal, value);
        value = Mathf.Min(maxVal, value);


        return value;
    }
}
