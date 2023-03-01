using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileSelector : MonoBehaviour
{
    [SerializeField] private Material defaultOutline;
    [SerializeField] private Material okOutline;
    [SerializeField] private Material notOkOutline;
    [SerializeField] private Material selectedOkOutline;
    [SerializeField] private Material selectedNotOkOutline;


    [SerializeField] public MapBase map;


    private Vector2 _selectedPos;
    private CaseInfo _caseInfo;

    public Action pendingAction;

    private PlayerInput playerInput;
    private RoundManager roundManager;

    public MapBase.StructureType planteID;

    public Vector2 selectedPos
    {
        get
        {
            return _selectedPos;
        }
        private set
        {
            _selectedPos = value;
        }
    }


    private void Awake()
    {
        playerInput = GameObject.Find("MovingCamera").GetComponent<PlayerInput>();
        roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>();
    }

    private void Start()
    {
        planteID = MapBase.StructureType.Tournesol;
    }

    void Update()
    {

        if (pendingAction == null) return;

        if (_caseInfo != null)
        {
            if (map.structures[_caseInfo.casePos.x, _caseInfo.casePos.y] == null)
                _caseInfo.SetOutline(_caseInfo.IsCaseUsable(planteID, pendingAction.actionType) ? okOutline : notOkOutline);
        }



        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100))
        {
            _caseInfo = hit.collider.gameObject.GetComponentInParent<CaseInfo>();
            _selectedPos = _caseInfo.casePos;


            if (map.structures[_caseInfo.casePos.x, _caseInfo.casePos.y] == null)
                _caseInfo.SetOutline(_caseInfo.IsCaseUsable(planteID, pendingAction.actionType) ? selectedOkOutline : selectedNotOkOutline);

            pendingAction.actionTile = _caseInfo;

            if (_caseInfo.IsCaseUsable(planteID, pendingAction.actionType))
            {
                pendingAction.Previsualize(map);
                if (playerInput.actions["Click"].ReadValue<float>() >= 0.5f)
                {

                    roundManager.currentPlayer.SetAction(pendingAction);
                    pendingAction = null;
                    ClearTileSelection();
                    _caseInfo = null;
                }
            }
            else
            {
                map.ResetTempStructure();
            }
        }
        else
        {
            _caseInfo = null;
            _selectedPos = Vector2.left + Vector2.down; //-1,-1 = no case selected;
        }
    }

    public void ClearTileSelection()
    {

        List<CaseInfo> possible = new List<CaseInfo>();
        List<CaseInfo> notPossible = new List<CaseInfo>();

        map.GetPossibleCases(possible, notPossible, planteID, Action.ActionType.Pass);

        for (int i = 0; i < map.width; i++)
        {
            for (int j = 0; j < map.height; j++)
            {
                CaseInfo tile = map.tilesInfos[i, j];
                if (map.structures[tile.casePos.x, tile.casePos.y] == null)
                {
                    tile.SetOutline(defaultOutline);
                }
                else
                {
                    tile.SetOutline(map.structures[tile.casePos.x, tile.casePos.y].player == map.roundManager.players[0] ? map.blueTeamOutline : map.redTeamOutline);
                }
            }
        }
    }

    public void BeginPlantAction()
    {
        ClearTileSelection();

        map.ResetTempStructure();
        pendingAction = null;

        switch (planteID)
        {
            case MapBase.StructureType.Cactus:
                pendingAction = new Action(Action.ActionType.PlantCactus);
                break;
            case MapBase.StructureType.Carnivore:
                pendingAction = new Action(Action.ActionType.PlantCarnivore);
                break;
            case MapBase.StructureType.Lierre:
                pendingAction = new Action(Action.ActionType.PlantLierre);
                break;
            case MapBase.StructureType.Pousse:
                pendingAction = new Action(Action.ActionType.PlantPousse);
                break;
            case MapBase.StructureType.Tournesol:
                pendingAction = new Action(Action.ActionType.PlantTournesol);
                break;
        }


        List<CaseInfo> possible = new List<CaseInfo>();
        List<CaseInfo> notPossible = new List<CaseInfo>();

        map.GetPossibleCases(possible, notPossible, planteID, pendingAction.actionType);

        foreach (CaseInfo tile in possible)
        {
            tile.SetOutline(okOutline);
        }

        foreach (CaseInfo tile in notPossible)
        {
            //tile.SetOutline(notOkOutline);
        }
    }

    public void BeginAttackAction()
    {

        ClearTileSelection();
        map.ResetTempStructure();
        pendingAction = new Action(Action.ActionType.Attack);

        List<CaseInfo> possible = new List<CaseInfo>();
        List<CaseInfo> notPossible = new List<CaseInfo>();

        map.GetPossibleCases(possible, notPossible, planteID, Action.ActionType.Attack);

        foreach (CaseInfo tile in possible)
        {
            tile.SetOutline(okOutline);
        }

        foreach (CaseInfo tile in notPossible)
        {
            //tile.SetOutline(notOkOutline);
        }
    }

    public void BeginArroserAction()
    {

        ClearTileSelection();
        map.ResetTempStructure();
        pendingAction = new Action(Action.ActionType.Arroser);

        List<CaseInfo> possible = new List<CaseInfo>();
        List<CaseInfo> notPossible = new List<CaseInfo>();

        map.GetPossibleCases(possible, notPossible, planteID, Action.ActionType.Arroser);

        foreach (CaseInfo tile in possible)
        {
            tile.SetOutline(okOutline);
        }

        foreach (CaseInfo tile in notPossible)
        {
            //tile.SetOutline(notOkOutline);
        }
    }

    public void changeSelectedPlant(MapBase.StructureType newPlantID)
    {
        this.planteID = newPlantID;
        pendingAction = null;
        map.ResetTempStructure();
        ClearTileSelection();

    }

    public MapBase.StructureType getPlantID()
    {
        return planteID;
    }
}
