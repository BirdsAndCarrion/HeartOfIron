using UnityEngine;
using TMPro;
using HeartOfIron.Enums;

public class MechInfoDisplayInternal : MonoBehaviour
{

    [Header("Generic Mech Info UI Elements")]
    [SerializeField] private TextMeshProUGUI _mechNameText;
    [SerializeField] private TextMeshProUGUI _mechTypeText;
    [SerializeField] private TextMeshProUGUI _mechLevelText;
    [SerializeField] private TextMeshProUGUI _mechExpText;
    [SerializeField] private TextMeshProUGUI _mechPilotNameText;
    [SerializeField] private TextMeshProUGUI _mechMoveSpeedText;

    [Header("Mech Weapon Info UI Elements")]
    [SerializeField] private TextMeshProUGUI _mechWeapon1Text;
    [SerializeField] private TextMeshProUGUI _mechWeapon2Text;

    [Header("Mech Energy Shield Info UI Elements")]
    [SerializeField] private TextMeshProUGUI _mechEnergyShieldText;
    [SerializeField] private ResourceBar _mechEnergyShieldBar;

    [Header("Mech Health Info UI Elements")]
    [SerializeField] private ResourceBar _mechHeadHealthBar;
    [SerializeField] private ResourceBar _mechTorsoHealthBar;
    [SerializeField] private ResourceBar _mechLeftArmHealthBar;
    [SerializeField] private ResourceBar _mechRightArmHealthBar;
    [SerializeField] private ResourceBar _mechLeftLegHealthBar;
    [SerializeField] private ResourceBar _mechRightLegHealthBar;

    [Header("Mech Armor Info UI Elements")]
    [SerializeField] private TextMeshProUGUI _mechHeadArmorText;
    [SerializeField] private ResourceBar _mechHeadArmorBar;
    [SerializeField] private TextMeshProUGUI _mechTorsoArmorText;
    [SerializeField] private ResourceBar _mechTorsoArmorBar;
    [SerializeField] private TextMeshProUGUI _mechLeftArmArmorText;
    [SerializeField] private ResourceBar _mechLeftArmArmorBar;
    [SerializeField] private ResourceBar _mechRightArmArmorBar;
    [SerializeField] private TextMeshProUGUI _mechLeftLegArmorText;
    [SerializeField] private ResourceBar _mechLeftLegArmorBar;
    [SerializeField] private ResourceBar _mechRightLegArmorBar;

    public void DisplayMechInfo(Mech mech)
    {
        if (mech != null)
        {
            // Get the name of the mech
            _mechNameText.text = mech.Name;
            // Get the type of the mech
            _mechTypeText.text = mech.Type.ToString();
            // Get the level of the mech
            _mechLevelText.text = mech.Level.ToString();
            _mechExpText.text = mech.Experience.ToString();
            if (mech.Pilot == null)
            {
                _mechPilotNameText.text = "No Pilot";
            }
            else
            {
                _mechPilotNameText.text = mech.Pilot.Name;
            }
           
            _mechMoveSpeedText.text = mech.MaxSpeed.ToString();

            // Get the weapons of the mech
            for (int i = 0; i < mech.Inventory.Weapons.Length; i++)
            {
                if (i == 0)
                {
                    _mechWeapon1Text.text = mech.Inventory.Weapons[i] != null ? mech.Inventory.Weapons[i].Name : "None";
                }
                else if (i == 1)
                {
                    _mechWeapon2Text.text = mech.Inventory.Weapons[i] != null ? mech.Inventory.Weapons[i].Name : "None";
                }
            }
            // Get the energy shield values of the mech
            if (mech.Inventory.EnergyShield != null)
            {
                _mechEnergyShieldText.text = mech.Inventory.EnergyShield.Name;
                _mechEnergyShieldBar.Setup(mech.Inventory.EnergyShield.CurrentCapacity, mech.Inventory.EnergyShield.MaxCapacity);
            }
            else
            {
                _mechEnergyShieldText.text = "None";
                _mechEnergyShieldBar.Setup(0, 1); // Set to 0/1 to indicate no shield  
            }

            // Get the health values of the mech
            _mechHeadHealthBar.Setup(mech.Health[(int)BodyPart.Head], mech.MaxHealth[(int)BodyPart.Head]);
            _mechTorsoHealthBar.Setup(mech.Health[(int)BodyPart.Torso], mech.MaxHealth[(int)BodyPart.Torso]);
            _mechLeftArmHealthBar.Setup(mech.Health[(int)BodyPart.LeftArm], mech.MaxHealth[(int)BodyPart.LeftArm]);
            _mechRightArmHealthBar.Setup(mech.Health[(int)BodyPart.RightArm], mech.MaxHealth[(int)BodyPart.RightArm]);
            _mechLeftLegHealthBar.Setup(mech.Health[(int)BodyPart.LeftLeg], mech.MaxHealth[(int)BodyPart.LeftLeg]);
            _mechRightLegHealthBar.Setup(mech.Health[(int)BodyPart.RightLeg], mech.MaxHealth[(int)BodyPart.RightLeg]);

            // Get the armor values of the mech
            _mechHeadArmorText.text = mech.Inventory.Armors[(int)ArmorType.Head] != null ? mech.Inventory.Armors[(int)ArmorType.Head].Name : "None";
            _mechHeadArmorBar.Setup(mech.Inventory.Armors[(int)ArmorType.Head] != null ? mech.Inventory.Armors[(int)ArmorType.Head].GetDurability() : 0, mech.Inventory.Armors[(int)ArmorType.Head] != null ? mech.Inventory.Armors[(int)ArmorType.Head].MaxDurability : 1);
            _mechTorsoArmorText.text = mech.Inventory.Armors[(int)ArmorType.Torso] != null ? mech.Inventory.Armors[(int)ArmorType.Torso].Name : "None";
            _mechTorsoArmorBar.Setup(mech.Inventory.Armors[(int)ArmorType.Torso] != null ? mech.Inventory.Armors[(int)ArmorType.Torso].GetDurability() : 0, mech.Inventory.Armors[(int)ArmorType.Torso] != null ? mech.Inventory.Armors[(int)ArmorType.Torso].MaxDurability : 1);
            _mechLeftArmArmorText.text = mech.Inventory.Armors[(int)ArmorType.Arms] != null ? mech.Inventory.Armors[(int)ArmorType.Arms].Name : "None";
            _mechLeftArmArmorBar.Setup(mech.Inventory.Armors[(int)ArmorType.Arms] != null ? mech.Inventory.Armors[(int)ArmorType.Arms].GetDurability(0) : 0, mech.Inventory.Armors[(int)ArmorType.Arms] != null ? mech.Inventory.Armors[(int)ArmorType.Arms].MaxDurability : 1);
            _mechRightArmArmorBar.Setup(mech.Inventory.Armors[(int)ArmorType.Arms] != null ? mech.Inventory.Armors[(int)ArmorType.Arms].GetDurability(1) : 0, mech.Inventory.Armors[(int)ArmorType.Arms] != null ? mech.Inventory.Armors[(int)ArmorType.Arms].MaxDurability : 1);
            _mechLeftLegArmorText.text = mech.Inventory.Armors[(int)ArmorType.Legs] != null ? mech.Inventory.Armors[(int)ArmorType.Legs].Name : "None";
            _mechLeftLegArmorBar.Setup(mech.Inventory.Armors[(int)ArmorType.Legs] != null ? mech.Inventory.Armors[(int)ArmorType.Legs].GetDurability(0) : 0, mech.Inventory.Armors[(int)ArmorType.Legs] != null ? mech.Inventory.Armors[(int)ArmorType.Legs].MaxDurability : 1);
            _mechRightLegArmorBar.Setup(mech.Inventory.Armors[(int)ArmorType.Legs] != null ? mech.Inventory.Armors[(int)ArmorType.Legs].GetDurability(1) : 0, mech.Inventory.Armors[(int)ArmorType.Legs] != null ? mech.Inventory.Armors[(int)ArmorType.Legs].MaxDurability : 1);
        }
    }
}
