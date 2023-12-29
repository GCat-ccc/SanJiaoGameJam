using System;
using System.Collections;
using DG.Tweening;
using Events;
using GCFramework.Extension;
using GCFramework.MessageCenter;
using UnityEngine;

namespace Entity.Cells
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class Food : BaseCell
    {
        private CircleCollider2D _collider2D;
        private SpriteRenderer _spriteRenderer;

        public bool isArrived = false;

        protected override void Awake()
        {
            this.RequireComponent(ref _spriteRenderer);
            this.RequireComponent(ref _collider2D);
        }

        private void Start()
        {
            _collider2D.isTrigger = true;
        }

        protected override IEnumerator Execute()
        {
            yield return null;
        }

        /// <summary>
        /// 移动到目标点，其是一格一格的移动的
        /// </summary>
        /// <param name="targetPos"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public IEnumerator MoveTo(Vector2Int targetPos, Vector2Int dir)
        {
            Debug.Log("food targetpos "  +  targetPos);
            var worldPos = Map.CellToWorld((Vector3Int)targetPos) + new Vector3(0.5f, 0.5f, 0);
            while (Vector2.Distance(transform.position, worldPos) > 0.01f)
            {
                if(isArrived)
                    yield break;
                transform.DOMove(worldPos, 0.5f);
                yield return null;
            } 
            transform.position = worldPos;
        }

        public void Moving()
        {
            _collider2D.enabled = false;
        }

        public void Stopped()
        {
            _collider2D.enabled = true;
        }

        private void Arrive()
        {
            _spriteRenderer.color = new Color(1, 1, 1, 0.4f);
            _collider2D.enabled = false;
            isArrived = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out GameDestination gameDestination) && !gameDestination.IsHasFood)
            {
                Arrive();
                Debug.Log("Food Arrive");
                gameDestination.IsHasFood = true;
                MessageCenter.Fire<ArrivalOfFood>();
            } 
        }
    }
}