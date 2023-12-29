using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour 
{ 
    public static UIManager Instance {  get; private set; }

    public PanelDataInventory inventory;
    public LevelData CurrentLevelData {  get; set; }

    Transform canvas;

    /// <summary>
    /// 当前激活的Panel
    /// </summary>
    Stack<BasePanel> _panelStack = new Stack<BasePanel>();
    Dictionary<PanelName, GameObject> _activePanelDic = new Dictionary<PanelName, GameObject>();

    /// <summary>
    /// 存在场景中但并未激活的Panel
    /// </summary>
    Dictionary<PanelName, BasePanel> _panelInSceneDic = new Dictionary<PanelName, BasePanel>();

    /// <summary>
    /// 存储所有的Panel
    /// </summary>
    Dictionary<PanelName, BasePanel> _allPanelsDic = new Dictionary<PanelName, BasePanel>();


    public GameObject ActivePanel {  get; private set; }
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }


        foreach (PanelData data in inventory.datas)
        {
           _allPanelsDic.Add(data.name, Resources.Load<BasePanel>(data.path));
        }
        OpenPanel(PanelName.StartPanel);
        DontDestroyOnLoad(gameObject);
    }
    public void OpenPanel(PanelName panelName)
    {
        if(_panelInSceneDic.TryGetValue(panelName, out BasePanel newPanel) && newPanel.gameObject.activeInHierarchy)
        {
            Debug.LogWarning(panelName + " already exists in the scenario");

            return;
        }

        if(_panelStack.Count > 0)
        {
            _panelStack.Peek().OnPause();
        }

        if(_panelInSceneDic.TryGetValue(panelName,  out newPanel))
        {
            newPanel.gameObject.SetActive(true);
        }
        else
        {
            newPanel = _allPanelsDic[panelName];

            if(canvas == null)
            {
                canvas = GameObject.Find("Canvas").transform;
            }

            newPanel = GameObject.Instantiate(newPanel.gameObject, canvas).GetComponent<BasePanel>() ;

            _panelInSceneDic[panelName] = newPanel;
        }

        _panelStack.Push(newPanel);
        _activePanelDic[panelName] = newPanel.gameObject;
        newPanel.gameObject.transform.SetAsLastSibling();
        ActivePanel = newPanel.gameObject;
    }
    public void CloseCurrentPanel()
    {
        if(_panelStack.Count == 0)
        {
            Debug.LogWarning("There is no UI to close");

            return;
        }

        _panelStack.Pop().gameObject.SetActive(false);

        if(_panelStack.Count > 0)
        {
            var panel = _panelStack.Peek();

            panel.OnResume();
            panel.gameObject.transform.SetAsLastSibling();
            ActivePanel = panel.gameObject;
        }
    }
    /// <summary>
    /// 切换关卡前调用
    /// </summary>
    public void ResetScene()
    {
        _panelStack.Clear();

        _panelInSceneDic.Clear();
    }
}
public enum PanelName
{
    StartPanel,
    GamePanel,
    SettingPanel,
    DeveloperIntroductionPanel,
    LevelSelectionPanel,
    SelectDirPanel,
    PassLevelPanel
}