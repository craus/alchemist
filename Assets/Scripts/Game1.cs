using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

public class Game1 : AbstractGameGenerator {
    public GameGenerator baseGenerator;

    public override Game CreateGame() {
        var game = new Game();
        var r = new List<Resource>();
        for (int i = 0; i < 40; i++) {
            r.Add(baseGenerator.CreateResource());
            game.resources.Add(r[i]);
        }
        game.reactions.Add(new Reaction().To(r[0]));
        game.reactions.Add(new Reaction().From(2, r[0]).To(r[1]));
        game.reactions.Add(new Reaction().From(2, r[1]).To(r[2]));

        return game;
    }
}
