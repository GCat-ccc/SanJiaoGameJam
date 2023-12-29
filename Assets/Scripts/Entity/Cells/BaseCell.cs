using System;
using System.Collections;
using DG.Tweening;
using Manager;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Tilemaps;
namespace Entity.Cells
{

    public abstract class BaseCell : MonoBehaviour
    {
        #region Component

        protected AudioSource ASource;

        protected SpriteRenderer SRenderer;

        private SpriteRenderer[] childSRenders;
        #endregion

        /// <summary>
        /// 朝向
        /// </summary>
        public Vector2Int Direction { get; set; }
        
        /// <summary>
        /// 顺序
        /// </summary>
        public int ActionSequence { get; set; }
        
        public bool IsDead { get; private set; }

        private Transform _arrowRoot;

        protected Tilemap Map
        {
            get
            {
                if (MapManager.Ins)
                {
                    return MapManager.Ins.CurTilemap;
                }

                return null;
            }
        }
        /// <summary>
        /// 细胞在瓦片地图上的位置
        /// </summary>
        public Vector2Int MapPosition => (Vector2Int)Map.WorldToCell(transform.position);

        protected virtual void Awake()
        {
            if(transform.childCount > 0)
                _arrowRoot = transform.GetChild(0).transform;
            Direction = Vector2Int.right;

            childSRenders = GetComponentsInChildren<SpriteRenderer>();
        }

        protected virtual void Update()
        {
            ControlArrowDirection();
        }

        private void ControlArrowDirection()
        {
            if(IsDead || !_arrowRoot) return;

            float angle = Vector2.Angle(Direction, Vector2.right);
            if (Direction.y < 0) angle = -angle;
            _arrowRoot.rotation = Quaternion.Euler(0, 0, angle);
        }

        /// <summary>
        /// 执行接口
        /// </summary>
        /// <returns></returns>
        public IEnumerator OnExecute()
        {
            yield return Execute();
#if UNITY_EDITOR
            if (gameObject != null) Debug.Log($"{gameObject.name}，执行完毕");
#endif
        }
        
        protected abstract IEnumerator Execute();

        /// <summary>
        /// 直接修改Cell的位置
        /// </summary>
        /// <param name="newPos"></param>
        public void SetPosition(Vector3 newPos)
        {
            // 将鼠标所在的坐标转为网格坐标
            var cellPosition = Map.WorldToCell(newPos);
            transform.position = Map.GetCellCenterWorld(cellPosition);
        }

        protected float _retreatDuration = 0.5f;
        /// <summary>
        /// 执行完后撤退
        /// </summary>
        public virtual void Retreat()
        {
            IsDead = true;
            
            UIManager.Instance.ActivePanel.GetComponent<GamePanel>().RemoveAction(this);

            if (SRenderer)
            {
                SRenderer.DOFade(0, _retreatDuration).OnComplete(() =>
                {
                    Destroy(gameObject);
                });
            }
            childSRenders = GetComponentsInChildren<SpriteRenderer>();
            if (childSRenders != null && childSRenders.Length != 0)
            {
                foreach (var sRenderer in childSRenders)
                {
                    Debug.Log(sRenderer.gameObject.name);
                    sRenderer.DOFade(0, _retreatDuration).OnComplete(() =>
                    {
                        Destroy(gameObject);
                    });
                }
            }
            
          
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
            transform.DOKill();

            if(childSRenders == null || childSRenders.Length == 0) { return; }
            foreach(var sRenderer in childSRenders)
            {
                if (sRenderer == null) continue;
                sRenderer.DOKill();
            }
        }
    }
}