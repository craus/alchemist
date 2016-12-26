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
}
