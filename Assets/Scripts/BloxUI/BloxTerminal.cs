using Assets.Scripts.Terminal.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// TODO: Add methods for compile, verify erros
///     Create a root node. This root node will be the start of the tree of  bloxes
/// </summary>
public class BloxTerminal : MonoBehaviour
{
    [SerializeField] RootBlox RootBlox;
    [SerializeField] ErrorPopup ErrorPopup;
    private const string ContentComponentName = "Viewport/Content";
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
            collision.gameObject.transform.parent = this.transform.parent;
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
        // Some nodes will have to wait for actions to execute
        // But this function (Play) is called by an event,
        // and that will stop every event execution. So we run the code runner async
        Task play = new Task(() => PlayTasks());
        play.Start();
    }

    public void PlayTasks()
    {
        try
        {
            //First validates
            List<BloxValidationError> validationErrors = RootBlox.Validate();

            //If has errors
            if (validationErrors != null && validationErrors.Count > 0)
            {
                //Puts errors in error popup
                ErrorPopup.LoadErrors(validationErrors);
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

                rootNode.Execute();
            }
        }catch (Exception ex)
        {

        }
    }
}
