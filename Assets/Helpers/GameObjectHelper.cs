using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;

public class GameObjectHelper
{
    /// <summary>
    /// Saves the centers of each boundary
    /// </summary>
    public struct BoundingBox2D
    {
        public Vector2 top;
        public Vector2 bottom;
        public Vector2 left;
        public Vector2 right;
    }


    /// <summary>
    /// Gets the bounding box of a game object. 
    /// We are assuming it has a RectTransform
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static BoundingBox2D getBoundingBox(GameObject gameObject)
    {
        BoundingBox2D boundingBox2D;
        Vector2 position = gameObject.transform.localPosition;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        Vector2 top = position;
        Vector2 bottom = position;
        Vector2 left = position;
        Vector2 right = position;

        top.y += height / 2;
        bottom.y -= height / 2;
        left.x -= width / 2;
        right.x += width / 2;

        boundingBox2D.top = top;
        boundingBox2D.bottom = bottom;
        boundingBox2D.left = left;
        boundingBox2D.right = right;
        
        return boundingBox2D;
    }


    /// <summary>
    /// Gets the bounding box of a game object in GUI coordinates. 
    /// We are assuming it has a RectTransform, and that the original scale of the objects is 1
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static BoundingBox2D getBoundingBoxInWorld(GameObject gameObject)
    {
        BoundingBox2D boundingBox2D;
        Vector2 position = gameObject.transform.position;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        // This scale varies from screen size to screensize and we need the object real size
        Vector2 lossyScale = rectTransform.localToWorldMatrix.lossyScale; 
        float width = rectTransform.rect.width * lossyScale.x;
        float height = rectTransform.rect.height * lossyScale.y;

        Vector2 top = position;
        Vector2 bottom = position;
        Vector2 left = position;
        Vector2 right = position;

        top.y += height / 2;
        bottom.y -= height / 2;
        left.x -= width / 2;
        right.x += width / 2;

        boundingBox2D.top = top;
        boundingBox2D.bottom = bottom;
        boundingBox2D.left = left;
        boundingBox2D.right = right;

        return boundingBox2D;
    }

    public static float getWidthFromBBox(BoundingBox2D bbox)
    {
        return bbox.right.x - bbox.left.x;
    }

    public static float getHeightFromBBox(BoundingBox2D bbox)
    {
        return bbox.top.y - bbox.bottom.y;
    }

    public static Vector2 point2DFromWorldToGUI(Vector2 point)
    {
        return Camera.main.WorldToScreenPoint(new Vector2(point.x, point.y));
    }

    public static Vector2 point3DFromWorldToGUI(Vector3 point)
    {
        return Camera.main.WorldToScreenPoint(new Vector3(point.x, point.y, point.z));
    }

    public static GameObject GetChildByName(GameObject gameObject, string name)
    {
        GameObject childObject = null;
        if(gameObject.transform.childCount > 0) 
            for(int i = 0; i < gameObject.transform.childCount && childObject == null; i++)
            {
                Transform child = gameObject.transform.GetChild(i);
                if (child.name == name)
                    childObject = child.gameObject;
            }
        return childObject;
    }

    public static string GetDropdownSelectedTextValue(Dropdown dropdown)
    {
        return dropdown.options[dropdown.value].text;
    }


    public static bool HasComponent<T>(GameObject gameObject)
    {
        bool hasComponent = false;
        try
        {
            hasComponent = gameObject.GetComponent<T>() != null;
        }
        catch (Exception){}
        return hasComponent;
    }
}