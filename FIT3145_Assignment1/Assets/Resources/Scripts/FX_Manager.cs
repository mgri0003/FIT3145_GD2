using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EParticleEffect
{
    HIT,
    LIGHTNING_IMPACT,

    MAX
}


public class FX_Manager : Singleton<FX_Manager>
{
    [SerializeField] private GameObject[] m_particleEffects = new GameObject[(int)EParticleEffect.MAX];
    
    public void SpawnParticleEffect(in EParticleEffect particleEffect, in Vector3 worldPos)
    {
        Instantiate(m_particleEffects[(int)particleEffect], worldPos, Quaternion.Euler(0,0,0));
    }

}
