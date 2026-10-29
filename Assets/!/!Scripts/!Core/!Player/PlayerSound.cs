using System.Collections;
using System.Collections.Generic;
using Luscious.Audio;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    private const string RUN_STATE = "RUN";

    [SerializeField] Animator animator;

    readonly List<SoundFXLifetimeData> ongoingSoundFX = new();
    int _lastStepIndex;

    public void PlayFootstep()
    {
        if (animator.IsInTransition(0) && !animator.GetNextAnimatorStateInfo(0).IsName(RUN_STATE))
        {
            return;
        }

        var steps = Game.SoundFX.Default.footsteps;
        var lifetimeData = Game.SoundFX.PlayRandomSoundFXClip(steps.clips.ToArray(), transform, steps.volume, _lastStepIndex, out int selectedIndex);
        _lastStepIndex = selectedIndex;

        AudioSource audioSource = lifetimeData.AudioSource;

        audioSource.pitch *= Random.Range(.95f, 1.05f);
        audioSource.volume *= Random.Range(0.8f, 1.2f);

        ongoingSoundFX.Add(lifetimeData);
        lifetimeData.OnLifeTimeOver = () => ongoingSoundFX.Remove(lifetimeData);
    }

    public void InterruptOngoing()
    {
        foreach(var lifeTimeData in ongoingSoundFX)
        {
            StartCoroutine(FadeVolumeRoutine(lifeTimeData));
        }
    }

    IEnumerator FadeVolumeRoutine(SoundFXLifetimeData lifetimeData)
    {
        float elapsed = 0f;
        float duration = Mathf.Min(lifetimeData.TimeLeft, 0.2f);

        AudioSource audioSource = lifetimeData.AudioSource;
        float startVolume = audioSource.volume;

        while (lifetimeData.Active)
        {
            float t = elapsed / duration;

            audioSource.volume = Mathf.Lerp(startVolume, 0, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
