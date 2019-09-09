using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ImprovementSegment : MonoBehaviour
{
    //References
    [SerializeField] private Text m_improvementSegmentText;
    [SerializeField] private Text m_improvementSegmentCostText;
    [SerializeField] private Image m_improvementSegmentFrame;
    [SerializeField] private Image m_improvementSegmentCostFrame;
    [SerializeField] private Image m_background;

    public ref Text AccessImprovementSegmentText() { return ref m_improvementSegmentText; }
    public ref Text AccessImprovementSegmentCostText() { return ref m_improvementSegmentCostText; }
    public ref Image AccessImprovementSegmentFrame() { return ref m_improvementSegmentFrame; }
    public ref Image AccessImprovementSegmentCostFrame() { return ref m_improvementSegmentCostFrame; }

    public void SetUISegmentOpacity(float value)
    {
        m_background.color = new Color(m_background.color.r, m_background.color.g, m_background.color.b, value);
        m_improvementSegmentText.color = new Color(m_improvementSegmentText.color.r, m_improvementSegmentText.color.g, m_improvementSegmentText.color.b, value);
        m_improvementSegmentCostText.color = new Color(m_improvementSegmentCostText.color.r, m_improvementSegmentCostText.color.g, m_improvementSegmentCostText.color.b, value);
        m_improvementSegmentFrame.color = new Color(m_improvementSegmentFrame.color.r, m_improvementSegmentFrame.color.g, m_improvementSegmentFrame.color.b, value);
        m_improvementSegmentCostFrame.color = new Color(m_improvementSegmentCostFrame.color.r, m_improvementSegmentCostFrame.color.g, m_improvementSegmentCostFrame.color.b, value);
    }
}
