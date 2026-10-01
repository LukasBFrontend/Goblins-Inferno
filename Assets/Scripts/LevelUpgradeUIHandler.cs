using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelUpgradeUIHandler : MonoBehaviour {
    [SerializeField] PanelRenderer panelRenderer;
    Button _upgradeButtonOne, _upgradeButtonTwo, _upgradeButtonThree;

    void OnEnable()
    {
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    void OnDisable()
    {
        panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        _upgradeButtonOne.clicked -= OnUpgradeButtonOneClick;
        _upgradeButtonTwo.clicked -= OnUpgradeButtonTwoClick;
        _upgradeButtonThree.clicked -= OnUpgradeButtonThreeClick;
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
