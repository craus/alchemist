using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public GameGenerator generator;
    public Game game;
    public Transform reactions;
    public Transform resources;
    public ReactionButton reactionPrefab;
    public ResourceIcon resourcePrefab;
    public Text status;

    public void Awake() {
        instance = this;
    }

    public void Start() {
        game = generator.CreateGame();
        RefreshUI();
    }

    public void RefreshUI() {
        reactions.Children().ForEach(c => Destroy(c.gameObject));
        game.reactions.ForEach(r => {
            var reaction = Instantiate(reactionPrefab);
            reaction.transform.SetParent(reactions);
            reaction.reaction = r;
        });
        RefreshResourcesUI();
    }

    public void RefreshResourcesUI() {
        resources.Children().ForEach(r => Destroy(r.gameObject));
        game.currentResources.Keys.ForEach(r => {
            if (game.currentResources[r] > 0) {
                var resource = Instantiate(resourcePrefab);
                resource.resource = r;
                resource.amount = game.currentResources[r];
                resource.transform.SetParent(resources);
            }
        });
    }

    public void Repaint() {

    }
}
