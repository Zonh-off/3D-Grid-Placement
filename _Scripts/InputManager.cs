using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public class InputManager : MonoBehaviour {
    public static InputManager Instance { get; private set; }

    public event Action OnClicked, OnExit;

    public event EventHandler<OnSelectedBuildingChangedEventArgs> OnSelectedBuildingChanged;
    public class OnSelectedBuildingChangedEventArgs : EventArgs {
        public BaseObject selectedBuilding;
    }

    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask placementLayermask;
    [SerializeField] private LayerMask placedLayermask;
    [SerializeField] private Grid grid;
    [SerializeField] private PlacementSystem placementSystem;

    private Vector3 lastPosition;

    private BaseObject baseBuilding;

    private void Start() {
        Instance = this;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.F)) {
            HandleInteractions();
        } else if(Input.GetKeyDown(KeyCode.G)) {
            SetSelectedBuilding(null);
        }

        if(Input.GetMouseButtonDown(0)) {
            OnClicked?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.Escape)) {
            OnExit?.Invoke();
        }
    }

    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject(0);

    public Vector3 OnSelectedMapPosition() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 1000, placementLayermask)) {
            lastPosition = hit.point;
        }
        return lastPosition;
    }

    private void HandleInteractions() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 1000, placedLayermask)) {
            if(hit.transform.TryGetComponent(out BaseObject baseBuilding)) {
                if(baseBuilding != this.baseBuilding) {
                    SetSelectedBuilding(baseBuilding);
                }
            } else {
                if(Input.GetKeyDown(KeyCode.Escape)) {
                    SetSelectedBuilding(null);
                }
            }
        } else {
            if(Input.GetKeyDown(KeyCode.Escape)) {
                SetSelectedBuilding(null);
            }
        }
    }

    private void SetSelectedBuilding(BaseObject selectedBuilding) {
        this.baseBuilding = selectedBuilding;
        OnSelectedBuildingChanged?.Invoke(this, new OnSelectedBuildingChangedEventArgs {
            selectedBuilding = selectedBuilding
        });
    }
}
