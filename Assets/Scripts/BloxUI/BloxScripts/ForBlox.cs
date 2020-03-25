using Assets.Scripts.Terminal.Nodes;
using Assets.Scripts.Terminal.Nodes.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForBlox : ABlox, IBloxVariable, ICompilableBlox
{
    [SerializeField] InputField CounterNameField;
    [SerializeField] InputField FromField;
    [SerializeField] InputField ToField;

    public string GetName()
    {
        return CounterNameField.text;
    }

    /// <summary>
    /// Only returns the starting value
    /// </summary>
    /// <returns></returns>
    public string GetValue()
    {
        return FromField.text;
    }

    VariableType IBloxVariable.GetType()
    {
        return VariableType.INT;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override bool ValidateNestToBottomIdented(GameObject objectToNest)
    {
        return true;
    }

    #region ICompilable region
    public List<BloxValidationError> Validate()
    {
        throw new System.NotImplementedException();
    }

    public void ToNodes(ICodeNode parentNode)
    {
        //Creates an integer node for the counter
        IntegerNode counter = new IntegerNode(int.Parse(GetValue()));
        counter.NodeName = this.GetName();
        //Adds it to parent
        parentNode.AddChildNode(counter);

        //Creates the for node
        ForNode forNode = new ForNode(counter, int.Parse(FromField.text), int.Parse(ToField.text));
        parentNode.AddChildNode(forNode);

        foreach (ABlox child in ChildBloxes)
        {
            if (GameObjectHelper.CanBeCastedAs<ICompilableBlox>(child))
            {
                //Creates nodes for each child. Each child will add the forNode as a parent
                ((ICompilableBlox)child).ToNodes(forNode);
            }
        }
    }
    #endregion
}
