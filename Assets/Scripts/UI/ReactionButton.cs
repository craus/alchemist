using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class ReactionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public Button button;
    public Slider slider;
    public Color usedColor;
    public Reaction reaction;
    public Transform formula;
    public ResourceIcon resourcePrefab;
    public GameObject arrowPrefab;
    public Manufacture manufacture;

    void OnEnable() {
        GameManager.instance.sliderEventHandler.AddSlider(slider);
    }

    void Start() {
        manufacture = new Manufacture(reaction);
        formula.Children().ForEach(c => Destroy(c.gameObject));
        reaction.reagents.ForEach(r => {
            var resourceIcon = Instantiate(resourcePrefab);
            resourceIcon.resource = r.Key;
            resourceIcon.amount = r.Value;
            resourceIcon.transform.SetParent(formula);
        });
        var arrowIcon = Instantiate(arrowPrefab);
        arrowIcon.transform.SetParent(formula);
        reaction.products.ForEach(r => {
            var resourceIcon = Instantiate(resourcePrefab);
            resourceIcon.resource = r.Key;
            resourceIcon.amount = r.Value;
            resourceIcon.transform.SetParent(formula);
        });
    }

    public bool Doable() {
        return !reaction.reagents.Keys.Any(r => reaction.reagents[r] > GameManager.instance.game.currentResources[r]);
    }

    public void DoReaction() {
        if (!Doable()) {
            return;
        }
        reaction.StartReaction();
        reaction.RetrieveReactionResults();
        GameManager.instance.RefreshResourcesUI();
        reaction.used++;
        if (reaction.used == 1) {
            var colors = button.colors;
            colors.normalColor *= usedColor;
            colors.highlightedColor *= usedColor;
            colors.pressedColor *= usedColor;
            button.colors = colors;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        GameManager.instance.status.text = string.Format("{0:0.0000e0} -> {1:0.0000e0} (x{2:0.0000})", reaction.reagents.Weight(), reaction.products.Weight(), reaction.products.Weight() / reaction.reagents.Weight());
    }

    public void OnPointerExit(PointerEventData eventData) {
        GameManager.instance.status.text = "";
    }

    public void SliderValueChangeCheck() {
        GameManager.instance.sliderEventHandler.OnValueChanged(slider);
        manufacture.isProgress = slider.value > 0f;
    }

    void OnDisable() {
        GameManager.instance.sliderEventHandler.RemoveSlider(slider);
    }
}