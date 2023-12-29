using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tips : MonoBehaviour
{
    private int index;
    private Button goOnButton;
    private List<string> tips = new List<string>();

    [SerializeField]private TextMeshProUGUI content;

    public void Initilize(List<string> tips)
    {
        if(tips.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }   
        if(!goOnButton) goOnButton = GetComponentInChildren<Button>();
        goOnButton.onClick.AddListener(OnClickGoOn);

        if (tips.Count == 1)
        {
            goOnButton.gameObject.SetActive(false);
        }
        content = GetComponentInChildren<TextMeshProUGUI>();
        this.tips = tips;
        index = 0;

        content.text = tips[index];
        gameObject.SetActive(true);
    }

    private void OnClickGoOn()
    {
        index += 1;

        if(index >= tips.Count)
        {
            index = 0;
        }
        content.text = tips[index];
    }
}
