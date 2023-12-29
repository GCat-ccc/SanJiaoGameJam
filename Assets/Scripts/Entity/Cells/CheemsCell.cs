using System;
using System.Collections;
using DG.Tweening;
using GCFramework.Audio;
using GCFramework.Extension;
using Manager;
using Other;
using UnityEngine;

namespace Entity.Cells
{
    /// <summary>
    /// Cheems细胞
    /// </summary>
    public class CheemsCell : BaseCell
    {
        private Animator _animator;

        protected override void Awake()
        {
            base.Awake();
            this.RequireComponent(ref ASource);
            this.RequireComponent(ref _animator);
            this.RequireComponent(ref SRenderer);
        }

        private void OnEnable()
        {
            SRenderer.color = new Color(1, 1, 1, 0.4f);
        }

        protected override IEnumerator Execute()
        {
            SRenderer.color = new Color(1, 1, 1, 1);
            var chopPos = MapPosition + Direction;
            var targetCell = MapManager.Ins.FindCellInSamePosition(chopPos);
            GCAudioManager.Ins.PlayOnceSound(ASource, AudioNameExtend.Cheems, pitch: 1.2f, onPlayCompleted: Retreat);
            yield return SprintCut();
            if (targetCell is NecrosisCell necrosisCell) necrosisCell.Hurt();
            _animator.SetTrigger("chop");
            yield return new WaitForSeconds(1.85f);

            if (targetCell != null && targetCell is not Food)
            {
                Debug.Log($"Cheems斩杀了{targetCell.gameObject.name}");
                targetCell.Retreat();
            }
            
            yield return new WaitForSeconds(0.4f);
        }

        private IEnumerator SprintCut()
        {
            Vector2Int curCellPos = MapPosition; 
            Vector2 targetSprintPos = Map.GetCellCenterWorld((Vector3Int)(MapPosition + Direction * 2));
            while (MapPosition != (curCellPos + Direction * 2))
            {
                transform.DOMove(targetSprintPos, 0.2f);
                yield return null;
            }
        }
    }
}