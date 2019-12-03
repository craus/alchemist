using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class ReactionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;
    public Color usedColor;
    public Reaction reaction;
    public Transform formula;
    public ResourceIcon resourcePrefab;
    public GameObject arrowPrefab;

    void Start() {
        formula.Children().ForEach(c => Destroy(c.gameObject));
        reaction.reagents.ForEach(r => {
            var resourceIcon = Instantiate(resourcePrefab);
            resourceIcon.resource = r.Key;
            resourceIcon.amount = r.Value;
            resourceIcon.transform.SetParent(formula);
        });
        var arrowIcon = Instantiate(arrowPrefab);
        arrowIcon.transform.SetParent(formula);
        reaction.products.ForEach(r => {
            var resourceIcon = Instantiate(resourcePrefab);
            resourceIcon.resource = r.Key;
            resourceIcon.amount = r.Value;
            resourceIcon.transform.SetParent(formula);
        });
    }

    public bool Doable() {
        return !reaction.reagents.Keys.Any(r => reaction.reagents[r] > GameManager.instance.game.currentResources[r]);
    }

    public void DoReaction() {
        if (!Doable()) {
            return;
        }
        reaction.reagents.ForEach(r => GameManager.instance.game.currentResources[r.Key] -= r.Value);
        reaction.products.ForEach(r => GameManager.instance.game.currentResources[r.Key] += r.Value);
        GameManager.instance.RefreshResourcesUI();
        reaction.used++;
        if (reaction.used == 1) {
            var colors = button.colors;
            colors.normalColor *= usedColor;
            colors.highlightedColor *= usedColor;
            colors.pressedColor *= usedColor;
            button.colors = colors;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        GameManager.instance.status.text = string.Format("{0:0.0000e0} -> {1:0.0000e0} (x{2:0.0000})", reaction.reagents.Weight(), reaction.products.Weight(), reaction.products.Weight() / reaction.reagents.Weight());
    }

    public void OnPointerExit(PointerEventData eventData) {
        GameManager.instance.status.text = "";
    }
}
