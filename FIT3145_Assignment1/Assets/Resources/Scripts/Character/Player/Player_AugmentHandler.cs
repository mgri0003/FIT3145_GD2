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
            GetAugment(augmentSlot).Use();
        }
    }

    public void AttachAugment(in EAugmentSlot augmentSlot, Augment newAugment)
    {
        Debug.Assert(augmentSlot != EAugmentSlot.MAX, "Invalid Augment Slot Value");
        if (CanAttachAugment(augmentSlot))
        {
            m_augments[(int)augmentSlot] = newAugment;
            m_augments[(int)augmentSlot].SetAugmentActive(true);
            m_augments[(int)augmentSlot].SetPlayer(GetComponent<Player_Core>());
        }
    }

    public void DetachAugment(in EAugmentSlot augmentSlot)
    {
        Debug.Assert(augmentSlot != EAugmentSlot.MAX, "Invalid Augment Slot Value");
        if (HasAugment(augmentSlot))
        {
            m_augments[(int)augmentSlot].SetAugmentActive(false);
            m_augments[(int)augmentSlot].SetPlayer(null);
            m_augments[(int)augmentSlot] = null;
        }
    }

    public bool CanAttachAugment(in EAugmentSlot augmentSlot)
    {
        Debug.Assert(augmentSlot != EAugmentSlot.MAX, "Invalid Augment Slot Value");
        return !HasAugment(augmentSlot);
    }

}
