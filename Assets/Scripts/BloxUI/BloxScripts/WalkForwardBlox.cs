﻿using Assets.Scripts.Terminal.Nodes.Functions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkForwardBlox : ABlox, ICompilableBlox
{
    [SerializeField] CubotController character;

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ToNodes(ICodeNode parentNode)
    {
        ActionExecutorNode node = new ActionExecutorNode(character.MoveForward, IsCharacterExecutingAction);
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


}
