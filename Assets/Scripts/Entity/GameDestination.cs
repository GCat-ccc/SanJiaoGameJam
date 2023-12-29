using System;
using Entity.Cells;
using Events;
using GCFramework.Audio;
using GCFramework.Extension;
using GCFramework.MessageCenter;
using Other;
using UnityEngine;

namespace Entity
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class GameDestination : MonoBehaviour
    {
        private BoxCollider2D _collider2D;
        [SerializeField] private Vector2 detectionRange = new Vector2(0.5f, 0.5f);
        /// <summary>
        /// 是否拥有了食物
        /// </summary>
        public bool IsHasFood { get; set; }

        private void Awake()
        {
            this.RequireComponent(ref _collider2D);
        }

        private void Start()
        {
            _collider2D.size = detectionRange;
            _collider2D.isTrigger = false;
        }
    }
}