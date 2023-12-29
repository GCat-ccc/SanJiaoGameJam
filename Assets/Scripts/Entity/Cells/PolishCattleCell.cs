using System;
using System.Collections;
using GCFramework.Audio;
using GCFramework.Extension;
using Other;
using UnityEngine;

namespace Entity.Cells
{
    /// <summary>
    /// 波兰牛细胞
    /// </summary>
    public class PolishCattleCell : BaseCell
    {

        protected override void Awake()
        {
            base.Awake();

            this.RequireComponent(ref ASource);
        }

        private void OnEnable()
        {
            GCAudioManager.Ins.PlayBgm(ASource, AudioNameExtend.PolishCattle, 0.05f);
        }

        private void OnDisable()
        {
            ASource.Stop();
        }

        protected override IEnumerator Execute()
        {
            yield break;
        }
    }
}