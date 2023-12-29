using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/UI/PanelDataInventory", fileName = "PanelDataInventory")]
public class PanelDataInventory : ScriptableObject
{
    public List<PanelData> datas;
}