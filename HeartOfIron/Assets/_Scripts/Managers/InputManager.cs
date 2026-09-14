using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;
using HeartOfIron.Enums;

[Serializable]
public class InputManager : MonoBehaviour
{

    // TODO: Test to make sure camera reference sets correctly through multiple loops of combat scene
    private Camera _camera;
    private bool _isHoldingMovement = false;
    private Vector3 _movementInput;
    private bool _isHoldingRotation = false;
    private Vector2 _rotationInput;
    [SerializeField] private float _movementSpeed = 10f;
    [SerializeField] private float _rotationSpeed = 20f;
    [SerializeField] private float _minCameraHeight = 5f; // Minimum height for the camera
    [SerializeField] private float _maxCameraHeight; // Maximum height for the camera
    [SerializeField] private float _maxCameraHeightBuffer = 50f; // Buffer to add to the maximum height based on terrain height
    private bool _isPointerOverUI = false;
    private bool _isGrabbing = false;
    private bool _isMessHallMode = false; // Flag to indicate if we are in Mess Hall mode

    void Start()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }
        _movementInput = Vector3.zero;
        _rotationInput = Vector2.zero;
    }

    void Update()
    {
        if (_isHoldingMovement && _isMessHallMode)
        {
            MessHallMoveCamera(_movementInput);
        } else if (_isHoldingMovement)
        {
            MoveCamera(_movementInput);
        }

        if (_isHoldingRotation && _isMessHallMode)
        {
            MessHallRotateCamera(_rotationInput);
        } else if (_isHoldingRotation)
        {
            RotateCamera(_rotationInput);
        }
        if (EventSystem.current != null)
        {
            _isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
        }
    }

    public void UpdateMaximumCameraHeight(float newMaxHeight)
    {
        _maxCameraHeight = newMaxHeight + _maxCameraHeightBuffer; // Add buffer to the maximum height based on terrain height
    }

    public void OnPing(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Ping action performed");
            if (_isPointerOverUI)
            {
                Debug.Log("Pointer is over a UI element");
                return;
            }
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~LayerMask.GetMask("Ignore Raycast")) && (CombatManager.Instance.CombatState == CombatState.Playing || CombatManager.Instance.CombatState == CombatState.Paused))
            {
                Debug.Log($"Ping hit {hit.collider.gameObject.name} at position {hit.point}");
                ActionMarker actionMarker = ActionMarkerAssembler.Instance.AssembleActionMarker(hit.point,CombatManager.Instance.ActiveMech);
            }
        }
        
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Debug.Log("Select action performed");
            if (_isPointerOverUI )
            {
                Debug.Log("Pointer is over a UI element");
                return;
            }
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit) && (CombatManager.Instance.CombatState == CombatState.Playing || CombatManager.Instance.CombatState == CombatState.Paused))
            {
                // Debug.Log($"Select hit {hit.collider.gameObject.name} at position {hit.point}");
                ISelectable selectable = hit.collider.GetComponent<ISelectable>();
                if (selectable != null)
                {
                    Debug.Log(selectable.OnSelected());
                }
                else
                {
                    Debug.Log("No ISelectable component found on the hit object.");
                }
            }
        }
        
    }

    public void OnToggle(InputAction.CallbackContext context)
    {
        if (context.performed && (CombatManager.Instance.CombatState == CombatState.Playing || CombatManager.Instance.CombatState == CombatState.Paused))
        {
            // Debug.Log("Toggle action performed");
            CombatManager.Instance.ToggleActiveMech();
        }
    }

    public void OnPlayPause(InputAction.CallbackContext context)
    {
        if (context.performed && (CombatManager.Instance.CombatState == CombatState.Playing || CombatManager.Instance.CombatState == CombatState.Paused))
        {
            // Debug.Log("Play/Pause action performed");
            if (CombatManager.Instance.CombatState == CombatState.Paused)
            {
                CombatManager.Instance.UpdateCombatState(CombatState.Playing);
            }
            else if (CombatManager.Instance.CombatState == CombatState.Playing)
            {
                CombatManager.Instance.UpdateCombatState(CombatState.Paused);
            }
            else 
            {
                Debug.LogWarning($"Combat state is {CombatManager.Instance.CombatState}, cannot toggle Play/Pause.");
            }
        }
    }

    public void OnCameraMove(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector3>();
        // Debug.Log($"Camera Move action performed: {_movementInput}");
        if (context.performed)
        {
            _isHoldingMovement = true;
            _isMessHallMode = false; // Ensure we are not in Mess Hall mode when moving the camera
        }
        else if (context.canceled)
        {
            _isHoldingMovement = false;
            _movementInput = Vector3.zero; // Reset movement input when the action is canceled
        }

        if (_isHoldingMovement)
        {
            // Move the camera based on the input movement vector
            MoveCamera(_movementInput);
        }
    }

    private void MoveCamera(Vector3 movement)
    {
        if (_camera == null)
        {
            Debug.LogError("Camera reference is not set in the InputManager.");
            return;
        } else if (movement == Vector3.zero)
        {
            return; // No movement input, so do nothing
        }
        _camera.transform.position = CalculateCameraBounds(movement);
    }

    private Vector3 CalculateCameraBounds(Vector3 movement)
    {
        TerrainGenerator terrainGenerator = FindFirstObjectByType<TerrainGenerator>();
        if (terrainGenerator == null)
        {
            Debug.LogError("TerrainGenerator not found in the scene.");
            return _camera.transform.position;
        }
        float minX = 0f;
        float maxX = terrainGenerator.terrainSize;
        float minZ = 0f;
        float maxZ = terrainGenerator.terrainSize;
        float minY = terrainGenerator.GetHeightAtPosition(_camera.transform.position.x, _camera.transform.position.z) + _minCameraHeight; // Minimum height above the terrain
        float maxY = _maxCameraHeight; // Maximum height for the camera

        // Break up movement into horizontal and vertical components

        Vector3 newPosition = _camera.transform.position + Vector3.Scale(_camera.transform.TransformDirection(movement), new Vector3(1, 0, 1)) * _movementSpeed * Time.deltaTime;
        Vector3 verticalMovement = new Vector3(0, movement.y, 0) * _movementSpeed * Time.deltaTime;
        newPosition += verticalMovement;
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
        return newPosition;
    }

    public void OnCameraRotate(InputAction.CallbackContext context)
    {
        _rotationInput = context.ReadValue<Vector2>();
        // Debug.Log($"Camera Rotate action performed: {_rotationInput}");
        if (context.performed)
        {
            _isHoldingRotation = true;
            _isMessHallMode = false; // Ensure we are not in Mess Hall mode when rotating the camera
        }
        else if (context.canceled)
        {
            _isHoldingRotation = false;
            _rotationInput = Vector2.zero; // Reset rotation input when the action is canceled
        }

        if (_isHoldingRotation)
        {
            // Rotate the camera based on the input rotation vector
            RotateCamera(_rotationInput);
        }
    }

    private void RotateCamera(Vector2 rotation)
    {
        _camera.transform.Rotate(Vector3.up, rotation.x * _rotationSpeed * Time.deltaTime);
        _camera.transform.Rotate(Vector3.right, -rotation.y * _rotationSpeed * Time.deltaTime);
    }

    public void OnSnapToHorizon(InputAction.CallbackContext context)
    {
        // Debug.Log("Snap to Horizon action triggered");
        if (context.performed)
        {
            // Debug.Log("Snap to Horizon action performed");
            Vector3 currentRotation = _camera.transform.eulerAngles;
            _camera.transform.eulerAngles = new Vector3(0f, currentRotation.y, 0f);
        }
    }

    public void OnMessHallMoveCamera(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector3>();
        // Debug.Log($"Camera Move action performed: {_movementInput}");
        if (context.performed)
        {
            _isHoldingMovement = true;
            _isMessHallMode = true; // Set Mess Hall mode when moving the camera
        }
        else if (context.canceled)
        {
            _isHoldingMovement = false;
            _movementInput = Vector3.zero; // Reset movement input when the action is canceled
        }

        if (_isHoldingMovement)
        {
            // Move the camera based on the input movement vector
            MessHallMoveCamera(_movementInput);
        }
    }

    private void MessHallMoveCamera(Vector3 movement)
    {
        if (_camera == null)
        {
            Debug.LogError("Camera reference is not set in the InputManager.");
            return;
        } else if (movement == Vector3.zero)
        {
            return; // No movement input, so do nothing
        }
        _camera.transform.position = CalculateMessHallCameraBounds(movement);
    }

    private Vector3 CalculateMessHallCameraBounds(Vector3 movement)
    {
        float minX = 0f;
        float maxX = 10f; // Assuming the Mess Hall area is 10 units
        float minZ = 0f;
        float maxZ = 10f; // Assuming the Mess Hall area is 10 units
        float minY = 1f; // Minimum height for the camera
        float maxY = 15f; // Maximum height for the camera

        Vector3 newPosition = _camera.transform.position + Vector3.Scale(_camera.transform.TransformDirection(movement), new Vector3(1, 0, 1)) * _movementSpeed * Time.deltaTime;
        Vector3 verticalMovement = new Vector3(0, movement.y, 0) * _movementSpeed * Time.deltaTime;
        newPosition += verticalMovement;
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
        return newPosition;
    }

    public void OnMessHallRotateCamera(InputAction.CallbackContext context)
    {
        _rotationInput = context.ReadValue<Vector2>();
        // Debug.Log($"Camera Rotate action performed: {_rotationInput}");
        if (context.performed)
        {
            _isHoldingRotation = true;
            _isMessHallMode = true; // Set Mess Hall mode when rotating the camera
        }
        else if (context.canceled)
        {
            _isHoldingRotation = false;
            _rotationInput = Vector2.zero; // Reset rotation input when the action is canceled
        }

        if (_isHoldingRotation)
        {
            // Rotate the camera based on the input rotation vector
            MessHallRotateCamera(_rotationInput);
        }
    }

    private void MessHallRotateCamera(Vector2 rotation)
    {
        _camera.transform.Rotate(Vector3.up, rotation.x * _rotationSpeed * Time.deltaTime);
        _camera.transform.Rotate(Vector3.right, -rotation.y * _rotationSpeed * Time.deltaTime);
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        if (context.performed && _isGrabbing && !_isPointerOverUI)
        {
            Vector2 input = context.ReadValue<Vector2>();
            // Debug.Log($"Grab action performed with input: {input}");
            //_isGrabbing = true;
            // Debug.Log("Grab action performed");
        }
        else if (context.canceled)
        {
            //_isGrabbing = false;
            // Debug.Log("Grab action canceled");
        }
    }

    public void CheckGrab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isGrabbing = true;
            // Debug.Log("Check Grab action performed");
        }
        else if (context.canceled)
        {
            _isGrabbing = false;
            // Debug.Log("Check Grab action canceled");
        }
    }
}
