using SimpleCity.AI;
using SVS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CameraMovement cameraMovement;
    public RoadManager roadManager;
    public InputManager inputManager;

    public UIController uiController;

    public StructureManager structureManager;

    public ObjectDetector objectDetector;

    public PathVisualizer pathVisualizer;

    public SaveSystem saveSystem;

    void Start()
    {
        uiController.OnRoadPlacement += RoadPlacementHandler;
        uiController.OnHousePlacement += HousePlacementHandler;
        uiController.OnSpecialPlacement += SpecialPlacementHandler;
        uiController.OnBigStructurePlacement += BigStructurePlacement;
        inputManager.OnEscape += HandleEscape;

        Invoke("LoadGame", 5);
    }

    private void HandleEscape()
    {
        ClearInputActions();
        uiController.ResetButtonColor();
        pathVisualizer.ResetPath();
        inputManager.OnMouseClick += TrySelectingAgent;
    }

    private void TrySelectingAgent(Ray ray)
    {
        GameObject hitObject = objectDetector.RaycastAll(ray);
        if(hitObject != null)
        {
            var agentScript = hitObject.GetComponent<AiAgent>();
            agentScript?.ShowPath();
        }
    }

    private void BigStructurePlacement()
    {
        ClearInputActions();

        inputManager.OnMouseClick += (pos) =>
        {
            ProcessInputAndCall(structureManager.PlaceBigStructure, pos);
        };
        inputManager.OnEscape += HandleEscape;
    }

    private void SpecialPlacementHandler()
    {
        ClearInputActions();

        inputManager.OnMouseClick += (pos) =>
        {
            ProcessInputAndCall(structureManager.PlaceSpecial, pos);
        };
        inputManager.OnEscape += HandleEscape;
    }

    private void HousePlacementHandler()
    {
        ClearInputActions();

        inputManager.OnMouseClick += (pos) =>
        {
            ProcessInputAndCall(structureManager.PlaceHouse, pos);
        };
        inputManager.OnEscape += HandleEscape;
    }

    private void RoadPlacementHandler()
    {
        ClearInputActions();

        inputManager.OnMouseClick += (pos) =>
        {
            ProcessInputAndCall(roadManager.PlaceRoad, pos);
        };
        inputManager.OnMouseUp += roadManager.FinishPlacingRoad;
        inputManager.OnMouseHold += (pos) =>
        {
            ProcessInputAndCall(roadManager.PlaceRoad, pos);
        };
        inputManager.OnEscape += HandleEscape;
    }

    private void ClearInputActions()
    {
        inputManager.ClearEvents();
    }

    private void ProcessInputAndCall(Action<Vector3Int> callback, Ray ray)
    {
        Vector3Int? result = objectDetector.RaycastGround(ray);
        if (result.HasValue)
            callback.Invoke(result.Value);
    }



    private void Update()
    {
        cameraMovement.MoveCamera(new Vector3(inputManager.CameraMovementVector.x, 0, inputManager.CameraMovementVector.y));
    }

    public void SaveGame() 
    {
        SaveDataSerization saveData = new SaveDataSerization();
        foreach (var structureData in structureManager.GetAllStructures())
        {
            saveData.AddStructureData(structureData.Key, structureData.Value.BuildingPrefabIndex, structureData.Value.BuildingType);
        }
        var jsonFormat = JsonUtility.ToJson(saveData);
        Debug.Log(jsonFormat);
        saveSystem.SaveData(jsonFormat);
    }
    public void LoadGame() 
    {
        var jsonFormatData = saveSystem.LoadData();
        if (String.IsNullOrEmpty(jsonFormatData))
            return;
        SaveDataSerization saveData = JsonUtility.FromJson<SaveDataSerization>(jsonFormatData);
        structureManager.ClearMap();
        foreach (var structureData in saveData.structuresData)
        {
            Vector3Int position = Vector3Int.RoundToInt(structureData.position.GetValue());
            if (structureData.buildingType == CellType.Road) 
            {
                roadManager.PlaceRoad(position);
                roadManager.FinishPlacingRoad();
            }
            else 
            {
                structureManager.PlacedLoadedStructure(position, structureData.buildingPrefabIndex, structureData.buildingType);
            }
        }
    }
}
