using Assets.Scripts.Terminal.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// TODO: Add methods for compile, verify erros
///     Create a root node. This root node will be the start of the tree of  bloxes
/// </summary>
public class BloxTerminal : MonoBehaviour
{
    [SerializeField] LevelHandler LevelHandler;
    [SerializeField] RootBlox RootBlox;
    [SerializeField] ErrorPopup ErrorPopup;
    [SerializeField] LevelFailurePanel FailurePanel;
    [SerializeField] LevelSuccessPanel SuccessPanel;

    private bool evaluationUpdated = false;
    private bool triggerErrorPopupDisplay = false;
    private LevelHandler.Evaluation evaluation;

    private const string ContentComponentName = "Viewport/Content";
    // Start is called before the first frame update
    void Start()
    {
        FailurePanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (evaluationUpdated)
        {
            if (!evaluation.Success)
           {

                FailurePanel.SetFailure(evaluation);
                FailurePanel.gameObject.SetActive(true);
                //evaluation.ObjectiveCheck.
            }
            else
            {
                SuccessPanel.SetSuccess(evaluation);
                SuccessPanel.gameObject.SetActive(true);
            }

            // Resets the level
            this.LevelHandler.ResetTilePlotState();
            this.LevelHandler.SetCharacter();

            evaluationUpdated = false;
        }
        if (triggerErrorPopupDisplay)
        {
            // Since error popup is disabled by default, the view triggering has to be handled here
            ErrorPopup.SetDisplayState(true);
            triggerErrorPopupDisplay = false;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collidedObject = collision.gameObject;
        bool isBloxAndIsBeingDragged = IsBloxAndIsBeingDragged(collidedObject);
        if (isBloxAndIsBeingDragged)
        {
            Transform contentTransform = this.transform.Find(ContentComponentName);
            collidedObject.transform.SetParent(contentTransform);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject collidedObject = collision.gameObject;
        bool isBloxAndIsBeingDragged = IsBloxAndIsBeingDragged(collidedObject);

        //if it is a blox and is child in content, put it on parent
        if (isBloxAndIsBeingDragged)
            collision.gameObject.transform.SetParent(this.transform.parent);
    }


    private bool IsBloxAndIsBeingDragged(GameObject gameObject)
    {
        bool validation = false;
        bool isBlox = GameObjectHelper.HasComponent<ABlox>(gameObject);
        if (isBlox)
        {
            ABlox collidedBlox = gameObject.GetComponent<ABlox>();
            bool isBeingDragged = collidedBlox.IsBeingDragged;
            validation = isBlox && isBeingDragged;
        }
        return validation;
    }


    public void Play()
    {
        //First validates
        List<BloxValidationError> validationErrors = RootBlox.Validate();

        //If has errors
        if (validationErrors != null && validationErrors.Count > 0)
        {
            //Puts errors in error popup
            ErrorPopup.LoadErrors(validationErrors);
            triggerErrorPopupDisplay = true;
        }
        else
        {
            ErrorPopup.CleanErrors();
            // Root blox does not have a parent, so Root node also does not
            RootBlox.ToNodes(null); // Compiles the bloxes bellow root to nodes
            RootNode rootNode = RootBlox.rootNode;

            // TODO: Pode haver situações de erro.
            //      1º Definir erros esperados (por exemplo, o boneco cai ao chão? Lança uma exception própria)
            //      3º Proteger isto contra erros como divisão por zero
            //      2º Erros genéricos (bugs): loggar


            // Some nodes will have to wait for actions to execute
            // But this function (Play) is called by an event,
            // and that will stop every event execution. So we run the code runner async
            Task play = new Task(() => ExecuteTask(rootNode));
            play.Start();
            

        }

    }

    public void ExecuteTask(RootNode rootNode)
    {
        try
        {
            this.LevelHandler.incrementAttempts();
            rootNode.Execute();

            evaluation = this.LevelHandler.EvaluateLevel(RootBlox);

            // We are going to do the rest of the evaluation logic in the update method
            // because this method is being called async, and Monobehaviour methods
            // can only be called on the main thread
            evaluationUpdated = true;

        }catch (CodeBloxException cbEx)
        {
            BloxValidationError error;
            error.ErrorMessage = cbEx.Message;
            error.TargetBlox = cbEx.blox;
            ErrorPopup.LoadErrors(new List<BloxValidationError> {error });
            triggerErrorPopupDisplay = true;
            //ErrorPopup.SetDisplayState(true);
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// This does not count bloxes but lines of "code". We are not counting the params and the rootblox here!
    /// </summary>
    public int CountCodeLines()
    {
        return this.RootBlox.GetChildBloxListInVerticalOrder().Count;
    }
}
