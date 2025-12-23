using QwertyGarden;
using UnityEngine;

[CreateAssetMenu(fileName = "FlowerSO", menuName = "Scriptable Objects/FlowerSO")]
public class FlowerSO : ScriptableObject
{
    public Flower FlowerPrefab;
    public Sprite FlowerCard;
    public string Name;
    public int Cost;
}
