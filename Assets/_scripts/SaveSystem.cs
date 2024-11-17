using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// Clase principal del sistema de guardado
public class SaveSystem : MonoBehaviour 
{
    [SerializeField] private ObjectsDatabaseSO database;
    [SerializeField] private ObjectPlacer objectPlacer;
    private GridData floorData;
    private GridData furnitureData;
    private Grid grid;

    private string SavePath => Path.Combine(Application.persistentDataPath, "scene_save.json");

    public void Initialize(GridData floorData, GridData furnitureData, Grid grid)
    {
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.grid = grid;
    }

    public void SaveScene()
    {
        var saveData = new SceneSaveData();
        
        // Guardar los datos del suelo
        SaveGridData(floorData, saveData.floorObjects);
        
        // Guardar los datos de los muebles
        SaveGridData(furnitureData, saveData.furnitureObjects);

        // Convertir a JSON y guardar
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);
        
        Debug.Log($"Scene saved to: {SavePath}");
    }

    private void SaveGridData(GridData gridData, List<SaveableObjectData> saveList)
    {
        // Obtener todos los objetos únicos (evitando duplicados por ocupar múltiples celdas)
        var processedIndices = new HashSet<int>();

        // Aquí necesitarías añadir un método en GridData para obtener todas las posiciones ocupadas
        var allPositions = gridData.GetAllPositions();
        
        foreach (var pos in allPositions)
        {
            int index = gridData.GetRepresentationIndex(pos);
            if (!processedIndices.Contains(index))
            {
                processedIndices.Add(index);
                
                Vector3 worldPos = grid.CellToWorld(pos);
                int id = gridData.GetObjectIDAt(pos);
                // Aquí deberías añadir un método para obtener la rotación del objeto
                bool isRotated = objectPlacer.IsObjectRotated(index);
                
                saveList.Add(new SaveableObjectData(id, worldPos, isRotated, index));
            }
        }
    }

    public void LoadScene()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("No save file found!");
            return;
        }

        // Limpiar la escena actual
        objectPlacer.RemoveAllObjects();
        floorData = new GridData();
        furnitureData = new GridData();

        // Cargar y parsear el JSON
        string json = File.ReadAllText(SavePath);
        SceneSaveData saveData = JsonUtility.FromJson<SceneSaveData>(json);

        // Recolocar todos los objetos
        LoadObjects(saveData.floorObjects, floorData);
        LoadObjects(saveData.furnitureObjects, furnitureData);
    }

    private void LoadObjects(List<SaveableObjectData> objectsData, GridData gridData)
    {
        foreach (var objData in objectsData)
        {
            // Encontrar el prefab en la base de datos
            var objectInfo = database.objectsData.Find(x => x.ID == objData.objectID);
            if (objectInfo != null)
            {
                Vector3 rotation = objData.isRotated ? new Vector3(0, 90, 0) : Vector3.zero;
                Vector3 worldPos = objData.position.ToVector3();
                Vector3Int gridPos = grid.WorldToCell(worldPos);

                // Recrear el objeto
                int index = objectPlacer.PlaceObject(objectInfo.Prefab, worldPos, rotation);

                // Actualizar la grid
                Vector2Int size = objData.isRotated ? 
                    new Vector2Int(objectInfo.Size.y, objectInfo.Size.x) : 
                    objectInfo.Size;

                gridData.AddObjectAt(gridPos, size, objData.objectID, index);
            }
        }
    }
}

