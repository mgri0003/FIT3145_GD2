using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAugmentType
{
    ACTIVE,
    PASSIVE,

    MAX
}

public enum EAugmentLocation
{
    TORSO,
    LEG_RIGHT,
    LEG_LEFT,

    MAX
}

public abstract class Augment : Item
{
    //--Variables--//
    protected Player_Core m_player = null;
    bool m_augmentActive = false;
    [SerializeField] private float m_maxCooldown = 0.0f;
    private float m_cooldown = 0.0f;
    [SerializeField] private EAugmentType m_augmentType = EAugmentType.ACTIVE;
    [SerializeField] private EAugmentLocation m_augmentLocation = EAugmentLocation.TORSO;
    [SerializeField] private Vector3 m_positionOffset = Vector3.zero;
    [SerializeField] private Vector3 m_rotationOffset = Vector3.zero;
    private Transform m_attachedTransform = null;


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
            UpdatePositionAndRotation();
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

    public void UpdatePositionAndRotation()
    {
        if(GetAttachedTransform())
        {
            transform.localPosition = m_positionOffset;
            transform.localRotation = Quaternion.Euler(
                m_rotationOffset.x,
                m_rotationOffset.y,
                m_rotationOffset.z);
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

    public EAugmentLocation GetAugmentLocation() { return m_augmentLocation; }

    public void SetAttachedTransform(in Transform newAttachedTransform)
    {
        m_attachedTransform = newAttachedTransform;
        transform.SetParent(m_attachedTransform);
    }
    public Transform GetAttachedTransform() { return m_attachedTransform; }

}
