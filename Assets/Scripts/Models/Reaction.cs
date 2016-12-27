using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[Serializable]
public class Reaction {
    public List<Resource> reagents = new List<Resource>();
    public List<Resource> products = new List<Resource>();
    public float time;

    public Reaction From(params Resource[] reagents) {
        this.reagents.AddRange(reagents);
        return this;
    }

    public Reaction To(params Resource[] products) {
        this.products.AddRange(products);
        return this;
    }

    public Reaction In(float time) {
        this.time = time;
        return this;
    }
}
