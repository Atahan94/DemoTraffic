using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveDataSerization 
{
    public List<BuilddingDataSerialization> structuresData = new List<BuilddingDataSerialization>();

    public void AddStructureData(Vector3Int position, int buildingPrefabIndex, CellType Type) 
    {
        structuresData.Add(new BuilddingDataSerialization(position, buildingPrefabIndex, Type));
    }
}

[Serializable]
public class BuilddingDataSerialization 
{
    public Vector3Serialization position;
    public int buildingPrefabIndex;
    public CellType buildingType;

    public BuilddingDataSerialization(Vector3Int position, int buildingPrefabIndex, CellType buildingType)
    {
        this.position = new Vector3Serialization(position);
        this.buildingPrefabIndex = buildingPrefabIndex;
        this.buildingType = buildingType;
    }
}

[Serializable]
public class Vector3Serialization
{
    public float x, y, z;

    public Vector3Serialization(Vector3 position)
    {
        this.x = position.x;
        this.y = position.y;
        this.z = position.z;
    }

    public Vector3 GetValue()
    {
        return new Vector3(x, y, z);
    }
}
