using UnityEngine;
using UnityEngine.SceneManagement;


public class Pause_Menu : MonoBehaviour
{
     /*************************************************************************************************
     *** Variables
     *************************************************************************************************/
     public string mainMenuScene = "Main Menu";

     private GameObject pauseMenu;


     /*************************************************************************************************
     *** Start
     *************************************************************************************************/
     void Start()
     {
          pauseMenu = gameObject.transform.GetChild(0).gameObject;
          IsPaused = false;
          pauseMenu.SetActive(false);

     }//void Start


     /*************************************************************************************************
     *** Update
     *************************************************************************************************/
     void Update()
     {
          if (Input.GetButtonDown("Cancel"))
               Pause_Resume_Game();

     }//void Update


     /*************************************************************************************************
     *** Properties
     *************************************************************************************************/
     public bool IsPaused
     {
          get; set;

     }//public bool IsPaused


     /*************************************************************************************************
     *** Methods
     *************************************************************************************************/
     public void Pause_Resume_Game()
     {
          IsPaused = !IsPaused;
          pauseMenu.SetActive(IsPaused);

          if (IsPaused) Time.timeScale = 0f;
          else if (!IsPaused) Time.timeScale = 1f;

     }//public void Pause_Game


     public void Quit_Game()
     {
          Time.timeScale = 1f;
          SceneManager.LoadScene(mainMenuScene);

     }//public void Quit_Game


}//public class Pause_Menu
