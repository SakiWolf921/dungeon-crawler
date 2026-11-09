using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuControl : MonoBehaviour {

    public void ButtonHandlerPlay () {
        SceneManager.LoadSceneAsync(1);

    }

    public void ButtonHandlerLoad() {
        GameState.loadGame = true;
        SceneManager.LoadSceneAsync(1);
    }
}
