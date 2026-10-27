using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using HeartOfIron.Enums;

/// <summary>
/// Manages the UI for the EngiBay, including displaying available research items for crafting, item details, and handling crafting actions.
/// </summary>
public class EngiBayUIManager : MonoBehaviour
{
    public static EngiBayUIManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject engiBayNavButton;
    [SerializeField] private GameObject engiBayOptionsDisplayPanel;
    [SerializeField] private TextMeshProUGUI itemInfoText;
    [SerializeField] private TextMeshProUGUI itemCostText;
    [SerializeField] private GameObject researchButtonPrefab;
    private ItemBlueprint currentItemBlueprint;
    private List<ItemType> _itemTypes = new List<ItemType> { ItemType.Weapon, ItemType.Armor, ItemType.EnergyShield, ItemType.Mech };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        engiBayNavButton.GetComponent<Image>().color = new Color32(0xAE, 0x4D, 0x00, 0xFF); // Change the color of the EngiBay nav button to indicate it's the current scene
    }

    void Start()
    {
        ShowEngiBayOptions();
    }

    /// <summary>
    /// Displays the available research options in the EngiBay.
    /// </summary>
    public void ShowEngiBayOptions()
    {
        List<ItemBlueprint> availableResearch = GameManager.Instance.AvailableResearch;
        itemInfoText.text = "Select an item to see its details.";
        foreach (Transform child in engiBayOptionsDisplayPanel.transform)
        {
            Destroy(child.gameObject);
        }
        if (availableResearch.Count > 0)
        {
            // Debug.Log($"Displaying {availableResearch.Count} available research items.");
            foreach (ItemBlueprint itemBlueprint in availableResearch)
            {
                if (!_itemTypes.Contains(itemBlueprint.ItemType))
                {
                    // Debug.Log($"Skipping item {itemBlueprint.ItemName} of type {itemBlueprint.ItemType} due to filter.");
                    continue; // Skip items that don't match the current filter
                }
                GameObject researchButton = Instantiate(researchButtonPrefab, engiBayOptionsDisplayPanel.transform);
                researchButton.GetComponentInChildren<TextMeshProUGUI>().text = itemBlueprint.ItemName;
                researchButton.GetComponent<Button>().onClick.AddListener(() => OnResearchButtonClicked(itemBlueprint));
                if (currentItemBlueprint == itemBlueprint)
                {
                    researchButton.GetComponent<Image>().color = Color.green; // Highlight the selected research option
                }
            }
        }
        else
        {
            Debug.LogWarning("No available research items to display.");
        }
    }

    /// <summary>
    /// Handles the event when a research item button is clicked.
    /// </summary>
    /// <param name="itemBlueprint">The blueprint of the item that was clicked.</param>
    void OnResearchButtonClicked(ItemBlueprint itemBlueprint)
    {
        currentItemBlueprint = itemBlueprint;
        UpdateItemInfoDisplay();
    }

    /// <summary>
    /// Updates the item information display based on the currently selected item blueprint.
    /// </summary>
    void UpdateItemInfoDisplay()
    {
        if (currentItemBlueprint != null)
        {
            itemInfoText.text = currentItemBlueprint.GetBlueprintInfo();
            itemCostText.text = $"Bolts: {GameManager.Instance.Bolts} \nCost: {currentItemBlueprint.Cost}";
        }
        else
        {
            itemInfoText.text = "Select an item to see its details.";
            itemCostText.text = "";
        }
    }

    /// <summary>
    /// Handles the event when the craft button is clicked.
    /// </summary>
    public void OnCraftButtonClicked()
    {
        if (currentItemBlueprint != null && IsItemCraftable(currentItemBlueprint))
        {
            Item newItem = ItemAssembler.AssembleItem(currentItemBlueprint);
            if (newItem != null)
            {
                GameManager.Instance.AddItemToInventory(newItem);
                Debug.Log($"Crafted {currentItemBlueprint.ItemName} and added to inventory.");
            }
            else
            {
                if (currentItemBlueprint is MechBlueprint)
                {
                    Debug.Log($"Crafted {currentItemBlueprint.ItemName} (Mech) and added to inventory.");
                }
                else
                {
                    Debug.LogWarning($"Failed to craft {currentItemBlueprint.ItemName}. ItemAssembler returned null.");
                }
            }
            // Debug.Log($"Crafting {currentItemBlueprint.ItemName}...");
        }
        else
        {
            Debug.LogWarning("No item selected for crafting or insufficient resources.");
        }
    }

    /// <summary>
    /// Determines whether the specified item blueprint can be crafted based on available resources.
    /// </summary>
    /// <param name="itemBlueprint">The blueprint of the item to check.</param>
    /// <returns>True if the item can be crafted; otherwise, false.</returns>
    private bool IsItemCraftable(ItemBlueprint itemBlueprint)
    {
        // Check if the player has enough resources to craft the item
        return GameManager.Instance.Bolts >= itemBlueprint.Cost;
    }

    /// <summary>
    /// Handles the event when the weapon filter button is clicked.
    /// </summary>
    /// <param name="button">The button that was clicked.</param>
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
        ShowEngiBayOptions(); // Refresh the options display to reflect the filter change
    }

    /// <summary>
    /// Handles the event when the armor filter button is clicked.
    /// </summary>
    /// <param name="button">The button that was clicked.</param>
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
        ShowEngiBayOptions(); // Refresh the options display to reflect the filter change
    }

    /// <summary>
    /// Handles the event when the energy shield filter button is clicked.
    /// </summary>
    /// <param name="button">The button that was clicked.</param>
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
        ShowEngiBayOptions(); // Refresh the options display to reflect the filter change
    }

    /// <summary>
    /// Handles the event when the mech filter button is clicked.
    /// </summary>
    /// <param name="button">The button that was clicked.</param>
    public void OnMechFilterButtonClicked(GameObject button)
    {
        if (_itemTypes.Contains(ItemType.Mech))
        {
            _itemTypes.Remove(ItemType.Mech);
        } else
        {
            _itemTypes.Add(ItemType.Mech);
        }
        button.GetComponent<Image>().color = _itemTypes.Contains(ItemType.Mech) ? new Color32(0xE5, 0x8E, 0x26, 0xFF) : new Color32(0xAE, 0x4D, 0x00, 0xFF); // Change button color based on filter state
        ShowEngiBayOptions(); // Refresh the options display to reflect the filter change
    }
}
