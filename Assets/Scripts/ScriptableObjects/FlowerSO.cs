using QwertyGarden;
using UnityEngine;

[CreateAssetMenu(fileName = "FlowerSO", menuName = "Scriptable Objects/FlowerSO")]
public class FlowerSO : ScriptableObject
{
    public Flower FlowerPrefab;
    public Sprite FlowerCard;
    public Sprite FlowerIcon;
    public string FlowerName;
    public int SeedCost;
    public int SellValue;
    public float GrowTime;
    public Sprite[] FlowerFrames;
}
