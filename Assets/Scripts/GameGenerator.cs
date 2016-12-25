using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameGenerator : MonoBehaviour {
    public List<string> names;
    public List<Sprite> images;
    public int resourceCount = 40;
    public double minWeight = 1e-250;
    public double maxWeight = 1e250;
    public double minReactionDuration = 1;
    public double maxReactionDuration = 1e6;
    public double minGameDuration = 1e7;
    public double idleLogarithmicPenalty = 0.25;
    public List<Vector2> lifecyclePhases;

    public double humanFactoredGameSpeed;
    public double expectedGameDuration;
    public double recommendedResourceCount;

    public Resource CreateResource() {
        var resource = new Resource();
        resource.name = names.Rnd();
        resource.image = images.Rnd();
        resource.color = UnityEngine.Random.ColorHSV();
        resource.weight = Math.Exp(Extensions.Rnd(Math.Log(minWeight), Math.Log(maxWeight)));
        return resource;
    }

    [ContextMenu("Recalculate")]
    public void Recalculate() {
        humanFactoredGameSpeed = lifecyclePhases.Sum(v => v.x * Math.Pow(v.y, idleLogarithmicPenalty)) / lifecyclePhases.Sum(v => v.x);
        expectedGameDuration = humanFactoredGameSpeed * minGameDuration;

    }

    public void Start() {
        Recalculate();
    }

    public Game CreateGame() {
        Recalculate();
        var game = new Game();
        for (int i = 0; i < resourceCount; i++) {
            game.resources.Add(CreateResource());
        }
        return game;
    }
}
