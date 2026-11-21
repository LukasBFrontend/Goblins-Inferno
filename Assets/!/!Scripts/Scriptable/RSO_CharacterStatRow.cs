using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "RSO_CharacterStatRows", menuName = "Runtime/RSO_CharacterStatRows")]
public class RSO_CharacterStatRows : ScriptableObject
{
    public List<StatRow> list = new();

    private HashSet<StatRow> _overLvlZero;
    private HashSet<StatRow> _levelZero;
    private HashSet<StatRow> _baseStat;
    private Dictionary<string, StatRow> _statRowMap;

    public void Initialize()
    {
        _overLvlZero = new();
        _levelZero = new();
        _baseStat = new();
        _statRowMap = new();

        Stat[] stats = Game.Player.Stats.Character.All();

        foreach (var stat in stats)
        {
            StatRow row = new(stat.Sprite, stat.Name, stat.MetaDescription, stat.Lvl, stat.Evaluate(), stat.IncludeAsUpgradeOption);
            _statRowMap.Add(stat.Name, row);

            if (stat.IncludeAsUpgradeOption)
            {
                _levelZero.Add(row);
            }
            else
            {
                _baseStat.Add(row);
            }
        }

        list = _overLvlZero.Concat(_levelZero.Concat(_baseStat)).ToList();
        HideText();
    }

    public void UpdateExisting()
    {
        Stat[] stats = Game.Player.Stats.Character.All();

        foreach (var stat in stats)
        {
            StatRow row = _statRowMap[stat.Name];

            if (stat.Lvl > 0 && !_overLvlZero.Contains(row))
            {
                _overLvlZero.Add(row);
                _levelZero.Remove(row);
            }
            row.Update(stat.Lvl, stat.Evaluate());
        }

        list = _overLvlZero.Concat(_levelZero.Concat(_baseStat)).ToList();
    }

    public void ShowText()
    {
        foreach (var row in list)
        {
            row.textOpacity = 1f;
        }
    }

    public void HideText()
    {
        foreach (var row in list)
        {
            row.textOpacity = 0f;
        }
    }

    [System.Serializable]
    public class StatRow
    {
        public Sprite sprite;
        public string title;
        public int lvl;
        public bool upgradeable;
        public float value;
        public string tooltip;

        public float textOpacity;

        [CreateProperty]
        public string ValueText => upgradeable ? $"{value*100}%" : value.ToString();

        [CreateProperty]
        public StyleEnum<DisplayStyle> DisplayStatLevel => upgradeable ? DisplayStyle.Flex : DisplayStyle.None;

        [CreateProperty]
        public StyleEnum<DisplayStyle> DisplayStatSprite => upgradeable && lvl > 0 ? DisplayStyle.Flex : DisplayStyle.None;


        public StatRow (Sprite sprite, string title, string tooltip, int lvl, float value, bool upgradeable)
        {
            this.sprite = sprite;
            this.title = title;
            this.tooltip = tooltip;
            this.lvl = lvl;
            this.value = value;
            this.upgradeable = upgradeable;
            this.textOpacity = 1f;
        }

        public void Update(int lvl, float value)
        {
            this.lvl = lvl;
            this.value = value;
        }
    }
}
