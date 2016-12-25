using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollToGrid : MonoBehaviour {
    public GridLayoutGroup grid;
    public int steps = 1;

    ScrollRect rect;

    void Start() {
        rect = GetComponent<ScrollRect>();
        rect.scrollSensitivity = -(grid.cellSize.x + grid.spacing.x) / steps;
    }
}
