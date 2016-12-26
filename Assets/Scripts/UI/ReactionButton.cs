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
            resourceIcon.resource = r;
            resourceIcon.transform.SetParent(formula);
        });
        var arrowIcon = Instantiate(arrowPrefab);
        arrowIcon.transform.SetParent(formula);
        reaction.products.ForEach(r => {
            var resourceIcon = Instantiate(resourcePrefab);
            resourceIcon.resource = r;
            resourceIcon.transform.SetParent(formula);
        });
    }

    public void DoReaction() {
        Map<Resource, int> cnts = new Map<Resource,int>();
        GameManager.instance.game.currentResources.ForEach(cr => cnts[cr]++);
        Map<Resource, int> reqcnts = new Map<Resource, int>();
        reaction.reagents.ForEach(r => reqcnts[r]++);
        if (reqcnts.Keys.Any(r => reqcnts[r] > cnts[r])) {
            return;
        }
        reaction.reagents.ForEach(r => GameManager.instance.game.currentResources.Remove(r));
        reaction.products.ForEach(r => GameManager.instance.game.currentResources.Add(r));
        GameManager.instance.RefreshResourcesUI();
    }
}
