using Entity.Cells;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Arrow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [NonSerialized]public BaseCell cell;

    [NonSerialized] public CellUI cellUI;

    public Color originColor;

    public Color targetColor;

    Vector3Int direction;
    public void Initialize(BaseCell cell)
    {
        this.cell = cell;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(eventData.pointerEnter.name == "arrow")
        {
            eventData.pointerEnter.GetComponent<SpriteRenderer>().color = targetColor;
        }
        direction = new Vector3Int((int)transform.up.x, (int)transform.up.y, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerEnter.name == "arrow")
        {
            eventData.pointerEnter.GetComponent<SpriteRenderer>().color = originColor;

        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        cell.Direction = (Vector2Int)direction;

        Destroy(gameObject.transform.parent.gameObject);

        CameraManager.Instance.ResetRotation();

        UIManager.Instance.ActivePanel.GetComponent<BasePanel>().OnResume();

        if (UIManager.Instance.ActivePanel.TryGetComponent<GamePanel>(out GamePanel gamePanel))
        {
            gamePanel.PlaceCell(cellUI);
        }
        else
        {
            Debug.LogError("Currently active panel is not GamePanel");
        }
    }
}
