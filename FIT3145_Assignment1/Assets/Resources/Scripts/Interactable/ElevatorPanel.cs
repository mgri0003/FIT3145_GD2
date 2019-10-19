using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649

public class ElevatorPanel : Interactable
{
    [SerializeField] private uint m_levelToMoveTo = 0;
    [SerializeField] public uint m_levelFrom = 0;
    [SerializeField] private Transform m_playerTeleportTransform;
    [SerializeField] private float m_rotationToBe = 0;


    public override void Interact()
    {
        PlayInteractionSound();

        GamePlayManager.Instance.MovePlayerToLevel(m_levelToMoveTo);

        FX_Manager.Instance.SpawnParticleEffect(EParticleEffect.ELEVATOR_TELEPORT, GamePlayManager.Instance.GetElevatorPanelLocation(m_levelToMoveTo));

        GamePlayManager.Instance.GetCurrentPlayer().m_playerRotator.SetDesiredRotation(m_rotationToBe);
    }

    public Transform GetPlayerTeleportTransform() { return m_playerTeleportTransform; }
}
