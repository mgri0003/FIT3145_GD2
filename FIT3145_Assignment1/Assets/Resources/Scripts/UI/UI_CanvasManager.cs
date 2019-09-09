using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CanvasManager : MonoBehaviour
{
    [SerializeField] float m_planeDistance = 0.3f;
    private Canvas m_canvas;
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
}
