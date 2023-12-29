using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassLevelPanel : BasePanel
{
    public Button againButton;

    public Button goBackSelectLevelButton;

    public Button nextLevelButton;

    protected override void Awake()
    {
        base.Awake();

        againButton.onClick.AddListener(OnClickAgainButton);

        goBackSelectLevelButton.onClick.AddListener(OnClickGoBackSelectLevelButton);

        nextLevelButton.onClick.AddListener(OnClickNextLevelButton);
    }

    public void OnClickAgainButton()
    {
        GameManager.Ins.LoadGameScene(GameManager.Ins.CurrentIndex);
    }
    public void OnClickGoBackSelectLevelButton()
    {
        UIManager.Instance.OpenPanel(PanelName.LevelSelectionPanel);
    }
    public void OnClickNextLevelButton()
    {
        GameManager.Ins.LoadGameScene(GameManager.Ins.CurrentIndex + 1);
    }
   
}
