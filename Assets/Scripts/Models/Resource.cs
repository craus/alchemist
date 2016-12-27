using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[Serializable]
public class Resource {
    public double weight;
    public string name;
    public Sprite image;
    public Color color;
    public Material material;

    public override string ToString() {
        return String.Format("{0} ({1})", name, weight);
    }
}
