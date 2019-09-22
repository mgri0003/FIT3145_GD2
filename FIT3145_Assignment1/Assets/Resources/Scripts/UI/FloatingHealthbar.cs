using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthbar : MonoBehaviour
{
    private Character_Core m_character;
    [SerializeField] private Image m_healthBar = null;
    //[SerializeField] private Text m_healthBarText = null;
    private Camera m_Camera;
    private Vector3 m_healthbarOffset;

    public void SetCharacter(Character_Core newCharacter) { m_character = newCharacter; }
    public void SetHealthBarOffset(Vector3 newOffset) { m_healthbarOffset = newOffset; }

    private void Start()
    {
        m_Camera = Camera_Main.GetMainCamera().GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        UI_UpdateHealthBar();
    }

    private void UI_UpdateHealthBar()
    {
        if(m_character)
        {
            if(m_healthBar)
            {
                float healthFillRatio = m_character.m_characterStats.AccessHealthStat().GetCurrent() / m_character.m_characterStats.AccessHealthStat().GetMax();
                m_healthBar.fillAmount = healthFillRatio;
            }

            transform.position = m_character.transform.position + m_healthbarOffset;
        }
    }

    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        if(m_Camera)
        {
            transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward, m_Camera.transform.rotation * Vector3.up);
        }
    }
}
