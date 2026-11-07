using System;
using System.Collections.Generic;
using Luscious.Audio;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class LevelUpgradeMenu : BaseSubmenu {
    [SerializeField] SoundEffectData upgradeSelectedSound;

    private List<Button> _upgradeButtons = new();
    private readonly Action[] _selectUpgradeActions = new Action[]
    {
        () => Game.UpgradeOptions.SelectOptionOne(),
        () => Game.UpgradeOptions.SelectOptionTwo(),
        () => Game.UpgradeOptions.SelectOptionThree(),
    };
    private bool _hoverClassActive = true;
    private PanelRenderer _panelRenderer;

    private void OnEnable()
    {
        _panelRenderer = GetComponent<PanelRenderer>();
        _panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnDisable()
    {
        _panelRenderer.UnregisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        _upgradeButtons.Clear();
        rootElement.Query<TemplateContainer>("UpgradeButton").ForEach(container =>
        {
            Button button = container.Q<Button>();
            int index = _upgradeButtons.Count;

            _upgradeButtons.Add(button);
            button.clicked += () => OnUpgradeButtonClick(_selectUpgradeActions[index]);
        });
    }

    private void Update()
    {
        if (UnityEngine.Cursor.visible)
        {
            foreach(Button button in _upgradeButtons)
            {
                button.AddToClassList("upgradeButtonHover");
            }

            _hoverClassActive = true;
        }
        else if (!UnityEngine.Cursor.visible)
        {
            foreach(Button button in _upgradeButtons)
            {
                button.RemoveFromClassList("upgradeButtonHover");
            }

            _hoverClassActive = false;
        }
    }

    private void OnUpgradeButtonClick(Action selectUpgrade)
    {
        selectUpgrade.Invoke();
        GameEvents.RaiseGameResumed();
        Game.SoundFX.PlaySoundFXClip(upgradeSelectedSound, Vector3.zero);
    }
}
