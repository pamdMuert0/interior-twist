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
     private InputManager inputManager;
     [SerializeField]
     private Grid grid;
     [SerializeField]
     private ObjectsDatabaseSO database;
     [SerializeField]
     private GameObject gridVisualization; 
     private GridData floorData, furnitureData;
     [SerializeField]
     private ObjectPlacer objectPlacer;
     [SerializeField]
     private PreviewSystem preview;
     private Vector3Int lastDetectedPosition = Vector3Int.zero;
     IBuildingState buildingState;

     [SerializeField]
     private SaveSystem saveSystem;

     private void Start()
     {
          gridVisualization.SetActive(false);
          preview.StopShowingPreview();
          floorData = new();
          furnitureData = new();
          saveSystem.Initialize(floorData,furnitureData,grid);
     }

     private void Update(){

          if (Input.GetKeyDown(KeyCode.R))
          {
               if (buildingState is IRotatable rotatableState)
               {
                    rotatableState.Rotate();
               }
          }

          // Guardar con S
          if (Input.GetKeyDown(KeyCode.S))
          {
               Debug.Log("jojo");
               saveSystem?.SaveScene();
          }
          
          // Cargar con L
          if (Input.GetKeyDown(KeyCode.L))
          {
               saveSystem?.LoadScene();
          }
          // if (Input.GetKey(KeyCode.Tab)){
          //      pnPause.SetActive(true);
          //      gridVisualization.SetActive(false);
          //      preview.StopShowingPreview();
          // }
          // // Reducir el tiempo actual basado en deltaTime
          // currentTime -= Time.deltaTime;
          // // Asegurarse de que el tiempo no sea negativo
          // if (currentTime < 0f)
          // {
          //      currentTime = 0f;
          // }
          // // Actualizar el fillAmount basado en el tiempo restante
          // radialImage.fillAmount = currentTime / totalTime;
          // // Opción: Si quieres que ocurra algo cuando el tiempo llega a 0
          // if (currentTime == 0f)
          // {
          //      // Acción cuando el tiempo se acaba
          //      Debug.Log("¡El tiempo se ha acabado!");
          // }

          if(buildingState == null){
               return;
          }
          Vector3 mousePosition = inputManager.GetSelectedMapPosition();
          Vector3Int gridPosition = grid.WorldToCell(mousePosition); 
          if(lastDetectedPosition != gridPosition){
               buildingState.UpdateState(gridPosition);
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
          buildingState.OnAction(gridPosition);
     }

     
     private void StopPlacement()
     {
          if(buildingState == null){
               return;
          }
          gridVisualization.SetActive(false);
          buildingState.EndState();
          inputManager.OnClicked -= PlaceStructure;
          inputManager.OnExit -= StopPlacement;
          lastDetectedPosition = Vector3Int.zero;
          buildingState = null;
     }

     public void StartPlacement(int ID)
     {
          StopPlacement();
          gridVisualization.SetActive(true);
          buildingState = new PlacementState(ID, 
                                             grid, 
                                             preview, 
                                             database, 
                                             floorData, 
                                             furnitureData, 
                                             objectPlacer);

          inputManager.OnClicked += PlaceStructure;
          inputManager.OnExit += StopPlacement;
     }

     public void StartRemoving(){
          StopPlacement();
          gridVisualization.SetActive(true);
          buildingState = new RemovingState(grid, preview, floorData, furnitureData, objectPlacer);
          inputManager.OnClicked += PlaceStructure;
          inputManager.OnExit += StopPlacement;
     }

     public void StartReplacementMode()
     {
          StopPlacement();
          gridVisualization.SetActive(true);
          buildingState = new RePlacementState(grid, 
                                             preview, 
                                             database, 
                                             floorData, 
                                             furnitureData, 
                                             objectPlacer);
          inputManager.OnClicked += PlaceStructure;
          inputManager.OnExit += StopPlacement;
     }

     /*
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
          //StopPlacement();
          pnPause.SetActive(false);
          placedGameObjects.Clear();

          gridVisualization.SetActive(false);
          preview.StopShowingPreview();
          floorData = new();
          furnitureData = new();

          currentTime = totalTime;
          pnGame.SetActive(false);
     }*/
}