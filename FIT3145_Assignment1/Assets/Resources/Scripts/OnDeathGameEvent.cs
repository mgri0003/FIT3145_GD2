using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDeathGameEvent : MonoBehaviour
{
    private bool m_gameEventDone = false;
    private Character_Core m_character = null;
    [SerializeField] private EGameEvent m_gameEvent = EGameEvent.MAX;

    private void Start()
    {
        m_character = GetComponent<Character_Core>();
        Debug.Assert(m_character, "OnDeathGameEvent Requires Character_Core!!!");
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_gameEventDone)
        {
            if(m_character.IsDead())
            {
                m_gameEventDone = true;
                GamePlayManager.Instance.ProcessGameEvent(m_gameEvent);
            }
        }
    }
}
