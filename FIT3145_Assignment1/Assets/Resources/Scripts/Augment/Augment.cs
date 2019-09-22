using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAugmentType
{
    ACTIVE,
    PASSIVE,

    MAX
}

public abstract class Augment : Item
{
    //--Variables--//
    protected Player_Core m_player = null;
    bool m_augmentActive = false;
    [SerializeField] private float m_maxCooldown = 0.0f;
    private float m_cooldown = 0.0f;
    [SerializeField] EAugmentType m_augmentType = EAugmentType.ACTIVE;


    //--Methods--//
    protected abstract void AugmentUpdate();
    protected abstract bool AugmentAbility();
    public abstract void AugmentOnDeath();

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
    public void ActivateAugmentAbility()
    {
        if (m_cooldown == 0)
        {
            if (AugmentAbility())
            {
                m_cooldown = m_maxCooldown;
            }
        }
    }
    public void SetPlayer(Player_Core player) { m_player = player; }
    public float GetCooldown() { return m_cooldown; }
    public float GetMaxCooldown() { return m_maxCooldown; }
    public float GetCooldownRatio() { return m_cooldown/m_maxCooldown; }
    public EAugmentType GetAugmentType() { return m_augmentType; }

    public override string GetItemTypeDescription()
    {
        return "Cooldown: " + GetMaxCooldown() + " second" + (GetMaxCooldown() > 1.0f ? "s" :"");
    }
}
