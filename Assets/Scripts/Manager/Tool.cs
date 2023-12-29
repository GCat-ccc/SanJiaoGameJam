using UnityEngine;

public enum CellName
{
    ����ţϸ��,
    cheemsϸ��,
    ����èϸ��,
    ���ƹ�ϸ��,
    ����ϸ��
}
[System.Serializable]
public class CellUIData
{
    public GameObject cell;

    public Sprite dragSprite;

    public Sprite actionSprite;
}
public class PlacementOperation
{
    public GameObject cell;

    public CellAction action;

    public int index;

    public PlacementOperation(GameObject cell, CellAction action, int index)
    {
        this.cell = cell;
        this.action = action;
        this.index = index;
    }

    public void Undo()
    {
        GameObject.Destroy(cell);

        GameObject.Destroy(action);
    }
}