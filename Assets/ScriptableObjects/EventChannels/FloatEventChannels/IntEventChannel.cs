using System;
using UnityEngine;

[CreateAssetMenu(menuName = "EventChannels/FloatEventChannel")]
public class IntEventChannel : ScriptableObject {
    public event Action<int> Channel;

    public void PostEvent(int args) {
        Channel?.Invoke(args);
    }
}