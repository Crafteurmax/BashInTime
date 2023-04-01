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

    [SerializeField] private SpriteRenderer ChellSprite;

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
        float deplacement = playerInput.actions["Move"].ReadValue<float>() * caracterSpeed * Time.deltaTime;

        if (deplacement == 0) return;

        // Calcule le déplacmeent gauche/droite de l'objet auquel ce scripte est attaché
        position.x += deplacement;
        ChellSprite.flipX = deplacement > 0;
        // On ne dépasse pas les bords de la pièce ...
        position.x = Mathf.Clamp(position.x, -roomWidth / 2, roomWidth / 2);
        transform.position = position;

        // Calcule la position de la camera et la met à jour en consequence
        Vector3 posCamGirl = mainCamera.transform.position;
        if (position.x > posCamGirl.x + dmax) posCamGirl.x = position.x - dmax;
        if (position.x < posCamGirl.x - dmax) posCamGirl.x = position.x + dmax;
        mainCamera.transform.position = posCamGirl;

    }
}
