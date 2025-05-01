using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField] private ObjectsDatabaseSO database;

    [SerializeField] private GameObject gridVisualization;

    [SerializeField] private PreviewSystem preview;

    [SerializeField] private ObjectPlacer objectPlacer;

    [SerializeField] private SoundFeedback soundFeedback;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    private GridData floorData, buidingData;

    IBuildingState buildingState;

    private void Start() {
        StopPlacement();
        floorData = new();
        buidingData = new();
    }

    public void StartPlacement(int ID) {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new PlacementState(ID,
                                         grid,
                                         preview,
                                         database,
                                         floorData,
                                         buidingData,
                                         objectPlacer,
                                         soundFeedback);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving() {
        StopPlacement();
        //gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, preview, floorData, buidingData, objectPlacer);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StopPlacement() {
        if(buildingState == null)
            return;
        gridVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    public void PlaceStructure() {
        if(inputManager.IsPointerOverUI())
            return;

        Vector3 mousePos = inputManager.OnSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePos);

        buildingState.OnAction(gridPosition);
    }

    //private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex) {
    //    GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;

    //    return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    //}


    void Update() {
        if(buildingState == null)
            return;
        Vector3 mousePos = inputManager.OnSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePos);
        if(lastDetectedPosition != gridPosition) {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }
}
