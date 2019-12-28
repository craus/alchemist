#define TURN_BASED1

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DiscreteController : MonoBehaviour {

    public static readonly long NEVER = DateTime.MaxValue.Ticks;
    bool loaded = false;
    bool stateChanged = false;
    bool speedChanged = false;

    //Map<Resource, List<ResourceChange>> map;
    long nextTime = NEVER;
    long currentTime;
    long stateStartTime;
    ResourceCollection stateResources;
    bool resourcesChanged;

    // Start is called before the first frame update
    void Start() {
        stateChanged = true;
    }

    private List<ReactionButton> GetActiveButtonsList() {
        return GameManager.instance.reactionButtons.FindAll(b => b.slider.value > 0);
    }

    // Update is called once per frame
    void Update() {
        resourcesChanged = false;
        currentTime = DateTime.UtcNow.Ticks;

        if (!loaded) {
            loaded = true;
            //on load(from disk)
            stateStartTime = currentTime;
            OnManufactureEventResourceCollectionEditor editor = new OnManufactureEventResourceCollectionEditor(GameManager.instance.game.currentResources);
            GameManager.instance.reactionButtons.ForEach(rb => rb.manufacture.manufactureListener = editor);
            editor.ActionPerformed = () => {
                resourcesChanged = true;
            };
            //stateResources = GameManager.instance.game.currentResources;
        }
        stateResources = GameManager.instance.game.currentResources;
        if (currentTime >= nextTime) {
            stateChanged = true;
        }
        if (speedChanged) {
            RecalculteEffortOnSpeedChange();
        }
        List<ReactionButton> activeList = GetActiveButtonsList();
        //TODO: order turn on depending proportion
        List<ReactionButton> toStartList = activeList.FindAll(b => !b.manufacture.isProgress).OrderByDescending(b => b.slider.value).ToList();
        toStartList.ForEach(b => {
            if (b.Doable()) {
                stateChanged = true;
                b.manufacture.StartReaction(currentTime);
            }
        });
#if (TURN_BASED)
        if (stateChanged) {
            stateChanged = false;
#endif

            if (activeList.Count > 0) {
                CalculateProgress();
            } else {
                nextTime = NEVER;
            }


            //the same. Assignment is unnecessary
            //GameManager.instance.game.currentResources = stateResources;
            GameManager.instance.RefreshOnModelChange(resourcesChanged);
#if (TURN_BASED)
        }
#endif
    }
    private void RecalculteEffortOnSpeedChange() {
        speedChanged = false;
        GameManager.instance.reactionButtons.ForEach(rb => rb.RecalculteEffort());
    }
    public void WorldChanged() {
        stateChanged = true;
        speedChanged = true;
    }

    private void CalculateProgress() {
        int counter = 0;
        bool inProgress;
        long nextTime = this.nextTime;
        do {
            inProgress = false;
            List<Manufacture> activeManufactureList = GameManager.instance.reactionButtons.Select(b => b.manufacture).Where(m => m.isProgress).ToList();
            if (activeManufactureList.Count == 0) {
                //slider moved, but no reagents for reaction
                //enabledManufactures.ForEach(m => m.Stop());
                nextTime = NEVER;
                break;
            }


            //next reaction finished
            Manufacture mf = activeManufactureList.MinBy(m => m.TimeToNextReaction());

            if (mf != null) {
                nextTime = mf.TimeToNextReaction();
#if (DEBUG_OUT)
                string timeC = new DateTime(currentTime).ToLongTimeString();
                string timeS = new DateTime(nextTime).ToLongTimeString();
                Debug.Log("ct" + timeC + "   " + currentTime);
                Debug.Log("nt" + timeS + "   " + nextTime);
#endif
                if (nextTime < currentTime) {
                    bool speedChanged = false;
                    //changeState!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    activeManufactureList.ForEach(m => {
                        m.Rewind(nextTime);
                        if (m.reaction.reagents.Any(r => stateResources[r.Key] < 0)) {
                            m.Stop();
                            speedChanged = true;
                        }
                    });
                    if (speedChanged) {
                        RecalculteEffortOnSpeedChange();
                    }
                    inProgress = true;
                }
#if (!TURN_BASED)
                else {
                    activeManufactureList.ForEach(m => {
                        m.Rewind(currentTime);
                    });
                }
#endif

            } else {
                throw new Exception("WTF!!!");
            }

            if (counter++ > 1000) {
                throw new Exception("ReInit loop");
            }
        } while (inProgress);
#if (DEBUG_OUT)
        Debug.Log("LOOP FINISHED");
#endif
        this.nextTime = nextTime;
    }

}
