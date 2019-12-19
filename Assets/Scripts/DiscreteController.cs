﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DiscreteController : MonoBehaviour {
    bool loaded = false;
    bool stateChanged = false;

    List<Manufacture> activeManufactureList;
    //Map<Resource, List<ResourceChange>> map;
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
            OnManufactureEventResourceCollectionEditor editor = new OnManufactureEventResourceCollectionEditor(GameManager.instance.game.currentResources);
            GameManager.instance.reactionButtons.ForEach(rb => rb.manufacture.manufactureListener = editor);
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
                b.manufacture.StartReaction(currentTime);
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

            //init
            //map = new Map<Resource, List<ResourceChange>>(() => new List<ResourceChange>());

            //activeManufactureList.ForEach((m) => {
            //    m.reaction.products.ForEach(r => {
            //        //-1
            //        map[r.Key].Add(new ResourceChange(r.Value, m.EstimatedSpeed(), m.startTime, m));
            //    });
            //    m.reaction.reagents.ForEach(r => {
            //        //<0!!!!!
            //        map[r.Key].Add(new ResourceChange(r.Value, -m.EstimatedSpeed(), m.startTime, m));
            //    });
            //});



            //long nextTime = map.Values.Select(l => nextTimeChange(l)).Min();

            //next reaction finished
            Manufacture mf = activeManufactureList.MinBy(m => m.TimeToNextReaction());
            //ResourceChange rc = map.Values.Select(list => ResListMinByNextTime(list)).MinBy(list2 => NextTimeChange(list2));

            //rc !=null to struct
            if (mf != null) {
                nextTime = mf.TimeToNextReaction();
                string timeC = new DateTime(currentTime).ToLongTimeString();
                string timeS = new DateTime(nextTime).ToLongTimeString();
                Debug.Log("ct" + timeC + "   " + currentTime);
                Debug.Log("nt" + timeS + "   " + nextTime);
                if (nextTime < currentTime) {

                    //changeState!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    activeManufactureList.ForEach(m => m.Rewind(nextTime));

                    if (mf.reaction.reagents.Any(r => stateResources[r.Key] < 0)) {
                            mf.Stop();
                            activeManufactureList.Remove(mf);
                    }

                    inProgress = true;
                }

            }
            if (counter++ > 1000) {
                throw new Exception("ReInit loop");
            }
        } while (inProgress);
        Debug.Log("LOOP FINISHED");
        this.nextTime = nextTime;
    }

    //private ResourceChange ResListMinByNextTime(List<ResourceChange> list) {
    //    return list.MinBy(rc => NextTimeChange(rc));
    //}

    //private long NextTimeChange(ResourceChange rc) {
    //    Debug.Log("NextTimeIncrement" + (long)(rc.speed));
    //    Debug.Log("NextTimeChange" + ((long)(rc.speed) + rc.startTime));
    //    return (long)(rc.speed) + rc.startTime;
    //}

    //void MoveToNextTime(long nextTime) {
    //    //TODO: move
    //    ChangeResourcesInGame(stateResources, nextTime);
    //    activeManufactureList.ForEach(m => m.startTime = nextTime);
    //    activeManufactureList.ForEach(m => m.Rewind(nextTime));
    //    stateStartTime = currentTime;
    //}

    //ResourceCollection ChangeResourcesInGame(ResourceCollection rCol, long nextTime) {
    //    map.ForEach(p => rCol[p.Key] += ResourceChange.ListSum(p.Value, nextTime));
    //    return rCol;
    //}

    //struct ResourceChange {

    //    //count*floor(x*speed+startTime);
    //    public int count;
    //    //or decimal?
    //    public float speed;
    //    public long startTime;
    //    public Manufacture parentManufacture;

    //    public ResourceChange(int count, float speed, long startTime, Manufacture parentManufacture) {
    //        this.count = count;
    //        this.speed = speed;
    //        this.startTime = startTime;
    //        this.parentManufacture = parentManufacture;
    //    }

    //    public int Value(long nextTime) {
    //        return count * (int)Math.Floor((nextTime - startTime) / speed);
    //    }

    //    public static int ListSum(List<ResourceChange> list, long nextTime) {
    //        return list.Sum(rch => rch.Value(nextTime));
    //    }
    //}
}
