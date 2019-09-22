using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649

public class ElevatorPanel : Interactable
{
    [SerializeField] private uint m_levelToMoveTo = 0;
    [SerializeField] public uint m_levelFrom = 0;
    [SerializeField] private Transform m_playerTeleportTransform;

    public override void Interact()
    {
        GamePlayManager.Instance.MovePlayerToLevel(m_levelToMoveTo);
    }

    public Transform GetPlayerTeleportTransform() { return m_playerTeleportTransform; }
}
