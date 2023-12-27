using MognomUtils;
using UnityEngine;

public class DespawnAnimationEventListener : MonoBehaviour {


    public void OnAnimationEnd() {
        this.gameObject.Recycle();

    }
}
