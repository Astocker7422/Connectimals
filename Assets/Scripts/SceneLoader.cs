using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Controls which scene is loaded
//Attached to empty object
public class SceneLoader : MonoBehaviour
{
    //Load the main scene
    public void LoadMainScene()
    {
        //CHANGE THIS TO MATCH NAME OF MAIN SCENE
        SceneManager.LoadScene("SampleScene");
    }

    //Load the main menu scene
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    //Exit the game
    public void Quit()
    {
        Application.Quit();
    }
}
