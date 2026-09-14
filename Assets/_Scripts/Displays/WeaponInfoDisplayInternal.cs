using UnityEngine;
using TMPro;
using HeartOfIron.Enums;
using System;

public class WeaponInfoDisplayInternal : MonoBehaviour
{
    [Header("Generic Elements")]
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _weightText;
    [SerializeField] private TextMeshProUGUI _slotsText;

    [Header("Ranged Elements")]
    [SerializeField] private GameObject _rangedElements;
    [SerializeField] private TextMeshProUGUI _dmgText;
    [SerializeField] private TextMeshProUGUI _ammoText;
    [SerializeField] private TextMeshProUGUI _fireRateText;
    [SerializeField] private TextMeshProUGUI _reloadTimeText;
    [SerializeField] private TextMeshProUGUI _numShotsText;
    [SerializeField] private TextMeshProUGUI _zraText;
    [SerializeField] private TextMeshProUGUI _accuracyDropoffText;

    [Header("Melee Elements")]
    [SerializeField] private TextMeshProUGUI _meleeDmgText;
    [SerializeField] private TextMeshProUGUI _meleeRangeText;
    [SerializeField] private TextMeshProUGUI _meleeAccuracyText;
    [SerializeField] private TextMeshProUGUI _swingRateText;

    public void DisplayWeaponInfo(Weapon weapon)
    {
        if (weapon != null)
        {
            _nameText.text = weapon.Name;
            if (weapon.Weight == Weight.Light)
            {
                _weightText.text = "L";   
            } else if (weapon.Weight == Weight.Medium)
            {
                _weightText.text = "M";
            } else if (weapon.Weight == Weight.Heavy)
            {
                _weightText.text = "H";
            }
            _slotsText.text = weapon.SlotsRequired.ToString();
            if (!weapon.IsMelee)
            {
                _rangedElements.SetActive(true);
                _dmgText.text = "Dmg:" + Math.Round((double)weapon.Damage, 2).ToString();
                _ammoText.text = "Ammo:" + Math.Round((double)weapon.MaxAmmo).ToString();
                _fireRateText.text = "Fire Rate:" + Math.Round((double)weapon.FireRate, 2).ToString();
                _reloadTimeText.text = "Reload:" + Math.Round((double)weapon.ReloadTime, 2).ToString();
                _numShotsText.text = "Shots:" + Math.Round((double)weapon.NumShots).ToString();
                _zraText.text = "Point Blank Acc:" + Math.Round((double)weapon.PointBlankAccuracy, 2).ToString();
                _accuracyDropoffText.text = "Acc Dropoff:" + Math.Round((double)weapon.AccuracyDropoff, 2).ToString();
            } else
            {
                _rangedElements.SetActive(false);
            }

            _meleeDmgText.text = "MDmg:" + Math.Round((double)weapon.MeleeDamage, 2).ToString();
            _meleeRangeText.text = "MRange:" + Math.Round((double)weapon.MeleeRange, 2).ToString();
            _meleeAccuracyText.text = "MAcc:" + Math.Round((double)weapon.MeleeAccuracy, 2).ToString();
            _swingRateText.text = "Swing Rate:" + Math.Round((double)weapon.SwingRate, 2).ToString();
        }
    }
}
