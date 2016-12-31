using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class Reaction {
    public ResourceCollection reagents = new ResourceCollection();
    public ResourceCollection products = new ResourceCollection();
    public float time;
    public int used;

    public Reaction From(params Resource[] reagents) {
        return From(1, reagents);
    }

    public Reaction From(int cnt = 1, params Resource[] reagents) {
        for (int i = 0; i < cnt; i++) {
            reagents.ForEach(r => this.reagents[r]++);
        }
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

    public bool NotWorse(Reaction other) {
        return reagents.All(r => other.reagents[r.Key] >= r.Value) && other.products.All(p => products[p.Key]-reagents[p.Key] >= p.Value-other.reagents[p.Key]);
    }
}
