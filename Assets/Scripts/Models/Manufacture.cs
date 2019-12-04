using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class Manufacture {

    public Reaction reaction;
    public float progressPercent;
    public bool isProgress;

    private float performance = 3f;

    public Manufacture(Reaction reaction) {
        this.reaction = reaction;
    }

    public float EstimatedDeltaProgress(float deltaTime, float effort) {
        return deltaTime * EstimatedSpeed(effort);
    }

    public float EstimatedSpeed(float effort) {
        return performance * effort;
    }

}
