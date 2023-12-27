using System;
using UnityEngine;

public class MultiProgressBar : MonoBehaviour {
    [SerializeField] private Transform[] fillLevels;
    [SerializeField] private ParticleSystem increaseEffectParticles;
    private float currentValue;
    
    // TODO smooth the fill level transition using Update and a lerp

    public void SetValue(float value) {
        currentValue = value;
        UpdateFillLevels();
    }

    private void UpdateFillLevels () {
        int lastFillLevel = (int) Mathf.Floor(currentValue);
        float currentFillValue = currentValue - lastFillLevel;

        // When fully filled, ensure it does not try to fill an extra imaginary level
        if (lastFillLevel <= fillLevels.Length - 1) {
            fillLevels[lastFillLevel].localScale = new Vector3(currentFillValue, 1, 1);

            // Skip the first fill level, since its not fully filled yed
            if (lastFillLevel > 0) {
                fillLevels[lastFillLevel - 1].localScale = Vector3.one;
            }

            for (int i = lastFillLevel + 1; i < fillLevels.Length; i++) {
                fillLevels[i].localScale = Vector3.zero;
            }
        } else {
            fillLevels[^1].localScale = new Vector3(1, 1, 1);
        }
    }

    public void PlayEffect() {
        int lastFillLevel = (int)Mathf.Floor(currentValue);
        float currentFillValue = currentValue - lastFillLevel;

        float horizontalOffset = currentFillValue * 1.5f;

        increaseEffectParticles.transform.position = fillLevels[lastFillLevel].transform.position + Vector3.right * horizontalOffset;
        increaseEffectParticles.Play();
    }
}
