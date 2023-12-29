using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class LevelPanel : BasePanel
{
    public float transitionTime = .5f;

    public Button leftButton;
    public Button rightButton;
    public Button closeButton;

    public RectTransform levelRTf;

    public List<Button> levelSelectionButtons = new List<Button>();
    protected override void Awake()
    {
        base.Awake();

        leftButton.onClick.AddListener(OnClickLeftButton);
        rightButton.onClick.AddListener(OnClickRightButton);
        closeButton.onClick.AddListener(OnClickCloseButton);

        foreach (var button in levelSelectionButtons)
        {
            button.onClick.AddListener(() =>
            {
                JumpLevel(int.Parse(button.GetComponentInChildren<TextMeshProUGUI>().text));
            });
        }
    }

    private void OnClickLeftButton()
    {
        levelRTf.DOLocalMove(new Vector2(0, 0), transitionTime);
    }
    private void OnClickRightButton()
    {
        levelRTf.DOLocalMove(new Vector2(-1430, 0), transitionTime);

    }
    private void OnClickCloseButton()
    {
        UIManager.Instance.CloseCurrentPanel();
        UIManager.Instance.OpenPanel(PanelName.StartPanel);
    }
    private void JumpLevel(int index)
    {
        GameManager.Ins.LoadGameScene(index - 1);
        //Load Level
    }
}
