using System.Collections;
using System.Threading;
using DG.Tweening;
using GCFramework.Extension;
using UnityEngine;

namespace Entity.Cells
{
    /// <summary>
    /// 坏死细胞
    /// </summary>
    public class NecrosisCell : BaseCell
    {
        private Material _hurtMaterial;
        [SerializeField] private float hurtDuration = 0.02f;

        protected override void Awake()
        {
            base.Awake();
            this.RequireComponent(ref SRenderer);
            _hurtMaterial = SRenderer.material;
        }

        protected override IEnumerator Execute()
        {
            yield return null;
        }

        public void Hurt()
        {
            transform.DOShakePosition(hurtDuration, 0.09f);
            _hurtMaterial.SetFloat("_FlashAmount", 1);
        }
    }
}