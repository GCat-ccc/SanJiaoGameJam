using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public Vector3 endRotationValue = new Vector3(10, 0, 0);

    public float rotationTime = .6f;
    private void Awake()
    {
        Instance = this;

       // PlaceCell();
    }
    public void PlaceCell()
    {
        transform.DORotate(endRotationValue, rotationTime, RotateMode.Fast);
    }
    public void ResetRotation()
    {
        transform.DORotate(Vector3.zero, rotationTime, RotateMode.Fast);
    }
}
