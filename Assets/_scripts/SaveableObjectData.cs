using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

// Clase serializable para guardar los datos de cada objeto
[Serializable]
public class SaveableObjectData
{
    public int objectID;
    public SerializableVector3 position;
    public bool isRotated;
    public int placedObjectIndex;

    public SaveableObjectData(int id, Vector3 pos, bool rotated, int index)
    {
        objectID = id;
        position = new SerializableVector3(pos);
        isRotated = rotated;
        placedObjectIndex = index;
    }
}

// Vector3 serializable ya que Vector3 no lo es por defecto
[Serializable]
public class SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

// Clase contenedora para todos los datos de la escena
[Serializable]
public class SceneSaveData
{
    public List<SaveableObjectData> floorObjects = new();
    public List<SaveableObjectData> furnitureObjects = new();
}

