using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Classe principale qui gère l'interface utilisateur et les interactions avec les souvenirs
public class PalaisMental : MonoBehaviour
{

    //Représentation d'un souvenir qui contient un nom, une description et une liste d'identifiants de souvenirs précédents et suivants.
    [Serializable]
    public class MemoryDescription
    {
        public int id;
        public string name;
        public string description;
        public int[] precedents;
        public int[] suivants;
    }

    //Liste des souvenirs
    [Serializable]
    public class Memories
    {
        public MemoryDescription[] memories;
    }

    [SerializeField] private TextAsset memoriesData;
    [SerializeField] private string lockedText = "?????";

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private float intervalleY;

    [Header("Memories Interface")]
    [SerializeField] private RectTransform buttonParent;
    [SerializeField] private Vector2 firstButtonPosition;

    [Header("Predecessor Interface")]
    [SerializeField] private GameObject predecessorPanel;
    [SerializeField] private RectTransform predecessorParent;
    [SerializeField] private Vector2 firstPredecessorButtonPosition;

    [Header("Sucessor Interface")]
    [SerializeField] private GameObject successorPanel;
    [SerializeField] private RectTransform successorParent;
    [SerializeField] private Vector2 firstSuccessorButtonPosition;

    [Header("Memories Description")]
    [SerializeField] private GameObject defaultText;
    [SerializeField] private TMPro.TextMeshProUGUI titreText;
    [SerializeField] private TMPro.TextMeshProUGUI descText;

    private int displayedButtonsCount;
    private int displayedPredecessorsButtonsCount;
    private int displayedSuccessorsButtonsCount;


    private bool[] unlockedMemories;
    private Memories memories;

    private Dictionary<string, int> ids;

    //Charge les souvenirs en les analysant depuis un fichier JSON et les stocke dans la variable memories.
    private void Start()
    {
        memories = JsonUtility.FromJson<Memories>(memoriesData.ToString());

        unlockedMemories = new bool[memories.memories.Length];

        InitDictionnary();  
    }

    //remplit un dictionnaire qui associe les noms de souvenirs à leurs identifiants dans la liste memories.memories.
    private void InitDictionnary()
    {
        ids = new Dictionary<string, int>();

        foreach(MemoryDescription mem in memories.memories)
        {
            ids.Add(mem.name, mem.id);
        }
    }

    //Est appelée chaque fois que le script est activé et désactive les panneaux de successeurs et de prédécesseurs et affiche un texte par défaut.
    private void OnEnable()
    {
        defaultText.SetActive(true);
        titreText.text = "";
        descText.text = "";
        predecessorPanel.SetActive(false);
        successorPanel.SetActive(false);
    }

    //Vérifie si le souvenir est affiché dans l'interface utilisateur.
    private bool IsMemoryDisplayed(MemoryDescription memDesc)
    {
        return unlockedMemories[memDesc.id];
    }

    //Ajoute un bouton d'interface utilisateur avec du texte et un événement de clic à un parent transform. Si le paramètre clickable est faux, le bouton est désactivé.
    private void AppendButton(MemoryDescription memDesc, RectTransform parent, Vector2 position, ref int buttonsCount, bool clickable)
    {
        parent.sizeDelta += intervalleY * Vector2.up;

        GameObject buttonObject = Instantiate(buttonPrefab);

        buttonObject.transform.SetParent(parent);

        buttonObject.transform.localPosition = position + (intervalleY * buttonsCount) * Vector2.down;
        buttonsCount++;

        Button button = buttonObject.GetComponent<Button>();
        button.name = memDesc.name;

        TMPro.TextMeshProUGUI textMesh = buttonObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        if (clickable)
        {
            button.onClick.AddListener(delegate { SelectMemory(memDesc.id); }); //Faire appeler la fonction select event avec l'id
            textMesh.text = memDesc.name;
        }
        else
        {
            button.interactable = false;
            textMesh.text = lockedText;
        }
    }

    //Ajoute un bouton pour un souvenir donné à l'interface utilisateur si le souvenir n'a pas encore été ajouté par mesDesc.
    private void AddMemory(MemoryDescription memDesc)
    {
        if (IsMemoryDisplayed(memDesc)) return;

        AppendButton(memDesc, buttonParent, firstButtonPosition, ref displayedButtonsCount, true);
        unlockedMemories[memDesc.id] = true;
    }


    //Les fonctions DisplayPredecessors et DisplaySucessors affichent des boutons pour les souvenirs prédécesseurs et successeurs d'un souvenir donné dans des panneaux dédiés.
    private void DisplayPredecessors(MemoryDescription memDesc)
    {
        predecessorPanel.SetActive(true);

        foreach (Transform child in predecessorParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        displayedPredecessorsButtonsCount = 0;
        predecessorParent.sizeDelta = new Vector2(predecessorParent.sizeDelta.x, firstPredecessorButtonPosition.y);

        foreach (int predecessorId in memDesc.precedents)
        {
            MemoryDescription precDesc = memories.memories[predecessorId];
            AppendButton(precDesc, predecessorParent, firstPredecessorButtonPosition, ref displayedPredecessorsButtonsCount, true);
        }
    }

    private void DisplaySuccessors(MemoryDescription memDesc)
    {
        successorPanel.SetActive(true);

        foreach (Transform child in successorParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        displayedSuccessorsButtonsCount = 0;
        successorParent.sizeDelta = new Vector2(successorParent.sizeDelta.x, firstSuccessorButtonPosition.y);

        foreach (int successorId in memDesc.suivants)
        {
            MemoryDescription succDesc = memories.memories[successorId];
            AppendButton(succDesc, successorParent, firstSuccessorButtonPosition, ref displayedSuccessorsButtonsCount, IsMemoryDisplayed(succDesc));
        }
    }

    //Est appelée lorsqu'un bouton de souvenir est cliqué. Elle affiche le titre et la description du souvenir sélectionné, ainsi que les boutons de prédécesseurs et de successeurs correspondants.
    public void SelectMemory(int id)
    {
        MemoryDescription desc = memories.memories[id];

        if (desc == null) return;

        defaultText.SetActive(false);
        titreText.text = desc.name;
        descText.text = desc.description;

        DisplayPredecessors(memories.memories[id]);
        DisplaySuccessors(memories.memories[id]);
    }

    //Ajoute un bouton pour un souvenir donné à l'interface utilisateur si le souvenir n'a pas encore été ajouté par l'id.
    public void AddMemory(int id)
    {
        AddMemory(memories.memories[id]);
    }

    //Ajoute un bouton pour un souvenir donné à l'interface utilisateur si le souvenir n'a pas encore été ajouté par le nom.
    public void AddMemory(string name)
    {
        AddMemory(ids[name]);
    }


    //DEBUG
    /*IEnumerator DebugTest()
    {

        for (int i = 0; i<4; i++)
        {
            yield return new WaitForSeconds(5);


            AddMemory("MemoryTest" + i);
        }
       
    }*/

    
}
