﻿using UnityEngine;
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
        resources.Children().ForEach(r => Destroy(r.gameObject));
        game.currentResources.ForEach(r => {
            var resource = Instantiate(resourcePrefab);
            resource.resource = r;
            resource.transform.SetParent(resources);
        });
    }
}
