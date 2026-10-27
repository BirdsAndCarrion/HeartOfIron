using UnityEngine;
using TMPro;
using UnityEngine.UI;
using HeartOfIron.Enums;
using System.Collections.Generic;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the Mess Hall UI, including displaying pilot options, showing pilot info, and handling mech assignments.
/// </summary>
public class MessHallUIManager : MonoBehaviour
{

    public static MessHallUIManager Instance { get; private set; }

    [SerializeField] private GameObject messHallNavButton;
    [SerializeField] private GameObject pilotOptionsDisplayPanel;
    [SerializeField] private GameObject pilotButtonPrefab;
    [SerializeField] private TMP_InputField pilotNameField;
    [SerializeField] private TextMeshProUGUI pilotLvlText;
    [SerializeField] private TMP_Dropdown assignedMechDropdown;
    [SerializeField] private TextMeshProUGUI pilotExpText;
    [SerializeField] private TextMeshProUGUI pilotUnspentSkillPointsText;
    [SerializeField] private List<TextMeshProUGUI> pilotSkillTexts;
    [SerializeField] private List<TextMeshProUGUI> pilotSkillBonusTexts;
    [SerializeField] private GameObject lilGuyPrefab;
    [SerializeField] private PlayerInput pi;
    [SerializeField] private Terrain terrain;
    private Pilot selectedPilot;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        messHallNavButton.GetComponent<Image>().color = new Color32(0xAE, 0x4D, 0x00, 0xFF); // Change the color of the Mess Hall nav button to indicate it's the current scene
    }

    void Start()
    {
        assignedMechDropdown.onValueChanged.AddListener(delegate { OnAssignedMechChanged(); });
        assignedMechDropdown.ClearOptions();
        ShowPilotOptions();
    }

    /// <summary>
    /// Displays the list of available pilots as buttons in the pilot options display panel.
    /// </summary>
    /// <remarks>
    /// This method clears any existing pilot buttons and creates new ones for each pilot in the player's roster.
    /// It also spawns a lilGuy for each pilot.
    /// </remarks>
    void ShowPilotOptions()
    {
        foreach (Transform child in pilotOptionsDisplayPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Pilot pilot in UnitManager.Instance.PlayerPilots)
        {
            GameObject pilotButton = Instantiate(pilotButtonPrefab, pilotOptionsDisplayPanel.transform);
            pilotButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = pilot.Name;
            pilotButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnPilotButtonClicked(pilot));
            SpawnLilGuy(pilot); // Spawn a lilGuy for each pilot
        }
    }

    /// <summary>
    /// Handles the event when a pilot button is clicked and updates the selected pilot.
    /// </summary>
    /// <param name="pilot">The pilot that was clicked.</param>
    void OnPilotButtonClicked(Pilot pilot)
    {
        selectedPilot = pilot;
        ShowPilotInfo(pilot);
    }

    /// <summary>
    /// Displays detailed information about the selected pilot, including skills and assigned mech.
    /// </summary>
    /// <param name="pilot">The pilot whose information is to be displayed.</param>
    void ShowPilotInfo(Pilot pilot)
    {
        pilotNameField.text = pilot.Name;
        pilotLvlText.text = $"Level: {pilot.Level}";
        pilotExpText.text = $"Experience: {pilot.Experience}";
        pilotUnspentSkillPointsText.text = $"Available Skill Points: {pilot.UnspentSkillPoints}";

        for (int i = 0; i < pilotSkillTexts.Count; i++)
        {
            if (i < pilot.Skills.Length)
            {
                pilotSkillTexts[i].text = $"Level: {pilot.Skills[i].level}";
                pilotSkillBonusTexts[i].text = $"Bonus: {pilot.Skills[i].bonus}";
            }
            else
            {
                pilotSkillTexts[i].text = "";
                pilotSkillBonusTexts[i].text = "";
            }
        }
        ShowAssignedMechDropdown();
    }

    /// <summary>
    /// Displays the dropdown for assigning a mech to the selected pilot.
    /// </summary>
    private void ShowAssignedMechDropdown()
    {
        assignedMechDropdown.ClearOptions();
        List<string> mechNames = new List<string>();
        foreach (Mech mech in UnitManager.Instance.PlayerMechs)
        {
            mechNames.Add(mech.Name);
        }
        assignedMechDropdown.AddOptions(mechNames);

        // Add option for "None" at end of list to unassign the pilot from any mech
        assignedMechDropdown.options.Add(new TMP_Dropdown.OptionData("None"));

        if (selectedPilot.AssignedMech != null)
        {
            int index = UnitManager.Instance.PlayerMechs.IndexOf(selectedPilot.AssignedMech);
            if (index >= 0)
            {
                assignedMechDropdown.value = index;
            }
        } else
        {
            Debug.Log("No mech assigned to the selected pilot. Setting dropdown to 'None'.");
            assignedMechDropdown.value = assignedMechDropdown.options.Count - 1; // Set to "None" if no mech is assigned
        }
    }

    /// <summary>
    /// Handles the event when the level up skill button is clicked for a specific skill of the selected pilot.
    /// </summary>
    /// <param name="skillIndex">The index of the skill to level up.</param>
    public void OnLevelUpSkillButtonClicked(int skillIndex)
    {
        if (selectedPilot != null)
        {
            SkillType skillType = selectedPilot.Skills[skillIndex].SkillType;
            selectedPilot.LevelUpSkill(skillType);
            ShowPilotInfo(selectedPilot); // Refresh the displayed info
        }
    }

    /// <summary>
    /// Handles the event when the increase skill bonus button is clicked for a specific skill of the selected pilot.
    /// </summary>
    /// <param name="skillIndex">The index of the skill to increase the bonus for.</param>
    public void OnIncreaseSkillBonusButtonClicked(int skillIndex)
    {
        if (selectedPilot != null)
        {
            SkillType skillType = selectedPilot.Skills[skillIndex].SkillType;
            selectedPilot.IncreaseSkillBonus(skillType);
            ShowPilotInfo(selectedPilot); // Refresh the displayed info
        }
    }

    /// <summary>
    /// Spawns a lilGuy prefab representing the specified pilot at a random position within the terrain bounds.
    /// </summary>
    /// <param name="pilot">The pilot to represent with the lilGuy prefab.</param>
    void SpawnLilGuy(Pilot pilot)
    {
        // Spawn a lilGuy prefab at a random position within the terrain bounds
        GameObject lilGuy = Instantiate(lilGuyPrefab);
        lilGuy.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // Scale down the lilGuy for better visibility
        Vector3 randomPosition = new Vector3(
            UnityEngine.Random.Range(0, terrain.terrainData.size.x),
            0,
            UnityEngine.Random.Range(0, terrain.terrainData.size.z)
        );
        lilGuy.transform.position = randomPosition;
        // Set the lilGuy's name to the pilot's name for identification
        lilGuy.name = pilot.Name;
        // Set lilGuy's color based on the pilot's color
        Renderer lilGuyRenderer = lilGuy.GetComponent<Renderer>();
        if (lilGuyRenderer != null)
        {
            lilGuyRenderer.material.color = pilot.Color;
        }
        // TODO: Add a component to the lilGuy prefab that allows it to display the pilot's name above its head, if desired.
        // TODO: Add a component to the lilGuy prefab that gives it a basic state machine for animations and behaviors, if desired.
    }

    /// <summary>
    /// Updates the name of the selected pilot based on the input field and updates the corresponding lilGuy in the scene.
    /// </summary>
    public void UpdatePilotName()
    {
        if (selectedPilot != null)
        {
            GameObject lilGuy = GameObject.Find(selectedPilot.Name);
            selectedPilot.UpdateName(pilotNameField.text.Trim());
            ShowPilotInfo(selectedPilot); // Refresh the displayed info
            ShowPilotOptions(); // Refresh the pilot options to reflect the name change
            // Update the lilGuy's name in the scene
            if (lilGuy != null)
            {
                lilGuy.name = selectedPilot.Name;
            }
        }
    }

    /// <summary>
    /// Switches the controls to typing mode, deactivating other input.
    /// </summary>
    public void SwitchControlsToTyping()
    {
        pi.DeactivateInput(); // Deactivate other controls to allow typing
        Debug.Log("Switched controls to typing mode.");
    }

    /// <summary>
    /// Switches from typing mode back to controls, reactivating other input.
    /// </summary>
    public void SwitchTypingToControls()
    {
        pi.ActivateInput(); // Reactivate other controls after typing
        Debug.Log("Switched typing mode to controls.");
    }

    /// <summary>
    /// Handles the event when the assigned mech dropdown value is changed for the selected pilot.
    /// </summary>
    public void OnAssignedMechChanged()
    {
        if (selectedPilot != null && assignedMechDropdown != null)
        {
            int index = assignedMechDropdown.value;
            if (index >= 0 && index < UnitManager.Instance.PlayerMechs.Count)
            {
                selectedPilot.AssignedMech = UnitManager.Instance.PlayerMechs[index];
            } else if (index == assignedMechDropdown.options.Count - 1) // "None" option
            {
                selectedPilot.AssignedMech = null;
            }
        }
    }
}
