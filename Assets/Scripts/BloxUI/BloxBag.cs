using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameObjectHelper;

public class BloxBag : MonoBehaviour
{
    [SerializeField] RectTransform Content;
    [SerializeField] RectTransform Pivot;
    [SerializeField] float margin = 5f;
    //[SerializeField] RectTransform ContentViewport;
    [SerializeField] List<ABlox> AvailableBloxes;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 pivotPos = Pivot.position;
        int col = 0;
        int row = 0;

        ABlox previousBlox = null;
        foreach (ABlox blox in AvailableBloxes)
        {
            blox.transform.SetParent(Content);
            float previousBloxWidth = 0;
            if (previousBlox == null)
            {
                previousBlox = blox;
            }
            else
            {
                BoundingBox2D pbBBox = GameObjectHelper.getBoundingBoxInWorld(previousBlox.gameObject);
                previousBloxWidth = GameObjectHelper.getWidthFromBBox(pbBBox);
            }

            BoundingBox2D bloxBBox = GameObjectHelper.getBoundingBoxInWorld(blox.gameObject);
            float bloxWidth = GameObjectHelper.getWidthFromBBox(bloxBBox);
            float bloxHeight = GameObjectHelper.getHeightFromBBox(bloxBBox);
            Vector3 newPos = pivotPos;
            newPos.x += col * (previousBloxWidth + margin) + bloxWidth / 2;
            newPos.y -= row * (bloxHeight + margin);

            blox.transform.position = newPos;

            if (col > 0)
            {
                row++;
                col = 0;
            }
            else
            {
                col++;
            }
        }

        /*BoundingBox2D contentBBox = GameObjectHelper.getBoundingBoxInWorld(ContentViewport.gameObject);
        float contentHeight = GameObjectHelper.getHeightFromBBox(contentBBox);
        float contentWidth = GameObjectHelper.getWidthFromBBox(contentBBox);
        Vector2 contentCenter = ContentViewport.position;
        Vector2 pivotPoint = contentCenter;
        pivotPoint.x += contentWidth  - margin;
        pivotPoint.y += contentHeight / 2 - margin;
        ABlox previousBlox = null;

        int col = 0;
        int row = 0;
        foreach (ABlox blox in AvailableBloxes)
        {
            blox.transform.SetParent(this.transform.parent.parent); // tempcontainer then canvas
            float previousBloxHeight = 0;
            float previousBloxWidth = 0;
            if (previousBlox != null)
            {
                BoundingBox2D previousBloxBBox = GameObjectHelper.getBoundingBoxInWorld(blox.gameObject);
                previousBloxHeight = GameObjectHelper.getHeightFromBBox(previousBloxBBox);
                previousBloxWidth = GameObjectHelper.getWidthFromBBox(previousBloxBBox);
            }
            else
                previousBlox = blox;

            //blox.gameObject.transform.SetParent(Content);
            BoundingBox2D bloxBBox = GameObjectHelper.getBoundingBoxInWorld(blox.gameObject);
            float bloxHeight = GameObjectHelper.getHeightFromBBox(bloxBBox);
            float bloxWidth = GameObjectHelper.getWidthFromBBox(bloxBBox);

            Vector2 bloxPosition = blox.transform.position;

            Vector2 bloxNewPosition = pivotPoint;
            bloxNewPosition.x += col * margin + bloxWidth / 2 + col*previousBloxWidth;
            bloxNewPosition.y -= row * (margin + bloxHeight) + bloxHeight / 2;

            blox.transform.position = bloxNewPosition;

            blox.transform.SetParent(Content);

            if(col > 0)
            {
                row++;
                col = 0;
            }
            else
            {
                col++;
            } 

        }*/
    }

    // Update is called once per frame
    void Update()
    {

    }
}
