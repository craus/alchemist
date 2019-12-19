using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class Manufacture {
    public const long TIME_IN_SEC = 10000000;
    public Reaction reaction;
    public long startTime;
    public float progressPercent;
    public bool isProgress;

    private float performance = 3f * TIME_IN_SEC;

    public float Effort = 0f;

    public OnStop OnStopListener;
    public ManufactureListener manufactureListener;
    public Manufacture(Reaction reaction) {
        this.reaction = reaction;
    }

    public float EstimatedDeltaProgress(long deltaTime) {
        return deltaTime * EstimatedSpeed();
    }

    public float EstimatedSpeed() {
        return performance * Effort;
    }
    public void StartReaction(long startTime) {
        this.startTime = startTime;
        isProgress = true;
        reaction.StartReaction();
        manufactureListener.OnStart(this);
    }

    //be sure to rewind before call stop
    public void Stop() {
        isProgress = false;
        progressPercent = 0f;
        manufactureListener.OnStop(this);
        if (OnStopListener != null) {
            OnStopListener.Invoke();
        }
    }

    public delegate void OnStop();

    public void Rewind(long nextTime) {
        //throw new NotImplementedException();

        //manufactureListener.OnIterationsChanged(this,);
    }
}
