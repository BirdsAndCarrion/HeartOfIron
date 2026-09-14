using UnityEngine;
using System;
using TMPro;
using HeartOfIron.Enums;
using UnityEngine.UI;

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
        EventRouter.ROnDeploymentStarted.AddListener(TestEventRecieved);
        EventRouter.ROnCombatStateChanged.AddListener(HandleCombatStateChanged);
        EventRouter.ROnActionMarkerSelected.AddListener(OnSelected);
    }

    void OnDestroy()
    {
        EventRouter.ROnCombatStarted.RemoveListener(EnterCombat);
        EventRouter.ROnCombatEnded.RemoveListener(ExitCombat);
        EventRouter.ROnDeploymentStarted.RemoveListener(ShowDeploymentPanel);
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

    private void TestEventRecieved()
    {
        Debug.Log("Test event received in CombatUIManager");
    }

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

    public void EnterCombat()
    {
        foreach (Transform child in _deploymentPanel.transform)
        {
            Destroy(child.gameObject);
        }
        _deploymentPanel.SetActive(false);
        _bottomBarPanel.SetActive(true);
    }

    public void OnSelected(MechController mechController)
    {
        Debug.Log($"MechController selected: {mechController.Mech.Name}");
        _selectedMechController = mechController;
        _selectedActionMarker = ActionMarkerAssembler.Instance.AssembleActionMarker(mechController.transform.position, mechController, false);
        _selectedMechController.InsertActionMarkerAt(_selectedActionMarker, 0);
        ShowMechInfoPanel(mechController);
        ShowActionSelectionPanel();
    }

    public void OnSelected(ActionMarker actionMarker)
    {
        Debug.Log($"ActionMarker selected: {actionMarker.Id}, Source Mech: {actionMarker.ActionPair.MovementAction.SourceMech.Mech.Name}");
        _selectedMechController = actionMarker.ActionPair.MovementAction.SourceMech;
        _selectedActionMarker = actionMarker;
        ShowMechInfoPanel(actionMarker.ActionPair.MovementAction.SourceMech);
        ShowActionSelectionPanel();
    }

    void OnDeselected()
    {
        HideMechInfoPanel();
        HideActionSelectionPanel();
    }

    public void OnGamePaused()
    {
        _pauseButton.GetComponent<Image>().sprite = _playIcon;
        _pauseButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _pauseButton.GetComponent<Button>().onClick.AddListener(OnPlayButtonClicked);
    }

    public void OnGameResumed()
    {
        _pauseButton.GetComponent<Image>().sprite = _pauseIcon;
        _pauseButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _pauseButton.GetComponent<Button>().onClick.AddListener(OnPauseButtonClicked);
    }

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

    private void UpdateHealthBars(MechController mechController)
    {
        UpdateResourceBar(mechController.Mech.Health[(int)BodyPart.Head], mechController.Mech.MaxHealth[(int)BodyPart.Head], _headHealthBar);
        UpdateResourceBar(mechController.Mech.Health[(int)BodyPart.Torso], mechController.Mech.MaxHealth[(int)BodyPart.Torso], _torsoHealthBar);
        UpdateResourceBar(mechController.Mech.Health[(int)BodyPart.RightArm], mechController.Mech.MaxHealth[(int)BodyPart.RightArm], _rightArmHealthBar);
        UpdateResourceBar(mechController.Mech.Health[(int)BodyPart.LeftArm], mechController.Mech.MaxHealth[(int)BodyPart.LeftArm], _leftArmHealthBar);
        UpdateResourceBar(mechController.Mech.Health[(int)BodyPart.RightLeg], mechController.Mech.MaxHealth[(int)BodyPart.RightLeg], _rightLegHealthBar);
        UpdateResourceBar(mechController.Mech.Health[(int)BodyPart.LeftLeg], mechController.Mech.MaxHealth[(int)BodyPart.LeftLeg], _leftLegHealthBar);
    }

    private void UpdateArmorBars(MechController mechController)
    {
        if (mechController.Mech.Inventory.Armors[0] != null)
        {
            UpdateResourceBar(mechController.Mech.Inventory.Armors[(int)ArmorType.Head].CurrentDurabilityR, mechController.Mech.Inventory.Armors[(int)ArmorType.Head].MaxDurability, _headArmorBar);
        }
        else
        {
            UpdateResourceBar(0, 1, _headArmorBar); // Set to empty if no armor
        }
        if (mechController.Mech.Inventory.Armors[(int)ArmorType.Torso] != null)
        {
            UpdateResourceBar(mechController.Mech.Inventory.Armors[(int)ArmorType.Torso].CurrentDurabilityR, mechController.Mech.Inventory.Armors[(int)ArmorType.Torso].MaxDurability, _torsoArmorBar);
        }
        else
        {
            UpdateResourceBar(0, 1, _torsoArmorBar); // Set to empty if no armor
        }
        if (mechController.Mech.Inventory.Armors[(int)ArmorType.Arms] != null)
        {
            UpdateResourceBar(mechController.Mech.Inventory.Armors[(int)ArmorType.Arms].CurrentDurabilityR, mechController.Mech.Inventory.Armors[(int)ArmorType.Arms].MaxDurability, _rightArmArmorBar);
            UpdateResourceBar(mechController.Mech.Inventory.Armors[(int)ArmorType.Arms].CurrentDurabilityL, mechController.Mech.Inventory.Armors[(int)ArmorType.Arms].MaxDurability, _leftArmArmorBar);
        }
        else
        {
            UpdateResourceBar(0, 1, _rightArmArmorBar); // Set to empty if no armor
            UpdateResourceBar(0, 1, _leftArmArmorBar); // Set to empty if no armor
        }
        if (mechController.Mech.Inventory.Armors[(int)ArmorType.Legs] != null)
        {
            UpdateResourceBar(mechController.Mech.Inventory.Armors[(int)ArmorType.Legs].CurrentDurabilityL, mechController.Mech.Inventory.Armors[(int)ArmorType.Legs].MaxDurability, _leftLegArmorBar);
            UpdateResourceBar(mechController.Mech.Inventory.Armors[(int)ArmorType.Legs].CurrentDurabilityR, mechController.Mech.Inventory.Armors[(int)ArmorType.Legs].MaxDurability, _rightLegArmorBar);
        }
        else
        {
            UpdateResourceBar(0, 1, _leftLegArmorBar); // Set to empty if no armor
            UpdateResourceBar(0, 1, _rightLegArmorBar); // Set to empty if no armor
        }
    }

    private void UpdateEnergyShieldBar(MechController mechController)
    {
        if (mechController.Mech.Inventory.EnergyShield != null)
        {
            UpdateResourceBar(mechController.Mech.Inventory.EnergyShield.CurrentCapacity, mechController.Mech.Inventory.EnergyShield.MaxCapacity, _energyShieldBar);
        }
        else
        {
            UpdateResourceBar(0, 1, _energyShieldBar); // Set to empty if no energy shield
        }
    }

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

    void HideMechInfoPanel()
    {
        _mechInfoPanel.SetActive(false);
    }

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
                _weaponDropdown.RefreshShownValue();
                _weaponDropdown.onValueChanged.AddListener(delegate { UpdateTargetDropdown(); });
                UpdateTargetDropdown();
                _confirmButton.onClick.RemoveAllListeners();
                _confirmButton.onClick.AddListener(() =>
                {
                    Weapon selectedWeapon = Array.Find(_selectedMechController.Mech.Inventory.Weapons, w => w != null && w.Name == _weaponDropdown.options[_weaponDropdown.value].text);
                    MechController targetMechController = CombatManager.Instance.EnemyMechs.Find(m => m.Mech.Name == _targetDropdown.options[_targetDropdown.value].text);
                    ShootAction shootAction = new ShootAction(_selectedMechController, targetMechController, selectedWeapon);
                    _selectedActionMarker.SetSecondaryAction(shootAction);
                    Debug.Log($"Shoot action set for ActionMarker {_selectedActionMarker.Id} with weapon {selectedWeapon.Name} targeting {targetMechController.Mech.Name}");
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
                _weaponDropdown.RefreshShownValue();
                _weaponDropdown.onValueChanged.AddListener(delegate { UpdateTargetDropdown(); });
                UpdateTargetDropdown();
                _confirmButton.onClick.RemoveAllListeners();
                _confirmButton.onClick.AddListener(() =>
                {
                    Weapon selectedWeapon = Array.Find(_selectedMechController.Mech.Inventory.Weapons, w => w != null && w.Name == _weaponDropdown.options[_weaponDropdown.value].text);
                    MechController targetMechController = CombatManager.Instance.EnemyMechs.Find(m => m.Mech.Name == _targetDropdown.options[_targetDropdown.value].text);
                    MeleeAction meleeAction = new MeleeAction(_selectedMechController, targetMechController, selectedWeapon);
                    _selectedActionMarker.SetSecondaryAction(meleeAction);
                    Debug.Log($"Melee action set for ActionMarker {_selectedActionMarker.Id} with weapon {selectedWeapon.Name} targeting {targetMechController.Mech.Name}");
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
                _weaponDropdown.RefreshShownValue();
                _confirmButton.onClick.RemoveAllListeners();
                _confirmButton.onClick.AddListener(() =>
                {
                    Weapon selectedWeapon = Array.Find(_selectedMechController.Mech.Inventory.Weapons, w => w != null && w.Name == _weaponDropdown.options[_weaponDropdown.value].text);
                    ReloadAction reloadAction = new ReloadAction(_selectedMechController, selectedWeapon);
                    _selectedActionMarker.SetSecondaryAction(reloadAction);
                    Debug.Log($"Reload action set for ActionMarker {_selectedActionMarker.Id} with weapon {selectedWeapon.Name}");
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
                _weaponDropdown.RefreshShownValue();
                _weaponDropdown.onValueChanged.AddListener(delegate { UpdateTargetDropdown(); });
                UpdateTargetDropdown();
                _confirmButton.onClick.RemoveAllListeners();
                _confirmButton.onClick.AddListener(() =>
                {
                    Weapon selectedWeapon = Array.Find(_selectedMechController.Mech.Inventory.Weapons, w => w != null && w.Name == _weaponDropdown.options[_weaponDropdown.value].text);
                    MechController targetMechController = CombatManager.Instance.EnemyMechs.Find(m => m.Mech.Name == _targetDropdown.options[_targetDropdown.value].text);
                    ChargeAction chargeAction = new ChargeAction(_selectedMechController, targetMechController, selectedWeapon);
                    _selectedActionMarker.SetSecondaryAction(chargeAction);
                    Debug.Log($"Charge action set for ActionMarker {_selectedActionMarker.Id} with weapon {selectedWeapon.Name} targeting {targetMechController.Mech.Name}");
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

    private void UpdateTargetDropdown()
    {
        // TODO: Update the target dropdown based on the selected weapon and available enemy mechs
        _targetDropdown.ClearOptions();
        var enemyMechs = CombatManager.Instance.EnemyMechs;
        var options = new System.Collections.Generic.List<string>();
        foreach (var enemyMech in enemyMechs)
        {
            options.Add(enemyMech.Mech.Name);
        }
        _targetDropdown.AddOptions(options);
    }

    void HidePopup()
    {
        _popupPanel.SetActive(false);
    }

    void ShowActionSelectionPanel()
    {
        _actionSelectionPanel.SetActive(true);
    }

    void HideActionSelectionPanel()
    {
        _actionSelectionPanel.SetActive(false);
    }

    public void OnShootButtonClicked()
    {
        Debug.Log("Shoot button clicked");
        ShowPopup("Shoot");
    }

    public void OnMeleeButtonClicked()
    {
        Debug.Log("Melee button clicked");
        ShowPopup("Melee");
    }

    public void OnReloadButtonClicked()
    {
        Debug.Log("Reload button clicked");
        ShowPopup("Reload");
    }

    public void OnMoveButtonClicked()
    {
        Debug.Log("Move button clicked");
        _selectedActionMarker.SetSecondaryAction(new MoveAction(_selectedMechController, _selectedActionMarker.transform.position));
        ClosePanels();
    }

    public void OnChargeButtonClicked()
    {
        Debug.Log("Charge button clicked");
        ShowPopup("Charge");
    }

    public void OnHunkerButtonClicked()
    {
        Debug.Log("Hunker button clicked");
        _selectedActionMarker.SetSecondaryAction(new HunkerAction(_selectedMechController));
        ClosePanels();
    }

    public void OnOverwatchButtonClicked()
    {
        Debug.Log("Overwatch button clicked");
        _selectedActionMarker.SetSecondaryAction(new OverwatchAction(_selectedMechController));
        ClosePanels();
    } 

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

    public void OnCancelButtonClicked()
    {
        Debug.Log("Cancel button clicked");
        ClosePanels();
    }

    public void OnPauseButtonClicked()
    {
        CombatManager.Instance.UpdateCombatState(CombatState.Paused);
        // Clear listeners from the pause button to prevent multiple subscriptions
        _pauseButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _pauseButton.GetComponent<Button>().onClick.AddListener(OnPlayButtonClicked);
    }

    public void OnPlayButtonClicked()
    {
        CombatManager.Instance.UpdateCombatState(CombatState.Playing);
        // Clear listeners from the play button to prevent multiple subscriptions
        _pauseButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _pauseButton.GetComponent<Button>().onClick.AddListener(OnPauseButtonClicked);
    }

    private void ClosePanels()
    {
        HideMechInfoPanel();
        HideActionSelectionPanel();
        HidePopup();
    }

    public void ExitCombat(bool isVictory, int rewards)
    {
        _exitCombatPanel.SetActive(true);
        _exitDeclarationText.text = isVictory ? "Victory!" : "Defeat!";
        _exitRewardsText.text = $"Rewards: {rewards}";
    }

    public void OnExitCombatButtonClicked()
    {
        Debug.Log("Exit Combat button clicked");
        _exitCombatPanel.SetActive(false);
        SceneLoader.Instance.LoadScene(SceneName.Garage);
    }

}
