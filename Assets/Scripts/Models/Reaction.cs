using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[Serializable]
public class Reaction {
    public ResourceCollection reagents = new ResourceCollection();
    public ResourceCollection products = new ResourceCollection();
    public float time;
    public int used;

    public Reaction From(params Resource[] reagents) {
        reagents.ForEach(r => this.reagents[r]++);
        return this;
    }

    public Reaction To(params Resource[] products) {
        products.ForEach(r => this.products[r]++);
        return this;
    }

    public Reaction In(float time) {
        this.time = time;
        return this;
    }
}
