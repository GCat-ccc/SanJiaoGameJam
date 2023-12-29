using System;
using System.Collections;
using System.Collections.Generic;
using Entity;
using Entity.Cells;
using GCFramework.Resource;
using GCFramework.单例;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Manager
{
    public class MapManager : SingletonMono<MapManager>
    {
#if UNITY_EDITOR
        [Header("测试参数，仅在测试有效！")]
        [SerializeField] private GameObject curSelectedCell;
        public int curCellIndex;
        [SerializeField] private string[] cellNames;
        public bool isDebug = true;
        [Space(20)]
#endif
        [SerializeField] private Transform foodRoot;
        [Header("关卡数量")] 
        [SerializeField] private int levelCount = 20;
        private readonly List<BaseCell> _cellsList = new();
        public Transform CellsRoot { get; private set; }
        /// <summary>
        /// 游戏终点访问接口
        /// </summary>
        public List<GameDestination> GameDestinations { get; private set; }
        private int _curLevelIndex;

        /// <summary>
        /// 当前关卡下标,1~N
        /// </summary>
        public int CurLevelIndex
        {
            get => _curLevelIndex;
            set
            {
                if (value > 0)
                    _curLevelIndex = value - 1;
            }
        }

        public Tilemap CurTilemap { get; private set; }
        public Level CurLevel { get; set; }
        
        public List<BaseCell> CellsList
        {
            get
            {
                if (!CellsRoot)
                    CellsRoot = new GameObject("CellRoot").transform;
                
                _cellsList.Clear();
                for (int i = 0; i < CellsRoot.childCount; i++)
                {
                    if (CellsRoot.GetChild(i).TryGetComponent(out BaseCell cell))
                    {
                        if(cell.gameObject.activeSelf && !cell.IsDead)
                            _cellsList.Add(cell);
                    }
                }
                
                if(_cellsList != null && CurLevel != null && CurLevel.CellsListOnLevel != null)
                    _cellsList.AddRange(CurLevel.CellsListOnLevel);

                return _cellsList;
            }
        }

        protected override void InitAwake()
        {
            base.InitAwake();
            GameDestinations = new List<GameDestination>();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (isDebug)
            {
                if(Input.GetAxis("Mouse ScrollWheel") != 0)
                    SwitchCell(Input.GetAxis("Mouse ScrollWheel"));

                if (Input.GetMouseButtonDown(0))
                    Place();
                
                if(Input.GetKeyDown(KeyCode.E))
                    SwitchNextLevel();
            }
#endif
        }

        #region DEBUG

#if UNITY_EDITOR
        private void SwitchCell(float scrollWheel)
        {
            if (scrollWheel > 0)
            {
                if (curCellIndex > 0)
                {
                    curCellIndex--;
                    string cellName = cellNames[curCellIndex];
                    curSelectedCell = GCResourceManager.Ins.LoadRes<GameObject>(cellName, AssetType.Prefab);
                }
            }
            else
            {
                if (curCellIndex < cellNames.Length - 1)
                {
                    curCellIndex++;
                    string cellName = cellNames[curCellIndex];
                    curSelectedCell = GCResourceManager.Ins.LoadRes<GameObject>(cellName, AssetType.Prefab);
                }
            }
        }
        
        private void Place()
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(!IsCanPlaced(mousePos)) return;
            
            var cell = Instantiate<GameObject>(curSelectedCell, CellsRoot).GetComponent<BaseCell>();
            cell.SetPosition(mousePos);
        }
#endif

        #endregion
        
        /// <summary>
        /// 生成游戏地图
        /// </summary>
        /// <param name="levelIndex">关卡编号，从 1 开始</param>
        public void CreateGameMap(int levelIndex = 1)
        {
            if(CurLevel)
            {
                Destroy(CurLevel.gameObject);
            }
            CurLevelIndex = levelIndex;
            var level = Instantiate(GCResourceManager.Ins.LoadRes<GameObject>("Level " + (_curLevelIndex + 1), AssetType.Prefab), gameObject.transform);
            CurTilemap = level.GetComponent<Tilemap>();
            CurLevel = level.GetComponent<Level>();
            if(GameDestinations != null)
                GameDestinations.Clear();
            GameDestinations = CurLevel.gameDestinations;
        }
        
        /// <summary>
        /// 是否在地板上
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsOnTheFloors(Vector2Int pos)
        {
            return CurTilemap.GetTile((Vector3Int)pos);
        }

        /// <summary>
        /// 找到位置相同的细胞
        /// </summary>
        /// <param name="pos">网格位置</param>
        /// <returns></returns>
        public BaseCell FindCellInSamePosition(Vector2Int pos)
        {
            foreach (var cell in CellsList)
            {
                if(cell is CheemsCell)
                {
                    continue;
                }
                if (cell.MapPosition == pos)
                    return cell;
            }

            return null;
        }

        /// <summary>
        /// 是否可以放置
        /// </summary>
        /// <param name="pos">世界位置</param>
        /// <returns></returns>
        public bool IsCanPlaced(Vector2 pos)
        {
            
            Vector2Int cellPosition = (Vector2Int)GetComponent<GridLayout>().WorldToCell(pos);
            BaseCell result = FindCellInSamePosition(cellPosition);
#if UNITY_EDITOR
            Debug.LogWarning($"放置的位置是世界坐标: {pos}, 放置的位置存在其他细胞？: {FindCellInSamePosition(cellPosition) != null}，放置的位置是地面？: {IsOnTheFloors(cellPosition)}");
#endif
            // 如何该位置没有其他细胞 或 Cheems在所在位置上，且在地板上，则可放置
            if ((result == null || result is CheemsCell) 
                && IsOnTheFloors(cellPosition) && !ForbiddenPlace(cellPosition))
                return true;
            return false;
        }

        /// <summary>
        /// 在禁止放置的位置上？
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private bool ForbiddenPlace(Vector2Int pos)
        {
            foreach (var gDest in GameDestinations)
            {
                if (pos == (Vector2Int)CurTilemap.WorldToCell(gDest.transform.position))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 是否可通过
        /// </summary>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        public bool IsCanPass(Vector2Int targetPos)
        {
            var cell = FindCellInSamePosition(targetPos);
            if ((cell == null || cell is CheemsCell ||
                 (cell is Food && cell.GetComponent<Food>().isArrived)) && IsOnTheFloors(targetPos))
                return true;
            return false;
        }

        /// <summary>
        /// 切换到下一个关卡
        /// </summary>
        /// <param name="onCompleted">加载场景完成后的操作</param>
        public void SwitchNextLevel(Action onCompleted = null)
        {
            if(!CurLevel) return;
            
            _curLevelIndex++;
            if (_curLevelIndex >= levelCount)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"超出关卡数量范围！");
#endif
                return;
            }
            Destroy(CurLevel.gameObject);
            foreach (var cell in CellsList)
                Destroy(cell.gameObject);

            LoadLevelAsync(onCompleted);
        }

        /// <summary>
        /// 重置关卡
        /// </summary>
        /// <param name="onCompleted">场景重新加载完成后</param>
        public void ResetLevel(Action onCompleted = null)
        {
            foreach (var cell in CellsList)
            {
                 Destroy(cell.gameObject);
            }
            Destroy(CurLevel.gameObject);


            LoadLevelAsync(onCompleted);
        }

        /// <summary>
        /// 卸载当前场景
        /// </summary>
        public void DestroyLevel()
        {
            Destroy(CurLevel.gameObject);
            foreach (var cell in CellsList)
            {
                Destroy(cell.gameObject);
            }
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="onCompleted"></param>
        /// <returns></returns>
        private void LoadLevelAsync(Action onCompleted = null)
        {
            var levelPrefab = GCResourceManager.Ins.LoadRes<GameObject>("Level " + (_curLevelIndex + 1), AssetType.Prefab);
            
            if (levelPrefab != null)
            {
                var level = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity, gameObject.transform);
                CurLevel = level.GetComponent<Level>();
                CurTilemap = level.GetComponent<Tilemap>();
                GameDestinations.Clear();
                GameDestinations = CurLevel.gameDestinations;
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning($"加载场景失败！当前为第{_curLevelIndex + 1}关");
#endif
            }
            onCompleted?.Invoke();
        }

        /// <summary>
        /// 停止所有细胞的行为
        /// </summary>
        public void StopAllCellCoroutine()
        {
            Debug.Log(CellsRoot.transform.childCount);
           for(int i = 0; i < CellsRoot.transform.childCount; ++i)
           {
                CellsRoot.GetChild(i).GetComponent<MonoBehaviour>().StopAllCoroutines();
                Debug.Log(CellsRoot.transform.GetChild(0).gameObject.name);
           }
            foodRoot = transform.GetChild(0).Find("Cell Root");
           for(int i = 0; i < foodRoot.childCount; ++i)
            {
                if(foodRoot.GetChild(i).TryGetComponent<Food>(out Food f))
                {
                    f.StopAllCoroutines();
                    Debug.Log(f.gameObject.name);
                }
            }
        }
    }
}