using UnityEngine;
using UnityEngine.SceneManagement;


public class Main_Menu : MonoBehaviour
{
     /*************************************************************************************************
     *** Variables
     *************************************************************************************************/
     public string gameSceneName;


     /*************************************************************************************************
     *** Start
     *************************************************************************************************/
     void Start()
     {
          Time.timeScale = 1f;

     }//void Start


     /*************************************************************************************************
     *** Update
     *************************************************************************************************/
     void Update ()
     {
          if (Input.GetButtonDown("Submit"))
               Start_Game();

          if (Input.GetButtonDown("Cancel"))
               Exit_Game();
          	
     }//void Update

     
     /*************************************************************************************************
     *** Methods
     *************************************************************************************************/
     public void Start_Game()
     {
          SceneManager.LoadScene(gameSceneName);

     }//public void Start_Game


     public void Exit_Game()
     {
          Application.Quit();
          
     }//public void Exit_Game
     
     
}//public class UI_Menu_Buttons
