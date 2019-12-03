using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameGenerator : AbstractGameGenerator {
    const int TRIES = 10000;
    const int BIG_TRIES = 10;

    public Resource philosophersStone;
    public List<string> names;
    public Material whiteImagesMaterial;
    public Material blackImagesMaterial;
    public List<Sprite> whiteImages;
    public List<Sprite> blackImages;
    public List<Sprite> images;
    public int resourceCount = 40;
    public int reactionCount = 100;
    public double minWeight = 1e-250;
    public double maxWeight = 1e250;
    public double doublingTime = 180;
    public double idleLogarithmicPenalty = 0.25;
    public int startResources = 10;
    public int badWeightMultiplier = 4;
    public double reactionMultiplier = 1.05;
    public bool useReactionMultiplier = false;
    public int resourceDelta = 5;

    public double minReactionDuration = 1;
    public double maxReactionDuration = 1e6;
    public double minGameDuration = 1e7;
    public List<Vector2> lifecyclePhases;

    public double humanFactoredGameSpeed;
    public double expectedGameDuration;
    public double recommendedResourceCount;

    public Resource CreateResource() {
        var resource = new Resource();
        resource.name = names.Rnd();
        resource.image = images.Rnd();
        if (whiteImages.Contains(resource.image)) {
            resource.material = whiteImagesMaterial;
            resource.color = UnityEngine.Random.ColorHSV();
        } else {
            resource.material = blackImagesMaterial;
            resource.color = UnityEngine.Random.ColorHSV(0, 1, 0, 1, 0, 0.7f);
        }
        resource.weight = Math.Exp(Extensions.Rnd(Math.Log(minWeight), Math.Log(maxWeight)));
        return resource;
    }

    public bool Trivial(Reaction reaction) {
        return Math.Abs(reaction.reagents.Sum(r => r.Key.weight * r.Value) - reaction.products.Sum(r => r.Key.weight * r.Value)) < 1e-9;
    }

    public float MultiplierToTime(double multiplier) {
        return (float)Math.Pow(Math.Log(Math.Max(multiplier, 1.0001), 2) * doublingTime, 1.0 / (1 - idleLogarithmicPenalty));
    }

    public double TimeToMultiplier(float time) {
        return Math.Pow(2, Math.Pow(time, 1 - idleLogarithmicPenalty) / doublingTime);
    }

    public Reaction CreateReaction(Game game) {
        Reaction reaction = null;
        for (int j = 0; j < BIG_TRIES; j++) {
            if (j == BIG_TRIES - 1) {
                Debug.LogError("Too much big tries");
            }
            reaction = new Reaction();
            int reagentsCount = (int)(Math.Exp(Extensions.GaussianRnd() * 0.27) * 1.6) + 1;

            double recommendedReagentWeight = Math.Exp(Extensions.Rnd(Math.Log(minWeight), Math.Log(maxWeight)));
            var pivotReagentResource = game.resources.LastOrDefault(r => r.weight < recommendedReagentWeight);
            int pivotReagent = game.resources.IndexOf(pivotReagentResource);
            int minReagent = Mathf.Clamp(pivotReagent - 2 * resourceDelta, 0, game.resources.Count - 1);
            int maxReagent = Mathf.Clamp(pivotReagent, 0, game.resources.Count - 1) + 1;

            List<Resource> availableReagents = game.resources.Range(from: minReagent, to: maxReagent).RndSelection(reagentsCount);
            while(reaction.reagents.Weight() < recommendedReagentWeight) {
                reaction.reagents[availableReagents.ToList().Rnd()]++;
            }
            double reagentsWeight = reaction.reagents.Weight();
            double productsWeight = 0;
            float recommendedTime = (float)Math.Pow(10, Math.Exp(Extensions.GaussianRnd() * 0.27) * 1.4);
            if (useReactionMultiplier) {
                recommendedTime = MultiplierToTime(reactionMultiplier);
            }
            double minAcceptableMultiplier = TimeToMultiplier(recommendedTime / 2);
            //  Debug.LogFormat("minAcceptableMultiplier = {0}", minAcceptableMultiplier);
            double maxAcceptableMultiplier = TimeToMultiplier(recommendedTime * 2);
            double recommendedMultiplier = TimeToMultiplier(recommendedTime);
            int shift = (int)(Math.Log(recommendedMultiplier) / Math.Log(maxWeight / minWeight) * resourceCount);
            int pivotProduct = pivotReagent + shift;
            int minProduct = Mathf.Clamp(pivotProduct - resourceDelta, 0, game.resources.Count - 1);
            int maxProduct = Mathf.Clamp(pivotProduct + resourceDelta, 0, game.resources.Count - 1);
            List<Resource> availableProducts = game.resources.Range(from: minProduct, to: maxProduct).RndSelection(Math.Max(1, 5 - reaction.reagents.Keys.Count));

            double minProductsWeight = reagentsWeight * minAcceptableMultiplier + game.resources[0].weight;
            double maxProductsWeight = reagentsWeight * maxAcceptableMultiplier + game.resources[0].weight;
            double recommendedProductsWeight = reagentsWeight * recommendedMultiplier + game.resources[0].weight;
            bool tooMuchTries = false;
            for (int i = 0; i < TRIES; i++) {
                if (minProductsWeight < productsWeight && productsWeight < maxProductsWeight) {
                    break;
                }
                if (productsWeight > recommendedProductsWeight && reaction.products.Count > 1) {
                    var resource = reaction.products.Keys.ToList().Rnd();
                    reaction.products[resource]--;
                    if (reaction.products[resource] < 0) {
                        Debug.LogError("-");
                        return new Reaction();
                    }
                    productsWeight -= resource.weight;
                } else {
                    var resource = availableProducts.ToList().Rnd();
                    reaction.products[resource]++;
                    productsWeight += resource.weight;
                }
                if (i == TRIES-1) {
                    tooMuchTries = true;
                }
            }

            reaction.time = MultiplierToTime((float)(productsWeight / reagentsWeight));
            if (!Trivial(reaction) && !tooMuchTries && reaction.products.Count <= 20 && !game.reactions.Any(other => other.NotWorse(reaction))) {
                if (productsWeight < minProductsWeight) {
                    Debug.LogFormat("productsWeight < minProductsWeight, tooMuchTries = {0}", tooMuchTries);
                }
                if (reaction.products.Weight() / reaction.reagents.Weight() < 1) {
                    Debug.LogFormat("reaction.products.Weight() / reaction.reagents.Weight() < 1, tooMuchTries = {0}", tooMuchTries);
                }
                //Debug.LogFormat("[{2}], {0} - {1}, [{3}] <<{4}>>", minAcceptableMultiplier, maxAcceptableMultiplier, recommendedTime, reaction.time, reaction.products.Count);
                //if (reaction.products.Weight() / reaction.reagents.Weight() < 1) {
                //    Debug.LogFormat("{0}", reaction);
                //    Debug.LogFormat("{0}", minProductsWeight);
                //    Debug.LogFormat("{0}", maxProductsWeight);
                //    Debug.LogFormat("{0}", reagentsWeight);
                //}
                return reaction;
            }
        }
        Debug.LogError("Failed to generate non-trivial reaction");
        return reaction;
    }

    [ContextMenu("Recalculate")]
    public void Recalculate() {
        humanFactoredGameSpeed = lifecyclePhases.Sum(v => v.x / Math.Pow(v.y, idleLogarithmicPenalty)) / lifecyclePhases.Sum(v => v.x);
        expectedGameDuration = minGameDuration / humanFactoredGameSpeed;

        blackImages = Resources.LoadAll<Sprite>("ResourceImages/Black").ToList();
        whiteImages = Resources.LoadAll<Sprite>("ResourceImages/White").ToList();
        images = blackImages.Concat(whiteImages).ToList();
    }

    [ContextMenu("Test")]
    public void Test() {
        ResourceCollection rc = new ResourceCollection();
        var r = new Resource();
        rc[r]++;
        //rc[r]--;
        Debug.LogFormat("rc {0}", rc);
        Debug.LogFormat("rc[r] {0}", rc[r]);
        Debug.LogFormat("rc {0}", rc);
    }

    public void Start() {
        Recalculate();
    }

    public override Game CreateGame() {
        Recalculate();
        //UnityEngine.Random.InitState(43);
        var game = new Game();
        for (int i = 0; i < resourceCount; i++) {
            game.resources.Add(CreateResource());
        }
        game.resources = game.resources.OrderBy(r => r.weight).ToList();
        for (int i = 0; i < reactionCount; i++) {
            game.reactions.Add(CreateReaction(game));
        }
        var resOrder = new List<Resource>(game.resources);
        var reqRes = new HashSet<Resource>(game.resources);

        //game.reactions.ForEach(r => {
        //    int maxReagentIndex = r.reagents.Max(x => resOrder.IndexOf(x.Key));
        //    r.products.ForEach(p => {
        //        if (resOrder.IndexOf(p.Key) > maxReagentIndex) {
        //            reqRes.Remove(p.Key);
        //        }
        //    });
        //});
        reqRes.Remove(resOrder[0]);

        reqRes.ForEach(r => {
            var x = resOrder.cyclicNext(r, -1);
            game.reactions.Add(new Reaction().From((int)(r.weight/x.weight+0.5), x).To(r));
        });

        game.reactions.Add(new Reaction().To(resOrder.First()));

        game.reactions.RemoveAll(a => game.reactions.Any(b => b != a && b.NotWorse(a)));

        game.reactions = game.reactions.OrderBy(r => r.reagents.Weight()).ToList();
        //game.resources.Add(philosophersStone);
        //philosophersStone.weight = maxWeight;


        return game;
    }
}
