using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
     [SerializeField]
     private GameObject mouseIndicator, cellIndicator;

     [SerializeField]
     private InputManager inputManager;
     [SerializeField]
     private Grid grid;

     [SerializeField]
     private ObjectsDatabaseSO database;

     [SerializeField]
     private GameObject gridVisualization; 

     private int selectedObjectIndex = -1;

     // [SerializeField]
     // private AudioClip correctPlacementClip, wrongPlacementClip;
     // [SerializeField]
     // private AudioSource source;

     // private GridData floorData, furnitureData;

     // [SerializeField]
     // private PreviewSystem preview;

     // private Vector3Int lastDetectedPosition = Vector3Int.zero;

     // [SerializeField]
     // private ObjectPlacer objectPlacer;

     // IBuildingState buildingState;

     // [SerializeField]
     // private SoundFeedback soundFeedback;

     private void Start()
     {
          gridVisualization.SetActive(false);
          cellIndicator.SetActive(false);
          //floorData = new();
          //furnitureData = new();
     }

     private void Update(){
          if(selectedObjectIndex < 0)
               return;
          Vector3 mousePosition = inputManager.GetSelectedMapPosition();
          Vector3Int gridPosition = grid.WorldToCell(mousePosition); 
          mouseIndicator.transform.position = mousePosition;
          cellIndicator.transform.position = grid.CellToWorld(gridPosition);
     }

     private void PlaceStructure(){
          if(inputManager.IsPointerOverUI()){
               Debug.Log("Is pointer over UI");
               return;
          }
          Vector3 mousePosition = inputManager.GetSelectedMapPosition();
          Vector3Int gridPosition = grid.WorldToCell(mousePosition); 
          GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
          newObject.transform.position = grid.CellToWorld(gridPosition);
     }

     private void StopPlacement()
     {
          // soundFeedback.PlaySound(SoundType.Click);
          // if (buildingState == null)
          //      return;
          selectedObjectIndex = -1;
          gridVisualization.SetActive(false);
          cellIndicator.SetActive(false);
          //buildingState.EndState();
          inputManager.OnClicked -= PlaceStructure;
          inputManager.OnExit -= StopPlacement;
          //lastDetectedPosition = Vector3Int.zero;
          //buildingState = null;
     }
     public void StartPlacement(int ID)
     {
          StopPlacement();
          selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
          if(selectedObjectIndex < 0){
               Debug.LogError($"Object not found {ID}");
               return;
          }
          gridVisualization.SetActive(true);
          cellIndicator.SetActive(true);
          inputManager.OnClicked += PlaceStructure;
          inputManager.OnExit += StopPlacement;
     }
}