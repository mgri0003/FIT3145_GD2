using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    private AudioSource m_audioSource = null;

    void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    protected void PlayInteractionSound()
    {
        if(m_audioSource)
        {
            if(m_audioSource.clip)
            {
                m_audioSource.Play();
            }
        }
    }

    public abstract void Interact();
}
