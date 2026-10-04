using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class LevelHUD : MonoBehaviour {
    ProgressBar _healthBar, _expBar;
    PanelRenderer _panelRenderer;
    Player _player;

    void OnEnable()
    {
        _panelRenderer = GetComponent<PanelRenderer>();
        _panelRenderer.RegisterUIReloadCallback(OnUIReload);
        GameEvents.HealthChanged.AddListener(OnHealthChanged);
        GameEvents.ExpChanged.AddListener(OnExpChanged);
        GameEvents.LvlUpEvent.AddListener(OnLvlUp);
    }

    void OnDisable()
    {
        _panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        GameEvents.HealthChanged.RemoveListener(OnHealthChanged);
        GameEvents.ExpChanged.RemoveListener(OnExpChanged);
        GameEvents.LvlUpEvent.RemoveListener(OnLvlUp);
    }

    void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        _healthBar = rootElement.Q<ProgressBar>("HealthBar");
        _expBar = rootElement.Q<ProgressBar>("ExpBar");

        OnHealthChanged(_player.Health.Current, _player.Health.Max);
        OnExpChanged(_player.Exp, _player.ExpToLvlUp(_player.Lvl));
        OnLvlUp(0, _player.Lvl);
    }

    void Awake()
    {
        _player = Game.Player;
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

    void OnLvlUp(int oldLvl, int newLvl)
    {
        _expBar.title = $"Lvl: {newLvl}";
    }
}
