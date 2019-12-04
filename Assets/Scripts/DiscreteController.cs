using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DiscreteController : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        bool stateChanged = false;
        List<ReactionButton> activeList = GameManager.instance.reactionButtons.FindAll(b => b.slider.value > 0);
        List<ReactionButton> toStartList = activeList.FindAll(b => !b.manufacture.isProgress).OrderByDescending(b => b.slider.value).ToList();
        toStartList.ForEach(b => {
            if (b.Doable()) {
                stateChanged = true;
                b.reaction.StartReaction();
            }
        });
        List<ReactionButton> toCalcFutureList = activeList.FindAll(b => b.manufacture.isProgress);



        if (stateChanged) {
            GameManager.instance.RefreshResourcesUI();
        }
    }

    public void WorldChanged() {
        throw new NotImplementedException();
    }
}
