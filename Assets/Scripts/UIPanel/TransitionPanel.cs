using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionPanel : BasePanel
{
    public float transitionTime = .6f;

    public override void OnEnable()
    {
        base.OnEnable();


    }
    private IEnumerator TransitionCoroutine()
    {
        float t = 0;

        Color color = GetComponent<Image>().color;

        while (t < transitionTime)
        {
            t += Time.deltaTime;

            GetComponent<Image>().color = new Color(color.r, color.g, color.b, t / transitionTime);

            yield return null;
        }
       
    }
}
