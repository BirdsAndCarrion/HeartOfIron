using UnityEngine;
using System;
using TMPro;
using HeartOfIron.Enums;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

/// <summary>
/// Manages the UI elements within the combat scene, including mech info displays, action selection menus, and combat feedback.
/// Meant to provide a centralized interface for updating and controlling combat-related UI elements.
/// Works in conjunction with CombatManager to reflect the current state of combat.
/// </summary>
public class CombatUIManager : MonoBehaviour
{
    private readonly float TOOLTIP_OFFSET_X = 0.5f;
    private readonly int RANGED_WEAPON_INDEX = 0;
    private readonly int MELEE_WEAPON_INDEX = 1;
    private readonly int RANGED_TARGET_INDEX = 2;
    private readonly int MELEE_TARGET_INDEX = 3;

    public static CombatUIManager Instance { get; private set; }

    [SerializeField] private RectTransform _canvasRectTransform;
    [SerializeField] private GameObject _hoverableItemTooltip;
    [Header("Mech Info Panel Elements")]
    [SerializeField] private GameObject _mechInfoPanel;
    [SerializeField] private TextMeshProUGUI _mechNameText;
    [SerializeField] private TextMeshProUGUI _pilotNameText;
    [SerializeField] private TextMeshProUGUI _mechTypeLevelText;
    [Header("Weapon Info Panel Elements")]
    [SerializeField] private GameObject _weaponPanel;
    [SerializeField] private HoverableItemTooltip _weaponTooltip;
    [SerializeField] private TextMeshProUGUI _weaponNameText;
    [SerializeField] private TextMeshProUGUI _weaponAmmoText;
    [SerializeField] private GameObject _weapon2Panel;
    [SerializeField] private HoverableItemTooltip _weapon2Tooltip;
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
    [SerializeField] private HoverableItemTooltip _headArmorTooltip;
    [SerializeField] private GameObject _headArmorBar;
    [SerializeField] private HoverableItemTooltip _torsoArmorTooltip;
    [SerializeField] private GameObject _torsoArmorBar;
    [SerializeField] private HoverableItemTooltip _leftArmArmorTooltip;
    [SerializeField] private GameObject _leftArmArmorBar;
    [SerializeField] private HoverableItemTooltip _rightArmArmorTooltip;
    [SerializeField] private GameObject _rightArmArmorBar;
    [SerializeField] private HoverableItemTooltip _leftLegArmorTooltip;
    [SerializeField] private GameObject _leftLegArmorBar;
    [SerializeField] private HoverableItemTooltip _rightLegArmorTooltip;
    [SerializeField] private GameObject _rightLegArmorBar;
    [Header("Mech Energy Shield Panel Elements")]
    [SerializeField] private GameObject _mechEnergyShieldPanel;
    [SerializeField] private HoverableItemTooltip _energyShieldTooltip;
    [SerializeField] private GameObject _energyShieldBar;

    [Header("Pop-up Elements")]
    [SerializeField] private GameObject _popupPanel;
    //[SerializeField] private TextMeshProUGUI _popupText;
    //[SerializeField] private TMP_Dropdown _weaponDropdown;
    //[SerializeField] private TMP_Dropdown _targetDropdown;
    [SerializeField] private GameObject _confirmButton;

    [SerializeField] private GameObject _popupOptionButtonPrefab;
    [SerializeField] private GameObject _currentSelectionsPanel;
    [SerializeField] private GameObject _currentWeaponSelectionButton;
    [SerializeField] private GameObject _currentTargetSelectionButton;
    [SerializeField] private GameObject _optionsPanel;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Color _popupOptionButtonColor;
    [SerializeField] private Color _popupOptionButtonSelectedColor;

    [Header("Action Selection Elements")]
    [SerializeField] private GameObject _actionSelectionPanel;
    [SerializeField] private GameObject _shootButton;
    [SerializeField] private GameObject _meleeButton;
    [SerializeField] private GameObject _reloadButton;
    [SerializeField] private GameObject _moveButton;
    [SerializeField] private GameObject _chargeButton;
    [SerializeField] private GameObject _hunkerButton;
    [SerializeField] private GameObject _overwatchButton;

    [Header("Player Mech Action Displays")]
    [SerializeField] private GameObject _playerMechActionDisplayPrefab;
    [SerializeField] private Transform _playerMechActionDisplayParent;
    [SerializeField] private GameObject _playerMechActionPrefab;
    private Dictionary<MechController, GameObject> _playerMechActionDisplays = new Dictionary<MechController, GameObject>();

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
        _optionsPanel.SetActive(false);
        EventRouter.ROnCombatStarted.AddListener(EnterCombat);
        EventRouter.ROnCombatEnded.AddListener(ExitCombat);
        EventRouter.ROnDeploymentStarted.AddListener(ShowDeploymentPanel);
        // EventRouter.ROnDeploymentStarted.AddListener(TestEventRecieved);
        EventRouter.ROnCombatStateChanged.AddListener(HandleCombatStateChanged);
        EventRouter.ROnActionMarkerSelected.AddListener(OnSelected);
        EventRouter.ROnHoverableMechControllerInteractionEnter.AddListener(OnHoverableMechControllerInteractionEnter);
        EventRouter.ROnHoverableMechControllerInteractionExit.AddListener(OnHoverableMechControllerInteractionExit);
        EventRouter.ROnHoverableItemTooltipEnter.AddListener(OnHoverableItemTooltipEnter);
        EventRouter.ROnHoverableItemTooltipExit.AddListener(OnHoverableItemTooltipExit);
    }

    void Start()
    {
        _popupPanel.transform.position = new Vector3(_actionSelectionPanel.transform.position.x, _popupPanel.transform.position.y, _popupPanel.transform.position.z);
        _optionsPanel.transform.position = new Vector3(_actionSelectionPanel.transform.position.x, _optionsPanel.transform.position.y, _optionsPanel.transform.position.z);
    }

    void OnDestroy()
    {
        EventRouter.ROnCombatStarted.RemoveListener(EnterCombat);
        EventRouter.ROnCombatEnded.RemoveListener(ExitCombat);
        EventRouter.ROnDeploymentStarted.RemoveListener(ShowDeploymentPanel);
        // EventRouter.ROnDeploymentStarted.RemoveListener(TestEventRecieved);
        EventRouter.ROnCombatStateChanged.RemoveListener(HandleCombatStateChanged);
        EventRouter.ROnActionMarkerSelected.RemoveListener(OnSelected);
        EventRouter.ROnHoverableMechControllerInteractionEnter.RemoveListener(OnHoverableMechControllerInteractionEnter);
        EventRouter.ROnHoverableMechControllerInteractionExit.RemoveListener(OnHoverableMechControllerInteractionExit);
        EventRouter.ROnHoverableItemTooltipEnter.RemoveListener(OnHoverableItemTooltipEnter);
        EventRouter.ROnHoverableItemTooltipExit.RemoveListener(OnHoverableItemTooltipExit);
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
    public async void OnSelected(MechController mechController)
    {
        Debug.Log($"MechController selected: {mechController.Mech.Name}");
        _selectedMechController = mechController;
        _selectedActionMarker = ActionMarkerAssembler.Instance.AssembleActionMarker(mechController.transform.position, mechController, false);
        _selectedMechController.InsertActionMarkerAt(_selectedActionMarker, 0);
        ShowMechInfoPanel(mechController);
        await ShowActionSelectionPanel();
    }

    /// <summary>
    /// Handles the selection of an action marker.
    /// Updates the selected mech controller and action marker, and displays the relevant UI panels.
    /// </summary>
    /// <param name="actionMarker">The action marker that was selected.</param>
    public async void OnSelected(ActionMarker actionMarker)
    {
        Debug.Log($"ActionMarker selected: {actionMarker.Id}, Source Mech: {actionMarker.ActionPair.MovementAction.SourceMech.Mech.Name}");
        _selectedMechController = actionMarker.ActionPair.MovementAction.SourceMech;
        _selectedActionMarker = actionMarker;
        ShowMechInfoPanel(actionMarker.ActionPair.MovementAction.SourceMech);
        await ShowActionSelectionPanel();
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
        if (mechController.Mech.Pilot != null)
            _pilotNameText.text = mechController.Mech.Pilot.Name;
        _mechTypeLevelText.text = $"{mechController.Mech.Type} - Level {mechController.Mech.Level}";
        if (mechController.Mech.Inventory.Weapons[(int)Hand.Right] != null)
        {
            _weaponNameText.text = mechController.Mech.Inventory.Weapons[(int)Hand.Right].Name;
            _weaponAmmoText.text = $"Ammo: {mechController.Mech.Inventory.Weapons[(int)Hand.Right].CurrentAmmo}/{mechController.Mech.Inventory.Weapons[(int)Hand.Right].MaxAmmo}";
            _weaponTooltip.item = mechController.Mech.Inventory.Weapons[(int)Hand.Right];
            _weaponPanel.SetActive(true);
        }
        else
        {
            _weaponNameText.text = "No Weapon";
            _weaponAmmoText.text = "Ammo: N/A";
            _weaponTooltip.item = null;
            _weaponPanel.SetActive(false);
        }
        if (mechController.Mech.Inventory.Weapons[(int)Hand.Left] != null)
        {
            _weapon2NameText.text = mechController.Mech.Inventory.Weapons[(int)Hand.Left].Name;
            _weapon2AmmoText.text = $"Ammo: {mechController.Mech.Inventory.Weapons[(int)Hand.Left].CurrentAmmo}/{mechController.Mech.Inventory.Weapons[(int)Hand.Left].MaxAmmo}";
            _weapon2Tooltip.item = mechController.Mech.Inventory.Weapons[(int)Hand.Left];
            _weapon2Panel.SetActive(true);
        }
        else
        {
            _weapon2NameText.text = "No Weapon";
            _weapon2AmmoText.text = "Ammo: N/A";
            _weapon2Tooltip.item = null;
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
            _headArmorTooltip.item = armors[(int)ArmorType.Head];
        }
        else
        {
            UpdateResourceBar(0, 1, _headArmorBar); // Set to empty if no armor
        }
        if (armors[(int)ArmorType.Torso] != null)
        {
            UpdateResourceBar(armors[(int)ArmorType.Torso].GetDurability(), armors[(int)ArmorType.Torso].MaxDurability, _torsoArmorBar);
            _torsoArmorTooltip.item = armors[(int)ArmorType.Torso];
        }
        else
        {
            UpdateResourceBar(0, 1, _torsoArmorBar); // Set to empty if no armor
        }
        if (armors[(int)ArmorType.Arms] != null)
        {
            UpdateResourceBar(armors[(int)ArmorType.Arms].GetDurability(1), armors[(int)ArmorType.Arms].MaxDurability, _rightArmArmorBar);
            _rightArmArmorTooltip.item = armors[(int)ArmorType.Arms];
            UpdateResourceBar(armors[(int)ArmorType.Arms].GetDurability(0), armors[(int)ArmorType.Arms].MaxDurability, _leftArmArmorBar);
            _leftArmArmorTooltip.item = armors[(int)ArmorType.Arms];
        }
        else
        {
            UpdateResourceBar(0, 1, _rightArmArmorBar); // Set to empty if no armor
            UpdateResourceBar(0, 1, _leftArmArmorBar); // Set to empty if no armor
        }
        if (armors[(int)ArmorType.Legs] != null)
        {
            UpdateResourceBar(armors[(int)ArmorType.Legs].GetDurability(0), armors[(int)ArmorType.Legs].MaxDurability, _leftLegArmorBar);
            _leftLegArmorTooltip.item = armors[(int)ArmorType.Legs];
            UpdateResourceBar(armors[(int)ArmorType.Legs].GetDurability(1), armors[(int)ArmorType.Legs].MaxDurability, _rightLegArmorBar);
            _rightLegArmorTooltip.item = armors[(int)ArmorType.Legs];
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
            _energyShieldTooltip.item = energyShield;
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

    private void UpdateCurrentWeaponSelectionButton(string weaponName)
    {
        // Update the current weapon selection button with the specified weapon name
        _currentWeaponSelectionButton.GetComponentInChildren<TMP_Text>().text = weaponName;
    }

    private void UpdateCurrentTargetSelectionButton(string targetName)
    {
        // Update the current target selection button with the specified target name
        _currentTargetSelectionButton.GetComponentInChildren<TMP_Text>().text = targetName;
    }

    private void UpdateCurrentSelectionsPanel()
    {
        AttackListMount attackListMount = _popupPanel.GetComponent<AttackListMount>();
        if (attackListMount != null)
        {
            StringBuilder sb = new StringBuilder();
            if (attackListMount.HasRangedAttacks())
            {
                foreach (RangedAttack attack in attackListMount.attacks)
                {
                    if (attack != null)
                    {
                        if (sb.Length > 0 && attack != attackListMount.attacks[0])
                            sb.Append(", ");
                        sb.Append(attack.Weapon.Name);
                    }
                }
            }
            if (attackListMount.HasMeleeAttacks())
            {
                foreach (MeleeAttack attack in attackListMount.meleeAttacks)
                {
                    if (attack != null)
                    {
                        if (sb.Length > 0 && attack != attackListMount.meleeAttacks[0])
                            sb.Append(", ");
                        sb.Append(attack.Weapon.Name);
                    }
                }
                if (sb.Length == 0)
                    sb.Append("None");
            }
            UpdateCurrentSelectionsPanel(
                sb.ToString(),
                attackListMount.target != null ? attackListMount.target.name : "None"
            );
        }
    }

    private void UpdateCurrentSelectionsPanel(string weaponName, string targetName)
    {
        UpdateCurrentWeaponSelectionButton(weaponName);
        UpdateCurrentTargetSelectionButton(targetName);
    }

    private void EnableConfirmButton(bool enable)
    {
        _confirmButton.GetComponent<Button>().interactable = enable;
        if (enable)
        {
            _confirmButton.GetComponent<Button>().onClick.AddListener(async () => await OnConfirmButtonClicked());
        }
        else
        {
            _confirmButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private async Task OnConfirmButtonClicked()
    {
        AttackListMount attackListMount = _popupPanel.GetComponent<AttackListMount>();
        switch (attackListMount.actionType)
        {
            case ActionType.Shoot:
                // Handle shoot action confirmation
                List<RangedAttack> rangedAttacks = attackListMount.attacks;
                MechController target = attackListMount.target;
                if (rangedAttacks != null && rangedAttacks.Count > 0 && target != null)
                {
                    ShootAction shootAction = new ShootAction(_selectedMechController, target, rangedAttacks);
                    _selectedActionMarker.SetSecondaryAction(shootAction);
                    await ClosePanels();
                }
                break;
            case ActionType.Melee:
                // Handle melee action confirmation
                List<MeleeAttack> meleeAttacks = attackListMount.meleeAttacks;
                MechController mTarget = attackListMount.target;
                if (meleeAttacks != null && meleeAttacks.Count > 0 && mTarget != null)
                {
                    MeleeAction meleeAction = new MeleeAction(_selectedMechController, mTarget, meleeAttacks);
                    _selectedActionMarker.SetSecondaryAction(meleeAction);
                    await ClosePanels();
                }
                break;
            case ActionType.Reload:
                // Handle reload action confirmation
                List<RangedAttack> rAttacks = attackListMount.attacks;
                if (rAttacks != null && rAttacks.Count > 0)
                {
                    ReloadAction reloadAction = new ReloadAction(_selectedMechController, rAttacks);
                    _selectedActionMarker.SetSecondaryAction(reloadAction);
                    await ClosePanels();
                }
                break;
            case ActionType.Charge:
                // Handle charge action confirmation
                List<MeleeAttack> chargeAttacks = attackListMount.meleeAttacks;
                MechController cTarget = attackListMount.target;
                if (chargeAttacks != null && chargeAttacks.Count > 0 && cTarget != null)
                {
                    ChargeAction chargeAction = new ChargeAction(_selectedMechController, cTarget, chargeAttacks);
                    _selectedActionMarker.SetSecondaryAction(chargeAction);
                    await ClosePanels();
                }
                break;
            case ActionType.Move:
                // Handle move action confirmation
                await ClosePanels();
                break;
        }
    }

    private void EnableTargetSelectionButton(bool enable)
    {
        _currentTargetSelectionButton.GetComponent<Button>().interactable = enable;
    }

    private void ActivateTargetSelectionButton(bool activate)
    {
        _currentTargetSelectionButton.SetActive(activate);
    }

    private async Task ShowOptionsPanel(int showOptionsType)
    {
        if (_optionsPanel.transform.position.x == _actionSelectionPanel.transform.position.x)
        {
            Vector3 originalPosition = _optionsPanel.transform.position;
            Vector3 targetPosition = originalPosition - new Vector3(_actionSelectionPanel.GetComponent<RectTransform>().rect.width + 0.5f, 0f, 0f);
            _optionsPanel.SetActive(true);
            float currentFade = 1f;
            while (currentFade > 0f)
            {
                currentFade -= Time.deltaTime / fadeDuration;
                _optionsPanel.transform.position = Vector3.Lerp(originalPosition, targetPosition, 1f - currentFade);
                await Task.Yield();
            }
        }
        switch (showOptionsType)
        {
            case 0:
                UpdateOptionsPanelRangedWeapons();
                break;
            case 1:
                UpdateOptionsPanelMeleeWeapons();
                break;
            case 2:
                UpdateOptionsPanelRangedTargets();
                break;
            case 3:
                UpdateOptionsPanelMeleeTargets();
                break;
        }
    }

    private async Task HideOptionsPanel()
    {
        if (_optionsPanel.transform.position.x != _actionSelectionPanel.transform.position.x)
        {
            Vector3 originalPosition = _optionsPanel.transform.position;
            Vector3 targetPosition = new Vector3(_actionSelectionPanel.transform.position.x, _optionsPanel.transform.position.y, _optionsPanel.transform.position.z);
            _optionsPanel.SetActive(true);
            float currentFade = 1f;
            while (currentFade > 0f)
            {
                currentFade -= Time.deltaTime / fadeDuration;
                _optionsPanel.transform.position = Vector3.Lerp(originalPosition, targetPosition, 1f - currentFade);
                await Task.Yield();
            }
        }
        _optionsPanel.SetActive(false);
    }

    public void UpdateOptionsPanelRangedWeapons()
    {
        List<RangedAttack> rangedAttacks = _selectedMechController.Mech.Inventory.RangedAttacks;
        // Clear existing options in the options panel
        foreach (Transform child in _optionsPanel.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (RangedAttack rangedAttack in rangedAttacks)
        {
            GameObject option = Instantiate(_popupOptionButtonPrefab, _optionsPanel.transform);
            option.GetComponentInChildren<TMP_Text>().text = rangedAttack.Weapon.Name;
            option.GetComponent<Button>().onClick.AddListener(() => OnRangedWeaponSelected(option, rangedAttack));
        }

    }

    private void OnRangedWeaponSelected(GameObject option, RangedAttack rangedAttack)
    {
        AttackListMount attackListMount = _popupPanel.GetComponent<AttackListMount>();
        attackListMount.AddRangedAttack(rangedAttack);
        UpdateCurrentSelectionsPanel();
        option.GetComponent<Image>().color = _popupOptionButtonSelectedColor;
        option.GetComponent<Button>().onClick.RemoveAllListeners();
        option.GetComponent<Button>().onClick.AddListener(() => OnRangedWeaponUnSelected(option, rangedAttack));
        if (_currentTargetSelectionButton.activeInHierarchy)
        {
            EnableTargetSelectionButton(true);
        } else
        {
            EnableConfirmButton(true);
        }
    }

    private void OnRangedWeaponUnSelected(GameObject option, RangedAttack rangedAttack)
    {
        AttackListMount attackListMount = _popupPanel.GetComponent<AttackListMount>();
        attackListMount.RemoveRangedAttack(rangedAttack);
        UpdateCurrentSelectionsPanel();
        option.GetComponent<Image>().color = _popupOptionButtonColor;
        option.GetComponent<Button>().onClick.RemoveAllListeners();
        option.GetComponent<Button>().onClick.AddListener(() => OnRangedWeaponSelected(option, rangedAttack));
        if (attackListMount.attacks.Count <= 0 && _currentTargetSelectionButton.activeInHierarchy)
        {
            EnableTargetSelectionButton(false);
        } else if (attackListMount.attacks.Count <= 0)
        {
            EnableConfirmButton(false);
        }
    }

    public void UpdateOptionsPanelMeleeWeapons()
    {
        List<MeleeAttack> meleeAttacks = _selectedMechController.Mech.Inventory.MeleeAttacks;
        // Clear existing options in the options panel
        foreach (Transform child in _optionsPanel.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (MeleeAttack meleeAttack in meleeAttacks)
        {
            GameObject option = Instantiate(_popupOptionButtonPrefab, _optionsPanel.transform);
            option.GetComponentInChildren<TMP_Text>().text = meleeAttack.Weapon.Name;
            option.GetComponent<Button>().onClick.AddListener(() => OnMeleeWeaponSelected(option, meleeAttack));
        }
    }

    private void OnMeleeWeaponSelected(GameObject option, MeleeAttack meleeAttack)
    {
        AttackListMount attackListMount = _popupPanel.GetComponent<AttackListMount>();
        attackListMount.AddMeleeAttack(meleeAttack);
        UpdateCurrentSelectionsPanel();
        option.GetComponent<Image>().color = _popupOptionButtonSelectedColor;
        option.GetComponent<Button>().onClick.RemoveAllListeners();
        option.GetComponent<Button>().onClick.AddListener(() => OnMeleeWeaponUnSelected(option, meleeAttack));
        if (_currentTargetSelectionButton.activeInHierarchy)
        {
            EnableTargetSelectionButton(true);
        } else
        {
            EnableConfirmButton(true);
        }
    }

    private void OnMeleeWeaponUnSelected(GameObject option, MeleeAttack meleeAttack)
    {
        AttackListMount attackListMount = _popupPanel.GetComponent<AttackListMount>();
        attackListMount.RemoveMeleeAttack(meleeAttack);
        UpdateCurrentSelectionsPanel();
        option.GetComponent<Image>().color = _popupOptionButtonColor;
        option.GetComponent<Button>().onClick.RemoveAllListeners();
        option.GetComponent<Button>().onClick.AddListener(() => OnMeleeWeaponSelected(option, meleeAttack));
        if (attackListMount.meleeAttacks.Count <= 0 && _currentTargetSelectionButton.activeInHierarchy)
        {
            EnableTargetSelectionButton(false);
        } else if (attackListMount.meleeAttacks.Count <= 0)
        {
            EnableConfirmButton(false);
        }
    }

    private void UpdateOptionsPanelRangedTargets()
    {
        foreach (Transform child in _optionsPanel.transform)
        {
            Destroy(child.gameObject);
        }
        List<MechController> potentialTargets = CombatManager.Instance.GetVisibleEnemyMechs();
        foreach (MechController target in potentialTargets)
        {
            GameObject option = Instantiate(_popupOptionButtonPrefab, _optionsPanel.transform);
            option.GetComponentInChildren<TMP_Text>().text = target.Mech.Name;
            option.GetComponent<Button>().onClick.AddListener(() => OnTargetSelected(option, target));
            option.AddComponent<HoverableMechControllerInteraction>().MechController = target;
        }
    }

    private void UpdateOptionsPanelMeleeTargets()
    {
        foreach (Transform child in _optionsPanel.transform)
        {
            Destroy(child.gameObject);
        }
        List<MechController> potentialTargets = CombatManager.Instance.GetVisibleEnemyMechs();
        foreach (MechController target in potentialTargets)
        {
            if (!IsInMeleeRange(target))
            {
                continue;
            }
            GameObject option = Instantiate(_popupOptionButtonPrefab, _optionsPanel.transform);
            option.GetComponentInChildren<TMP_Text>().text = target.Mech.Name;
            option.GetComponent<Button>().onClick.AddListener(() => OnTargetSelected(option, target));
            option.AddComponent<HoverableMechControllerInteraction>().MechController = target;
        }
    }

    private void OnTargetHovered(MechController target)
    {
        target.OnHovered();
    }

    private void OnTargetExited(MechController target)
    {
        target.OnHoverExit();
    }

    private void OnHoverableMechControllerInteractionEnter(GameObject go, MechController mechController)
    {
        OnTargetHovered(mechController);
    }

    private void OnHoverableMechControllerInteractionExit(GameObject go, MechController mechController)
    {
        OnTargetExited(mechController);
    }

    /// <summary>
    /// Handles the event when the mouse enters a hoverable item and displays the appropriate tooltip.
    /// </summary>
    /// <param name="go">The GameObject representing the UI element being hovered over.</param>
    /// <param name="item">The item associated with the UI element.</param>
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
            
        }
        else if (item is Armor armor)
        {
            // Display armor tooltip for item and equivalent armor in mechs inventory
            GameObject armorTooltip = CreateArmorTooltip(armor);
            armorTooltip.transform.SetParent(go.transform, false);
            armorTooltip.transform.localPosition = GetInitialOffsetX(go) / _canvasRectTransform.localScale.x;
            
        }
        else if (item is EnergyShield shield)
        {
            // Display energy shield tooltip for item and equivalent energy shield in mechs inventory
            GameObject shieldTooltip = CreateEnergyShieldTooltip(shield);
            shieldTooltip.transform.SetParent(go.transform, false);
            shieldTooltip.transform.localPosition = GetInitialOffsetX(go) / _canvasRectTransform.localScale.x;
            
        }
    }

    /// <summary>
    /// Calculates the initial offset for positioning the tooltip relative to the UI element.
    /// </summary>
    /// <param name="go">The GameObject representing the UI element.</param>
    /// <returns>The initial offset as a Vector3.</returns>
    private Vector3 GetInitialOffsetX(GameObject go)
    {
        return new Vector3(go.GetComponent<RectTransform>().rect.width / 2 + TOOLTIP_OFFSET_X, go.GetComponent<RectTransform>().rect.height / 2, 0);
    }

    /// <summary>
    /// Creates a tooltip GameObject for the specified weapon.
    /// </summary>
    /// <param name="weapon">The weapon for which to create the tooltip.</param>
    /// <returns>The created weapon tooltip GameObject.</returns>
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

    /// <summary>
    /// Creates a tooltip GameObject for the specified armor.
    /// </summary>
    /// <param name="armor">The armor for which to create the tooltip.</param>
    /// <returns>The created armor tooltip GameObject.</returns>
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

    /// <summary>
    /// Creates a tooltip GameObject for the specified energy shield.
    /// </summary>
    /// <param name="shield">The energy shield for which to create the tooltip.</param>
    /// <returns>The created energy shield tooltip GameObject.</returns>
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

    /// <summary>
    /// Handles the event when the mouse exits a hoverable item and destroys the associated tooltip.
    /// </summary>
    /// <param name="go">The GameObject representing the UI element that the mouse exited.</param>
    private void OnHoverableItemTooltipExit(GameObject go)
    {
        // Handle tooltip exit event
        foreach (Transform child in go.transform)
        {
            if (child.gameObject.name.EndsWith("Tooltip"))
                GameObject.Destroy(child.gameObject);
        }
    }

    private void OnTargetSelected(GameObject option, MechController target)
    {
        AttackListMount attackListMount = _popupPanel.GetComponent<AttackListMount>();
        attackListMount.target = target;
        foreach (Transform child in _optionsPanel.transform)
        {
            child.GetComponent<Image>().color = _popupOptionButtonColor;
        }
        option.GetComponent<Image>().color = _popupOptionButtonSelectedColor;
        UpdateCurrentSelectionsPanel();
        EnableConfirmButton(true);
    }

    private bool IsInMeleeRange(MechController target)
    {
        if ((_selectedMechController == null && _selectedActionMarker == null) || target == null)
            return false;
        AttackListMount attackListMount = _popupPanel.GetComponent<AttackListMount>();
        if (_selectedMechController != null && _selectedActionMarker == null)
        {
            float distance = Vector3.Distance(_selectedMechController.transform.position, target.transform.position);
            return distance <= attackListMount.GetMinimumMeleeRange();
        } else if (_selectedActionMarker != null && _selectedMechController == null)
        {
            float distance = Vector3.Distance(_selectedActionMarker.transform.position, target.transform.position);
            return distance <= attackListMount.GetMinimumMeleeRange();
        }
        return false;
    }


    public async Task ShowPopup(ActionType actionType)
    {
        if (_popupPanel.transform.position.x == _actionSelectionPanel.transform.position.x)
        {
            Vector3 originalPosition = _popupPanel.transform.position;
            // Move the popup panel in the negative x direction by half its width, plus half of action selection panels width, plus 0.5f for padding
            Vector3 targetPosition = originalPosition - new Vector3(_actionSelectionPanel.GetComponent<RectTransform>().rect.width + 0.5f, 0f, 0f);
            _popupPanel.SetActive(true);
            float currentFade = 1f;
            while (currentFade > 0f)
            {
                currentFade -= Time.deltaTime / fadeDuration;
                _popupPanel.transform.position = Vector3.Lerp(originalPosition, targetPosition, 1f - currentFade);
                await Task.Yield();
            }
        }
        _popupPanel.GetComponent<AttackListMount>().actionType = actionType;
        switch (actionType)
        {
            case ActionType.Shoot:

                UpdateCurrentSelectionsPanel("None", "None");
                ActivateTargetSelectionButton(true);
                EnableTargetSelectionButton(false);
                EnableConfirmButton(false);
                _currentWeaponSelectionButton.GetComponent<Button>().onClick.RemoveAllListeners();
                _currentWeaponSelectionButton.GetComponent<Button>().onClick.AddListener(async () => await ShowOptionsPanel(RANGED_WEAPON_INDEX));
                _currentTargetSelectionButton.GetComponent<Button>().onClick.RemoveAllListeners();
                _currentTargetSelectionButton.GetComponent<Button>().onClick.AddListener(async () => await ShowOptionsPanel(RANGED_TARGET_INDEX));
                break;
            case ActionType.Melee:
                UpdateCurrentSelectionsPanel("None", "None");
                ActivateTargetSelectionButton(true);
                EnableTargetSelectionButton(false);
                EnableConfirmButton(false);
                _currentWeaponSelectionButton.GetComponent<Button>().onClick.RemoveAllListeners();
                _currentWeaponSelectionButton.GetComponent<Button>().onClick.AddListener(async () => await ShowOptionsPanel(MELEE_WEAPON_INDEX));
                _currentTargetSelectionButton.GetComponent<Button>().onClick.RemoveAllListeners();
                _currentTargetSelectionButton.GetComponent<Button>().onClick.AddListener(async () => await ShowOptionsPanel(MELEE_TARGET_INDEX));
                break;
            case ActionType.Reload:
                UpdateCurrentSelectionsPanel("None", "None");
                EnableTargetSelectionButton(false);
                ActivateTargetSelectionButton(false);
                EnableConfirmButton(false);
                _currentWeaponSelectionButton.GetComponent<Button>().onClick.RemoveAllListeners();
                _currentWeaponSelectionButton.GetComponent<Button>().onClick.AddListener(async () => await ShowOptionsPanel(RANGED_WEAPON_INDEX));
                break;
            case ActionType.Charge:
                UpdateCurrentSelectionsPanel("None", "None");
                ActivateTargetSelectionButton(true);
                EnableTargetSelectionButton(false);
                EnableConfirmButton(false);
                _currentWeaponSelectionButton.GetComponent<Button>().onClick.RemoveAllListeners();
                _currentWeaponSelectionButton.GetComponent<Button>().onClick.AddListener(async () => await ShowOptionsPanel(MELEE_WEAPON_INDEX));
                _currentTargetSelectionButton.GetComponent<Button>().onClick.RemoveAllListeners();
                _currentTargetSelectionButton.GetComponent<Button>().onClick.AddListener(async () => await ShowOptionsPanel(RANGED_TARGET_INDEX));
                break;
            default:
                UpdateCurrentSelectionsPanel("None", "None");
                ActivateTargetSelectionButton(false);
                EnableTargetSelectionButton(false);
                EnableConfirmButton(false);
                break;
        }
    }

    /// <summary>
    /// Hides the popup panel.
    /// </summary>
    async Task HidePopup()
    {
        await HideOptionsPanel();
        if (_popupPanel.transform.position.x != _actionSelectionPanel.transform.position.x)
        {
            float currentFade = 1f;
            while (currentFade > 0f)
            {
                currentFade -= Time.deltaTime / fadeDuration;
                _popupPanel.transform.position = Vector3.Lerp(_popupPanel.transform.position, new Vector3(_actionSelectionPanel.transform.position.x, _popupPanel.transform.position.y, _popupPanel.transform.position.z), 1f - currentFade);
                await Task.Yield();
            }
        }
        _popupPanel.SetActive(false);
    }

    /// <summary>
    /// Shows the action selection panel.
    /// </summary>
    async Task ShowActionSelectionPanel()
    {
        _actionSelectionPanel.SetActive(true);
        SetActionButtonColorsUnselected();
        if (_selectedActionMarker != null)
        {
            if (_selectedActionMarker.ActionPair.SecondaryAction != null)
            {
                ActionType actionType = _selectedActionMarker.ActionPair.SecondaryAction.ActionType;
                SetColorOfActionButton(actionType);
                if (actionType == ActionType.Shoot || actionType == ActionType.Melee || actionType == ActionType.Reload || actionType == ActionType.Charge)
                {
                    await ShowPopup(actionType);
                }
            } else 
            {
                SetColorOfActionButton(ActionType.Move);
            }
        }
    }

    /// <summary>
    /// Hides the action selection panel.
    /// </summary>
    void HideActionSelectionPanel()
    {
        _actionSelectionPanel.SetActive(false);
        SetActionButtonColorsUnselected();
    }

    /// <summary>
    /// Handles the event when the shoot button is clicked.
    /// </summary>
    public async void OnShootButtonClicked()
    {
        Debug.Log("Shoot button clicked");
        SetActionButtonColorsUnselected();
        SetColorOfActionButton(ActionType.Shoot);
        await ShowPopup(ActionType.Shoot);
    }

    /// <summary>
    /// Handles the event when the melee button is clicked.
    /// </summary>
    public async void OnMeleeButtonClicked()
    {
        Debug.Log("Melee button clicked");
        SetActionButtonColorsUnselected();
        SetColorOfActionButton(ActionType.Melee);
        await ShowPopup(ActionType.Melee);
    }

    /// <summary>
    /// Handles the event when the reload button is clicked.
    /// </summary>
    public async void OnReloadButtonClicked()
    {
        Debug.Log("Reload button clicked");
        SetActionButtonColorsUnselected();
        SetColorOfActionButton(ActionType.Reload);
        await ShowPopup(ActionType.Reload);
    }

    /// <summary>
    /// Handles the event when the move button is clicked.
    /// </summary>
    public async void OnMoveButtonClicked()
    {
        Debug.Log("Move button clicked");
        SetActionButtonColorsUnselected();
        _selectedActionMarker.SetSecondaryAction(new MoveAction(_selectedMechController, _selectedActionMarker.transform.position));
        await ClosePanels();
    }

    /// <summary>
    /// Handles the event when the charge button is clicked.
    /// </summary>
    public async void OnChargeButtonClicked()
    {
        Debug.Log("Charge button clicked");
        SetActionButtonColorsUnselected();
        SetColorOfActionButton(ActionType.Charge);
        await ShowPopup(ActionType.Charge);
    }

    /// <summary>
    /// Handles the event when the hunker button is clicked.
    /// </summary>
    public async void OnHunkerButtonClicked()
    {
        Debug.Log("Hunker button clicked");
        SetActionButtonColorsUnselected();
        _selectedActionMarker.SetSecondaryAction(new HunkerAction(_selectedMechController));
        await ClosePanels();
    }

    /// <summary>
    /// Handles the event when the overwatch button is clicked.
    /// </summary>
    public async void OnOverwatchButtonClicked()
    {
        Debug.Log("Overwatch button clicked");
        SetActionButtonColorsUnselected();
        _selectedActionMarker.SetSecondaryAction(new OverwatchAction(_selectedMechController));
        await ClosePanels();
    } 

    /// <summary>
    /// Handles the event when the remove button is clicked.
    /// </summary>
    public async void OnRemoveButtonClicked()
    {
        Debug.Log("Remove button clicked");
        SetActionButtonColorsUnselected();
        if (_selectedActionMarker != null)
        {
            _selectedMechController.RemoveActionMarker(_selectedActionMarker);
            _selectedActionMarker = null;
        }
        await ClosePanels();
    }

    /// <summary>
    /// Handles the event when the cancel button is clicked.
    /// </summary>
    public async void OnCancelButtonClicked()
    {
        Debug.Log("Cancel button clicked");
        SetActionButtonColorsUnselected();
        await ClosePanels();
    }

    private void SetColorOfActionButton(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.Shoot:
                _shootButton.GetComponent<Image>().color = _popupOptionButtonSelectedColor;
                break;
            case ActionType.Melee:
                _meleeButton.GetComponent<Image>().color = _popupOptionButtonSelectedColor;
                break;
            case ActionType.Reload:
                _reloadButton.GetComponent<Image>().color = _popupOptionButtonSelectedColor;
                break;
            case ActionType.Move:
                _moveButton.GetComponent<Image>().color = _popupOptionButtonSelectedColor;
                break;
            case ActionType.Charge:
                _chargeButton.GetComponent<Image>().color = _popupOptionButtonSelectedColor;
                break;
            case ActionType.Hunker:
                _hunkerButton.GetComponent<Image>().color = _popupOptionButtonSelectedColor;
                break;
            case ActionType.Overwatch:
                _overwatchButton.GetComponent<Image>().color = _popupOptionButtonSelectedColor;
                break;
        }
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
    private async Task ClosePanels()
    {
        await HidePopup();
        HideMechInfoPanel();
        HideActionSelectionPanel();
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

    private void SetActionButtonColorsUnselected()
    {
        foreach (Transform button in _actionSelectionPanel.transform)
        {
            if (button == null) continue;
            button.GetComponent<Image>().color = _popupOptionButtonColor;
        }
    }

    private void CreatePlayerMechActionDisplays()
    {
        if (_playerMechActionDisplayPrefab == null || _playerMechActionDisplayParent == null)
            return;
        // Clear existing action displays
        foreach (Transform child in _playerMechActionDisplayParent)
        {
            Destroy(child.gameObject);
        }
        List<MechController> playerMechs = CombatManager.Instance.PlayerMechs;
        for (int i = 0; i < playerMechs.Count; i++)
        {
            GameObject actionDisplay = GameObject.Instantiate(_playerMechActionDisplayPrefab, _playerMechActionDisplayParent);
            float yOffset = i * actionDisplay.GetComponent<RectTransform>().rect.width;
            actionDisplay.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -yOffset);
            actionDisplay.GetComponent<RectTransform>().localScale = Vector3.one;
            actionDisplay.name = $"PlayerMechActionDisplay_{playerMechs[i].Mech.Name}";
            actionDisplay.GetComponentInChildren<Text>().text = playerMechs[i].Mech.Name;
            _playerMechActionDisplays[playerMechs[i]] = actionDisplay;
        }
    }

    public void AddActionToDisplay(ActionMarker actionMarker)
    {
        MechController mechController = actionMarker.SourceMech;
        if (_playerMechActionDisplays.ContainsKey(mechController))
        {
            GameObject actionDisplay = _playerMechActionDisplays[mechController];
            GameObject action = GameObject.Instantiate(_playerMechActionPrefab, actionDisplay.transform);
            StringBuilder sb = new StringBuilder();
            sb.Append(actionMarker.ActionPair.MovementAction.Destination);
            sb.Append(" ");
            if (actionMarker.ActionPair.SecondaryAction != null && actionMarker.ActionPair.MovementAction.ActionType != ActionType.Charge)
                sb.Append(actionMarker.ActionPair.SecondaryAction.ActionType);
            else if (actionMarker.ActionPair.MovementAction.ActionType == ActionType.Charge)
                sb.Append("Charge");
            else
                sb.Append("Move");
            action.GetComponentInChildren<Text>().text = sb.ToString();
            if (actionMarker.ActionPair.MovementAction.ActionType == ActionType.Charge)
            {
                action.GetComponent<DragDrop>().interactable = false;
                action.GetComponent<Image>().color = _popupOptionButtonSelectedColor;
            } else
            {
                action.GetComponent<DragDrop>().interactable = true;
                action.GetComponent<Image>().color = _popupOptionButtonColor;
            }
            action.GetComponent<DragDrop>().SetActionMarker(actionMarker);
            action.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            action.GetComponentInChildren<Button>().onClick.AddListener(() => RemoveActionFromDisplay(action, actionMarker));
        }
    }

    public void RemoveActionFromDisplay(GameObject action, ActionMarker actionMarker)
    {
        MechController mechController = actionMarker.SourceMech;
        mechController.RemoveActionMarker(actionMarker);
        Destroy(action);
    }

    // TODO: Implement functionality to display the action markers information in the UI when an action is selected in the action display
}
