using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Camera_Main : MonoBehaviour
{
    //--pre-declared variables--
    private enum ECamera_ViewMode
    {
        LEFT_SIDE,
        RIGHT_SIDE,
        MAX
    };

    public const int CAMERA_RANGE_MAX = 40;
    public const int CAMERA_RANGE_MIN = -15;


    //--variables--
    private Transform m_target = null; //The transform that the camera holder will follow
    [SerializeField] private Transform m_cameraHolder = null;
    [SerializeField] private Transform m_aimTarget = null; //The invisible object the player will be aiming at
    private float m_currentRotationX;
    private float m_currentRotationY;
    private ECamera_ViewMode m_cameraViewMode = ECamera_ViewMode.RIGHT_SIDE;
    const float CameraView_LeftSideX    = -1;
    const float CameraView_RightSideX   = 1;
    static Camera_Main m_mainInstance = null;



    //--methods--
    public static Camera_Main GetMainCamera()
    {
        Debug.Assert(m_mainInstance, "Main Camera Variable Not Initialised?");
        return m_mainInstance;
    }

    private void Awake()
    {
        Debug.Assert(m_mainInstance == null, "Main Camera Variable Is Already Initialised?!?!?");
        if (m_mainInstance == null)
        {
            m_mainInstance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //error check references!
        Debug.Assert(m_cameraHolder != null, "Camera Holder Is Null");
        Debug.Assert(m_aimTarget != null, "Aim Target Is NULL?!?!?");

        //hide n lock da mouse (debug stoof)(will need to be somewhere else probs)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        RefreshCameraViewMode();
    }

    // Update is called once per frame
    void Update()
    {
        //Camera Position
        if(m_target)
        {
            m_cameraHolder.SetPositionAndRotation(m_target.position, m_cameraHolder.rotation);

            //Camera Rotation
            m_currentRotationX = m_target.GetComponent<Player_Rotator>().GetDesiredRotation();
            m_currentRotationY -= Input.GetAxis("Mouse Y");
            m_currentRotationY = Mathf.Clamp(m_currentRotationY, CAMERA_RANGE_MIN, CAMERA_RANGE_MAX);
            m_cameraHolder.transform.rotation = Quaternion.Euler(m_currentRotationY, m_currentRotationX, 0);

            if (Input.GetMouseButtonDown(2))
            {
                CycleCameraViewMode();
            }
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
                transform.localPosition = new Vector3(CameraView_LeftSideX, transform.localPosition.y , transform.localPosition.z);
            };
            break;

            case ECamera_ViewMode.RIGHT_SIDE:
            {
                transform.localPosition = new Vector3(CameraView_RightSideX, transform.localPosition.y, transform.localPosition.z);
            };
            break;
        }
    }

    public Transform GetAimTarget()
    {
        return m_aimTarget;
    }

    public void SetTarget(Transform newTarget)
    {
        m_target = newTarget;
    }

}
