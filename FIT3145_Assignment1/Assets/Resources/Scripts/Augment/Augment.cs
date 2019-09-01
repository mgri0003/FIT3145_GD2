using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Augment : Item
{
    //--Variables--//
    protected Player_Core m_player = null;
    bool m_augmentActive = false;
    [SerializeField] private float m_maxCooldown = 0.0f;
    private float m_cooldown = 0.0f;


    //--Methods--//
    public bool IsAugmentActive() { return m_augmentActive; }
    public void SetAugmentActive(bool val) { m_augmentActive = val; }
    private void Update()
    {
        if(IsAugmentActive())
        {
            m_cooldown -= Time.deltaTime;
            m_cooldown = Mathf.Clamp(m_cooldown, 0, m_maxCooldown);

            AugmentUpdate();
        }
    }
    public void Use()
    {
        if(m_cooldown == 0)
        {
            if(AugmentAbility())
            {
                m_cooldown = m_maxCooldown;
            }
        }
    }
    protected abstract void AugmentUpdate();
    protected abstract bool AugmentAbility();
    public void SetPlayer(Player_Core player) { m_player = player; }
    public float GetCooldown() { return m_cooldown; }
}
