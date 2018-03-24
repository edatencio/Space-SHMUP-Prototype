using UnityEngine;
using System.Collections;


public class Shield : MonoBehaviour
{
     /*************************************************************************************************
     *** Variables
     *************************************************************************************************/
     public float rotationsPerSecond = 0.1f;

     private int levelShown = 0;


     /*************************************************************************************************
     *** Start
     *************************************************************************************************/
     //void Start()
     //{


     //}//void Start


     /*************************************************************************************************
     *** Update
     *************************************************************************************************/
     void Update()
     {
          //Read the current shield level from the Hero Singleton
          int curLevel = Mathf.FloorToInt(Hero.S.shieldLevel);

          //If this is different from levelShown...
          if (curLevel != levelShown)
          {
               levelShown = curLevel;
               Material mat = gameObject.GetComponent<Renderer>().material;

               //Adjust the texture offset to show different shield level
               mat.mainTextureOffset = new Vector2(0.2f * levelShown, 0);

          }//if

          //Rotate the shield a bit every second
          float rZ = (rotationsPerSecond * Time.time * 360) % 360f;
          gameObject.transform.rotation = Quaternion.Euler(0, 0, rZ);

     }//void Update


}//public class Shield