using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;


public class SliderEventHandler {

    public List<Slider> Sliders = new List<Slider>();

    public float Summary;

    public void AddSlider(Slider slider) {
        Sliders.Add(slider);
    }

    public void RemoveSlider(Slider slider) {
        Sliders.Remove(slider);
    }

    public void OnValueChanged(Slider current) {
        onSlidersSettingFinished();
    }

    public void onSlidersSettingFinished() {
        Summary = Sliders.Sum(s => s.value);
        GameManager.instance.GameStateChanged();
    }
}