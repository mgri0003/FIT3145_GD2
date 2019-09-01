﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : Augment
{
    private Vector3 m_dashValue = new Vector3(0, 0, 0);
    private const float m_dashStrength = 10;

    protected override bool AugmentAbility()
    {
        if (m_player.GetDirectionInput().magnitude != 0)
        {
            m_dashValue = m_player.GetDirectionInput().normalized;
            return true;
        }

        return false;
    }

    protected override void AugmentUpdate()
    {
        m_player.transform.Translate(m_dashValue * m_dashStrength * Time.deltaTime);
        m_dashValue *= 0.9f;
        if (m_dashValue.magnitude < 0.1f)
        {
            m_dashValue = new Vector3(0, 0, 0);
        }
    }
}