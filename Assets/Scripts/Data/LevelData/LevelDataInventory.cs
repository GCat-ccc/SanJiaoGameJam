using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Level/LevelDataInventory", fileName = "LevelDataInventory")]
public class LevelDataInventory : ScriptableObject
{
    public List<LevelData> levelDatas;

    public LevelData GetLevelData(int levelIndex)
    {
        if(levelIndex > levelDatas.Count - 1)
        {
            return null;
        }

        return levelDatas[levelIndex];
    }
}

