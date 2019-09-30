using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAugmentSlot
{
    Q,
    E,
    SPACE,
    PASSIVE_1,
    PASSIVE_2,

    MAX
}


public class Player_AugmentHandler : MonoBehaviour
{
    //--variables--//
    private Augment[] m_augments = new Augment[(int)EAugmentSlot.MAX];
    [SerializeField] private Transform[] m_augmentLocationTransforms = new Transform[(int)EAugmentLocation.MAX];

    //--Methods--//
    public Augment GetAugment(in EAugmentSlot augmentSlot)
    {
        Debug.Assert(augmentSlot != EAugmentSlot.MAX, "Invalid Augment Slot Value");

        if (augmentSlot != EAugmentSlot.MAX)
        {
            return m_augments[(int)augmentSlot];
        }

        return null;
    }

    public bool HasAugment(in EAugmentSlot augmentSlot)
    {
        Debug.Assert(augmentSlot != EAugmentSlot.MAX, "Invalid Augment Slot Value");

        if (augmentSlot != EAugmentSlot.MAX)
        {
            return m_augments[(int)augmentSlot] != null;
        }

        return false;
    }

    public void UseAugment(in EAugmentSlot augmentSlot)
    {
        Debug.Assert(augmentSlot != EAugmentSlot.MAX, "Invalid Augment Slot Value");
        Debug.Assert(augmentSlot != EAugmentSlot.PASSIVE_1 && augmentSlot != EAugmentSlot.PASSIVE_2, "Can't Use Passive Augments!");

        if (HasAugment(augmentSlot))
        {
            GetAugment(augmentSlot).ActivateAugmentAbility();
        }
    }

    public bool AttachAugment(in EAugmentSlot augmentSlot, Augment newAugment)
    {
        Debug.Assert(augmentSlot != EAugmentSlot.MAX, "Invalid Augment Slot Value");
        if (CanAttachAugmentToSlot(augmentSlot, newAugment))
        {
            m_augments[(int)augmentSlot] = newAugment;
            m_augments[(int)augmentSlot].SetAugmentActive(true);
            m_augments[(int)augmentSlot].SetPlayer(GetComponent<Player_Core>());
            m_augments[(int)augmentSlot].SetAttachedTransform(m_augmentLocationTransforms[(int)m_augments[(int)augmentSlot].GetAugmentLocation()]);
            return true;
        }

        return false;
    }

    public void DetachAugment(in EAugmentSlot augmentSlot)
    {
        Debug.Assert(augmentSlot != EAugmentSlot.MAX, "Invalid Augment Slot Value");
        if (HasAugment(augmentSlot))
        {
            m_augments[(int)augmentSlot].SetAttachedTransform(null);
            m_augments[(int)augmentSlot].SetAugmentActive(false);
            m_augments[(int)augmentSlot].SetPlayer(null);
            m_augments[(int)augmentSlot] = null;
        }
    }

    public bool CanAttachAugmentToSlot(in EAugmentSlot augmentSlot, in Augment aug)
    {
        Debug.Assert(augmentSlot != EAugmentSlot.MAX, "Invalid Augment Slot Value");

        bool validSlotForActive = ((int)augmentSlot < (int)EAugmentSlot.PASSIVE_1) && (aug.GetAugmentType() == EAugmentType.ACTIVE);
        bool validSlotForPassive = ((int)augmentSlot >= (int)EAugmentSlot.PASSIVE_1) && (aug.GetAugmentType() == EAugmentType.PASSIVE);
        bool validAugmentType = (validSlotForActive || validSlotForPassive);

        bool validDuplicate = CheckAugmentDuplicateAttached(aug);


        return !HasAugment(augmentSlot) && validAugmentType && validDuplicate;
    }

    public bool CheckAugmentDuplicateAttached(in Augment aug)
    {
        bool retval = true;
        foreach (Augment playerAug in m_augments)
        {
            if (playerAug)
            {
                //if the augment name is the same, its probs the same :P
                if(playerAug.GetItemName() == aug.GetItemName())
                {
                    retval = false;
                }
            }
        }

        return retval;
    }

    public void CallOnAllAugments_AugmentOnDeath()
    {
        foreach(Augment aug in m_augments)
        {
            if(aug)
            {
                aug.AugmentOnDeath();
            }
        }
    }
}
