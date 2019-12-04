using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;


public class SliderEventHandler {

    const int SLIDER_INT_MULTIPLIER = 100000;
    const int SLIDER_INT_MAX_VALUE = SLIDER_INT_MULTIPLIER;

    public List<Slider> Sliders = new List<Slider>();

    bool processing;

    public void AddSlider(Slider slider) {
        Sliders.Add(slider);
    }

    public void RemoveSlider(Slider slider) {
        Sliders.Remove(slider);
    }

    private int sliderIntValue(Slider slider) {
        return (int)(slider.value * SLIDER_INT_MULTIPLIER);
    }

    public void OnValueChanged(Slider current) {
        if (processing) {
            return;
        }
        processing = true;
        for (int summary = Sliders.Sum(s => sliderIntValue(s)); summary > SLIDER_INT_MAX_VALUE; summary = Sliders.Sum(s => sliderIntValue(s))) {
            List<Slider> list = Sliders.FindAll(s => sliderIntValue(s) > 0);
            list.Remove(current);
            int min = sliderIntValue(list.OrderBy(s => sliderIntValue(s)).First());
            int reduce = Math.Min((summary - SLIDER_INT_MAX_VALUE) / list.Count, min);
            if (reduce == 0) {
                list[0].value = Math.Max(0, list[0].value - (summary - SLIDER_INT_MAX_VALUE) / (float)SLIDER_INT_MAX_VALUE);
                //Debug.Log("reduce 0");
            } else {
                list.ForEach(s => s.value -= (float)reduce / SLIDER_INT_MULTIPLIER);
            }
        }
        processing = false;
    }


}