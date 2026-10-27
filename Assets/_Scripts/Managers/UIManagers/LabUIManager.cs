using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using HeartOfIron.Enums;

/// <summary>
/// Manages the UI for the laboratory, including research options, item filtering, and research functionality.
/// </summary>
public class LabUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _labNavButton;
    [SerializeField] private GameObject _researchOptionsPanel;
    [SerializeField] private GameObject _researchOptionPrefab;
    [SerializeField] private TextMeshProUGUI _researchInfoText;
    private Item _selectedItem;
    /// <summary>
    /// The list of item types currently selected for filtering in the research options.
    /// </summary>
    private List<ItemType> _itemTypes = new List<ItemType> { ItemType.Weapon, ItemType.Armor, ItemType.EnergyShield };

    void Start()
    {
        _labNavButton.GetComponent<Image>().color = new Color32(0xAE, 0x4D, 0x00, 0xFF); // Change the color of the Lab nav button to indicate it's the current scene
        UpdateResearchOptions();
    }

    /// <summary>
    /// Shows and updates the available research options based on the current item type filters.
    /// </summary>
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
            if (_selectedItem == item)
            {
                option.GetComponent<Image>().color = Color.green; // Highlight the selected research option
            }
        }
    }

    /// <summary>
    /// Handles the event when a research option is selected.
    /// Updates the research info text and sets the selected item.
    /// </summary>
    /// <param name="item">The item that was selected for research.</param>
    void OnResearchOptionSelected(Item item)
    {
        _researchInfoText.text = item.GetDescription();
        // Here you would typically start the research process, deduct resources, etc.
        _selectedItem = item;
        // Debug.Log($"Selected research option: {item.Name}");
    }

    /// <summary>
    /// Handles the event when the weapon filter button is clicked and updates the filter state.
    /// </summary>
    /// <param name="button">The GameObject representing the weapon filter button that was clicked.</param>
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

    /// <summary>
    /// Handles the event when the armor filter button is clicked and updates the filter state.
    /// </summary>
    /// <param name="button">The GameObject representing the armor filter button that was clicked.</param>
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

    /// <summary>
    /// Handles the event when the energy shield filter button is clicked and updates the filter state.
    /// </summary>
    /// <param name="button">The GameObject representing the energy shield filter button that was clicked.</param>
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

    /// <summary>
    /// Handles the event when the research button is clicked and initiates the research process for the selected item.
    /// </summary>
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
