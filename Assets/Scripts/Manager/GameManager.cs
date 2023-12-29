using GCFramework.µ¥Àý;
using Manager;
using System;
using GCFramework.MessageCenter;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Entity;
using Events;
using GCFramework.Extension;
using GCFramework.Audio;
using Other;

public class GameManager : SingletonMono<GameManager>
{
    public LevelDataInventory LevelInventory;

    public CellUIData cheems;
    public CellUIData suckingCat;
    public CellUIData polishCattle;
    public CellUIData necrosis;
    public CellUIData pushDog;

    public Image transitionImage;

    public float transitionTime = 10f;

    public int levelNum = 20;

    public int CurrentIndex { get; set; }

    private Stack<PlacementOperation> placementOperations = new Stack<PlacementOperation>();

    private Dictionary<CellName, CellUIData> cellUIDataDic = new Dictionary<CellName,CellUIData>();

    private void Awake()
    {
        cellUIDataDic[CellName.cheemsÏ¸°û] = cheems;
        cellUIDataDic[CellName.ÎüÎüÃ¨Ï¸°û] = suckingCat;
        cellUIDataDic[CellName.²¨À¼Å£Ï¸°û] = polishCattle;
        cellUIDataDic[CellName.»µËÀÏ¸°û] = necrosis;
        cellUIDataDic[CellName.ÍÆÍÆ¹·Ï¸°û] = pushDog;

        MessageCenter.Subscribe<ArrivalOfFood>(OnArrivalOfFood).UnRegisterWhenGameObjectDestroyed(this);
        transitionImage.raycastTarget = false;
    }
    private void Start()
    {
        GCAudioManager.Ins.PlayBgm(AudioNameExtend.GameBgm);
    }
    private void OnArrivalOfFood(ArrivalOfFood food)
    {
       
        foreach(var g in MapManager.Ins.GameDestinations)
        {
          
            if (g.IsHasFood == false)
            {
                return;
            }
        }
        StartCoroutine(NextLevel());
    }

    IEnumerator  NextLevel()
    {
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.OpenPanel(PanelName.PassLevelPanel);
    }
    public CellUIData GetCell(CellName cellName)
    {
        return cellUIDataDic[cellName];
    }
    public void LoadGameScene(int index)
    {
        if(index == levelNum)
        {
            StartCoroutine(LoadStartSceneCoroutine("StartScene"));

            Debug.Log("GameOver");
            return;
        }
        StartCoroutine(LoadGameSceneCoroutine("GameScene", index));


    }
    public void LoadStartScene()
    {
        StartCoroutine(LoadStartSceneCoroutine("StartScene"));
    }

    public void AddPlacementOperation(PlacementOperation placementOperation)
    {
        placementOperations.Push(placementOperation);
    }
    public PlacementOperation GetAndUndoRecentPlacementOperation()
    {
        if(placementOperations.Count == 0)
        {
            return null;
        }
        return placementOperations.Pop();
    }
    public void ClearPlacementOperation()
    {
        placementOperations.Clear();
    }
    IEnumerator LoadGameSceneCoroutine(string name, int index)
    {
        //AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name);

        //asyncOperation.allowSceneActivation = false;

        transitionImage.raycastTarget = true;

        yield return Fade(1);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name);

        //asyncOperation.allowSceneActivation = false;

        //asyncOperation.allowSceneActivation = true;


        while (asyncOperation.isDone == false)
        {

            yield return null;
        }
        this.CurrentIndex = index;
        MapManager.Ins.CreateGameMap(index + 1);
        UIManager.Instance.ResetScene();
        UIManager.Instance.CurrentLevelData = LevelInventory.levelDatas[index];
        UIManager.Instance.OpenPanel(PanelName.GamePanel);
       
        yield return Fade(0);
        transitionImage.raycastTarget = false;
    }
    IEnumerator LoadStartSceneCoroutine(string name)
    {
      

        transitionImage.raycastTarget = true;

        yield return Fade(1);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name);

        //asyncOperation.allowSceneActivation = false;

        //asyncOperation.allowSceneActivation = true;

        
        while(asyncOperation.isDone == false)
        {

            yield return null;
        }
        UIManager.Instance.ResetScene();
        UIManager.Instance.OpenPanel(PanelName.StartPanel);
        //yield return new WaitForSeconds(3f);
        yield return Fade(0);
        transitionImage.raycastTarget = false;
    }
    private IEnumerator Fade(float target)
    {
        float t = 0;

        Color color = transitionImage.color;

        float a = color.a;

        float originA = a;

        while (t < transitionTime)
        {
            t += Time.deltaTime;

           a = Mathf.Lerp(originA, target, t / transitionTime);

            transitionImage.color = new Color(color.r, color.g, color.b, a);

            yield return null;
        }
    }
}
