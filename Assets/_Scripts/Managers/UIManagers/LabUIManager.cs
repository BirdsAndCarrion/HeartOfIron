using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using HeartOfIron.Enums;


public class LabUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _labNavButton;
    [SerializeField] private GameObject _researchOptionsPanel;
    [SerializeField] private GameObject _researchOptionPrefab;
    [SerializeField] private TextMeshProUGUI _researchInfoText;
    private Item _selectedItem;
    private List<ItemType> _itemTypes = new List<ItemType> { ItemType.Weapon, ItemType.Armor, ItemType.EnergyShield };

    void Start()
    {
        _labNavButton.GetComponent<Image>().color = new Color32(0xAE, 0x4D, 0x00, 0xFF); // Change the color of the Lab nav button to indicate it's the current scene
        UpdateResearchOptions();
    }

    void UpdateResearchOptions()
    {
        // Clear existing options
        _researchInfoText.text = "Select an item to see its details.";
        foreach (Transform child in _researchOptionsPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Populate with available items for research
        foreach (Item item in GameManager.Instance.PlayerInventory)
        {
            if (item.Owner != null || !_itemTypes.Contains(item.Type))
            {
                // Debug.Log($"Skipping item {item.Name} of type {item.Type} due to filter or ownership.");
                continue; // Skip items that are already owned or don't match the current filter
            }
            var option = Instantiate(_researchOptionPrefab, _researchOptionsPanel.transform);
            var button = option.GetComponent<Button>();
            var text = option.GetComponentInChildren<TextMeshProUGUI>();

            text.text = item.Name;
            button.onClick.AddListener(() => OnResearchOptionSelected(item));
        }
    }

    void OnResearchOptionSelected(Item item)
    {
        _researchInfoText.text = item.GetDescription();
        // Here you would typically start the research process, deduct resources, etc.
        _selectedItem = item;
        // Debug.Log($"Selected research option: {item.Name}");
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
        UpdateResearchOptions();
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
        UpdateResearchOptions();
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
        UpdateResearchOptions();
    }

    public void OnResearchButtonClicked()
    {
        // This method can be used to handle the research button click event
        // For example, you might want to start the research process for the selected blueprint
        if (_selectedItem != null)
        {
            // Debug.Log($"Research button clicked for item: {_selectedItem.Name}");
            // Start research process here
            GameManager.Instance.ResearchItem(_selectedItem);
            UpdateResearchOptions(); // Refresh the options after research
        }
        else
        {
            Debug.LogWarning("No item selected for research.");
        }
        // Debug.Log("Research button clicked.");
    }

}
