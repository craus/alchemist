using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DiscreteController : MonoBehaviour {
    bool loaded = false;
    bool stateChanged = false;

    List<Manufacture> activeManufactureList;
    Map<Resource, List<ResourceChange>> map;
    long nextTime = DateTime.MaxValue.Ticks;
    long currentTime;
    long stateStartTime;
    ResourceCollection stateResources;
    // Start is called before the first frame update
    void Start() {
        stateChanged = true;


    }

    // Update is called once per frame
    void Update() {
        currentTime = DateTime.UtcNow.Ticks;

        if (!loaded) {
            loaded = true;
            //on load(from disk)
            stateStartTime = currentTime;
            //stateResources = GameManager.instance.game.currentResources;
        }
        stateResources = GameManager.instance.game.currentResources;
        if (currentTime >= nextTime) {
            stateChanged = true;
        }
        List<ReactionButton> activeList = GameManager.instance.reactionButtons.FindAll(b => b.slider.value > 0);
        //TODO: order turn on depending proportion
        List<ReactionButton> toStartList = activeList.FindAll(b => !b.manufacture.isProgress).OrderByDescending(b => b.slider.value).ToList();
        toStartList.ForEach(b => {
            if (b.Doable()) {
                stateChanged = true;
                b.manufacture.StartReaction();
                b.manufacture.startTime = currentTime;
            }
        });
        if (stateChanged) {
            stateChanged = false;



            ReInit(activeList);


            //the same. Assignment is unnecessary
            //GameManager.instance.game.currentResources = stateResources;
            GameManager.instance.RefreshResourcesAfterIdle();
        }
    }

    public void WorldChanged() {
        stateChanged = true;
    }

    private void ReInit(List<ReactionButton> activeList) {
        int counter = 0;
        bool inProgress;
        long nextTime = this.nextTime;
        do {
            inProgress = false;
            activeManufactureList = activeList.Select(b => b.manufacture).Where(m => m.isProgress).ToList();
            map = new Map<Resource, List<ResourceChange>>(() => new List<ResourceChange>());
            activeManufactureList.ForEach((m) => {
                m.reaction.products.ForEach(r => {
                    //-1
                    map[r.Key].Add(new ResourceChange(r.Value, m.EstimatedSpeed(), m.startTime, m));
                });
                m.reaction.reagents.ForEach(r => {
                    //<0!!!!!
                    map[r.Key].Add(new ResourceChange(r.Value, -m.EstimatedSpeed(), m.startTime, m));
                });
            });



            //long nextTime = map.Values.Select(l => nextTimeChange(l)).Min();

            //next reaction finished
            ResourceChange rc = map.Values.Select(list => ResListMinByNexTtime(list)).MinBy(list2 => NextTimeChange(list2));
            //rc !=null to struct
            if (rc.parentManufacture != null) {
                nextTime = NextTimeChange(rc);
                string timeC = new DateTime(currentTime).ToLongTimeString();
                string timeS = new DateTime(nextTime).ToLongTimeString();
                Debug.Log(timeC + "   " + currentTime);
                Debug.Log(timeS + "   " + nextTime);
                if (nextTime < currentTime) {

                    //changeState!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    MoveToNextTime(nextTime);
                    inProgress = true;
                }
                if (rc.parentManufacture.reaction.reagents.Any(r => stateResources[r.Key] + ResourceChange.ListSum(map[r.Key], nextTime) < 0)) {
                    if (nextTime <= currentTime) {
                        inProgress = true;
                        rc.parentManufacture.Stop();
                        /////change resource collection
                        MoveToNextTime(nextTime);
                        activeManufactureList.Remove(rc.parentManufacture);
                    }
                }
            }
            if (counter++ > 1000) {
                throw new Exception("ReInit loop");
            }
        } while (inProgress);
        this.nextTime = nextTime;
    }

    private ResourceChange ResListMinByNexTtime(List<ResourceChange> list) {
        return list.MinBy(rc => NextTimeChange(rc));
    }

    private long NextTimeChange(ResourceChange rc) {
        return (long)(1 / rc.speed) + rc.startTime;
    }

    void MoveToNextTime(long nextTime) {
        //TODO: move
        ChangeResourcesInGame(stateResources, nextTime);
        activeManufactureList.ForEach(m => m.startTime = nextTime);
        activeManufactureList.ForEach(m => m.Rewind(nextTime));
        stateStartTime = currentTime;
    }

    ResourceCollection ChangeResourcesInGame(ResourceCollection rCol, long nextTime) {
        map.ForEach(p => rCol[p.Key] += ResourceChange.ListSum(p.Value, nextTime));
        return rCol;
    }

    struct ResourceChange {

        //count*floor(x*speed+startTime);
        public int count;
        //or decimal?
        public float speed;
        public long startTime;
        public Manufacture parentManufacture;

        public ResourceChange(int count, float speed, long startTime, Manufacture parentManufacture) {
            this.count = count;
            this.speed = speed;
            this.startTime = startTime;
            this.parentManufacture = parentManufacture;
        }

        public int Value(long nextTime) {
            return count * (int)Math.Floor((nextTime - startTime) * speed);
        }

        public static int ListSum(List<ResourceChange> list, long nextTime) {
            return list.Sum(rch => rch.Value(nextTime));
        }
    }
}
