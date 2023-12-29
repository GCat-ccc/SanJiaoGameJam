using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroductionPanel : BasePanel
{
    // Start is called before the first frame update
    public Button goBackButton;

    protected override void Awake()
    {
        base.Awake();
        goBackButton.onClick.AddListener(OnClickGoBackButton);
    }
    public override void OnEnable()
    {
        
    }
    public override void OnDisable()
    {

    }

    void OnClickGoBackButton()
    {
        UIManager.Instance.CloseCurrentPanel();
    }
}
