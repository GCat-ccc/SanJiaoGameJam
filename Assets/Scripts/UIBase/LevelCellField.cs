//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class LevelCellField : MonoBehaviour
//{
//    private int count;

//    private UsableCellUI cellUI;

//    public TextMeshProUGUI countText;

//    private GameObject originCellUI;

//    private UsableCellData data;
//    public void Initialize(UsableCellData data, GameObject originCellUI)
//    {
//        countText = GetComponentInChildren<TextMeshProUGUI>();

//        if (data.count == 0)
//        {
//            countText.text = "";

//            countText.gameObject.SetActive(false);

//            Debug.LogWarning("Possible level data error");

//            return;
//        }
//        count = data.count;

//        this.originCellUI = originCellUI;

//        this.data = data;

//        cellUI = GameObject.Instantiate(this.originCellUI, transform).GetComponent<UsableCellUI>();

//        cellUI.Initialize(this.data.cell.gameObject, data.sprite);

//        cellUI.GetComponent<RectTransform>().localPosition = Vector3.zero;

//        countText.text = count.ToString();

//        countText.gameObject.SetActive(true);
//    }
//    public void RefreshCell()
//    {
//        count -= 1;

//        if(count == 0)
//        {
//            countText.text = "";

//            countText.gameObject.SetActive(false);

//            return;
//        }

//        countText.text = count.ToString();

//        cellUI = GameObject.Instantiate(this.originCellUI, transform).GetComponent<UsableCellUI>();

//        cellUI.Initialize(this.data.cell.gameObject, data.sprite);

//        cellUI.GetComponent<RectTransform>().localPosition = Vector3.zero;
//    }
//    public void Clear()
//    {
//        count = 0;

//        cellUI = null;

//        if(transform.GetComponentInChildren<UsableCellUI>() != null)
//        {
//            Destroy(transform.GetComponentInChildren(typeof(UsableCellUI)).gameObject);
//        }

//        if(countText != null)
//        {
//            countText.gameObject.SetActive(false);
//        }
//    }
//}