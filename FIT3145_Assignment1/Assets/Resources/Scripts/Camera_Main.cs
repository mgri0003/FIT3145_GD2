﻿using System.Collections;
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
    public const int CAMERA_RANGE_MIN = -45;


    //--variables--
    private Transform m_target = null; //The transform that the camera holder will follow
    [SerializeField] private Transform m_cameraHolder = null;
    [SerializeField] private Transform m_aimTarget = null; //The invisible object the player will be aiming at
    private float m_currentRotationX;
    private float m_currentRotationY;
    private ECamera_ViewMode m_cameraViewMode = ECamera_ViewMode.RIGHT_SIDE;
    const float CameraView_SideX = 0.6f;
    static Camera_Main m_mainInstance = null;
    private float m_currentCameraZ = m_CAMERA_LIMIT_Z_MIN;
    private const float m_CAMERA_LIMIT_Z_MIN = -2.5f;
    private const float m_CAMERA_LIMIT_Z_MAX = -0.8f;
    private const float m_CAMERA_Z_RANGE = 2.5f;
    private bool m_forceZoom = false;


    //--methods--
    public static Camera_Main GetMainCamera()
    {
        Debug.Assert(m_mainInstance, "Main Camera Variable Not Initialised?");
        return m_mainInstance;
    }

    public void SetForceZoom(bool val) { m_forceZoom = val; }
    public bool GetForceZoom() { return m_forceZoom; }

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
            m_currentRotationX = GetCalculatedRotation();
            m_currentRotationY = Mathf.Clamp(m_currentRotationY, CAMERA_RANGE_MIN, CAMERA_RANGE_MAX);
            m_cameraHolder.transform.rotation = Quaternion.Euler(m_currentRotationY, m_currentRotationX, 0);

            //Collisions
            UpdateCollision();
        }
    }

    private float GetCalculatedRotation()
    {
        float retVal = 0.0f;

        if(m_target)
        {
            retVal = m_target.GetComponent<Player_Rotator>().GetDesiredRotation();
        }

        return retVal;
    }

    public void CycleCameraViewMode()
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
                transform.localPosition = new Vector3(-CameraView_SideX, transform.localPosition.y , transform.localPosition.z);
            };
            break;

            case ECamera_ViewMode.RIGHT_SIDE:
            {
                transform.localPosition = new Vector3(CameraView_SideX, transform.localPosition.y, transform.localPosition.z);
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

    void UpdateCollision()
    {
        RaycastHit rayHit;
        bool isColliding = Physics.Linecast(m_cameraHolder.position, m_cameraHolder.position + -m_cameraHolder.forward * m_CAMERA_Z_RANGE, out rayHit);
        Debug.DrawLine(m_cameraHolder.position, m_cameraHolder.position + -m_cameraHolder.forward * m_CAMERA_Z_RANGE);
        if ( (isColliding && !rayHit.transform.root.CompareTag("Character")) || GetForceZoom())
        {
            m_currentCameraZ += 0.1f;
        }
        else if (!isColliding)
        {
            m_currentCameraZ -= 0.1f;
        }
        m_currentCameraZ = Mathf.Clamp(m_currentCameraZ, m_CAMERA_LIMIT_Z_MIN, m_CAMERA_LIMIT_Z_MAX);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, m_currentCameraZ);
    }

    public void MinusCurrentRotationY(in float rotY)
    {
        m_currentRotationY -= rotY;
    }
}
