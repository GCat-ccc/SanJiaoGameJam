using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace 测试
{
    public class CTest : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private Transform go;

        private void Start()
        {
            go.position = _tilemap.CellToWorld(new Vector3Int(-2, 0, 0));
            Debug.Log(_tilemap.WorldToCell(go.position));
        }
    }
}