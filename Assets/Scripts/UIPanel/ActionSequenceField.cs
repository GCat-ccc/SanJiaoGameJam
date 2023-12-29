using Entity.Cells;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionSequenceField : MonoBehaviour
{
    public List<CellAction> actions = new List<CellAction>();

    /// <summary>
    /// 未初始化的行为UI预制体
    /// </summary>
    public GameObject originPre;

    public int count = 0;

    public CellAction AddAction(Sprite sprite, BaseCell cell)
    {
        if(cell is PolishCattleCell)
        {
            return null;
        }
        CellAction g = Instantiate(originPre, transform).GetComponent<CellAction>();

        g.Initialize(sprite, cell);

        actions.Add(g);

        count += 1;

        return g;
    }

    public void RemoveAction(CellAction action)
    {
        if(action == null)
        {
            return;
        }
        actions.Remove(action);
        Destroy(action.gameObject);
        count -= 1;
    }
    public void Clear()
    {
        while(actions.Count > 0)
        {
            Destroy(actions[0].gameObject);
            actions.RemoveAt(0);
            count -= 1;
        }
        //foreach(CellAction action in actions)
        //{
        //    actions.Remove(action);
        //    Destroy(action);
        //}
        count = 0;
    }
    public IEnumerator Excute()
    {
        while(actions.Count > 0)
        {
            yield return actions[0].Execute();
            Destroy(actions[0].gameObject);
            actions.RemoveAt(0);
            count--;
        }
      
    }
}