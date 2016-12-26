using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public GameGenerator generator;
    public Game game;
    public Transform reactions;
    public ReactionButton reactionPrefab;

    public void Start() {
        game = generator.CreateGame();
        reactions.Children().ForEach(c => Destroy(c.gameObject));
        game.reactions.ForEach(r => {
            var reaction = Instantiate(reactionPrefab);
            reaction.transform.SetParent(reactions);
            reaction.reaction = r;
        });
    }
}
