using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LevelButton : MonoBehaviour
{
    private Button button;

    private int levelIndex;

    private void Awake()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(OnClick);

        levelIndex = int.Parse(transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
    }

    private void OnClick()
    {
        Debug.Log(levelIndex);
    }
}
