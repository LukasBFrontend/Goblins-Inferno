using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "RSO_WeaponLevelData", menuName = "Runtime/RSO_WeaponLevelData")]
public class RSO_WeaponLevelData : ScriptableObject
{
    public int lvl;
    public Sprite sprite;

    [CreateProperty]
    public StyleEnum<Visibility> Visible => lvl != 0 ? Visibility.Visible : Visibility.Hidden;

    public void SetValues(int lvl, Sprite sprite)
    {
        this.lvl = lvl;
        this.sprite = sprite;
    }
}
