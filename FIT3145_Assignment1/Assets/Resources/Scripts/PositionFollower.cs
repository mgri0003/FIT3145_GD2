using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionFollower : MonoBehaviour
{
    private Transform m_transformToFollow;
    public void SetTransformToFollow(Transform newTransform) { m_transformToFollow = newTransform; }

    // Update is called once per frame
    void Update()
    {
        transform.position = m_transformToFollow.position;
    }
}
