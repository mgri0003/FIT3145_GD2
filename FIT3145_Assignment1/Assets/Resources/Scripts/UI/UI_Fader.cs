using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Fader : MonoBehaviour
{
    [SerializeField] Image m_image;
    [SerializeField] float m_duration = 0.1f;
    [SerializeField] Color m_targetColor = Color.clear;

    private void Start()
    {
        if(m_image == null)
        {
            //attempt to grab image
            m_image = GetComponent<Image>();
        }

        if (m_image)
        {
            m_image.CrossFadeColor(m_targetColor, m_duration, true, true);
        }
    }
}
