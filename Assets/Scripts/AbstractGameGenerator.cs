using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

public abstract class AbstractGameGenerator : MonoBehaviour {
    public abstract Game CreateGame();
}
