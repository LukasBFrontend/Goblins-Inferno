using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class VolumeSnapshot : MonoBehaviour
{
    [Header("Volume Profiles")]
    [SerializeField] VolumeProfile snapshot;
    VolumeProfile _live;

    public VolumeProfile Default => snapshot;

    [ContextMenu("Reset To Snapshot")]
    public void ResetToSnapshot()
    {
        ApplyOverrides(snapshot, _live);
    }

    [ContextMenu("Apply Overrides")]
    public void ApplyToSnapshot()
    {
        ApplyOverrides(_live, snapshot);
    }

    void ApplyOverrides(VolumeProfile source, VolumeProfile target)
    {
        foreach (VolumeComponent component in source.components)
        {
            Type type = component.GetType();

            VolumeComponent targetComponent = target.components.FirstOrDefault(c => c.GetType() == type);

            if (targetComponent == null)
            {
                targetComponent = target.Add(type);
            }

            component.Override(targetComponent, 1);
        }
    }

    void OnValidate()
    {
        if (enabled == false || !TryGetComponent<Volume>(out var volume) || volume.profile == null)
        {
            Debug.LogWarning($"{name} is missing a {nameof(Volume)}-component or the {nameof(Volume)}-component is missing a {nameof(VolumeProfile)} reference.");
            return;
        }

        _live = volume.profile;
    }

}
