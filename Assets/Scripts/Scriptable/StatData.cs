using UnityEngine;

[System.Serializable]
public class StatData
{
    public bool IncludeAsUpgradeOption;
    [Header("Meta")]
    public Sprite UISprite;
    public string name;
    public string shortName;
}

[System.Serializable]
public class MultiplicativeStatData : StatData
{
    [Header("Scaling")]
    [Tooltip("By how many percentiles should the stat scale per level?")]
    [Range(0, 30)] public int multiplierScaling = 15;
}

[System.Serializable]
public class AdditiveStatData : StatData
{
    [Header("Scaling")]
    public float baseValue;
    [Tooltip("By what flat amount should the stat increase per level?")]
    public float flatScaling;
}
