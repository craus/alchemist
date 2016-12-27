using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ReactionButton : MonoBehaviour {
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
    }
}
