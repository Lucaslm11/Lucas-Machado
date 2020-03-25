using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// TODO: Add methods for compile, verify erros
///     Create a root node. This root node will be the start of the tree of  bloxes
/// </summary>
public class BloxTerminal : MonoBehaviour
{
    [SerializeField] ABlox RootBlox;
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

}
