using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class Manufacture {

    public Reaction reaction;
    public long startTime;
    public float progressPercent;
    public bool isProgress;

    private float performance = 3f* 10000000;

    public float Effort = 0f;

    public OnStop OnStopListener;

    public Manufacture(Reaction reaction) {
        this.reaction = reaction;
    }

    public float EstimatedDeltaProgress(long deltaTime) {
        return deltaTime * EstimatedSpeed();
    }

    public float EstimatedSpeed() {
        return performance * Effort;
    }
    public void StartReaction() {
        isProgress = true;
        reaction.StartReaction();
    }

    public void Stop() {
        isProgress = false;
        progressPercent = 0f;
        if (OnStopListener != null) {
            OnStopListener.Invoke();
        }
    }

    public delegate void OnStop();

    public void Rewind(long nextTime) {
        throw new NotImplementedException();
    }
}
