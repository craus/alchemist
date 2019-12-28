using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Manufacture {
    public const long TIME_IN_SEC = 10000000;
    public const double EPSILON = 1d / TIME_IN_SEC;
    public Reaction reaction;
    public long lastTime;
    public double progressPart;
    public bool isProgress;

    private double performance = 3d;

    public float Effort = 0f;

    public OnStop OnStopListener;
    public ManufactureListener manufactureListener;

    public Manufacture(Reaction reaction) {
        this.reaction = reaction;
    }

    public double EstimatedDeltaProgress(long deltaTime) {
        return deltaTime * EstimatedSpeed();
    }

    public double EstimatedSpeed() {
        return performance * Effort;
    }

    public void StartReaction(long startTime) {
        this.lastTime = startTime;
        isProgress = true;
        progressPart = 0f;
        manufactureListener.OnStart(this);
    }

    //be sure to rewind before call stop
    public void Stop() {
        isProgress = false;
        progressPart = 0f;
        //if(oldIsProgress)
        manufactureListener.OnStop(this);
        if (OnStopListener != null) {
            OnStopListener.Invoke();
        }
    }

    public long TimeToNextReaction() {
        double deltaTime = Math.Ceiling(TIME_IN_SEC / EstimatedSpeed() * (1 - progressPart));
        if (double.IsInfinity(deltaTime) || double.IsNaN(deltaTime) || deltaTime > DiscreteController.NEVER / 2) {
            return DiscreteController.NEVER;
        } else {
            return lastTime + (long)deltaTime;
        }
    }

    public delegate void OnStop();

    public void Rewind(long nextTime) {
        long deltaTime = nextTime - lastTime;
        if (deltaTime > 0) {
            double progress = EstimatedSpeed() * deltaTime / TIME_IN_SEC + progressPart;
#if (DEBUG_OUT)
            Debug.Log("progress " + progress);
#endif
            int iterations = (int)progress;
            //int iterations = (int)(progress + EPSILON);
            progressPart = Math.Max(0d, progress - iterations);
            lastTime = nextTime;
            manufactureListener.OnIterationsChanged(this, iterations);
        }

    }
}
