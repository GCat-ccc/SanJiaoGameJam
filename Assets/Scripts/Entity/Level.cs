using System;
using System.Collections.Generic;
using Entity.Cells;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entity
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private Transform cellRoot;

        private readonly List<BaseCell> _cellsList = new();

        public List<GameDestination> gameDestinations = new List<GameDestination>();

        public List<BaseCell> CellsListOnLevel
        {
            get
            {
                if (!cellRoot)
                {
                    cellRoot = transform.GetChild(0);
                    return null;
                }
                _cellsList.Clear();
                for (int i = 0; i < cellRoot.childCount; i++)
                {
                    if (cellRoot.GetChild(i).TryGetComponent<BaseCell>(out var baseCell))
                    {
                        if(baseCell.gameObject.activeSelf && !baseCell.IsDead)
                            _cellsList.Add(baseCell);
                    }

                    if (cellRoot.GetChild(i).TryGetComponent<GameDestination>(out var gDest))
                    {
                        gameDestinations.Add(gDest);
                    }
                }

                return _cellsList;
            }
        }

        private void Awake()
        {
            cellRoot = transform.GetChild(0);
            Debug.Log($"关卡生成");
            if (!cellRoot)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Cell Root为空，你并未给关卡放置场上存在的细胞！");
#endif
            }
        }
    }
}