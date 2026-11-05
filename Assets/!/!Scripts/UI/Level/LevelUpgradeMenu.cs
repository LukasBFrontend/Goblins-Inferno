using Luscious.Audio;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class LevelUpgradeMenu : BaseSubmenu {
    [SerializeField] SoundEffectData upgradeSelectedSound;

    Button _upgradeButtonOne, _upgradeButtonTwo, _upgradeButtonThree;
    PanelRenderer _panelRenderer;

    void OnEnable()
    {
        _panelRenderer = GetComponent<PanelRenderer>();
        _panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    void OnDisable()
    {
        _panelRenderer.UnregisterUIReloadCallback(OnUIReload);
    }

    void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        _upgradeButtonOne = rootElement.Q<TemplateContainer>("UpgradeButtonOne").Q<Button>();
        _upgradeButtonTwo = rootElement.Q<TemplateContainer>("UpgradeButtonTwo").Q<Button>();
        _upgradeButtonThree = rootElement.Q<TemplateContainer>("UpgradeButtonThree").Q<Button>();

        _upgradeButtonOne.clicked += OnUpgradeButtonOneClick;
        _upgradeButtonTwo.clicked += OnUpgradeButtonTwoClick;
        _upgradeButtonThree.clicked += OnUpgradeButtonThreeClick;
    }

    void OnUpgradeButtonOneClick()
    {
        Game.UpgradeOptions.SelectOptionOne();
        GameEvents.RaiseGameResumed();
        PlayUpgradeSelectedSound();
    }

    void OnUpgradeButtonTwoClick()
    {
        Game.UpgradeOptions.SelectOptionTwo();
        GameEvents.RaiseGameResumed();
        PlayUpgradeSelectedSound();
    }

    void OnUpgradeButtonThreeClick()
    {
        Game.UpgradeOptions.SelectOptionThree();
        GameEvents.RaiseGameResumed();
        PlayUpgradeSelectedSound();
    }

    void PlayUpgradeSelectedSound()
    {
        Game.SoundFX.PlaySoundFXClip(upgradeSelectedSound, Vector3.zero);
    }
}
