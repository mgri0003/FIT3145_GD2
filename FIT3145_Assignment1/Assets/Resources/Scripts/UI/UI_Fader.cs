using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Fader : MonoBehaviour
{
    [SerializeField] Image m_image;
    [SerializeField] float m_duration = 0.1f;
    [SerializeField] Color m_targetColor = Color.clear;
    [SerializeField] Color m_startColor = Color.clear;

    private void Awake()
    {
        Debug.Assert(m_image, "Missing Image, Cant Fade");
    }

    public void StartFade()
    {
        if (m_image)
        {
            m_image.CrossFadeColor(m_targetColor, m_duration, true, true);
        }
    }

    public void Reset()
    {
        m_image.color = m_startColor;
    }
}
