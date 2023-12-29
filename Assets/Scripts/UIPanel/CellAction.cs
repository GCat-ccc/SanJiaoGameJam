using Entity.Cells;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellAction : MonoBehaviour
{
    public BaseCell cell;
    public void Initialize(Sprite sprite, BaseCell cell)
    {
        GetComponent<Image>().sprite = sprite;
        GetComponent<Image>().SetNativeSize();

        this.cell = cell;
    }
    private void OnDestroy()
    {
    }
    public IEnumerator Execute()
    {
        yield return cell.OnExecute();
    }
}
