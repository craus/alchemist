using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExitScript : MonoBehaviour {

    void Update() {
        if (Input.GetKey(KeyCode.Escape)) {
            Debug.Log("start quit");
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
               Application.Quit();
            #endif
        }
    }
}