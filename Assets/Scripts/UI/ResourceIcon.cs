﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResourceIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public Resource resource;
    public Image image;
    public Text amountText;
    public GameObject amountCircle;

    public int amount;

    void Start() {
        image.sprite = resource.image;
        image.color = resource.color;
        image.material = resource.material;
        UpdateAmountCircle();
        if (amount == 0) {
            Destroy(gameObject);
        }
    }

    public void UpdateAmountCircle() {
        amountCircle.SetActive(amount > 1);
        amountText.text = amount.ToString();
    }

    //public void Decrement() {
    //    --amount;
    //    amountText.text = amount.ToString();
    //    if (amount < 2) {
    //        remove
    //    }
    //}

    public void OnPointerEnter(PointerEventData eventData) {
        GameManager.instance.status.text = string.Format("{0} {1:0.0000e0}", resource.name, resource.weight);
    }

    public void OnPointerExit(PointerEventData eventData) {
        GameManager.instance.status.text = "";
    }
}
