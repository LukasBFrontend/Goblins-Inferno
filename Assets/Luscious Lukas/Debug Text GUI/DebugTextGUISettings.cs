using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebugTextGUISettings", menuName = "Tools/Debug Text GUI Settings")]
public class DebugTextGUISettings : ScriptableObject
{
    [SerializeField] bool enabled = true;
    [SerializeField] List<VariableReference> variablesToTrack = new();

    [SerializeField] StyleData style;

    static DebugTextGUISettings instance;
    static bool spawned;

    public bool Enabled => enabled;
    public List<VariableReference> VariablesToTrack => variablesToTrack;
    public StyleData Style => style;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        instance = Resources.Load<DebugTextGUISettings>("DebugTextGUISettings");

        if (instance != null && instance.enabled)
        {
            instance.Spawn();
        }
    }

    void Spawn()
    {
        if (spawned)
            return;

        spawned = true;

        GameObject gameObject = new(nameof(DebugTextGUISettings));
        DontDestroyOnLoad(gameObject);

        gameObject.AddComponent<DebugTextGUI>().Initialize(this);
    }

    void OnValidate()
    {
        if (style.containerWidth == 0)
        {
            style.containerWidth = 300f;
        }
        if (style.containerHeight == 0)
        {
            style.containerHeight = 30f;
        }
        if (style.fontSize == 0)
        {
            style.fontSize = 20;
        }
    }

    [System.Serializable]
    public struct StyleData
    {
        public float containerWidth;
        public float containerHeight;
        public int fontSize;
        public Color color;
        [Header("Debug Grid")]
        public bool showGrid;
        public Color gridColor;
        public float gridLineThickness;
    }
}
