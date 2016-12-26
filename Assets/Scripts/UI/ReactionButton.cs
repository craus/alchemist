using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
        //GameManager.instance.game.currentResources
    }
}
