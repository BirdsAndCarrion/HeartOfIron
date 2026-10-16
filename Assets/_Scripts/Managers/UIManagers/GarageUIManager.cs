using UnityEngine;
using System.Collections.Generic;
using TMPro;
using HeartOfIron.Enums;
using UnityEngine.UI;
using System.Text;
using System.Threading.Tasks;


public class GarageUIManager : MonoBehaviour
{

    private readonly float TOOLTIP_OFFSET_X = 0.5f;

    [SerializeField] RectTransform _canvasRectTransform;
    [SerializeField] private GameObject _garageNavButton;
    [Header("MechSelection UI Elements")]
    [SerializeField] private GameObject _mechSelectionPanel;
    [SerializeField] private GameObject _mechSelectionOptionsPanel;
    [SerializeField] private TextMeshProUGUI _mechSelectionText;
    [SerializeField] private GameObject _mechSelectionButtonPrefab;
    private Mech _selectedMech;

    [Header("MechMod UI Elements")]
    [SerializeField] private GameObject _mechModPanel;
    [SerializeField] private GameObject _mechModOptionsPanel;
    [SerializeField] private TMP_InputField _mechModNameText;
    [SerializeField] private MechInfoDisplayInternal _mechInfoDisplayInternal;
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
        Debug.Log("Showing mech selection panel.");
        ShowMechSelectionPanel();
    }

    void OnDestroy()
    {
        EventRouter.ROnHoverableItemTooltipEnter.RemoveListener(OnHoverableItemTooltipEnter);
        EventRouter.ROnHoverableItemTooltipExit.RemoveListener(OnHoverableItemTooltipExit);
    }

    void ShowMechSelectionPanel()
    {
        foreach (Transform child in _mechSelectionOptionsPanel.transform)
        {
            Destroy(child.gameObject);
        }
        _mechModPanel.SetActive(false);
        List<Mech> playerMechs = UnitManager.Instance.PlayerMechs;
        if (_selectedMech == null)
        {
            if (playerMechs.Count > 0)
            {
                // Debug.Log($"No mech selected. Defaulting to the first mech: {playerMechs[0].Name}");
                _selectedMech = playerMechs[0];
            }
        } else
        {
            // Debug.Log($"Currently selected mech: {_selectedMech.Name}");
        }
        if (playerMechs.Count != 0)
        {
            foreach (Mech mech in playerMechs)
            {
                GameObject button = Instantiate(_mechSelectionButtonPrefab, _mechSelectionOptionsPanel.transform);
                button.GetComponentInChildren<TextMeshProUGUI>().text = mech.Name;
                button.GetComponent<Button>().onClick.AddListener(() => OnMechSelected(mech));
                if (_selectedMech == mech)
                {
                    button.GetComponent<Image>().color = Color.green;
                }
            }
            _mechSelectionText.text = $"{_selectedMech.Name} selected. Select equip to modify it.";
        }
        _garageNavButton.GetComponent<Image>().color = new Color32(0xAE, 0x4D, 0x00, 0xFF); // Change the color of the garage nav button to indicate it's the current scene
        _mechSelectionPanel.SetActive(true);
    }

    void OnMechSelected(Mech mech)
    {
        _selectedMech = mech;
        // TODO: Possibly spawn the selected mech in the garage scene
        ShowMechSelectionPanel();
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
        _mechModNameText.text = _selectedMech.Name;
        DisplayMechInfo(); // Refresh the displayed info
        foreach (Transform child in _mechModOptionsPanel.transform)
        {
            Destroy(child.gameObject);
        }
        _itemTypes.Sort((a, b) => a.CompareTo(b));
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
            button.GetComponent<Button>().onClick.AddListener(async () => await OnItemSelected(button, weapon));
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
            button.GetComponent<Button>().onClick.AddListener(async () => await OnItemSelected(button, armor));
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
            button.GetComponent<Button>().onClick.AddListener(async () => await OnItemSelected(button, shield));
            if (_selectedMech.Inventory.HasItem(shield))
            {
                button.GetComponent<Image>().color = Color.green; // Indicate that the shield is already equipped
            }
        }
    }

    async Task OnItemSelected(GameObject itemObject, Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("Selected item is null.");
            return;
        } else if (item is Weapon)
        {
            Weapon weapon = item as Weapon;
            // Handle weapon selection
            if (item.Owner != null && item.Owner == _selectedMech)
            {
                _selectedMech.UnequipWeapon(weapon);
            } else if (item.Owner == null)
            {
                bool success = _selectedMech.EquipWeapon(weapon);
                if (!success)
                {
                    await HandleEquipFailure(itemObject);
                }
            } else
            {
                Debug.LogWarning("Weapon is already equipped by another mech.");
                return; // Exit the method if the weapon is already equipped by another mech
            }
        }
        else if (item is Armor)
        {
            Armor armor = item as Armor;
            // Handle armor selection
            if (item.Owner != null && item.Owner == _selectedMech)
            {
                _selectedMech.UnequipArmor(armor);
            } else if (item.Owner == null)
            {
                bool success = _selectedMech.EquipArmor(armor);
                if (!success)
                {
                    await HandleEquipFailure(itemObject);
                }
            } else
            {
                Debug.LogWarning("Armor is already equipped by another mech.");
                return; // Exit the method if the armor is already equipped by another mech
            }
        }
        else if (item is EnergyShield)
        {
            EnergyShield shield = item as EnergyShield;
            // Handle energy shield selection
            if (item.Owner != null && item.Owner == _selectedMech)
            {
                _selectedMech.UnequipEnergyShield(shield);
            } else if (item.Owner == null)
            {
                bool success = _selectedMech.EquipEnergyShield(shield);
                if (!success)
                {
                    await HandleEquipFailure(itemObject);
                }
            } else
            {
                Debug.LogWarning("Energy shield is already equipped by another mech.");
                return; // Exit the method if the energy shield is already equipped by another mech
            }
        }
        ShowMechModPanel(); // Refresh the mech mod panel to reflect the selection change
    }

    private async Task HandleEquipFailure(GameObject itemObject)
    {
        // Flash object red for a brief moment to indicate equip failure
        Image itemImage = itemObject.GetComponent<Image>();
        if (itemImage != null)
        {
            Color originalColor = itemImage.color;
            itemImage.color = Color.red;
            await Task.Delay(500); // Wait for 0.5 seconds
            itemImage.color = originalColor;
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
        if (item == null)
        {
            return;
        } else if (item is Weapon weapon)
        {
            // Display weapon tooltip for item and for all weapons in mechs inventory
            // Debug.Log($"Displaying tooltip for weapon: {weapon?.Name}");
            GameObject weaponTooltip = CreateWeaponTooltip(weapon);
            weaponTooltip.transform.SetParent(go.transform, false);
            weaponTooltip.transform.localPosition = GetInitialOffsetX(go) / _canvasRectTransform.localScale.x;
            Weapon[] weaponsInInventory = _selectedMech.Inventory.Weapons;
            for (int i = 0; i < weaponsInInventory.Length; i++)
            {
                Weapon w = weaponsInInventory[i];
                if (w == null) continue;
                if (weapon == w) continue;
                // Display tooltip for each weapon in inventory
                GameObject weaponListtooltip = CreateWeaponTooltip(w);
                // Position and show the tooltip as needed
                Vector3 wOffset = GetChildOffsetX(go);
                weaponListtooltip.transform.SetParent(go.transform, false);
                weaponListtooltip.transform.localPosition = wOffset / _canvasRectTransform.localScale.x;
            }
            
        }
        else if (item is Armor armor)
        {
            // Display armor tooltip for item and equivalent armor in mechs inventory
            GameObject armorTooltip = CreateArmorTooltip(armor);
            armorTooltip.transform.SetParent(go.transform, false);
            armorTooltip.transform.localPosition = GetInitialOffsetX(go) / _canvasRectTransform.localScale.x;
            Armor armorInInventory = _selectedMech.Inventory.Armors[(int)armor.ArmorType];
            if (armorInInventory == null) return;
            if (armorInInventory == armor) return;
            GameObject armorInInventoryTooltip = CreateArmorTooltip(armorInInventory);
            Vector3 aOffset = GetChildOffsetX(go);
            armorInInventoryTooltip.transform.SetParent(go.transform, false);
            armorInInventoryTooltip.transform.localPosition = aOffset / _canvasRectTransform.localScale.x;
            
        }
        else if (item is EnergyShield shield)
        {
            // Display energy shield tooltip for item and equivalent energy shield in mechs inventory
            GameObject shieldTooltip = CreateEnergyShieldTooltip(shield);
            shieldTooltip.transform.SetParent(go.transform, false);
            shieldTooltip.transform.localPosition = GetInitialOffsetX(go) / _canvasRectTransform.localScale.x;
            EnergyShield shieldInInventory = _selectedMech.Inventory.EnergyShield;
            if (shieldInInventory == null) return;
            if (shieldInInventory == shield) return;
            GameObject shieldInInventoryTooltip = CreateEnergyShieldTooltip(shieldInInventory);
            Vector3 sOffset = GetChildOffsetX(go);
            shieldInInventoryTooltip.transform.SetParent(go.transform, false);
            shieldInInventoryTooltip.transform.localPosition = sOffset / _canvasRectTransform.localScale.x;
            
        }
    }

    private Vector3 GetInitialOffsetX(GameObject go)
    {
        return new Vector3(go.GetComponent<RectTransform>().rect.width / 2 + TOOLTIP_OFFSET_X, go.GetComponent<RectTransform>().rect.height / 2, 0);
    }

    private Vector3 GetChildOffsetX(GameObject go)
    {
        Vector3 offset = GetInitialOffsetX(go);
        float offsetX = offset.x;
        Debug.Log($"Initial offset X for {go.name}: {offsetX}");
        foreach (Transform child in go.transform)
        {
            if (!child.gameObject.name.EndsWith("Tooltip"))
                continue;
            foreach (Transform grandChild in child)
            {
                if(!grandChild.gameObject.name.EndsWith("background"))
                    continue;
                offsetX += grandChild.GetComponent<RectTransform>().rect.width + TOOLTIP_OFFSET_X;
            }
        }
        Debug.Log($"Calculated child offset X for {go.name}: {offsetX}");
        return new Vector3(offsetX, offset.y, 0);
    }

    private GameObject CreateWeaponTooltip(Weapon weapon)
    {
        // Instantiate and configure the weapon tooltip GameObject
        GameObject tooltip = Instantiate(_hoverableItemTooltip);
        tooltip.name = "WeaponTooltip";
        // Add components and set up the tooltip as needed
        TooltipScreenSpaceUI tooltipUI = tooltip.GetComponent<TooltipScreenSpaceUI>();
        if (tooltipUI == null)
            tooltipUI = tooltip.AddComponent<TooltipScreenSpaceUI>();
        tooltipUI.DisplayTooltip(weapon.GetDescription());
        return tooltip;
    }

    private GameObject CreateArmorTooltip(Armor armor)
    {
        // Instantiate and configure the armor tooltip GameObject
        GameObject tooltip = Instantiate(_hoverableItemTooltip);
        tooltip.name = "ArmorTooltip";
        // Add components and set up the tooltip as needed
        TooltipScreenSpaceUI tooltipUI = tooltip.GetComponent<TooltipScreenSpaceUI>();
        if (tooltipUI == null)
            tooltipUI = tooltip.AddComponent<TooltipScreenSpaceUI>();
        tooltipUI.DisplayTooltip(armor.GetDescription());
        return tooltip;
    }

    private GameObject CreateEnergyShieldTooltip(EnergyShield shield)
    {
        // Instantiate and configure the energy shield tooltip GameObject
        GameObject tooltip = Instantiate(_hoverableItemTooltip);
        tooltip.name = "EnergyShieldTooltip";
        // Add components and set up the tooltip as needed
        TooltipScreenSpaceUI tooltipUI = tooltip.GetComponent<TooltipScreenSpaceUI>();
        if (tooltipUI == null)
            tooltipUI = tooltip.AddComponent<TooltipScreenSpaceUI>();
        tooltipUI.DisplayTooltip(shield.GetDescription());
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
