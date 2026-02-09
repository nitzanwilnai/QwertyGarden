using QwertyGarden;
using UnityEngine;

[CreateAssetMenu(fileName = "FlowerSO", menuName = "Scriptable Objects/FlowerSO")]
public class FlowerSO : ScriptableObject
{
    public Flower FlowerPrefab;
    public Sprite FlowerIcon;
    public string FlowerName;
    public string Achievement;
    public double SeedCost;
    public double SellValue;
    public float GrowTime;
    public Sprite[] FlowerFrames;
}
