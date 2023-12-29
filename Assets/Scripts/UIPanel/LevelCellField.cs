using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCellField : MonoBehaviour
{
    /// <summary>
    /// 放置可选细胞的UI方格
    /// </summary>
    private List<GameObject> cellGridList = new List<GameObject>(); 

    private List<CellName> cellNameList = new List<CellName>();

    private List<TextMeshProUGUI> cellCountUIList = new List<TextMeshProUGUI>();

    public GameObject originCellUI;

    public CanvasGroup canvasGroup;

    public void Initialize(LevelData data)
    {
        for(int i = 0; i < transform.childCount; i++)
        { 
            if(i >= 4)
            {
                Debug.LogError("LevelCellField 有多余的UI框");
            }    
            cellGridList.Add(transform.GetChild(i).gameObject);

            cellCountUIList.Add(transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>());
        }
        
        for(int i = 0; i < data.cellsList.Count; i++)
        {
            CreateCellUI(i, data.cellsList[i].name);

            cellNameList.Add(data.cellsList[i].name);

            cellCountUIList[i].gameObject.SetActive(true);

            cellCountUIList[i].text = data.cellsList[i].count.ToString();

        }
        for(int i = data.cellsList.Count; i < transform.childCount; ++i)
        {
            cellCountUIList[i].gameObject.SetActive(false);
        }
        canvasGroup.blocksRaycasts = true;
    }
    public void CloseBlockRaycasts()
    {
        canvasGroup.blocksRaycasts = false;
    }
    public void OpenBlockRaycasts()
    {
        canvasGroup.blocksRaycasts = true;
    }
    public void PlaceCell(int index)
    {
        int count = int.Parse(cellCountUIList[index].text);

        count -= 1;

        if (count == 0)
        {
            cellCountUIList[index].text = "";

            return;
        }

        CreateCellUI(index, cellNameList[index]);

        cellCountUIList[index].text = count.ToString();

    }
    public void Clear()
    {
        for(int i = 0; i < cellGridList.Count; ++i)
        {
            if (cellGridList[i].GetComponentInChildren<CellUI>())
            {
                Destroy(cellGridList[i].GetComponentInChildren<CellUI>().gameObject);

                cellCountUIList[i].text = "";

            }
        }
    }
    public void Undo(int index)
    {
        if (cellCountUIList[index].text == "")
        {
            cellCountUIList[index].text = "1";
            CreateCellUI(index, cellNameList[index]);
        }
        else
        {
            cellCountUIList[index].text = (int.Parse(cellCountUIList[index].text) + 1).ToString();
        }
    }
    private void CreateCellUI(int index, CellName name)
    {
        GameObject g = Instantiate(originCellUI, cellGridList[index].transform);

        g.GetComponent<CellUI>().Initialize(GameManager.Ins.GetCell(name), index);
    }
    
}
