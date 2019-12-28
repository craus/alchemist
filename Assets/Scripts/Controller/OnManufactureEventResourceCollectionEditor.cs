using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class OnManufactureEventResourceCollectionEditor : ManufactureListener {

    public Action ActionPerformed;

    ResourceCollection col;

    public OnManufactureEventResourceCollectionEditor(ResourceCollection col) {
        this.col = col;
    }


    void ManufactureListener.OnStart(Manufacture m) {
        m.reaction.reagents.ForEach(r => col[r.Key] -= r.Value);
        if (ActionPerformed != null) {
            ActionPerformed.Invoke();
        }
    }

    void ManufactureListener.OnIterationsChanged(Manufacture m, int iterations) {
        if (iterations > 0) {
#if (DEBUG_OUT)
            Debug.Log("reaction " + m.reaction.ToString());
            Debug.Log("iterations " + iterations);
#endif
            m.reaction.reagents.ForEach(r => col[r.Key] -= (r.Value * iterations));
            m.reaction.products.ForEach(r => col[r.Key] += (r.Value * iterations));

            if (ActionPerformed != null) {
                ActionPerformed.Invoke();
            }
        }
    }


    void ManufactureListener.OnStop(Manufacture m) {
        m.reaction.reagents.ForEach(r => col[r.Key] += r.Value);

        if (ActionPerformed != null) {
            ActionPerformed.Invoke();
        }
    }
}
