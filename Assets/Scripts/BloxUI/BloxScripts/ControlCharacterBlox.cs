using Assets.Scripts.Terminal.Nodes.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCharacterBlox : ABlox, ICompilableBlox
{
    Dictionary<CharacterAction, Action> actionMap;
    public enum CharacterAction
    {
        MOVE_FORWARD,
        CLIMB_UP,
        CLIMB_DOWN,
        TURN_RIGHT,
        TURN_LEFT,
        SPECIAL_ACTION
    }

    [SerializeField] CharacterAction bloxAction;
    [SerializeField] CubotController character;

    protected override void OnStart()
    {
        actionMap = new Dictionary<CharacterAction, Action>();
        actionMap[CharacterAction.MOVE_FORWARD] = character.MoveForward;
        actionMap[CharacterAction.CLIMB_UP] = character.ClimbUp;
        actionMap[CharacterAction.CLIMB_DOWN] = character.ClimbDown;
        actionMap[CharacterAction.TURN_RIGHT] = character.TurnRight;
        actionMap[CharacterAction.TURN_LEFT] = character.TurnLeft;
        actionMap[CharacterAction.SPECIAL_ACTION] = SpecialAction;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ToNodes(ICodeNode parentNode)
    {
        HighlightableButton highlightableButton = (GameObjectHelper.HasComponent<HighlightableButton>(this.gameObject)) ? this.GetComponent<HighlightableButton>() : null;
        ActionExecutorNode node = new ActionExecutorNode(highlightableButton, actionMap[bloxAction], IsCharacterExecutingAction);
        parentNode.AddChildNode(node);
    }

    public List<BloxValidationError> Validate()
    {
        return new List<BloxValidationError>();
    }


    private bool IsCharacterExecutingAction()
    {
        return character.ExecutingAction;
    }

    protected virtual void SpecialAction()
    {

    }


}
