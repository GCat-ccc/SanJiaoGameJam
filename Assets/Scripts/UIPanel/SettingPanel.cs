using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Manager;

public class SettingPanel : BasePanel
{
    public Button resetLevelButton;
    public Button backToSelectionLevelButton;
    public Button backToMainMenuButton;
    public Button closeButton;

    protected override void Awake()
    {
        base.Awake();

        resetLevelButton.onClick.AddListener(OnClickResetLevelButton);
        backToSelectionLevelButton.onClick.AddListener(OnClickBackToSelectionLevelButton);
        backToMainMenuButton.onClick.AddListener (OnClickBackToMainMenuButton);
        closeButton.onClick.AddListener(OnClickCloseButton);
    }
    void OnClickResetLevelButton()
    {
        //Ë¢ÐÂ
        MapManager.Ins.ResetLevel(() => { });

        UIManager.Instance.CloseCurrentPanel();

        UIManager.Instance.ActivePanel.GetComponent<GamePanel>().ResetCurrentLevel();
    }
    void OnClickBackToSelectionLevelButton()
    {
        MapManager.Ins.DestroyLevel();
        UIManager.Instance.OpenPanel(PanelName.LevelSelectionPanel);
    }
    void OnClickBackToMainMenuButton()
    {
        GameManager.Ins.LoadStartScene();
    }
    void OnClickCloseButton()
    {
        UIManager.Instance.CloseCurrentPanel();
    }
}
