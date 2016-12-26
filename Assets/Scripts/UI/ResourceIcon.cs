using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResourceIcon : MonoBehaviour {
    public Resource resource;
    public Image image;
    public Text amount;
    public GameObject amountCircle;

    void Start() {
        image.sprite = resource.image;
        image.color = resource.color;
        amountCircle.SetActive(false);
    }
}
