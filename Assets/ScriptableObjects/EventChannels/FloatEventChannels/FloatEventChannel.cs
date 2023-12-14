using System;
using UnityEngine;

[CreateAssetMenu(menuName = "EventChannels/FloatEventChannel")]
public class FloatEventChannel : ScriptableObject {
    public event Action<float> Channel;

    public void PostEvent(float args) {
        Channel?.Invoke(args);
    }
}