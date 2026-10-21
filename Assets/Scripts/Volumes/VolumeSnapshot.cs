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

    [ContextMenu("Apply Snapshot")]
    public void ApplySnapshot()
    {
        foreach (VolumeComponent component in snapshot.components)
        {
            Type type = component.GetType();

            VolumeComponent liveComponent = _live.components.FirstOrDefault(c => c.GetType() == type);

            if (liveComponent == null)
            {
                liveComponent = _live.Add(type);
            }

            component.Override(liveComponent, 1);
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
