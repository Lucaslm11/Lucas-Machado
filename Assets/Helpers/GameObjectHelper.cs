using UnityEngine;
using UnityEditor;

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
    /// We are assuming it has a RectTransform
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    [System.Obsolete]
    public static BoundingBox2D getBoundingBoxInGUI(GameObject gameObject)
    {
        BoundingBox2D boundingBox2D;
        Vector2 position = gameObject.transform.position;
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

        boundingBox2D.top = point2DFromWorldToGUI(top);
        boundingBox2D.bottom = point2DFromWorldToGUI(bottom);
        boundingBox2D.left = point2DFromWorldToGUI(left);
        boundingBox2D.right = point2DFromWorldToGUI(right);

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
}