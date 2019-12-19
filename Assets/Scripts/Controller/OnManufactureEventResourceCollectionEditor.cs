using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class OnManufactureEventResourceCollectionEditor : ManufactureListener {

    ResourceCollection col;

    public OnManufactureEventResourceCollectionEditor(ResourceCollection col) {
        this.col = col;
    }


    void ManufactureListener.OnStart(Manufacture m) {
        m.reaction.reagents.ForEach(r => col[r.Key] -= r.Value);
    }

    void ManufactureListener.OnIterationsChanged(Manufacture m, int iterations) {
        m.reaction.reagents.ForEach(r => col[r.Key] -= (r.Value * iterations));
        m.reaction.products.ForEach(r => col[r.Key] += (r.Value * iterations));
    }


    void ManufactureListener.OnStop(Manufacture m) {
        m.reaction.reagents.ForEach(r => col[r.Key] += r.Value);
    }
}
