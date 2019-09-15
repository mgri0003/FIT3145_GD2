using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649

public class UIScreen_InGameHud : UIScreenBase
{
    //--variables--//
    private Player_Core m_player = null;

    //--ui elements--//

    //defaults
    [SerializeField] private Sprite m_defaultHandSprite;
    [SerializeField] private Sprite m_EmptyAugmentSprite;

    //refs
    [SerializeField] private Image m_healthBar;
    [SerializeField] private Image[] m_handDisplays = new Image[2];
    [SerializeField] private Image[] m_handDisplaysAmmo = new Image[2];
    [SerializeField] private Text[] m_handDisplaysAmmoText = new Text[2];

    [SerializeField] private Image[] m_augmentsDisplay = new Image[(int)EAugmentSlot.MAX];
    [SerializeField] private Image[] m_augmentsDisplay_Cooldown = new Image[(int)EAugmentSlot.MAX];
    [SerializeField] private Image[] m_augmentsDisplay_Frame = new Image[(int)EAugmentSlot.MAX];


    //--Methods--//
    protected override void RegisterMethods()
    {
        
    }

    protected override void OnEnable()
    {
        m_player = GamePlayManager.Instance.GetCurrentPlayer();
        if(m_player)
        {
            m_player.GetComponent<Player_Controller>().SetEnableInput(true);
        }
    }

    protected override void OnDisable()
    {
        if(m_player)
        {
            m_player.GetComponent<Player_Controller>().SetEnableInput(false);
        } 
    }

    protected override void OnGUI()
    {
        UI_DisplayPickupable();
        UI_UpdateDisplayAugments();
        
        UI_UpdateHealthBar();
        UI_UpdateHandDisplays();
    }

    protected override void OnBack()
    {
        throw new System.NotImplementedException();
    }

    private void UI_UpdateHandDisplays()
    {
        for (uint i = 0; i < (uint)EPlayerHand.MAX; ++i)
        {
            EPlayerHand hand = (EPlayerHand)i;
            bool displayAmmo = false;

            if (m_player.m_playerWeaponHolder.IsHoldingWeaponInHand(hand))
            {
                Item item = m_player.m_playerWeaponHolder.GetWeaponInHand(hand);
                if(item)
                {
                    if(item.GetItemSprite())
                    {
                        m_handDisplays[i].sprite = item.GetItemSprite();
                    }

                    Weapon_Base baseWeapon = item as Weapon_Base;
                    if (baseWeapon.GetWeaponType() == EWeaponType.RANGED)
                    {
                        Weapon_Ranged rangedWeapon = baseWeapon as Weapon_Ranged;
                        if (rangedWeapon)
                        {
                            //set the fill amount to ration of ammo
                            float ammoRatio = rangedWeapon.GetCurrentAmmo() / rangedWeapon.AccessWeaponStat(EWeaponStat.RANGED_CLIP_SIZE).GetCurrent();
                            m_handDisplaysAmmo[i].fillAmount = ammoRatio;

                            //set ammo text
                            m_handDisplaysAmmoText[i].text = rangedWeapon.GetCurrentAmmo().ToString();

                            //reload effect
                            if (rangedWeapon.IsReloading())
                            {
                                m_handDisplaysAmmo[i].color = Color.green;
                                m_handDisplaysAmmo[i].fillAmount = 1 - (rangedWeapon.GetCurrentReloadTime() / rangedWeapon.AccessWeaponStat(EWeaponStat.RANGED_RELOAD_TIME).GetCurrent());
                                m_handDisplaysAmmoText[i].text = "R";
                            }
                            else
                            {
                                m_handDisplaysAmmo[i].color = Color.white;
                            }
                        }

                        displayAmmo = true;
                    }
                }
            }
            else
            {
                m_handDisplays[i].sprite = m_defaultHandSprite;
            }

            m_handDisplaysAmmo[i].transform.parent.gameObject.SetActive(displayAmmo);
        }
    }

    private void UI_UpdateHealthBar()
    {
        float healthFillRatio = m_player.m_characterStats.AccessHealthStat().GetCurrent() / m_player.m_characterStats.AccessHealthStat().GetMax();
        m_healthBar.fillAmount = healthFillRatio;
    }

    private void UI_UpdateDisplayAugments()
    {
        for(uint i = 0; i < (uint)EAugmentSlot.MAX; ++i)
        {
            EAugmentSlot augSlot = (EAugmentSlot)i;

            if (m_player.m_playerAugmentHandler.HasAugment(augSlot))
            {
                m_augmentsDisplay[i].sprite = m_player.m_playerAugmentHandler.GetAugment(augSlot).GetItemSprite();
                m_augmentsDisplay_Cooldown[i].fillAmount = (m_player.m_playerAugmentHandler.GetAugment(augSlot).GetCooldownRatio());

                if (m_player.m_playerAugmentHandler.GetAugment(augSlot).GetCooldown() == 0)
                {
                    m_augmentsDisplay_Frame[i].color = Color.green;
                }
                else
                {
                    m_augmentsDisplay_Frame[i].color = Color.white;
                }
            }
            else
            {
                m_augmentsDisplay[i].sprite = m_EmptyAugmentSprite;
                m_augmentsDisplay_Frame[i].color = Color.white;
                m_augmentsDisplay_Cooldown[i].fillAmount = 0;
            }
        }
    }

    private void UI_DisplayPickupable()
    {
        if (m_player.IsItemNearby())
        {
            GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2, 200, 30), "Press 'E' To Pick Up");
        }
    }

    public void SetPlayer(in Player_Core newPlayer)
    {
        m_player = newPlayer;
    }
}
