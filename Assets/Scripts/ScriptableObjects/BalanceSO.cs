using UnityEngine;

[CreateAssetMenu(fileName = "BalanceSO", menuName = "Scriptable Objects/BalanceSO")]
public class BalanceSO : ScriptableObject
{
    public int StartingCoins;
    public FlowerSO[] Flowers;
    public int KeyboardStartingPrice = 10;
    public float KeyboardPriceIncrease = 2.5f;

    public double PrestigeCost;
    public double PrestigeMultiplier;
}
