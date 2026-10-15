using UnityEngine;
using System.Collections.Generic;
using TMPro;
using HeartOfIron.Enums;
using UnityEngine.UI;
using System.Text;

public class GarageUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _garageNavButton;
    [Header("MechSelection UI Elements")]
    [SerializeField] private GameObject _mechSelectionPanel;
    [SerializeField] private GameObject _mechSelectionOptionsPanel;
    [SerializeField] private TextMeshProUGUI _mechSelectionText;
    [SerializeField] private GameObject _mechSelectionButtonPrefab;
    [SerializeField] private Mech _selectedMech;

    [Header("MechMod UI Elements")]
    [SerializeField] private GameObject _mechModPanel;
    [SerializeField] private GameObject _mechModOptionsPanel;
    [SerializeField] private GameObject _weaponStatsDisplay;
    [SerializeField] private WeaponInfoDisplayInternal _weaponInfoDisplayInternal;
    [SerializeField] private GameObject _armorStatsDisplay;
    [SerializeField] private ArmorInfoDisplayInternal _armorInfoDisplayInternal;
    [SerializeField] private GameObject _energyShieldStatsDisplay;
    [SerializeField] private EnergyShieldInfoDisplayInternal _energyShieldInfoDisplayInternal;
    //[SerializeField] private TextMeshProUGUI _mechModText;
    [SerializeField] private TMP_InputField _mechModNameText;
    //[SerializeField] private TextMeshProUGUI _mechModInfoText;
    [SerializeField] private MechInfoDisplayInternal _mechInfoDisplayInternal;
    [SerializeField] private GameObject _mechModEquipButton;
    [SerializeField] private GameObject _mechModUnequipButton;
    [SerializeField] private GameObject _mechColorButton;
    [SerializeField] private GameObject _pilotColorButton;
    [SerializeField] private GameObject _colorPickerPanel;
    [SerializeField] private RuntimeColorPicker _runtimeColorPicker;
    [SerializeField] private GameObject _hoverableItemTooltip;
    private Item _selectedItem;
    private List<ItemType> _itemTypes = new List<ItemType> { ItemType.Weapon, ItemType.Armor, ItemType.EnergyShield };

    void Awake()
    {
        EventRouter.ROnHoverableItemTooltipEnter.AddListener(OnHoverableItemTooltipEnter);
        EventRouter.ROnHoverableItemTooltipExit.AddListener(OnHoverableItemTooltipExit);
    }

    void Start()
    {
        ShowMechSelectionPanel();
    }

    void OnDestroy()
    {
        EventRouter.ROnHoverableItemTooltipEnter.RemoveListener(OnHoverableItemTooltipEnter);
        EventRouter.ROnHoverableItemTooltipExit.RemoveListener(OnHoverableItemTooltipExit);
    }

    void ShowMechSelectionPanel()
    {
        _mechModPanel.SetActive(false);
        List<Mech> playerMechs = UnitManager.Instance.PlayerMechs;
        if (playerMechs.Count != 0)
        {
            foreach (Mech mech in playerMechs)
            {
                GameObject button = Instantiate(_mechSelectionButtonPrefab, _mechSelectionOptionsPanel.transform);
                button.GetComponentInChildren<TextMeshProUGUI>().text = mech.Name;
                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnMechSelected(mech));
            }
            _selectedMech = playerMechs[0];
            _mechSelectionText.text = $"{_selectedMech.Name} selected. Select equip to modify it.";
        }
        _garageNavButton.GetComponent<Image>().color = new Color32(0xAE, 0x4D, 0x00, 0xFF); // Change the color of the garage nav button to indicate it's the current scene
        _mechSelectionPanel.SetActive(true);
    }

    void OnMechSelected(Mech mech)
    {
        _selectedMech = mech;
        // TODO: Possibly spawn the selected mech in the garage scene
        _mechSelectionText.text = $"{_selectedMech.Name} selected. Select equip to modify it.";
    }

    public void OnEquipMechButtonClicked()
    {
        if (_selectedMech == null)
        {
            Debug.LogWarning("No mech selected. Please select a mech first.");
            return;
        }
        ShowMechModPanel();
    }

    void ShowMechModPanel()
    {
        _mechSelectionPanel.SetActive(false);
        _colorPickerPanel.SetActive(false);
        _weaponStatsDisplay.SetActive(false);
        _armorStatsDisplay.SetActive(false);
        _energyShieldStatsDisplay.SetActive(false);
        _mechModNameText.text = _selectedMech.Name;
        DisplayMechInfo(); // Refresh the displayed info
        foreach (Transform child in _mechModOptionsPanel.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (ItemType itemType in _itemTypes)
        {
            switch (itemType)
            {
                case ItemType.Weapon:
                    ShowWeaponOptions();
                    break;
                case ItemType.Armor:
                    ShowArmorOptions();
                    break;
                case ItemType.EnergyShield:
                    ShowEnergyShieldOptions();
                    break;
                default:
                    Debug.LogWarning($"Unhandled item type: {itemType}");
                    break;
            }
        }
        _mechModPanel.SetActive(true);
        _mechColorButton.GetComponent<Button>().onClick.AddListener(() => ShowColorPickerPanel(_mechColorButton));
        _mechColorButton.GetComponent<Image>().color = _selectedMech.Color; // Set the button's color to the mech's current color
        if (_selectedMech.Pilot == null)
        {
            Debug.LogWarning("Selected mech has no pilot. Please assign a pilot first.");
            _pilotColorButton.GetComponent<Button>().interactable = false; // Disable the pilot color button if there's no pilot
            _pilotColorButton.GetComponent<Image>().color = new Color32(0xE5, 0x8E, 0x26, 0xFF); // Set to default color when no pilot is assigned
        }
        else
        {
            _pilotColorButton.GetComponent<Button>().interactable = true; // Enable the pilot color button if there's a pilot
            _pilotColorButton.GetComponent<Button>().onClick.AddListener(() => ShowColorPickerPanel(_pilotColorButton));
            _pilotColorButton.GetComponent<Image>().color = _selectedMech.Pilot.Color; // Set the button's color to the pilot's current color
        }
    }

    void ShowWeaponOptions()
    {
        List<Weapon> playerWeapons = GameManager.Instance.GetAllWeapons();
        foreach (Weapon weapon in playerWeapons)
        {
            if (weapon == null || weapon.Owner != null && weapon.Owner != _selectedMech)
            {
                continue; // Skip weapons that are null or already owned by another mech
            }
            GameObject button = Instantiate(_mechSelectionButtonPrefab, _mechModOptionsPanel.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = weapon.Name;
            button.GetComponent<HoverableItemTooltip>().item = weapon;
            button.GetComponent<Button>().onClick.AddListener(() => OnItemSelected(weapon));
            if (_selectedMech.Inventory.HasItem(weapon))
            {
                button.GetComponent<Image>().color = Color.green; // Indicate that the weapon is already equipped
            }
        }
    }

    void ShowArmorOptions()
    {
        List<Armor> playerArmors = GameManager.Instance.GetAllArmors();
        foreach (Armor armor in playerArmors)
        {
            if (armor == null || armor.Owner != null && armor.Owner != _selectedMech)
            {
                continue; // Skip armors that are null or already owned by another mech
            }
            GameObject button = Instantiate(_mechSelectionButtonPrefab, _mechModOptionsPanel.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = armor.Name;
            button.GetComponent<HoverableItemTooltip>().item = armor;
            button.GetComponent<Button>().onClick.AddListener(() => OnItemSelected(armor));
            if (_selectedMech.Inventory.HasItem(armor))
            {
                button.GetComponent<Image>().color = Color.green; // Indicate that the armor is already equipped
            }
        }
    }

    void ShowEnergyShieldOptions()
    {
        List<EnergyShield> playerShields = GameManager.Instance.GetAllEnergyShields();
        foreach (EnergyShield shield in playerShields)
        {
            if (shield == null || shield.Owner != null && shield.Owner != _selectedMech)
            {
                continue; // Skip shields that are null or already owned by another mech
            }
            GameObject button = Instantiate(_mechSelectionButtonPrefab, _mechModOptionsPanel.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = shield.Name;
            button.GetComponent<HoverableItemTooltip>().item = shield;
            button.GetComponent<Button>().onClick.AddListener(() => OnItemSelected(shield));
            if (_selectedMech.Inventory.HasItem(shield))
            {
                button.GetComponent<Image>().color = Color.green; // Indicate that the shield is already equipped
            }
        }
    }

    void OnItemSelected(Item item)
    {
        _selectedItem = item;
        ShowItemDetails();
        if (_selectedMech.Inventory.HasItem(item))
        {
            _mechModEquipButton.SetActive(false);
            _mechModUnequipButton.SetActive(true);
        }
        else
        {
            _mechModEquipButton.SetActive(true);
            _mechModUnequipButton.SetActive(false);
        }
    }

    public void OnPlayButtonClicked()
    {
        SceneLoader.Instance.LoadScene(SceneName.CombatScene);
    }

    public void OnMainMenuButtonClicked()
    {
        SceneLoader.Instance.LoadScene(SceneName.MainMenu);
    }

    public void OnWeaponFilterButtonClicked(GameObject button)
    {
        if (_itemTypes.Contains(ItemType.Weapon))
        {
            _itemTypes.Remove(ItemType.Weapon);
        } else
        {
            _itemTypes.Add(ItemType.Weapon);
        }
        button.GetComponent<Image>().color = _itemTypes.Contains(ItemType.Weapon) ? new Color32(0xE5, 0x8E, 0x26, 0xFF) : new Color32(0xAE, 0x4D, 0x00, 0xFF); // Change button color based on filter state
        ShowMechModPanel(); // Refresh the mech mod panel to reflect the filter change
    }

    public void OnArmorFilterButtonClicked(GameObject button)
    {
        if (_itemTypes.Contains(ItemType.Armor))
        {
            _itemTypes.Remove(ItemType.Armor);
        } else
        {
            _itemTypes.Add(ItemType.Armor);
        }
        button.GetComponent<Image>().color = _itemTypes.Contains(ItemType.Armor) ? new Color32(0xE5, 0x8E, 0x26, 0xFF) : new Color32(0xAE, 0x4D, 0x00, 0xFF); // Change button color based on filter state
        ShowMechModPanel(); // Refresh the mech mod panel to reflect the filter change
    }

    public void OnEnergyShieldFilterButtonClicked(GameObject button)
    {
        if (_itemTypes.Contains(ItemType.EnergyShield))
        {
            _itemTypes.Remove(ItemType.EnergyShield);
        } else
        {
            _itemTypes.Add(ItemType.EnergyShield);
        }
        button.GetComponent<Image>().color = _itemTypes.Contains(ItemType.EnergyShield) ? new Color32(0xE5, 0x8E, 0x26, 0xFF) : new Color32(0xAE, 0x4D, 0x00, 0xFF); // Change button color based on filter state
        ShowMechModPanel(); // Refresh the mech mod panel to reflect the filter change
    }

    public void OnEquipButtonClicked()
    {
        if (_selectedItem == null)
        {
            Debug.LogWarning("No item selected. Please select an item first.");
            return;
        }
        if (_selectedItem is Weapon weapon)
        {
            if (_selectedMech.EquipWeapon(weapon))
            {
                _mechModEquipButton.SetActive(false);
                _mechModUnequipButton.SetActive(true);
            }
        }
        else if (_selectedItem is Armor armor)
        {
            if (_selectedMech.EquipArmor(armor))
            {
                _mechModEquipButton.SetActive(false);
                _mechModUnequipButton.SetActive(true);
            }
        }
        else if (_selectedItem is EnergyShield shield)
        {
            if (_selectedMech.EquipEnergyShield(shield))
            {
                _mechModEquipButton.SetActive(false);
                _mechModUnequipButton.SetActive(true);
            }
        }
        DisplayMechInfo(); // Refresh the displayed info
        ShowMechModPanel(); // Refresh the mech mod panel to reflect the equipped item
    }

    public void OnUnequipButtonClicked()
    {
        if (_selectedItem == null)
        {
            Debug.LogWarning("No item selected. Please select an item first.");
            return;
        }
        if (_selectedItem is Weapon weapon)
        {
            _selectedMech.UnequipWeapon(weapon);
        }
        else if (_selectedItem is Armor armor)
        {
            _selectedMech.UnequipArmor(armor);
        }
        else if (_selectedItem is EnergyShield shield)
        {
            _selectedMech.UnequipEnergyShield(shield);
        }
        _mechModEquipButton.SetActive(true);
        _mechModUnequipButton.SetActive(false);
        DisplayMechInfo(); // Refresh the displayed info
        ShowMechModPanel(); // Refresh the mech mod panel to reflect the unequipped item
    }

    public void UpdateMechName()
    {
        if (_selectedMech == null)
        {
            Debug.LogWarning("No mech selected. Please select a mech first.");
            return;
        }
        string newName = _mechModNameText.text;
        if (string.IsNullOrWhiteSpace(newName))
        {
            Debug.LogWarning("Mech name cannot be empty.");
            return;
        }
        _selectedMech.UpdateName(newName.Trim());
        _mechSelectionText.text = $"{_selectedMech.Name} selected. You can now modify it.";
        DisplayMechInfo(); // Refresh the displayed info
    }

    private void DisplayMechInfo()
    {
        if (_selectedMech != null)
        {
            // _mechModInfoText.text = _selectedMech.GetInfo();

            _mechInfoDisplayInternal.DisplayMechInfo(_selectedMech);
        }
    }

    private void ShowItemDetails()
    {
        //_mechModText.text = _selectedItem.GetDescription();
        if (_selectedItem is Weapon weapon)
        {
            _weaponStatsDisplay.SetActive(true);
            _weaponInfoDisplayInternal.DisplayWeaponInfo(weapon);
            _armorStatsDisplay.SetActive(false);
            _energyShieldStatsDisplay.SetActive(false);
        }
        else if (_selectedItem is Armor armor)
        {
            _weaponStatsDisplay.SetActive(false);
            _armorStatsDisplay.SetActive(true);
            _armorInfoDisplayInternal.DisplayArmorInfo(armor);
            _energyShieldStatsDisplay.SetActive(false);
        }
        else if (_selectedItem is EnergyShield shield)
        {
            _weaponStatsDisplay.SetActive(false);
            _armorStatsDisplay.SetActive(false);
            _energyShieldStatsDisplay.SetActive(true);
            _energyShieldInfoDisplayInternal.DisplayEnergyShieldInfo(shield);
        }
    }

    private void ShowColorPickerPanel(GameObject callingButton)
    {
        _colorPickerPanel.SetActive(true);
        if (callingButton == _mechColorButton)
        {
            _runtimeColorPicker.SetTargetMech(_mechColorButton, _selectedMech, false);
        }
        else if (callingButton == _pilotColorButton)
        {
            _runtimeColorPicker.SetTargetMech(_pilotColorButton, _selectedMech, true);
        }
        callingButton.GetComponent<Button>().onClick.AddListener(HideColorPickerPanel);
    }

    private void HideColorPickerPanel()
    {
        _colorPickerPanel.SetActive(false);
        _runtimeColorPicker.ClearTargetMech();
    }

    private void OnHoverableItemTooltipEnter(GameObject go, Item item)
    {
        // Debug.Log($"OnHoverableItemTooltipEnter invoked for GameObject: {go.name}, Item: {item?.Name}");
        // Create and display the tooltip for the hovered item to the right of the item
        if (item == null)
        {
            return;
        } else if (item is Weapon weapon)
        {
            // Display weapon tooltip for item and for all weapons in mechs inventory
            GameObject weaponTooltip = CreateWeaponTooltip(weapon);
            weaponTooltip.transform.SetParent(go.transform, false);
            weaponTooltip.transform.localPosition += new Vector3(go.GetComponent<RectTransform>().rect.width + 0.1f, 0, 0);
            Weapon[] weaponsInInventory = _selectedMech.Inventory.Weapons;
            for (int i = 0; i < weaponsInInventory.Length; i++)
            {
                Weapon w = weaponsInInventory[i];
                if (w == null) continue;
                if (weapon == w) continue;
                // Display tooltip for each weapon in inventory
                GameObject weaponListtooltip = CreateWeaponTooltip(w);
                // Position and show the tooltip as needed
                weaponListtooltip.transform.SetParent(go.transform, false);
                weaponListtooltip.transform.localPosition += new Vector3((go.GetComponent<RectTransform>().rect.width + 0.1f) * (i + 1), 0, 0);
            }
            
        }
        else if (item is Armor armor)
        {
            // Display armor tooltip for item and equivalent armor in mechs inventory
            GameObject armorTooltip = CreateArmorTooltip(armor);
            armorTooltip.transform.SetParent(go.transform, false);
            armorTooltip.transform.localPosition += new Vector3(go.GetComponent<RectTransform>().rect.width + 0.1f, 0, 0);
            Armor armorInInventory = _selectedMech.Inventory.Armors[(int)armor.ArmorType];
            if (armorInInventory == null) return;
            if (armorInInventory == armor) return;
            GameObject armorInInventoryTooltip = CreateArmorTooltip(armorInInventory);
            armorInInventoryTooltip.transform.SetParent(go.transform, false);
            armorInInventoryTooltip.transform.localPosition += new Vector3((go.GetComponent<RectTransform>().rect.width + 0.1f) * 2, 0, 0);
            
        }
        else if (item is EnergyShield shield)
        {
            // Display energy shield tooltip for item and equivalent energy shield in mechs inventory
            GameObject shieldTooltip = CreateEnergyShieldTooltip(shield);
            shieldTooltip.transform.SetParent(go.transform, false);
            shieldTooltip.transform.localPosition += new Vector3(go.GetComponent<RectTransform>().rect.width + 0.1f, 0, 0);
            EnergyShield shieldInInventory = _selectedMech.Inventory.EnergyShield;
            if (shieldInInventory == null) return;
            if (shieldInInventory == shield) return;
            GameObject shieldInInventoryTooltip = CreateEnergyShieldTooltip(shieldInInventory);
            shieldInInventoryTooltip.transform.SetParent(go.transform, false);
            shieldInInventoryTooltip.transform.localPosition += new Vector3((go.GetComponent<RectTransform>().rect.width + 0.1f) * 2, 0, 0);
            
        }

    }

    private GameObject CreateWeaponTooltip(Weapon weapon)
    {
        // Instantiate and configure the weapon tooltip GameObject
        GameObject tooltip = Instantiate(_hoverableItemTooltip);
        tooltip.name = "WeaponTooltip";
        // Add components and set up the tooltip as needed
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(weapon.WeaponName.ToString());
        sb.Append("Weight: ");
        sb.AppendLine(weapon.Weight.ToString());
        sb.Append("Slots: ");
        sb.AppendLine(weapon.SlotsRequired.ToString());
        sb.AppendLine();
        if (!weapon.IsMelee)
        {
            sb.Append("Damage: ");
            sb.AppendLine(weapon.Damage.ToString());
            sb.Append("Ammo: ");
            sb.AppendLine(weapon.MaxAmmo.ToString());
            sb.Append("Fire Rate: ");
            sb.AppendLine(weapon.FireRate.ToString());
            sb.Append("Reload: ");
            sb.AppendLine(weapon.ReloadTime.ToString());
            sb.Append("Number of Shots: ");
            sb.AppendLine(weapon.NumShots.ToString());
            sb.Append("Point blank accuracy: ");
            sb.AppendLine(weapon.PointBlankAccuracy.ToString());
            sb.Append("Accuracy Dropoff: ");
            sb.AppendLine(weapon.AccuracyDropoff.ToString());
            sb.AppendLine();
        }
        sb.Append("Melee Damage: ");
        sb.AppendLine(weapon.MeleeDamage.ToString());
        sb.Append("MeleeRange: ");
        sb.AppendLine(weapon.MeleeRange.ToString());
        sb.Append("Melee Accuracy: ");
        sb.AppendLine(weapon.MeleeAccuracy.ToString());
        sb.Append("Swing Rate: ");
        sb.AppendLine(weapon.SwingRate.ToString());
        TooltipScreenSpaceUI tooltipUI = tooltip.GetComponent<TooltipScreenSpaceUI>();
        if (tooltipUI == null)
            tooltipUI = tooltip.AddComponent<TooltipScreenSpaceUI>();
        tooltipUI.DisplayTooltip(sb);
        return tooltip;
    }

    private GameObject CreateArmorTooltip(Armor armor)
    {
        // Instantiate and configure the armor tooltip GameObject
        GameObject tooltip = Instantiate(_hoverableItemTooltip);
        tooltip.name = "ArmorTooltip";
        // Add components and set up the tooltip as needed
        return tooltip;
    }

    private GameObject CreateEnergyShieldTooltip(EnergyShield shield)
    {
        // Instantiate and configure the energy shield tooltip GameObject
        GameObject tooltip = Instantiate(_hoverableItemTooltip);
        tooltip.name = "EnergyShieldTooltip";
        // Add components and set up the tooltip as needed
        return tooltip;
    }

    private void OnHoverableItemTooltipExit(GameObject go)
    {
        // Handle tooltip exit event
        foreach (Transform child in go.transform)
        {
            if (child.gameObject.name.EndsWith("Tooltip"))
                GameObject.Destroy(child.gameObject);
        }
    }
}
