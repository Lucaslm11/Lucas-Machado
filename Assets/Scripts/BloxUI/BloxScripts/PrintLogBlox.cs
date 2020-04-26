using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Terminal.Nodes.Types;
using Assets.Scripts.Terminal.Nodes;
using Assets.Scripts.Terminal.Nodes.Functions;

public class PrintLogBlox : ABlox, ICompilableBlox
{
    [SerializeField] OutputPanel LogOutputPanel;
 

    // Update is called once per frame
    void Update()
    {
        
    }


    public override bool ValidateNestToTheSide(GameObject objectToNest)
    {
        // Only allows nesting a component to the side of this if it is an VariableChoiceBlox
        return GameObjectHelper.HasComponent<VariableChoiceBlox>(objectToNest); //&& BloxParams.Count == 0;
    }

    public List<BloxValidationError> Validate()
    {
        List<BloxValidationError> errors = new List<BloxValidationError>();
        if(BloxParams.Count <= 0)
        {
            errors.Add(new BloxValidationError()
            {
                TargetBlox = this,
                ErrorMessage = BloxValidationErrorMessages.PRINT_BLOX_NO_PARAMS
            });
        }

        errors.AddRange(ValidateBloxList(BloxParams));
        return errors;
    }

    public void ToNodes(ICodeNode parentNode)
    {
        RootNode rootNode = parentNode.GetRootNode();
        List<ObjectNode> variables = BloxParams.Where(p => GameObjectHelper.CanBeCastedAs<VariableChoiceBlox>(p)).Select(p => (p as VariableChoiceBlox).ToObjectNode(rootNode)).ToList();

        OutputMessageNode outputMsgNode = new OutputMessageNode(this.GetComponent<HighlightableButton>(), this.LogOutputPanel.AddMessage, variables);

        parentNode.AddChildNode(outputMsgNode);
    }
}
