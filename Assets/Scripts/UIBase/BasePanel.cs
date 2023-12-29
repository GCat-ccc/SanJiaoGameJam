using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class BasePanel : MonoBehaviour
{
    private CanvasGroup _csGroup;

    protected virtual void Awake()
    {
         gameObject.AddComponent<CanvasGroup>();

        _csGroup = gameObject.GetComponent<CanvasGroup>();
    }
    public virtual void OnEnable()
    {

    }
    public virtual void OnPause()
    {
        if(_csGroup == null)
        {
            _csGroup = gameObject.GetComponent<CanvasGroup>();

        }
        if(GetComponent<CanvasGroup>() == null)
        {
            Debug.Log("_csGroup null ");
        }
        _csGroup.blocksRaycasts = false;
    }
    public virtual void OnResume()
    {
        _csGroup.blocksRaycasts = true;
    }
    public virtual void OnDisable()
    {

    }
}
