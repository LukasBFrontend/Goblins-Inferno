using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class LevelHUD : MonoBehaviour {
    [SerializeField] RSO_WeaponLevelData weaponOneData;
    [SerializeField] RSO_WeaponLevelData weaponTwoData;
    [SerializeField] RSO_WeaponLevelData weaponThreeData;

    ProgressBar _healthBar, _expBar;
    Label _scoreText;
    PanelRenderer _panelRenderer;
    Player _player;

    void OnEnable()
    {
        _panelRenderer = GetComponent<PanelRenderer>();
        _panelRenderer.RegisterUIReloadCallback(OnUIReload);
        GameEvents.HealthChanged.AddListener(OnHealthChanged);
        GameEvents.ExpChanged.AddListener(OnExpChanged);
        GameEvents.ScoreChanged.AddListener(OnScoreChanged);
        GameEvents.LvlUpEvent.AddListener(OnLvlUp);
        GameEvents.GameResumed.AddListener(ReadWeaponData);
    }

    void OnDisable()
    {
        _panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        GameEvents.HealthChanged.RemoveListener(OnHealthChanged);
        GameEvents.ExpChanged.RemoveListener(OnExpChanged);
        GameEvents.ScoreChanged.RemoveListener(OnScoreChanged);
        GameEvents.LvlUpEvent.RemoveListener(OnLvlUp);
        GameEvents.GameResumed.RemoveListener(ReadWeaponData);
    }

    void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        _healthBar = rootElement.Q<ProgressBar>("HealthBar");
        _expBar = rootElement.Q<ProgressBar>("ExpBar");
        _scoreText = rootElement.Q<Label>("ScoreText");

        OnHealthChanged(_player.Health.Current, _player.Health.Max);
        OnExpChanged(_player.Exp, _player.ExpToLvlUp(_player.Lvl));
        OnScoreChanged(0);
        OnLvlUp(0, _player.Lvl);
    }

    void Awake()
    {
        _player = Game.Player;
    }

    void Start()
    {
        ReadWeaponData();
    }

    void OnHealthChanged(int current, int max) {
        _healthBar.highValue = max;
        _healthBar.value = current;
        _healthBar.title = $"{current}/{max}";
    }

    void OnExpChanged(int currentExp, int maxExp )
    {
        _expBar.highValue = maxExp;
        _expBar.value = currentExp;
    }

    void OnScoreChanged(int newScore)
    {
        _scoreText.text = newScore.ToString();
    }

    void OnLvlUp(int oldLvl, int newLvl)
    {
        _expBar.title = $"Lvl: {newLvl}";
    }

    void ReadWeaponData()
    {
        Weapon[] unlockedWeapons = Game.Player.Weapons.Unlocked.ToArray();
        RSO_WeaponLevelData[] weaponLevelDatas = { weaponOneData, weaponTwoData, weaponThreeData };

        int i = 0;
        while (i < weaponLevelDatas.Length)
        {
            int lvl;
            Sprite sprite;

            if (i < unlockedWeapons.Length)
            {
                lvl = unlockedWeapons[i].Lvl;
                sprite = unlockedWeapons[i].Sprite;
            }
            else
            {
                lvl = 0;
                sprite = null;
            }

            weaponLevelDatas[i].SetValues(
                lvl,
                sprite
            );

            i++;
        }
    }
}
