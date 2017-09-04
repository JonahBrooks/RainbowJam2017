using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonScript : MonoBehaviour {

	// Launches game
    public void OnClick()
    {
        SceneManager.LoadScene("Game");
    }

    private void Update()
    {
        if(Input.anyKeyDown)
        {
            SceneManager.LoadScene("Game");
        }
    }
}
