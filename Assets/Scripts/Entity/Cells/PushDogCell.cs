using System;
using System.Collections;
using DG.Tweening;
using GCFramework.Audio;
using GCFramework.Extension;
using Manager;
using Other;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


namespace Entity.Cells
{
    /// <summary>
    /// 推推狗细胞
    /// </summary>
    public class PushDogCell : BaseCell
    {
        [SerializeField] private Transform headRoot;
        [SerializeField] private Transform mouth;
        [SerializeField] private Transform nose;
        [SerializeField] private float expansionVelocity = 10f; // 伸缩速度

        SpriteRenderer[] renders;
        /// <summary>
        /// 可抵达的位置
        /// </summary>
        private Vector2Int _reachablePos;

        protected override void Awake()
        {
            base.Awake();
            this.RequireComponent(ref ASource);

            GameObject head = transform.Find("Head").gameObject;

            renders = GetComponentsInChildren<SpriteRenderer>();
        }
        
        protected override IEnumerator Execute()
        {
            Food food = null;
            GCAudioManager.Ins.PlayOnceSound(ASource, AudioNameExtend.LongNoseDog);
            yield return RotateHead();
            
            for (int i = 1; i <= 10; i++)
            {
                _reachablePos = MapPosition + Direction * i;
                var cell = MapManager.Ins.FindCellInSamePosition(_reachablePos);
                if ((Vector2Int)Map.WorldToCell(nose.position) != _reachablePos)
                    yield return MouthExtended();
                if(!MapManager.Ins.IsOnTheFloors(_reachablePos)) break;
                if(cell == null) continue;
                if(cell is not Food && cell is not CheemsCell) break;

                food = cell as Food;
                break;
            }

            if (!food)
            {
                yield return MouthShortened();
                yield break;
            }
            
            yield return PushTarget(food);

            //yield return RestoreMouth();
        }
        public override void Retreat()
        {
            base.Retreat();
            //renders = GetComponentsInChildren<SpriteRenderer>();
            //foreach (var sRenderer in renders)
            //{
            //    Debug.Log(sRenderer.gameObject.name);
            //    sRenderer.DOFade(0, _retreatDuration).OnComplete(() =>
            //    {
            //        Destroy(gameObject);
            //    });
            //}
          
        }

        /// <summary>
        /// 将头旋转到指定方向
        /// </summary>
        /// <returns></returns>
        private IEnumerator RotateHead()
        {
            float angle = Vector2.Angle(Direction, Vector2.down);
            Quaternion targetAngle = Quaternion.Euler(0, 0, Direction.x > 0 ? angle : -angle);
            headRoot.localScale = new Vector3(Direction.x > 0 ? 1 : -1, 1, 1);
            while (Quaternion.Angle(targetAngle, headRoot.rotation) > Mathf.Epsilon)
            {
                headRoot.rotation = Quaternion.Lerp(headRoot.rotation, targetAngle, 5f * Time.deltaTime);
                yield return null;
            }
        }

        /// <summary>
        /// 恢复嘴巴位置
        /// </summary>
        /// <returns></returns>
        private IEnumerator RestoreMouth()
        {
            Quaternion normalAngle = Quaternion.Euler(0, 0, 0);
            while (Quaternion.Angle(normalAngle, mouth.rotation) > Mathf.Epsilon)
            {
                mouth.rotation = Quaternion.Lerp(mouth.rotation, normalAngle, 5f * Time.deltaTime);
                yield return null;
            }
        }

        /// <summary>
        /// 嘴巴伸长
        /// </summary>
        private IEnumerator MouthExtended()
        {
            Vector2Int nosePos = (Vector2Int)Map.WorldToCell(nose.position);
            while (nosePos != _reachablePos && MapManager.Ins.IsOnTheFloors(nosePos)) //&& MapManager.Ins.IsOnTheFloors(nosePos)
            {
                nosePos = (Vector2Int)Map.WorldToCell(nose.position);
                float scaleY = mouth.localScale.y;
                scaleY += expansionVelocity * Time.deltaTime;
                mouth.localScale = new Vector3(1, scaleY, 1);
                yield return null;
            }
        }

        /// <summary>
        /// 嘴巴缩短
        /// </summary>
        /// <returns></returns>
        private IEnumerator MouthShortened()
        {
            while (mouth.localScale.y > 1)
            {
                float scaleY = mouth.localScale.y;
                scaleY -= expansionVelocity * Time.deltaTime;
                mouth.localScale = new Vector3(1, scaleY, 1);
                yield return null;
            }
            
            ASource.Stop();
        }

        /// <summary>
        /// 将目标往前推
        /// </summary>
        /// <param name="food"></param>
        /// <returns></returns>
        private IEnumerator PushTarget(Food food)
        {
            if (food.isArrived)
            {
                yield return MouthShortened();
                yield break;
            }
            
            food.Moving();
            Debug.Log($"{MapManager.Ins.IsCanPass(food.MapPosition + Direction)}, " +
                      $"{MapManager.Ins.FindCellInSamePosition(food.MapPosition + Direction)}");
            while (MapManager.Ins.IsCanPass(food.MapPosition + Direction)) // && MapManager.Ins.IsOnTheFloors((Vector2Int)Map.WorldToCell(nose.position))
            {
                _reachablePos = food.MapPosition;
                yield return MouthExtended();
                yield return food.MoveTo(food.MapPosition + Direction, Direction);
            }
            yield return MouthShortened();
            food.Stopped();
        }
    }
}