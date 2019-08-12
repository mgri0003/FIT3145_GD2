using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Main : MonoBehaviour
{
    //--pre-declared variables--
    enum ECamera_ViewMode
    {
        LEFT_SIDE,
        RIGHT_SIDE,
        MAX
    };


    //--variables--
    [SerializeField] private Transform m_target = null;
    [SerializeField] private Transform m_cameraHolder = null;
    private float m_currentRotationX;
    private float m_currentRotationY;
    const float m_currentZoom = 2.5f;
    const float m_currentHeight = 1.5f;
    private ECamera_ViewMode m_cameraViewMode = ECamera_ViewMode.RIGHT_SIDE;

    readonly Vector3 CameraView_LeftSide    = new Vector3(-1 , m_currentHeight, -m_currentZoom);
    readonly Vector3 CameraView_RightSide   = new Vector3(1  , m_currentHeight, -m_currentZoom);


    //--methods--

    

    // Start is called before the first frame update
    void Start()
    {
        //error check references!
        Debug.Assert(m_target != null, "Target Is Null");
        Debug.Assert(m_cameraHolder != null, "Camera Holder Is Null");

        //hide da mouse
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        RefreshCameraViewMode();
    }

    // Update is called once per frame
    void Update()
    {
        //Camera Position
        m_cameraHolder.SetPositionAndRotation(m_target.position, m_cameraHolder.rotation);

        //Camera Rotation
        m_currentRotationX = m_target.GetComponent<Player_Core>().GetDesiredRotation();
        m_currentRotationY -= Input.GetAxis("Mouse Y");
        m_currentRotationY = Mathf.Clamp(m_currentRotationY, -15, 60);
        m_cameraHolder.transform.rotation = Quaternion.Euler(m_currentRotationY, m_currentRotationX, 0);

        if (Input.GetMouseButtonDown(2))
        {
            CycleCameraViewMode();
        }
    }


    void CycleCameraViewMode()
    {
        if((int)m_cameraViewMode < (int)ECamera_ViewMode.MAX - 1)
        {
            m_cameraViewMode++;
        }
        else
        {
            m_cameraViewMode = (ECamera_ViewMode)(0);
        }

        RefreshCameraViewMode();
    }

    void RefreshCameraViewMode()
    {
        switch (m_cameraViewMode)
        {
            case ECamera_ViewMode.LEFT_SIDE:
            {
                transform.localPosition = CameraView_LeftSide;
            };
            break;

            case ECamera_ViewMode.RIGHT_SIDE:
            {
                transform.localPosition = CameraView_RightSide;
            };
            break;
        }
    }
}
