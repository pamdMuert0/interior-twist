using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlacementSystem : MonoBehaviour
{

     public Image radialImage;  // La imagen radial
     public float totalTime = 60f;  // Tiempo total para que la imagen se vacíe
     private float currentTime;  // Tiempo actual del contador

     public GameObject pnPause;
     public GameObject pnGame;

     [SerializeField]
     private GameObject mouseIndicator;

     [SerializeField]
     private InputManager inputManager;
     [SerializeField]
     private Grid grid;

     [SerializeField]
     private ObjectsDatabaseSO database;

     [SerializeField]
     private GameObject gridVisualization; 

     private int selectedObjectIndex = -1;

     private GridData floorData, furnitureData;

     private List<GameObject> placedGameObjects = new ();

     [SerializeField]
     private PreviewSystem preview;

     private Vector3Int lastDetectedPosition = Vector3Int.zero;

     private void Start()
     {
          gridVisualization.SetActive(false);
          preview.StopShowingPreview();
          floorData = new();
          furnitureData = new();
          StopPlacement();
     }

     private void Update(){
          if (Input.GetKey(KeyCode.Tab)){
               pnPause.SetActive(true);
               gridVisualization.SetActive(false);
               preview.StopShowingPreview();
          }
          // Reducir el tiempo actual basado en deltaTime
          currentTime -= Time.deltaTime;

          // Asegurarse de que el tiempo no sea negativo
          if (currentTime < 0f)
          {
               currentTime = 0f;
          }

          // Actualizar el fillAmount basado en el tiempo restante
          radialImage.fillAmount = currentTime / totalTime;

          // Opción: Si quieres que ocurra algo cuando el tiempo llega a 0
          if (currentTime == 0f)
          {
               // Acción cuando el tiempo se acaba
               Debug.Log("¡El tiempo se ha acabado!");
          }

          if(selectedObjectIndex < 0)
               return;
          Vector3 mousePosition = inputManager.GetSelectedMapPosition();
          Vector3Int gridPosition = grid.WorldToCell(mousePosition); 
          if(lastDetectedPosition != gridPosition){
               bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
               mouseIndicator.transform.position = mousePosition;
               preview.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
               lastDetectedPosition = gridPosition;
          }
     }

     private void PlaceStructure(){
          if(inputManager.IsPointerOverUI()){
               Debug.Log("Is pointer over UI");
               return;
          }
          Vector3 mousePosition = inputManager.GetSelectedMapPosition();
          Vector3Int gridPosition = grid.WorldToCell(mousePosition); 

          bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
          if(!placementValidity)
               return;

          GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
          newObject.transform.position = grid.CellToWorld(gridPosition);
          placedGameObjects.Add(newObject);
          GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData  : furnitureData;
          selectedData.AddObjectAt(gridPosition, 
                                   database.objectsData[selectedObjectIndex].Size, 
                                   database.objectsData[selectedObjectIndex].ID, 
                                   placedGameObjects.Count - 1);
          preview.UpdatePosition(grid.CellToWorld(gridPosition), false);
     }

     private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex){
          GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData  : furnitureData;

          return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
     }

     private void StopPlacement()
     {
          selectedObjectIndex = -1;
          gridVisualization.SetActive(false);
          preview.StopShowingPreview();
          inputManager.OnClicked -= PlaceStructure;
          inputManager.OnExit -= StopPlacement;
          lastDetectedPosition = Vector3Int.zero;
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
          preview.StartShowingPlacementPreview(
               database.objectsData[selectedObjectIndex].Prefab,
               database.objectsData[selectedObjectIndex].Size
          );
          inputManager.OnClicked += PlaceStructure;
          inputManager.OnExit += StopPlacement;
     }
     public void restart(){
          StopPlacement();
          pnPause.SetActive(false);
          placedGameObjects.Clear();

          gridVisualization.SetActive(false);
          preview.StopShowingPreview();
          floorData = new();
          furnitureData = new();

     }
     public void iniciar(){
          StopPlacement();
          pnPause.SetActive(false);
          placedGameObjects.Clear();

          gridVisualization.SetActive(false);
          preview.StopShowingPreview();
          floorData = new();
          furnitureData = new();

          currentTime = totalTime;
          pnGame.SetActive(false);
     }
}