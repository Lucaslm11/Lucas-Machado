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

    
    }

    // Update is called once per frame
    void Update()
    {

    }
}
