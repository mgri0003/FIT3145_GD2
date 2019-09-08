using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UpgradeSegment : MonoBehaviour
{
    //References
    [SerializeField] private Text m_upgradeSegmentText;
    [SerializeField] private Text m_upgradeSegmentCostText;
    [SerializeField] private Image m_upgradeSegmentFrame;
    [SerializeField] private Image m_upgradeSegmentCostFrame;
    [SerializeField] private Image m_background;

    public ref Text AccessUpgradeSegmentText() { return ref m_upgradeSegmentText; }
    public ref Text AccessUpgradeSegmentCostText() { return ref m_upgradeSegmentCostText; }
    public ref Image AccessUpgradeSegmentFrame() { return ref m_upgradeSegmentFrame; }
    public ref Image AccessUpgradeSegmentCostFrame() { return ref m_upgradeSegmentCostFrame; }

    public void SetUISegmentOpacity(float value)
    {
        m_background.color = new Color(m_background.color.r, m_background.color.g, m_background.color.b, value);
        m_upgradeSegmentText.color = new Color(m_upgradeSegmentText.color.r, m_upgradeSegmentText.color.g, m_upgradeSegmentText.color.b, value);
        m_upgradeSegmentCostText.color = new Color(m_upgradeSegmentCostText.color.r, m_upgradeSegmentCostText.color.g, m_upgradeSegmentCostText.color.b, value);
        m_upgradeSegmentFrame.color = new Color(m_upgradeSegmentFrame.color.r, m_upgradeSegmentFrame.color.g, m_upgradeSegmentFrame.color.b, value);
        m_upgradeSegmentCostFrame.color = new Color(m_upgradeSegmentCostFrame.color.r, m_upgradeSegmentCostFrame.color.g, m_upgradeSegmentCostFrame.color.b, value);
    }
}
