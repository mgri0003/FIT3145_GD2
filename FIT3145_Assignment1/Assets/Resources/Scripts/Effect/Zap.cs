using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zap : Effect
{
    public override void UpdateEffect()
    {
        m_parentCharacter.ReceiveHit(20);
        SetLifeTime(0);
    }
}
