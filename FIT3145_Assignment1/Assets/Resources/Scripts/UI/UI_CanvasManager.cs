using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CanvasManager : MonoBehaviour
{
    [SerializeField] float m_planeDistance = 0.3f;
    private static Canvas m_canvas;
    private Camera m_camera;

    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }

    public void Init()
    {
        if(m_canvas == null)
        {
            m_canvas = GetComponentInChildren<Canvas>();
        }
        Debug.Assert(m_canvas, "Canvas not found!?!?");

        if (m_canvas)
        {
            foreach (Camera cam in Camera.allCameras)
            {
                if (cam.name == "Main Camera" || cam.name == "ThirdPersonCamera")
                {
                    m_camera = cam;
                }
            }

            Debug.Assert(m_camera, "Failed To Find Camera");
            if (m_camera)
            {
                m_canvas.worldCamera = m_camera;
                m_canvas.planeDistance = m_planeDistance;
            }
        }
    }

    public static Canvas GetCanvas() { return m_canvas; }

    public static Vector2 ConvertCanvasLocalPositionToScreenPosition(Vector2 localPos)
    {
        localPos.x /= m_canvas.GetComponent<RectTransform>().sizeDelta.x;
        localPos.y /= m_canvas.GetComponent<RectTransform>().sizeDelta.y;

        localPos.x *= m_canvas.pixelRect.width;
        localPos.y *= m_canvas.pixelRect.height;

        return localPos;
    }

    public static Vector2 ConvertScreenPositionToCanvasLocalPosition(Vector2 screenPos)
    {
        screenPos.x /= m_canvas.pixelRect.width;
        screenPos.y /= m_canvas.pixelRect.height;

        screenPos.x *= m_canvas.GetComponent<RectTransform>().sizeDelta.x;
        screenPos.y *= m_canvas.GetComponent<RectTransform>().sizeDelta.y;

        return screenPos;
    }

    public static Vector2 GetMousePositionFromScreenCentre()
    {
        Rect canvasSize = m_canvas.pixelRect;
        return Input.mousePosition - new Vector3(canvasSize.width / 2, canvasSize.height / 2, 0.0f);
    }

    public static Vector2 GetMousePositionPercent()
    {
        Vector2 mousePosPercent = Input.mousePosition;
        mousePosPercent.x /= Screen.width;
        mousePosPercent.y /= Screen.height;
        return mousePosPercent;
    }

    public static Vector2 GetMousePositionPercentFromScreenCentre()
    {
        Vector2 mousePosPercent = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, 0.0f);
        mousePosPercent.x /= Screen.width;
        mousePosPercent.y /= Screen.height;
        return mousePosPercent;
    }

    public static Vector3 MousePositionPercentToScreenPixel(Vector2 mousePosPercent)
    {
        mousePosPercent.x *= Screen.width;
        mousePosPercent.y *= Screen.height;
        return mousePosPercent;
    }

    public static Vector3 MousePositionPercentToCanvasPixel(Vector2 mousePosPercent)
    {
        mousePosPercent.x *= m_canvas.pixelRect.width;
        mousePosPercent.y *= m_canvas.pixelRect.height;
        return mousePosPercent;
    }

    /// <summary>
    /// This only works against rect Transforms whose parent is the Canvas/RootUIScreen
    /// </summary>
    /// <param name="rectTrans"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static bool IsPointInsideRect(in RectTransform rectTrans, in Vector2 pos)
    {
        //Reference : https://stackoverflow.com/questions/40566250/unity-recttransform-contains-point?rq=1

        // Get the rectangular bounding box of your UI element
        Rect rect = rectTrans.rect;

        //convert position to bottom left being 0,0
        // canvasSize = m_canvas.pixelRect;
        //pos -= new Vector2(canvasSize.width / 2, canvasSize.height / 2);

        // Get the left, right, top, and bottom boundaries of the rect
        float leftSide = rectTrans.anchoredPosition.x - rect.width / 2;
        float rightSide = rectTrans.anchoredPosition.x + rect.width / 2;
        float topSide = rectTrans.anchoredPosition.y + rect.height / 2;
        float bottomSide = rectTrans.anchoredPosition.y - rect.height / 2;

        //Debug.Log(leftSide + ", " + rightSide + ", " + topSide + ", " + bottomSide + " | " + pos.x + ", " + pos.y);

        //Check to see if the point is in the calculated bounds
        if (pos.x >= leftSide && pos.x <= rightSide &&
            pos.y >= bottomSide && pos.y <= topSide)
        {
            return true;
        }

        return false;
    }
}
