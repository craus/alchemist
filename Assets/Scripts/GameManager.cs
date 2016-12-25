using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public GameGenerator generator;
    public Game game;

    public void Start() {
        game = generator.CreateGame();
    }
}
