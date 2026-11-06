
using System.Collections;
using System.Linq;
using UnityEngine;

public sealed class EnergyExplosionProjectile : BaseProjectile
{
    public override IEnumerator ProjectileRoutine(BaseCharacter target)
    {
        TargetTracker.SetTarget(target);
        float elapsed = 0f;
        float duration = ParticleSystem.controls.EffectiveDuration;
        bool hasAttempedHits = false, hasPlayedSound = false;

        while (elapsed < duration)
        {
            float elapsedRelative = elapsed / duration;

            if (elapsedRelative >= ParticleSystem.soundEffectAt && !hasPlayedSound)
            {
                StopAliveSound();
                if (Data.onDestroy.clips.Count > 0)
                {
                    Game.SoundFX.PlayRandomSoundFXClip(Data.onDestroy, transform.position);
                }

                hasPlayedSound = true;
            }

            if (elapsedRelative >= ParticleSystem.tryRegisterHitsAt && !hasAttempedHits)
            {
                HitTargetIfWithinRange(target);

                hasAttempedHits = true;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // elapsedRelative is unlikely to land at exactly 1f.

        if (!hasPlayedSound)
        {
            StopAliveSound();
            if (Data.onDestroy.clips.Count > 0)
            {
                Game.SoundFX.PlayRandomSoundFXClip(Data.onDestroy, transform.position);
            }
        }

        if (!hasAttempedHits)
        {
            HitTargetIfWithinRange(target);
        }

        Game.ObjectPooler.Return(this);
    }

    void OnValidate()
    {
        if (ParticleSystem.controls == null)
        {
            Debug.LogWarning($"{nameof(EnergyExplosionProjectile)} <b><color=white>{name}</color></b> with '{nameof(Data)} = {nameof(ProjectileArcMode.None)}' is missing field <b><color=white>{nameof(ParticleSystemControls)}</color></b>");
        }
    }

    private void HitTargetIfWithinRange(BaseCharacter target)
    {
        if (TargetTracker.InRange(Data.newTargetRange, Data.targetsLayerMask).Contains(target))
        {
            RegisterHit(target);
        }
    }
}
