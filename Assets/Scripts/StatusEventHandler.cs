using UnityEngine;
using Vuforia;

public class StatusEventHandler : MonoBehaviour {
    public bool TargetTracked = false;

    private ObserverBehaviour mObserverBehaviour;
    
    void Awake() {
        mObserverBehaviour = GetComponent<ObserverBehaviour>();

        if (mObserverBehaviour != null)
            mObserverBehaviour.OnTargetStatusChanged += MObserverBehaviour_OnTargetStatusChanged;
            // mObserverBehaviour.OnTargetStatusUpdated += OnStatusChanged;
    }

    void OnDestroy() {
        if (mObserverBehaviour != null)
            mObserverBehaviour.OnTargetStatusChanged -= MObserverBehaviour_OnTargetStatusChanged;
    }

    private void MObserverBehaviour_OnTargetStatusChanged(ObserverBehaviour arg1, TargetStatus status) {
        // || status.Status == Status.EXTENDED_TRACKED
        if (status.Status == Status.TRACKED /*|| status.Status == Status.EXTENDED_TRACKED*/) {
            TargetTracked = true;
        } else {
            TargetTracked = false;
        }
    }
}