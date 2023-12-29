using Entity.Cells;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class LevelData
{
    [Header("ÿ�ؿ���ϸ��������")]
    public List<UsableCellData> cellsList;

    [Header("�ؿ�tips")]
    public List<string> tips;
}

[System.Serializable]
public class UsableCellData
{
    public int count;

    public CellName name;

    //public BaseCell cell;
}
