using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : BasePanel
{
    public Button startGameButton;

    public Button introductionButton;

    public Button quitGameButton;

    protected override void Awake()
    {
        base.Awake ();

        startGameButton.onClick.AddListener(OnClickStartGameButton);

        introductionButton.onClick.AddListener(OnClickIntroductionButton);

        quitGameButton.onClick.AddListener(OnClickQuitGameButton);
    }
    public override void OnEnable()
    {

    }

    public override void OnPause()
    {
        base.OnPause();
    }

    public override void OnResume()
    {
        base.OnResume();
    }
    public override void OnDisable()
    {
       
    }
    void OnClickStartGameButton()
    {
        //选择关卡
        UIManager.Instance.CloseCurrentPanel();
        UIManager.Instance.OpenPanel(PanelName.LevelSelectionPanel);
    }
    void OnClickIntroductionButton()
    {
        //打开介绍界面
        Debug.Log("Introduction");
        UIManager.Instance.OpenPanel(PanelName.DeveloperIntroductionPanel);
    }
    void OnClickQuitGameButton()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif

    }
}
