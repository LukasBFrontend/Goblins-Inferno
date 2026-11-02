using System.Collections;
using UnityEngine;

namespace Luscious.Audio
{
    public static class AudioUtils
    {
        const float MINIMUM_VOLUME_DB = -80f;

        public static float PercentToDecibel(float percent)
        {
            percent = Mathf.Clamp(percent, 0f, 100f);

            if (percent <= 0f)
            {
                return MINIMUM_VOLUME_DB;
            }

            float linear = percent / 100f;
            return Mathf.Log10(linear) * 20f;
        }

        public static float DecibelToPercent(float db)
        {
            if (db <= MINIMUM_VOLUME_DB)
            {
                return 0f;
            }

            float linear = Mathf.Pow(10f, db / 20f);
            return linear * 100f;
        }

        public static void FadeVolume(MonoBehaviour monoBehaviour, AudioSource audioSource, float duration)
        {
            monoBehaviour.StartCoroutine(FadeVolumeRoutine(audioSource, duration));
        }

        private static IEnumerator FadeVolumeRoutine(AudioSource audioSource, float duration)
        {
            float elapsed = 0f;
            float startVolume = audioSource.volume;

            while (elapsed <= duration && audioSource != null)
            {
                float t = elapsed / duration;

                audioSource.volume = Mathf.Lerp(startVolume, 0, t);

                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
