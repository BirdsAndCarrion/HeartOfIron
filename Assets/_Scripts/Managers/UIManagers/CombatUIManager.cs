using UnityEngine;
using System;
using TMPro;
using HeartOfIron.Enums;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Manages the UI elements within the combat scene, including mech info displays, action selection menus, and combat feedback.
/// Meant to provide a centralized interface for updating and controlling combat-related UI elements.
/// Works in conjunction with CombatManager to reflect the current state of combat.
/// </summary>
public class CombatUIManager : MonoBehaviour
{

    public static CombatUIManager Instance { get; private set; }

    [Header("Mech Info Panel Elements")]
    [SerializeField] private GameObject _mechInfoPanel;
    [SerializeField] private TextMeshProUGUI _mechNameText;
    [SerializeField] private TextMeshProUGUI _pilotNameText;
    [SerializeField] private TextMeshProUGUI _mechTypeLevelText;
    [Header("Weapon Info Panel Elements")]
    [SerializeField] private GameObject _weaponPanel;
    [SerializeField] private TextMeshProUGUI _weaponNameText;
    [SerializeField] private TextMeshProUGUI _weaponAmmoText;
    [SerializeField] private GameObject _weapon2Panel;
    [SerializeField] private TextMeshProUGUI _weapon2NameText;
    [SerializeField] private TextMeshProUGUI _weapon2AmmoText;
    [Header("Mech Health Panel Elements")]
    [SerializeField] private GameObject _mechHealthPanel;
    [SerializeField] private GameObject _headHealthBar;
    [SerializeField] private GameObject _torsoHealthBar;
    [SerializeField] private GameObject _leftArmHealthBar;
    [SerializeField] private GameObject _rightArmHealthBar;
    [SerializeField] private GameObject _leftLegHealthBar;
    [SerializeField] private GameObject _rightLegHealthBar;
    [Header("Mech Armor Panel Elements")]
    [SerializeField] private GameObject _mechArmorPanel;
    [SerializeField] private GameObject _headArmorBar;
    [SerializeField] private GameObject _torsoArmorBar;
    [SerializeField] private GameObject _leftArmArmorBar;
    [SerializeField] private GameObject _rightArmArmorBar;
    [SerializeField] private GameObject _leftLegArmorBar;
    [SerializeField] private GameObject _rightLegArmorBar;
    [Header("Mech Energy Shield Panel Elements")]
    [SerializeField] private GameObject _mechEnergyShieldPanel;
    [SerializeField] private GameObject _energyShieldBar;

    [Header("Pop-up Elements")]
    [SerializeField] private GameObject _popupPanel;
    [SerializeField] private TextMeshProUGUI _popupText;
    [SerializeField] private TMP_Dropdown _weaponDropdown;
    [SerializeField] private TMP_Dropdown _targetDropdown;
    [SerializeField] private Button _confirmButton;

    [Header("Action Selection Elements")]
    [SerializeField] private GameObject _actionSelectionPanel;
    [SerializeField] private GameObject _shootButton;
    [SerializeField] private GameObject _meleeButton;
    [SerializeField] private GameObject _reloadButton;
    [SerializeField] private GameObject _moveButton;
    [SerializeField] private GameObject _chargeButton;
    [SerializeField] private GameObject _hunkerButton;
    [SerializeField] private GameObject _overwatchButton;

    [Header("Bottom Bar Elements")]
    [SerializeField] private GameObject _bottomBarPanel;
    [SerializeField] private GameObject _timerText;
    [SerializeField] private Sprite _pauseIcon;
    [SerializeField] private Sprite _playIcon;
    [SerializeField] private GameObject _pauseButton;

    [Header("Deployment Elements")]
    [SerializeField] private GameObject _deploymentPanel;
    [SerializeField] private GameObject _deployButton;
    [SerializeField] private GameObject _ghostUnitPrefab;
    [SerializeField] private Material _validPlacementMaterial;
    [SerializeField] private Material _invalidPlacementMaterial;
    // [SerializeField] private GameObject _deploymentAreaPrefab;

    [Header("Exit Combat Elements")]
    [SerializeField] private GameObject _exitCombatPanel;
    [SerializeField] private TextMeshProUGUI _exitDeclarationText;
    [SerializeField] private TextMeshProUGUI _exitRewardsText;

    private MechController _selectedMechController;
    private ActionMarker _selectedActionMarker;
    private CombatState _currentCombatState;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _mechInfoPanel.SetActive(false);
        _popupPanel.SetActive(false);
        _actionSelectionPanel.SetActive(false);
        _bottomBarPanel.SetActive(false);
        _deploymentPanel.SetActive(false);
        _exitCombatPanel.SetActive(false);
        EventRouter.ROnCombatStarted.AddListener(EnterCombat);
        EventRouter.ROnCombatEnded.AddListener(ExitCombat);
        EventRouter.ROnDeploymentStarted.AddListener(ShowDeploymentPanel);
        // EventRouter.ROnDeploymentStarted.AddListener(TestEventRecieved);
        EventRouter.ROnCombatStateChanged.AddListener(HandleCombatStateChanged);
        EventRouter.ROnActionMarkerSelected.AddListener(OnSelected);
    }

    void OnDestroy()
    {
        EventRouter.ROnCombatStarted.RemoveListener(EnterCombat);
        EventRouter.ROnCombatEnded.RemoveListener(ExitCombat);
        EventRouter.ROnDeploymentStarted.RemoveListener(ShowDeploymentPanel);
        // EventRouter.ROnDeploymentStarted.RemoveListener(TestEventRecieved);
        EventRouter.ROnCombatStateChanged.RemoveListener(HandleCombatStateChanged);
        EventRouter.ROnActionMarkerSelected.RemoveListener(OnSelected);
    }
    
    void Update()
    {
        if (_currentCombatState == CombatState.Playing)
        {
            _timerText.SetActive(true);
            _timerText.GetComponent<TextMeshProUGUI>().text = $"{CombatManager.Instance.CombatTimer:F2}s";
        }
    }

    /// <summary>
    /// Handles changes in the combat state and updates the internal state accordingly.
    /// Can be used to trigger UI updates or other responses based on the new combat state.
    /// </summary>
    /// <param name="newState">The new state of the combat.</param>
    private void HandleCombatStateChanged(CombatState newState)
    {
        _currentCombatState = newState;
        if (newState == CombatState.Paused)
        {
            OnGamePaused();
        }
        else if (newState == CombatState.Playing)
        {
            OnGameResumed();
        }
    }

    /// <summary>
    /// Handles the test event for debugging purposes. Logs a message to the console.
    /// This method is primarily used to verify that event routing and listener registration are functioning correctly.
    /// </summary>
    private void TestEventRecieved()
    {
        Debug.Log("Test event received in CombatUIManager");
    }

    /// <summary>
    /// Displays the deployment panel for the player to position their mechs before combat begins.
    /// Clears any existing deployment buttons and creates new ones for each player mech.
    /// </summary>
    public void ShowDeploymentPanel()
    {
        Debug.Log("Showing deployment panel");
        foreach (Transform child in _deploymentPanel.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Mech mech in UnitManager.Instance.PlayerMechs)
        {
            if (mech == null || mech.Pilot == null)
            {
                Debug.LogWarning("Null mech found in PlayerMechs list. Skipping.");
                continue;
            }
            Debug.Log($"Creating deploy button for mech: {mech.Name}");
            GameObject mechObject = GameObject.Instantiate(_deployButton, _deploymentPanel.transform);
            mechObject.GetComponentInChildren<TextMeshProUGUI>().text = mech.Name;
            DraggableUnit dU = mechObject.AddComponent<DraggableUnit>();
            dU.Initialize(mech, _ghostUnitPrefab, _validPlacementMaterial, _invalidPlacementMaterial);
        }
        _deploymentPanel.SetActive(true);
    }

    /// <summary>
    /// Transitions the UI from the deployment phase to the combat phase.
    /// Clears the deployment panel and activates the bottom bar panel for combat.
    /// </summary>
    public void EnterCombat()
    {
        foreach (Transform child in _deploymentPanel.transform)
        {
            Destroy(child.gameObject);
        }
        _deploymentPanel.SetActive(false);
        _bottomBarPanel.SetActive(true);
    }

    /// <summary>
    /// Handles the selection of a mech controller.
    /// Updates the selected mech controller and action marker, and displays the relevant UI panels.
    /// </summary>
    /// <param name="mechController">The mech controller that was selected.</param>
    public void OnSelected(MechController mechController)
    {
        Debug.Log($"MechController selected: {mechController.Mech.Name}");
        _selectedMechController = mechController;
        _selectedActionMarker = ActionMarkerAssembler.Instance.AssembleActionMarker(mechController.transform.position, mechController, false);
        _selectedMechController.InsertActionMarkerAt(_selectedActionMarker, 0);
        ShowMechInfoPanel(mechController);
        ShowActionSelectionPanel();
    }

    /// <summary>
    /// Handles the selection of an action marker.
    /// Updates the selected mech controller and action marker, and displays the relevant UI panels.
    /// </summary>
    /// <param name="actionMarker">The action marker that was selected.</param>
    public void OnSelected(ActionMarker actionMarker)
    {
        Debug.Log($"ActionMarker selected: {actionMarker.Id}, Source Mech: {actionMarker.ActionPair.MovementAction.SourceMech.Mech.Name}");
        _selectedMechController = actionMarker.ActionPair.MovementAction.SourceMech;
        _selectedActionMarker = actionMarker;
        ShowMechInfoPanel(actionMarker.ActionPair.MovementAction.SourceMech);
        ShowActionSelectionPanel();
    }

    /// <summary>
    /// Handles the deselection of a mech or action marker.
    /// Hides the mech info and action selection panels.
    /// </summary>
    void OnDeselected()
    {
        HideMechInfoPanel();
        HideActionSelectionPanel();
    }

    /// <summary>
    /// Handles the game pause event.
    /// Updates the pause button to display the play icon and sets its click listener to resume the game.
    /// </summary>
    public void OnGamePaused()
    {
        _pauseButton.GetComponent<Image>().sprite = _playIcon;
        _pauseButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _pauseButton.GetComponent<Button>().onClick.AddListener(OnPlayButtonClicked);
    }

    /// <summary>
    /// Handles the game resume event.
    /// Updates the pause button to display the pause icon and sets its click listener to pause the game.
    /// </summary>
    public void OnGameResumed()
    {
        _pauseButton.GetComponent<Image>().sprite = _pauseIcon;
        _pauseButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _pauseButton.GetComponent<Button>().onClick.AddListener(OnPauseButtonClicked);
    }

    /// <summary>
    /// Displays the mech info panel with details about the specified mech controller.
    /// Updates the weapon, health, armor, and energy shield panels based on the mech's inventory and type.
    /// </summary>
    /// <param name="mechController">The mech controller whose information is to be displayed.</param>
    void ShowMechInfoPanel(MechController mechController)
    {
        _mechInfoPanel.SetActive(true);
        _mechNameText.text = mechController.Mech.Name;
        //_pilotNameText.text = mechController.Pilot.Name;
        _mechTypeLevelText.text = $"{mechController.Mech.Type} - Level {mechController.Mech.Level}";
        if (mechController.Mech.Inventory.Weapons[(int)Hand.Right] != null)
        {
            _weaponNameText.text = mechController.Mech.Inventory.Weapons[(int)Hand.Right].Name;
            _weaponAmmoText.text = $"Ammo: {mechController.Mech.Inventory.Weapons[(int)Hand.Right].CurrentAmmo}/{mechController.Mech.Inventory.Weapons[(int)Hand.Right].MaxAmmo}";
            _weaponPanel.SetActive(true);
        }
        else
        {
            _weaponNameText.text = "No Weapon";
            _weaponAmmoText.text = "Ammo: N/A";
            _weaponPanel.SetActive(false);
        }
        if (mechController.Mech.Inventory.Weapons[(int)Hand.Left] != null)
        {
            _weapon2NameText.text = mechController.Mech.Inventory.Weapons[(int)Hand.Left].Name;
            _weapon2AmmoText.text = $"Ammo: {mechController.Mech.Inventory.Weapons[(int)Hand.Left].CurrentAmmo}/{mechController.Mech.Inventory.Weapons[(int)Hand.Left].MaxAmmo}";
            _weapon2Panel.SetActive(true);
        }
        else
        {
            _weapon2NameText.text = "No Weapon";
            _weapon2AmmoText.text = "Ammo: N/A";
            _weapon2Panel.SetActive(false);
        }

        if (mechController.Mech.Type == MechType.Light)
        {
            _mechHealthPanel.SetActive(true);
            UpdateHealthBars(mechController);
            _mechArmorPanel.SetActive(false);
            _mechEnergyShieldPanel.SetActive(true);
            UpdateEnergyShieldBar(mechController);
        }
        else if (mechController.Mech.Type == MechType.Medium)
        {
            _mechHealthPanel.SetActive(true);
            UpdateHealthBars(mechController);
            _mechArmorPanel.SetActive(true);
            UpdateArmorBars(mechController);
            _mechEnergyShieldPanel.SetActive(true);
            UpdateEnergyShieldBar(mechController);
        }
        else if (mechController.Mech.Type == MechType.Heavy)
        {
            _mechHealthPanel.SetActive(true);
            UpdateHealthBars(mechController);
            _mechArmorPanel.SetActive(true);
            UpdateArmorBars(mechController);
            _mechEnergyShieldPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Updates the health bars for each body part of the specified mech controller.
    /// </summary>
    /// <param name="mechController">The mech controller whose health bars are to be updated.</param>
    private void UpdateHealthBars(MechController mechController)
    {
        UpdateResourceBar(mechController.Mech.Health[(int)BodyPart.Head], mechController.Mech.MaxHealth[(int)BodyPart.Head], _headHealthBar);
        UpdateResourceBar(mechController.Mech.Health[(int)BodyPart.Torso], mechController.Mech.MaxHealth[(int)BodyPart.Torso], _torsoHealthBar);
        UpdateResourceBar(mechController.Mech.Health[(int)BodyPart.RightArm], mechController.Mech.MaxHealth[(int)BodyPart.RightArm], _rightArmHealthBar);
        UpdateResourceBar(mechController.Mech.Health[(int)BodyPart.LeftArm], mechController.Mech.MaxHealth[(int)BodyPart.LeftArm], _leftArmHealthBar);
        UpdateResourceBar(mechController.Mech.Health[(int)BodyPart.RightLeg], mechController.Mech.MaxHealth[(int)BodyPart.RightLeg], _rightLegHealthBar);
        UpdateResourceBar(mechController.Mech.Health[(int)BodyPart.LeftLeg], mechController.Mech.MaxHealth[(int)BodyPart.LeftLeg], _leftLegHealthBar);
    }

    /// <summary>
    /// Updates the armor bars for each body part of the specified mech controller.
    /// </summary>
    /// <param name="mechController">The mech controller whose armor bars are to be updated.</param>
    private void UpdateArmorBars(MechController mechController)
    {
        Armor[] armors = mechController.GetInventory().Armors;
        if (armors[(int)ArmorType.Head] != null)
        {
            UpdateResourceBar(armors[(int)ArmorType.Head].GetDurability(), armors[(int)ArmorType.Head].MaxDurability, _headArmorBar);
        }
        else
        {
            UpdateResourceBar(0, 1, _headArmorBar); // Set to empty if no armor
        }
        if (armors[(int)ArmorType.Torso] != null)
        {
            UpdateResourceBar(armors[(int)ArmorType.Torso].GetDurability(), armors[(int)ArmorType.Torso].MaxDurability, _torsoArmorBar);
        }
        else
        {
            UpdateResourceBar(0, 1, _torsoArmorBar); // Set to empty if no armor
        }
        if (armors[(int)ArmorType.Arms] != null)
        {
            UpdateResourceBar(armors[(int)ArmorType.Arms].GetDurability(1), armors[(int)ArmorType.Arms].MaxDurability, _rightArmArmorBar);
            UpdateResourceBar(armors[(int)ArmorType.Arms].GetDurability(0), armors[(int)ArmorType.Arms].MaxDurability, _leftArmArmorBar);
        }
        else
        {
            UpdateResourceBar(0, 1, _rightArmArmorBar); // Set to empty if no armor
            UpdateResourceBar(0, 1, _leftArmArmorBar); // Set to empty if no armor
        }
        if (armors[(int)ArmorType.Legs] != null)
        {
            UpdateResourceBar(armors[(int)ArmorType.Legs].GetDurability(0), armors[(int)ArmorType.Legs].MaxDurability, _leftLegArmorBar);
            UpdateResourceBar(armors[(int)ArmorType.Legs].GetDurability(1), armors[(int)ArmorType.Legs].MaxDurability, _rightLegArmorBar);
        }
        else
        {
            UpdateResourceBar(0, 1, _leftLegArmorBar); // Set to empty if no armor
            UpdateResourceBar(0, 1, _rightLegArmorBar); // Set to empty if no armor
        }
    }

    /// <summary>
    /// Updates the energy shield bar for the specified mech controller.
    /// </summary>
    /// <param name="mechController">The mech controller whose energy shield bar is to be updated.</param>
    private void UpdateEnergyShieldBar(MechController mechController)
    {
        EnergyShield energyShield = mechController.GetInventory().EnergyShield;
        if (energyShield != null)
        {
            UpdateResourceBar(energyShield.CurrentCapacity, energyShield.MaxCapacity, _energyShieldBar);
        }
        else
        {
            UpdateResourceBar(0, 1, _energyShieldBar); // Set to empty if no energy shield
        }
    }

    /// <summary>
    /// Updates the specified resource bar with the current and maximum values.
    /// </summary>
    /// <param name="current">The current value of the resource.</param>
    /// <param name="max">The maximum value of the resource.</param>
    /// <param name="barObject">The GameObject containing the ResourceBar component to be updated.</param>
    private void UpdateResourceBar(float current, float max, GameObject barObject)
    {
        ResourceBar resourceBar = barObject.GetComponent<ResourceBar>();
        if (resourceBar != null)
        {
            resourceBar.SetResourceMax(max);
            resourceBar.SetResourceCurrent(current);
        }
        else
        {
            Debug.LogWarning($"ResourceBar component not found on {barObject.name}");
        }
    }

    /// <summary>
    /// Hides the mech info panel.
    /// </summary>
    void HideMechInfoPanel()
    {
        _mechInfoPanel.SetActive(false);
    }

    /// <summary>
    /// Shows a popup message and updates the UI elements based on the message type.
    /// </summary>
    /// <param name="message">The message to be displayed in the popup.</param>
    void ShowPopup(string message)
    {
        _popupText.text = message;
        switch (message)
        {
            case "Shoot":
                _weaponDropdown.gameObject.SetActive(true);
                _targetDropdown.gameObject.SetActive(true);
                _weaponDropdown.ClearOptions();
                _targetDropdown.ClearOptions();
                
                for (int i = 0; i < _selectedMechController.Mech.Inventory.Weapons.Length; i++)
                {
                    Weapon weapon = _selectedMechController.Mech.Inventory.Weapons[i];
                    if (weapon != null && !weapon.IsMelee)
                    {
                        _weaponDropdown.options.Add(new TMP_Dropdown.OptionData(weapon.Name));
                    }
                }
                if (_weaponDropdown.options.Count > 1)
                {
                    _weaponDropdown.options.Add(new TMP_Dropdown.OptionData("All"));
                }
                if (_weaponDropdown.options.Count > 0)
                {
                    _weaponDropdown.SetValueWithoutNotify(0);
                }
                _weaponDropdown.RefreshShownValue();
                _weaponDropdown.onValueChanged.AddListener(delegate { UpdateTargetDropdownRanged(); });
                UpdateTargetDropdownRanged();
                _confirmButton.onClick.RemoveAllListeners();
                _confirmButton.onClick.AddListener(() =>
                {
                    List<RangedAttack> shootingWeapons = new List<RangedAttack>();
                    if (_weaponDropdown.options[_weaponDropdown.value].text == "All")
                    {
                        foreach (RangedAttack rangedAttack in _selectedMechController.Mech.Inventory.RangedAttacks)
                        {
                            if (rangedAttack != null)
                            {
                                shootingWeapons.Add(rangedAttack);
                            }
                        }
                    }
                    else
                    {
                        RangedAttack selectedRangedAttack = DetermineRangedAttackByDropdownSelection();
                        if (selectedRangedAttack != null)
                        {
                            shootingWeapons.Add(selectedRangedAttack);
                        }
                    }
                    MechController targetMechController = CombatManager.Instance.EnemyMechs.Find(m => m.Mech.Name == _targetDropdown.options[_targetDropdown.value].text);
                    ShootAction shootAction = new ShootAction(_selectedMechController, targetMechController, shootingWeapons);
                    _selectedActionMarker.SetSecondaryAction(shootAction);
                    ClosePanels();
                });
                break;
            case "Melee":
                _weaponDropdown.gameObject.SetActive(true);
                _targetDropdown.gameObject.SetActive(true);
                _weaponDropdown.ClearOptions();
                _targetDropdown.ClearOptions();
                for (int i = 0; i < _selectedMechController.Mech.Inventory.Weapons.Length; i++)
                {
                    Weapon weapon = _selectedMechController.Mech.Inventory.Weapons[i];
                    if (weapon != null)
                    {
                        _weaponDropdown.options.Add(new TMP_Dropdown.OptionData(weapon.Name));
                    }
                }
                if (_weaponDropdown.options.Count > 1)
                {
                    _weaponDropdown.options.Add(new TMP_Dropdown.OptionData("All"));
                }
                if (_weaponDropdown.options.Count > 0)
                {
                    _weaponDropdown.SetValueWithoutNotify(0);
                }
                _weaponDropdown.RefreshShownValue();
                _weaponDropdown.onValueChanged.AddListener(delegate { UpdateTargetDropdownMelee(); });
                UpdateTargetDropdownMelee();
                _confirmButton.onClick.RemoveAllListeners();
                _confirmButton.onClick.AddListener(() =>
                {
                    List<MeleeAttack> meleeWeapons = new List<MeleeAttack>();
                    if (_weaponDropdown.options[_weaponDropdown.value].text == "All")
                    {
                        foreach (MeleeAttack meleeAttack in _selectedMechController.Mech.Inventory.MeleeAttacks)
                        {
                            if (meleeAttack != null)
                            {
                                meleeWeapons.Add(meleeAttack);
                            }
                        }
                    }
                    else
                    {
                        MeleeAttack selectedMeleeAttack = DetermineMeleeAttackByDropdownSelection();
                        if (selectedMeleeAttack != null)
                        {
                            meleeWeapons.Add(selectedMeleeAttack);
                        }
                    }

                    MechController targetMechController = CombatManager.Instance.EnemyMechs.Find(m => m.Mech.Name == _targetDropdown.options[_targetDropdown.value].text);
                    MeleeAction meleeAction = new MeleeAction(_selectedMechController, targetMechController, meleeWeapons);
                    _selectedActionMarker.SetSecondaryAction(meleeAction);
                    ClosePanels();
                });
                break;
            case "Reload":
                _weaponDropdown.gameObject.SetActive(true);
                _targetDropdown.gameObject.SetActive(false);
                _weaponDropdown.ClearOptions();
                for (int i = 0; i < _selectedMechController.Mech.Inventory.Weapons.Length; i++)
                {
                    Weapon weapon = _selectedMechController.Mech.Inventory.Weapons[i];
                    if (weapon != null && weapon.CurrentAmmo < weapon.MaxAmmo && !weapon.IsMelee)
                    {
                        _weaponDropdown.options.Add(new TMP_Dropdown.OptionData(weapon.Name));
                    }
                }
                if (_weaponDropdown.options.Count > 1)
                {
                    _weaponDropdown.options.Add(new TMP_Dropdown.OptionData("All"));
                }
                if (_weaponDropdown.options.Count > 0)
                {
                    _weaponDropdown.SetValueWithoutNotify(0);
                }
                _weaponDropdown.RefreshShownValue();
                _confirmButton.onClick.RemoveAllListeners();
                _confirmButton.onClick.AddListener(() =>
                {
                    List<RangedAttack> weaponsToReload = new List<RangedAttack>();
                    if (_weaponDropdown.options[_weaponDropdown.value].text == "All")
                    {
                        foreach (RangedAttack rangedAttack in _selectedMechController.Mech.Inventory.RangedAttacks)
                        {
                            if (rangedAttack != null && rangedAttack.Weapon.CurrentAmmo < rangedAttack.Weapon.MaxAmmo)
                            {
                                weaponsToReload.Add(rangedAttack);
                            }
                        }
                    }
                    else
                    {
                        RangedAttack selectedRangedAttack = Array.Find(_selectedMechController.Mech.Inventory.RangedAttacks.ToArray(), ra => ra != null && ra.Weapon.Name == _weaponDropdown.options[_weaponDropdown.value].text);
                        if (selectedRangedAttack != null)
                        {
                            weaponsToReload.Add(selectedRangedAttack);
                        }
                    }
                    ReloadAction reloadAction = new ReloadAction(_selectedMechController, weaponsToReload);
                    _selectedActionMarker.SetSecondaryAction(reloadAction);
                    ClosePanels();
                });
                break;
            case "Charge":
                _weaponDropdown.gameObject.SetActive(true);
                _targetDropdown.gameObject.SetActive(true);
                _weaponDropdown.ClearOptions();
                _targetDropdown.ClearOptions();
                for (int i = 0; i < _selectedMechController.Mech.Inventory.Weapons.Length; i++)
                {
                    Weapon weapon = _selectedMechController.Mech.Inventory.Weapons[i];
                    if (weapon != null && weapon.IsMelee)
                    {
                        _weaponDropdown.options.Add(new TMP_Dropdown.OptionData(weapon.Name));
                    }
                }
                if (_weaponDropdown.options.Count > 1)
                {
                    _weaponDropdown.options.Add(new TMP_Dropdown.OptionData("All"));
                }
                if (_weaponDropdown.options.Count > 0)
                {
                    _weaponDropdown.SetValueWithoutNotify(0);
                }
                _weaponDropdown.RefreshShownValue();
                _weaponDropdown.onValueChanged.AddListener(delegate { UpdateTargetDropdownRanged(); });
                UpdateTargetDropdownRanged();
                _confirmButton.onClick.RemoveAllListeners();
                _confirmButton.onClick.AddListener(() =>
                {
                    List<MeleeAttack> weaponsToCharge = new List<MeleeAttack>();
                    if (_weaponDropdown.options[_weaponDropdown.value].text == "All")
                    {
                        foreach (MeleeAttack attack in _selectedMechController.Mech.Inventory.MeleeAttacks)
                        {
                            if (attack != null)
                            {
                                weaponsToCharge.Add(attack);
                            }
                        }
                    }
                    else
                    {
                        MeleeAttack selectedAttack = DetermineMeleeAttackByDropdownSelection();
                        if (selectedAttack != null)
                        {
                            weaponsToCharge.Add(selectedAttack);
                        }
                    }
                    MechController targetMechController = CombatManager.Instance.GetVisibleEnemyMechs().Find(m => m.Mech.Name == _targetDropdown.options[_targetDropdown.value].text);
                    ChargeAction chargeAction = new ChargeAction(_selectedMechController, targetMechController, weaponsToCharge);
                    _selectedActionMarker.SetSecondaryAction(chargeAction);
                    ClosePanels();
                });
                break;
            default:
                _weaponDropdown.gameObject.SetActive(false);
                _targetDropdown.gameObject.SetActive(false);
                break;
        }
        _popupPanel.SetActive(true);
    }

    /// <summary>
    /// Updates the target dropdown for ranged attacks based on visible enemy mechs.
    /// </summary>
    /// <remarks>
    /// This method clears the current options in the target dropdown and populates it with the names of all visible enemy mechs.
    /// </remarks>
    private void UpdateTargetDropdownRanged()
    {
        _targetDropdown.ClearOptions();
        var enemyMechs = CombatManager.Instance.GetVisibleEnemyMechs();
        var options = new List<string>();
        foreach (var enemyMech in enemyMechs)
        {
            options.Add(enemyMech.Mech.Name);
        }
        _targetDropdown.AddOptions(options);
    }

    /// <summary>
    /// Updates the target dropdown for melee attacks based on visible enemy mechs within melee range.
    /// </summary>
    /// <remarks>
    /// This method clears the current options in the target dropdown and populates it with the names of all visible enemy mechs that are within melee range of the selected mech or action marker.
    /// </remarks>
    private void UpdateTargetDropdownMelee()
    {
        _targetDropdown.ClearOptions();
        var enemyMechs = CombatManager.Instance.GetVisibleEnemyMechs();
        var options = new List<string>();
        float meleeRange = DetermineMeleeRangeByDropdownSelection();
        foreach (var enemyMech in enemyMechs)
        {
            if (_selectedActionMarker != null && Vector3.Distance(_selectedActionMarker.transform.position, enemyMech.transform.position) <= meleeRange)
            {
                options.Add(enemyMech.Mech.Name);
            } else if (_selectedMechController != null && Vector3.Distance(_selectedMechController.transform.position, enemyMech.transform.position) <= meleeRange)
            {
                options.Add(enemyMech.Mech.Name);
            }
        }
        _targetDropdown.AddOptions(options);
    }

    /// <summary>
    /// Determines the weapon selected in the weapon dropdown.
    /// </summary>
    /// <returns>The weapon corresponding to the selected option in the weapon dropdown, or null if no valid weapon is selected.</returns>
    private Weapon DetermineWeaponByDropdownSelection()
    {
        Weapon selectedWeapon = Array.Find(_selectedMechController.Mech.Inventory.Weapons, w => w != null && w.Name == _weaponDropdown.options[_weaponDropdown.value].text);
        return selectedWeapon;
    }

    /// <summary>
    /// Determines the ranged attack associated with the selected weapon in the weapon dropdown.
    /// </summary>
    /// <returns>The ranged attack corresponding to the selected weapon, or null if no valid ranged attack is found.</returns>
    private RangedAttack DetermineRangedAttackByDropdownSelection()
    {
        Weapon selectedWeapon = DetermineWeaponByDropdownSelection();
        return selectedWeapon != null && _selectedMechController.Mech.Inventory.WeaponToRangedAttackMap.ContainsKey(selectedWeapon) ? _selectedMechController.Mech.Inventory.WeaponToRangedAttackMap[selectedWeapon] : null;
    }

    /// <summary>
    /// Determines the melee attack associated with the selected weapon in the weapon dropdown.
    /// </summary>
    /// <returns>The melee attack corresponding to the selected weapon, or null if no valid melee attack is found.</returns>
    private MeleeAttack DetermineMeleeAttackByDropdownSelection()
    {
        Weapon selectedWeapon = DetermineWeaponByDropdownSelection();
        return selectedWeapon != null && _selectedMechController.Mech.Inventory.WeaponToMeleeAttackMap.ContainsKey(selectedWeapon) ? _selectedMechController.Mech.Inventory.WeaponToMeleeAttackMap[selectedWeapon] : null;
    }

    /// <summary>
    /// Determines the melee range of the selected weapon in the weapon dropdown.
    /// </summary>
    /// <returns>The melee range of the selected weapon, or 0 if no valid weapon is selected.</returns>
    private float DetermineMeleeRangeByDropdownSelection()
    {
        Weapon selectedWeapon = DetermineWeaponByDropdownSelection();
        return selectedWeapon != null ? selectedWeapon.MeleeRange : 0f;
    }

    /// <summary>
    /// Hides the popup panel.
    /// </summary>
    void HidePopup()
    {
        _popupPanel.SetActive(false);
    }

    /// <summary>
    /// Shows the action selection panel.
    /// </summary>
    void ShowActionSelectionPanel()
    {
        _actionSelectionPanel.SetActive(true);
    }

    /// <summary>
    /// Hides the action selection panel.
    /// </summary>
    void HideActionSelectionPanel()
    {
        _actionSelectionPanel.SetActive(false);
    }

    /// <summary>
    /// Handles the event when the shoot button is clicked.
    /// </summary>
    public void OnShootButtonClicked()
    {
        Debug.Log("Shoot button clicked");
        ShowPopup("Shoot");
    }

    /// <summary>
    /// Handles the event when the melee button is clicked.
    /// </summary>
    public void OnMeleeButtonClicked()
    {
        Debug.Log("Melee button clicked");
        ShowPopup("Melee");
    }

    /// <summary>
    /// Handles the event when the reload button is clicked.
    /// </summary>
    public void OnReloadButtonClicked()
    {
        Debug.Log("Reload button clicked");
        ShowPopup("Reload");
    }

    /// <summary>
    /// Handles the event when the move button is clicked.
    /// </summary>
    public void OnMoveButtonClicked()
    {
        Debug.Log("Move button clicked");
        _selectedActionMarker.SetSecondaryAction(new MoveAction(_selectedMechController, _selectedActionMarker.transform.position));
        ClosePanels();
    }

    /// <summary>
    /// Handles the event when the charge button is clicked.
    /// </summary>
    public void OnChargeButtonClicked()
    {
        Debug.Log("Charge button clicked");
        ShowPopup("Charge");
    }

    /// <summary>
    /// Handles the event when the hunker button is clicked.
    /// </summary>
    public void OnHunkerButtonClicked()
    {
        Debug.Log("Hunker button clicked");
        _selectedActionMarker.SetSecondaryAction(new HunkerAction(_selectedMechController));
        ClosePanels();
    }

    /// <summary>
    /// Handles the event when the overwatch button is clicked.
    /// </summary>
    public void OnOverwatchButtonClicked()
    {
        Debug.Log("Overwatch button clicked");
        _selectedActionMarker.SetSecondaryAction(new OverwatchAction(_selectedMechController));
        ClosePanels();
    } 

    /// <summary>
    /// Handles the event when the remove button is clicked.
    /// </summary>
    public void OnRemoveButtonClicked()
    {
        Debug.Log("Remove button clicked");
        if (_selectedActionMarker != null)
        {
            _selectedMechController.RemoveActionMarker(_selectedActionMarker);
            _selectedActionMarker = null;
        }
        ClosePanels();
    }

    /// <summary>
    /// Handles the event when the cancel button is clicked.
    /// </summary>
    public void OnCancelButtonClicked()
    {
        Debug.Log("Cancel button clicked");
        ClosePanels();
    }

    /// <summary>
    /// Handles the event when the pause button is clicked.
    /// </summary>
    public void OnPauseButtonClicked()
    {
        CombatManager.Instance.UpdateCombatState(CombatState.Paused);
        // Clear listeners from the pause button to prevent multiple subscriptions
        _pauseButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _pauseButton.GetComponent<Button>().onClick.AddListener(OnPlayButtonClicked);
    }

    /// <summary>
    /// Handles the event when the play button is clicked.
    /// </summary>
    public void OnPlayButtonClicked()
    {
        CombatManager.Instance.UpdateCombatState(CombatState.Playing);
        // Clear listeners from the play button to prevent multiple subscriptions
        _pauseButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _pauseButton.GetComponent<Button>().onClick.AddListener(OnPauseButtonClicked);
    }

    /// <summary>
    /// Closes all open panels.
    /// </summary>
    private void ClosePanels()
    {
        HideMechInfoPanel();
        HideActionSelectionPanel();
        HidePopup();
    }

    /// <summary>
    /// Displays the exit combat panel with the specified end condition and rewards.
    /// </summary>
    /// <param name="endCondition">The end condition of the combat.</param>
    /// <param name="rewards">The rewards earned from the combat.</param>
    public void ExitCombat(EndCondition endCondition, int rewards)
    {
        _exitCombatPanel.SetActive(true);
        _exitDeclarationText.text = endCondition == EndCondition.Victory ? "Victory!" : endCondition == EndCondition.Survived ? "Survived!" : "Defeat!";
        _exitRewardsText.text = $"Rewards: {rewards}";
    }
    /// <summary>
    /// Handles the event when the exit combat button is clicked.
    /// </summary>
    public void OnExitCombatButtonClicked()
    {
        Debug.Log("Exit Combat button clicked");
        _exitCombatPanel.SetActive(false);
        SceneLoader.Instance.LoadScene(SceneName.Garage);
    }

}
