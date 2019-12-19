using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ManufactureListener {

    void OnStart(Manufacture m);
    void OnStop(Manufacture m);
    void OnIterationsChanged(Manufacture m, int iterations);
}
