using System;
using System.Collections;
using GCFramework.Audio;
using GCFramework.Extension;
using Manager;
using Other;
using UnityEngine;

namespace Entity.Cells
{
    /// <summary>
    /// 吸吸猫细胞
    /// </summary>
    public class SuckingCatCell : BaseCell
    {
        private Animator _animator;
        
        protected override void Awake()
        {
            base.Awake();
            this.RequireComponent(ref ASource);
            this.RequireComponent(ref _animator);
            this.RequireComponent(ref SRenderer);
        }

        protected override IEnumerator Execute()
        {
            Food food = null;
            GCAudioManager.Ins.PlayOnceSound(ASource, AudioNameExtend.SuckingCat);
            
            for (int i = 1; i <= 10; i++)
            {
                _animator.SetBool("isOpen", true);
                var cell = MapManager.Ins.FindCellInSamePosition(MapPosition + Direction * i);
                if(!MapManager.Ins.IsOnTheFloors(MapPosition + Direction * i)) break;
                if(cell == null) continue;
                if(cell is not Food) break;
                
                food = cell as Food;
                if (food.isArrived)
                {
                    Debug.Log("查找下一个食物");
                    continue;
                }
                else
                {
                    Debug.Log(food.gameObject.name);
                }
                break;
            }
  
            if (!food)
            {
                _animator.SetBool("isOpen", false);
                yield break;
            }
            yield return SuckingFoodInFront(food);
        }

        /// <summary>
        /// 将食物吸到面前
        /// </summary>
        /// <param name="food"></param>
        /// <returns></returns>
        private IEnumerator SuckingFoodInFront(Food food)
        {
            if(food.isArrived) yield break;
            
            food.Moving();
            while (food.MapPosition != (MapPosition + Direction))
            {
                if(!MapManager.Ins.IsCanPass(food.MapPosition + -Direction))
                    break;
                
                yield return food.MoveTo(food.MapPosition + -Direction, -Direction);
            }
            _animator.SetBool("isOpen", false);
            food.Stopped();
        }
    }
}