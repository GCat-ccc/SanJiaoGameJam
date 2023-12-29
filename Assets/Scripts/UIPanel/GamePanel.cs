using Entity.Cells;
using Manager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    public LevelCellField levelCellField;

    public ActionSequenceField actionSequenceField;

    public TextMeshProUGUI levelNum;

    public Tips tips;

    public Button startButton;

    public Button settingButton;

    public Button resetButton;

    public Button undoButton;

    public GameObject serialNumber;

    public int maxActionCount = 8;

    protected override void Awake()
    {
        base.Awake();

        startButton.onClick.AddListener(OnClickStartButton);
        settingButton.onClick.AddListener(OnClickSettingButton);
        resetButton.onClick.AddListener(OnClickResetButton);
        undoButton.onClick.AddListener(OnClickUndoButton);
        //useableCellslist.Initialize(levelData);
        //RefreshLevelData(GameManager.Ins.inventory.GetLevelData(0));
    }
    private void Start()
    {
        levelNum.text = "Level " + (GameManager.Ins.CurrentIndex + 1).ToString();
    }
    public override void OnEnable()
    {
        base.OnEnable();

        RefreshLevelData(UIManager.Instance.CurrentLevelData);
    }
    public override void OnResume()
    {
        base.OnResume();

       // RefreshLevelData(UIManager.Instance.CurrentLevelData);
    }
    public void RefreshLevelData(LevelData levelData)
    {
        levelCellField.Clear();

        actionSequenceField.Clear();

        levelCellField.Initialize(levelData);

        if(levelData.tips.Count == 0)
        {
            tips.gameObject.SetActive(false);
        }
        else
        {
            tips.Initilize(levelData.tips);
        }
        
    }
    public void PlaceCell(CellUI cellUI)
    {
        levelCellField.PlaceCell(cellUI.Index);

        CellAction cellAction =  actionSequenceField.AddAction(cellUI.actionSprite, cellUI.newCellObj);

        if(actionSequenceField.count == maxActionCount)
        {
            levelCellField.CloseBlockRaycasts();
        }

        GameManager.Ins.AddPlacementOperation(new PlacementOperation(cellUI.newCellObj.gameObject, cellAction, cellUI.Index));

        if(cellUI.newCellObj is PolishCattleCell == false)
        {
            GameObject serialNum = Instantiate(serialNumber, cellUI.newCellObj.transform);

            Vector3 snLocalPos = serialNum.transform.localPosition;

            Vector3 cellLocalScale = cellUI.newCellObj.transform.localScale;

            serialNum.transform.localPosition = new Vector3(snLocalPos.x / cellLocalScale.x, snLocalPos.y / cellLocalScale.y, 0);

            serialNum.GetComponentInChildren<TextMesh>().text = actionSequenceField.count.ToString();

            serialNum.GetComponentInChildren<MeshRenderer>().sortingLayerName = serialNum.GetComponent<SpriteRenderer>().sortingLayerName;
            serialNum.GetComponentInChildren<MeshRenderer>().sortingOrder = serialNum.GetComponent<SpriteRenderer>().sortingOrder + 1;
        }


        Destroy(cellUI.gameObject);
    }
    private void OnClickStartButton()
    {
        levelCellField.CloseBlockRaycasts();

        levelCellField.gameObject.SetActive(false);

        tips.gameObject.SetActive(false);

        startButton.gameObject.SetActive(false);

        undoButton.gameObject.SetActive(false);

        StartCoroutine(actionSequenceField.Excute());
    }
    private void OnClickSettingButton()
    {
        UIManager.Instance.OpenPanel(PanelName.SettingPanel);
    }
    private void OnClickResetButton()
    {
        actionSequenceField.StopAllCoroutines();
        MapManager.Ins.StopAllCellCoroutine();
        GameManager.Ins.ClearPlacementOperation();
        StopAllCoroutines();
        ResetCurrentLevel();
    }
    public void RemoveAction(BaseCell cell)
    {
        foreach(var c in actionSequenceField.actions)
        {
            if(c.cell == cell)
            {
                actionSequenceField.RemoveAction(c);
                Debug.Log("Delete Action");
                return;
            }
        }
    }
    public void ResetCurrentLevel()
    {
        levelCellField.Clear();

        actionSequenceField.Clear();
      
        MapManager.Ins.ResetLevel();

        levelCellField.gameObject.SetActive(true);

        tips.Initilize(UIManager.Instance.CurrentLevelData.tips);

        startButton.gameObject.SetActive(true);

        undoButton.gameObject.SetActive(true);

        levelCellField.Initialize(UIManager.Instance.CurrentLevelData);
    }
    public void OnClickUndoButton()
    {
        PlacementOperation op = GameManager.Ins.GetAndUndoRecentPlacementOperation();
        if(op == null)
        {
            return; 
        }
        levelCellField.Undo(op.index);
        actionSequenceField.RemoveAction(op.action);
        Destroy(op.cell);
    }
}
