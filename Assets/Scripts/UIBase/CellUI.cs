using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;
using Entity.Cells;
using Manager;
using UIPanel;

public class CellUI : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public int Index { get; private set; }

    public Sprite actionSprite;

    Sprite dragUISprite;

     
    RectTransform canvasRTf;

    public GameObject CellResource { get; private set; }

    [SerializeField]public BaseCell newCellObj;

    public float returnTime = .3f;
    public RectTransform ownRTf { get; private set; }
    public void Initialize(CellUIData cellUIData, int index)
    {
        ownRTf = GetComponent<RectTransform>();

        ownRTf.localPosition = Vector3.zero;

        dragUISprite = cellUIData.dragSprite;

        actionSprite = cellUIData.actionSprite;

        GetComponent<Image>().sprite = dragUISprite;

        canvasRTf = GameObject.Find("Canvas").GetComponent<RectTransform>();

        canvasRTf = GameObject.Find("Canvas").GetComponent<RectTransform>();

        this.CellResource = cellUIData.cell;

        this.Index = index;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<Canvas>().overrideSorting = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 pos;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRTf, eventData.position, eventData.pressEventCamera, out pos);

        ownRTf.position = pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (MapManager.Ins.IsCanPlaced(mousePosition))
        {
            var newCell = Instantiate(CellResource, MapManager.Ins.CellsRoot).GetComponent<BaseCell>();

            newCell.SetPosition(mousePosition);

            this.newCellObj = newCell;

            // GameObject g = Instantiate(arrowAndCancel);
            //
            // g.transform.position = newCell.transform.position;
            //
            // g.GetComponentInChildren<Arrow>().cell = g.GetComponentInChildren<CancelPlacement>().cell = newCell;
            //
            // g.GetComponentInChildren<Arrow>().cellUI = g.GetComponentInChildren<CancelPlacement>().cellUI = this;
            // UIManager.Instance.ActivePanel.GetComponent<BasePanel>().OnPause();
            //
            // CameraManager.Instance.PlaceCell();

            //if(UIManager.Instance.ActivePanel.TryGetComponent<GamePanel>(out GamePanel gamePanel))
            //{
            //    gamePanel.PlaceCell(this);
            //}
            //else
            //{
            //    Debug.LogError("Currently active panel is not GamePanel");
            //}
            
            gameObject.SetActive(false);
            if (newCell is CheemsCell || newCell is PushDogCell || newCell is SuckingCatCell)
            {
                UIManager.Instance.OpenPanel(PanelName.SelectDirPanel);
                UIManager.Instance.ActivePanel.GetComponent<SelectDirPanel>().CurSelectCell = newCell;
                UIManager.Instance.ActivePanel.GetComponent<SelectDirPanel>().cellUI = this;
                UIManager.Instance.ActivePanel.GetComponent<SelectDirPanel>().OnResume();
                //CameraManager.Instance.PlaceCell();
            }
            else
            {
                UIManager.Instance.ActivePanel.GetComponent<GamePanel>().PlaceCell(this);
            }
            
        }
        else
        {
            ResetPosition();
        }
    }
    public void ResetPosition()
    {
        ownRTf.DOLocalMove(Vector3.zero, returnTime);

        GetComponent<Canvas>().overrideSorting = false;

        Debug.Log(GetComponent<Canvas>().sortingLayerID);
    }

  
}