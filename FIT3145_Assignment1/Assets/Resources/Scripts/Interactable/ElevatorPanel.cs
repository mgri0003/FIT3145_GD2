using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorPanel : Interactable
{
    [SerializeField] private uint m_levelToMoveTo;

    public override void Interact()
    {
        GamePlayManager.Instance.MovePlayerToLevel(m_levelToMoveTo);
    }
}
