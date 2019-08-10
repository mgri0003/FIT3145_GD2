using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Main : MonoBehaviour
{
    //variables
    [SerializeField] private Transform m_target = null;
    [SerializeField] private Transform m_cameraHolder = null;
    private float m_currentRotationX;
    private float m_currentRotationY;


    //methods

    // Start is called before the first frame update
    void Start()
    {
        //error check references!
        Debug.Assert(m_target != null, "Target Is Null");
        Debug.Assert(m_cameraHolder != null, "Camera Holder Is Null");

        //hide da mouse
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Camera Position
        m_cameraHolder.SetPositionAndRotation(m_target.position, m_cameraHolder.rotation);

        //Camera Rotation
        m_currentRotationX = m_target.rotation.eulerAngles.y;
        m_currentRotationY -= Input.GetAxis("Mouse Y");
        m_currentRotationY = Mathf.Clamp(m_currentRotationY, -15, 60);
        m_cameraHolder.transform.rotation = Quaternion.Euler(m_currentRotationY, m_currentRotationX, 0);
    }
}
