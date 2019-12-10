using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public AbstractGameGenerator generator;
    public Game game;
    public Transform reactions;
    public Transform resources;
    public ReactionButton reactionPrefab;
    public ResourceIcon resourcePrefab;
    public Text status;

    public List<ReactionButton> reactionButtons;

    public SliderEventHandler sliderEventHandler = new SliderEventHandler();
    public DiscreteController timeController;

    public void Awake() {
        instance = this;
    }

    public void Start() {
        //sliderEventHandler = new SliderEventHandler();
        game = generator.CreateGame();
        RefreshUI();
    }

    [ContextMenu("Generate")]
    public void Generate() {
        game = generator.CreateGame();
    }

    public void RefreshUI() {
        reactions.Children().ForEach(c => Destroy(c.gameObject));
        game.reactions.ForEach(r => {
            var reaction = Instantiate(reactionPrefab);
            reactionButtons.Add(reaction);
            reaction.transform.SetParent(reactions);
            reaction.reaction = r;
        });
        RefreshResourcesUI();
    }
    public void RefreshResourcesAfterIdle() {
        RefreshResources();
    }

    private void RefreshResources() {
        resources.Children().ForEach(r => Destroy(r.gameObject));
        game.currentResources.Keys.ForEach(r => {
            if (game.currentResources[r] > 0) {
                var resource = Instantiate(resourcePrefab);
                resource.resource = r;
                resource.amount = game.currentResources[r];
                resource.transform.SetParent(resources);
            }
        });
        //reactionButtons.ForEach(rb => {
        //    rb.gameObject.SetActive(rb.Doable());
        //});
    }

    public void RefreshResourcesUI() {
        RefreshResources();
        GameStateChanged();
    }

    public void GameStateChanged() {
        if (timeController != null) {
            timeController.WorldChanged();
        }
    }
}
