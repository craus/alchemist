using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[Serializable]
public class Game {
    public List<Resource> resources = new List<Resource>();
    public List<Reaction> reactions = new List<Reaction>();

    public List<Resource> currentResources = new List<Resource>();
}
