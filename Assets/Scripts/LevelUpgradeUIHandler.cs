using UnityEngine;
using UnityEngine.UIElements;

public class LevelUpgradeUIHandler : UIOverlay {

    Button _upgradeButtonOne, _upgradeButtonTwo, _upgradeButtonThree;
    VisualElement _overlayContainer;

    void OnEnable()
    {
        PanelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    void OnDisable()
    {
        PanelRenderer.UnregisterUIReloadCallback(OnUIReload);
        _upgradeButtonOne.clicked -= OnUpgradeButtonOneClick;
        _upgradeButtonTwo.clicked -= OnUpgradeButtonTwoClick;
        _upgradeButtonThree.clicked -= OnUpgradeButtonThreeClick;
    }

    void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        this.SetOverlayContainer(rootElement);
        _upgradeButtonOne = rootElement.Q<TemplateContainer>("UpgradeButtonOne").Q<Button>();
        _upgradeButtonTwo = rootElement.Q<TemplateContainer>("UpgradeButtonTwo").Q<Button>();
        _upgradeButtonThree = rootElement.Q<TemplateContainer>("UpgradeButtonThree").Q<Button>();

        _upgradeButtonOne.clicked += OnUpgradeButtonOneClick;
        _upgradeButtonTwo.clicked += OnUpgradeButtonTwoClick;
        _upgradeButtonThree.clicked += OnUpgradeButtonThreeClick;
    }

    void OnUpgradeButtonOneClick()
    {
        Game.UpgradeOptionManager.SelectOptionOne();
    }

    void OnUpgradeButtonTwoClick()
    {
        Game.UpgradeOptionManager.SelectOptionTwo();
    }

    void OnUpgradeButtonThreeClick()
    {
        Game.UpgradeOptionManager.SelectOptionThree();
    }
}
