
using System;
using DG.Tweening;
using Entity.Cells;
using GCFramework.Resource;
using Manager;
using UnityEngine;

namespace UIPanel
{
    public class SelectDirPanel : BasePanel
    {
        [SerializeField] private RectTransform selectBox;
        [SerializeField] private RectTransform handle;
        [SerializeField] private GameObject cancellation;
        [SerializeField] private GameObject tips;
        [SerializeField] private float cancellationRange = 50f;

        private readonly GameObject[] _selectionDirBox = new GameObject[2];
        public BaseCell CurSelectCell { get; set; }
        private Camera MainCamera => Camera.main;
        private Vector2 MousePos => Input.mousePosition;

        private Vector2 _centerPos;

        private Vector2 _offset;

        private bool _isCancel;

        public CellUI cellUI;

        #region 计算

        private float _th; // 菱形高
        private float _tw; // 菱形宽

        #endregion
        
        protected override void Awake()
        {
            base.Awake();
            _offset = handle.position - selectBox.position;
            var selectionPrefab = GCResourceManager.Ins.LoadRes<GameObject>("Selection", AssetType.Prefab);
            if (selectionPrefab)
            {
                for (int i = 0; i < _selectionDirBox.Length; i++)
                {
                    _selectionDirBox[i] = Instantiate(selectionPrefab);
                    _selectionDirBox[i].SetActive(false);
                }
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning($"方向提示框未加载成功！");
#endif
            }
        }

        public override void OnResume()
        {
            base.OnResume();
            _tw = selectBox.sizeDelta.x;
            _th = selectBox.sizeDelta.y;
            _centerPos = MainCamera.WorldToScreenPoint(CurSelectCell.transform.position);
            selectBox.position = _centerPos + _offset;
            
            handle.position = _centerPos;
        }

        private void Update()
        {
            SelectDirection();
            SetCellDirection();
        }

        private void SelectDirection()
        {
            if (!_isCancel)
            {
                Vector2 dir = MousePos - _centerPos;
                
                CurSelectCell.Direction = Mathf.Abs(dir.x) > Mathf.Abs(dir.y) 
                    ? new Vector2Int((int)Mathf.Sign(dir.x), 0) 
                    : new Vector2Int(0, (int)Mathf.Sign(dir.y));

                // 显示提示方块
                _selectionDirBox[0].transform.position = MapManager.Ins.CurTilemap.GetCellCenterWorld(
                    (Vector3Int)(CurSelectCell.MapPosition + CurSelectCell.Direction));
                _selectionDirBox[1].transform.position = MapManager.Ins.CurTilemap.GetCellCenterWorld(
                    (Vector3Int)CurSelectCell.MapPosition);

                handle.DOMove(_centerPos + (Vector2)CurSelectCell.Direction * 100f, 0.2f)
                    .SetEase(Ease.Flash);
            }
            else
            {
                handle.DOMove(_centerPos, 0.2f).SetEase(Ease.Flash);
            }
        }

        private void SetCellDirection()
        {
            if (CancelableWithinRange() || !Input.GetMouseButtonDown(0)) return;
            
            UIManager.Instance.CloseCurrentPanel();

            UIManager.Instance.ActivePanel.GetComponent<GamePanel>().PlaceCell(cellUI);
        }

        private bool CancelableWithinRange()
        {
            // 不在取消范围内
            if (Vector2.Distance(_centerPos, MousePos) >= cancellationRange)
            {
                tips.SetActive(true);
                cancellation.SetActive(false);
                handle.gameObject.SetActive(true);
                foreach (var box     in _selectionDirBox)
                    box.SetActive(true);

                _isCancel = false;
                return false;
            }

            _isCancel = true;
            foreach (var box     in _selectionDirBox)
                box.SetActive(false);
            handle.gameObject.SetActive(false);
            cancellation.SetActive(true);
            tips.SetActive(false);
            if (Input.GetMouseButtonDown(0))
            {
                UIManager.Instance.CloseCurrentPanel();

                cellUI.gameObject.SetActive(true);

                cellUI.GetComponent<CellUI>().ResetPosition();

                Destroy(CurSelectCell.gameObject);
            }
            return true;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            foreach (var box     in _selectionDirBox)
                box.SetActive(false);
        }

        private void OnDestroy()
        {
            foreach (var box in _selectionDirBox)
                Destroy(box);
        }
    }
}