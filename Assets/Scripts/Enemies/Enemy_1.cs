using UnityEngine;
using System.Collections;


//Enemy_1 extends the Enemy Class
public class Enemy_1 : Enemy
{
     /*************************************************************************************************
     *** Variables
     *************************************************************************************************/
     public float waveFrequency = 2f;   //Seconds for a full sine wave.
     public float waveWidth = 4f;       //Sine wave width in meters.
     public float waveRotY = 45f;

     private float x0 = -12345f;         //The initial X value of pos
     private float birthTime;


     /*************************************************************************************************
     *** Start
     *************************************************************************************************/
     void Start()
     {
          //Set x0 to the initial X position of Enemy_1

          /* This works fine because the position will have already been set by Main.SpawnEnemy()
           * before Start() runs (though Awake() would have been too early).
           * 
           * This is also good because there is no Start() method on Enemy. */

          x0 = pos.x;

          birthTime = Time.time;

     }//void Start


     /*************************************************************************************************
     *** Update
     *************************************************************************************************/
     //void Update()
     //{


     //}//void Update


     /*************************************************************************************************
     *** Methods
     *************************************************************************************************/
     //Override the Move function on Enemy
     public override void Move()
     {
          //Because pos is a property, you cant directly set pos.x
          //so get the pos as an editable Vector.3
          Vector3 tempPos = pos;

          //Theta adjusts based on time
          float age = Time.time - birthTime;
          float theta = Mathf.PI * 2 * age / waveFrequency;
          float sin = Mathf.Sin(theta);
          tempPos.x = x0 + waveWidth * sin;
          pos = tempPos;

          //Rotate a bit about Y
          Vector3 rot = new Vector3(0, sin * waveRotY, 0);
          gameObject.transform.rotation = Quaternion.Euler(rot);

          //Base.Move() still handles the movement down in Y
          base.Move();

     }//public override void Move


}//public class Enemy_1
