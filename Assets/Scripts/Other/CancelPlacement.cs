using Entity.Cells;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CancelPlacement : MonoBehaviour, IPointerClickHandler
{
    [NonSerialized]public BaseCell cell;

    [NonSerialized] public CellUI cellUI;
    public void OnPointerClick(PointerEventData eventData)
    {
        Destroy(gameObject.transform.parent.gameObject);

        Destroy(cell.gameObject);

        CameraManager.Instance.ResetRotation();

        UIManager.Instance.ActivePanel.GetComponent<BasePanel>().OnResume();

        cellUI.gameObject.SetActive(true);

        cellUI.ResetPosition();

        //其他
    }
   
}
