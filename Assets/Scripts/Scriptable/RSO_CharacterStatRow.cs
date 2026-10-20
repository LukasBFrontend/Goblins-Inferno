using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "RSO_CharacterStatRows", menuName = "Runtime/RSO_CharacterStatRows")]
public class RSO_CharacterStatRows : ScriptableObject
{
    public List<StatRow> list = new();

    public void Populate()
    {
        list = new();
        CharacterStats characterStats = Game.Player.Stats.Character;

        foreach (Stat stat in characterStats.All())
        {
            list.Add(new(stat.Sprite, stat.Name, stat.Lvl, stat.Evaluate(), stat.IncludeAsUpgradeOption));
        }
    }

    [System.Serializable]
    public struct StatRow
    {
        public Sprite sprite;
        public string title;
        public int lvl;
        public bool upgradeable;
        public float value;

        [CreateProperty]
        public readonly string ValueText => upgradeable ? $"{value*100}%" : value.ToString();

        [CreateProperty]
        public readonly StyleEnum<DisplayStyle> DisplayStatLevel => upgradeable ? DisplayStyle.Flex : DisplayStyle.None;

        [CreateProperty]
        public readonly StyleEnum<DisplayStyle> DisplayStatSprite => upgradeable ? DisplayStyle.Flex : DisplayStyle.None;


        public StatRow (Sprite sprite, string title, int lvl, float value, bool upgradeable)
        {
            this.sprite = sprite;
            this.title = title;
            this.lvl = lvl;
            this.value = value;
            this.upgradeable = upgradeable;
        }
    }
}
