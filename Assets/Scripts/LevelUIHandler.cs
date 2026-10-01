using UnityEngine;
using UnityEngine.UIElements;

public class LevelUIHandler : MonoBehaviour {
    [SerializeField] PanelRenderer panelRenderer;
    ProgressBar _healthBar, _expBar;
    Player _player;

    void OnEnable()
    {
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
        GameEvents.HealthChanged.AddListener(UpdateHealthBar);
        GameEvents.ExpChanged.AddListener(UpdateExpBar);
        GameEvents.LvlUpEvent.AddListener(UpdateLvlText);
    }

    void OnDisable()
    {
        panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        GameEvents.HealthChanged.RemoveListener(UpdateHealthBar);
        GameEvents.ExpChanged.RemoveListener(UpdateExpBar);
        GameEvents.LvlUpEvent.RemoveListener(UpdateLvlText);
    }

    void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        // Your UI initialization logic.
        _healthBar = rootElement.Q<ProgressBar>("HealthBar");
        _expBar = rootElement.Q<ProgressBar>("ExpBar");
        UpdateHealthBar(_player.Health.Current, _player.Health.Max);
        UpdateExpBar(_player.Exp, _player.ExpToLvlUp(_player.Lvl));
        UpdateLvlText(0, _player.Lvl);
    }

    void Awake()
    {
        _player = Game.Player;
    }

    void UpdateHealthBar(int current, int max) {
        _healthBar.highValue = max;
        _healthBar.value = current;
        _healthBar.title = $"{current}/{max}";
    }

    void UpdateExpBar(int currentExp, int maxExp )
    {
        _expBar.highValue = maxExp;
        _expBar.value = currentExp;
    }

    void UpdateLvlText(int oldLvl, int newLvl)
    {
        _expBar.title = $"Lvl: {newLvl}";
    }
}
