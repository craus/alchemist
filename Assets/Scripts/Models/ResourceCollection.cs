using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class ResourceCollection : Map<Resource, int> {
    public double Weight() {
        return this.Sum(r => r.Key.weight * r.Value);
    }

    public ResourceCollection() {
        Init();
    }

    public ResourceCollection(ResourceCollection rc) : base(rc) {
        Init();
    }

    void Init() {
        removeDefaultValues = x => x == 0;
    }

    public new void Remove(Resource r) {
        this[r]--;
        Debug.LogWarning("Remove");
    }
}
