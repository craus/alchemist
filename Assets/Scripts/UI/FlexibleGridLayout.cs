using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FlexibleGridLayout : MonoBehaviour {
    GridLayoutGroup grid;

    public RectTransform containerToFit;

    public Vector2 minCellSize;

    public Vector2 addCellPart;

    void Start() {
        grid = GetComponent<GridLayoutGroup>();
    }

    [ContextMenu("Update")]
    void Update() {
        if (grid == null) {
            grid = GetComponent<GridLayoutGroup>();
        }
        Vector2 cellCnt = new Vector2(
            (int)((containerToFit.rect.width - grid.padding.horizontal + grid.spacing.x) / (minCellSize.x + grid.spacing.x) - addCellPart.x) + addCellPart.x,
            (int)((containerToFit.rect.height - grid.padding.vertical + grid.spacing.y) / (minCellSize.y + grid.spacing.y) - addCellPart.y) + addCellPart.y
        ).Clamp(Vector2.one, Vector2.one * 100);
        grid.cellSize = (containerToFit.rect.size - new Vector2(grid.padding.horizontal, grid.padding.vertical) - grid.spacing.Scaled(cellCnt - Vector2.one)).Scaled(cellCnt.Inverse());
        grid.constraintCount = (int)cellCnt.y;
    }
}
