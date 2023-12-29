using Entity.Cells;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class LevelData
{
    [Header("每关可用细胞的数据")]
    public List<UsableCellData> cellsList;

    [Header("关卡tips")]
    public List<string> tips;
}

[System.Serializable]
public class UsableCellData
{
    public int count;

    public CellName name;

    //public BaseCell cell;
}
