using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToMainMenu : MonoBehaviour {
    public void GoToMainMenu() {
        SaveSys saveSys = FindObjectOfType<SaveSys>();
        PlayerControl player = PlayerControl.Instance;
        DungeonGen dungeonGen = FindObjectOfType<DungeonGen>();

        if (saveSys != null && player != null && dungeonGen != null) {
            saveSys.SaveGame(dungeonGen, player);
            //Debug.Log("saved!");
        } else {
            Debug.Log("not saved :(");
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void menuDead() {
        SceneManager.LoadScene(0);
        GameState.gameOver = false;
    }
}
