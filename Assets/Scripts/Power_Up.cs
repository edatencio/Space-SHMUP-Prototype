using UnityEngine;
using System.Collections;


public class Power_Up : MonoBehaviour
{
     /*************************************************************************************************
     *** Variables
     *************************************************************************************************/
     //This is an unusual but handy use of Vector2s, X holds a min value and Y a max value
     //for a Random.Range() that will be called later
     public Vector2 rotMinMax = new Vector2(15f, 90f);
     public Vector2 driftMinMax = new Vector2(0.25f, 2f);
     public float lifeTime = 6f;     //Seconds the PowerUp exists
     public float fadeTime = 4f;     //Seconds it will then fade

     [HideInInspector]
     public WeaponType type;        //The type of the PowerUp
     private GameObject cube;        //Reference to the Cube child
     private TextMesh letter;        //Reference to the TextMesh
     private Vector3 rotPerSecond;   //Euler rotation speed
     private float birthTime;


     /*************************************************************************************************
     *** Awake
     *************************************************************************************************/
     void Awake()
     {
          //Find the cube reference
          cube = gameObject.transform.GetChild(0).gameObject;

          //Find the TextMesh
          letter = gameObject.GetComponent<TextMesh>();

          /* Note: Random.onUnitSphere gives you a vector point that is somewhere
          * on the surface of the sphere with a radius of 1m around the origin */

          //Set a random velocity
          Vector3 vel = Random.onUnitSphere;      //Get random XYZ velocity

          vel.z = 0;          //Flatten the vel to the XY plane
          vel.Normalize();    //Make the length of the vel 1

          //Normalizing a Vector3 makes it length 1m
          vel *= Random.Range(driftMinMax.x, driftMinMax.y);

          gameObject.GetComponent<Rigidbody>().velocity = vel;

          //Set the rotation of this GameObject to R:[0,0,0]
          //Quaternion.identity is equal to no rotation
          gameObject.transform.rotation = Quaternion.identity;

          //Set up the rotPerSecond for the Cube child using rotMinMax x & y
          rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y), 
                         Random.Range(rotMinMax.x, rotMinMax.y), 
                         Random.Range(rotMinMax.x, rotMinMax.y));

          //CheckOffscreen() every 2 seconds
          InvokeRepeating("CheckOffscreen", 2f, 2f);

          birthTime = Time.time;

     }//void Awake


     /*************************************************************************************************
	*** Update
	*************************************************************************************************/
     void Update()
     {
          //Manually rotate the Cube child every Update()
          //Multiplying it bu Time.time causes the rotation to be time based
          cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

          //Fade out the Power_Up over time. Given the default values, a Power_Up will exists
          //for 10 seconds and then fade out over 4 seconds
          float u = (Time.time - (birthTime + lifeTime)) / fadeTime;

          //For lifeTime seconds, u will be <=0. Then it will transition to 1 over fadeTime seconds
          //If u >= 1, destroy this PowerUp
          if (u >= 1)
          {
               Color c = cube.GetComponent<Renderer>().material.color;
               c.a = 1f - u;
               cube.GetComponent<Renderer>().material.color = c;

               //Fade the letter too, just not as much
               c = letter.color;
               c.a = 1f - (u * 0.5f);
               letter.color = c;

          }//if

     }//void Update


     /*************************************************************************************************
	*** Methods
	*************************************************************************************************/
     //This SetType() differs from those on Weapon and Projectile
     public void SetType(WeaponType wt)
     {
          //Grab the WeaponDefinition from Main
          WeaponDefinition def = Main.GetWeaponDefinition(wt);

          //Set the coor of the Cube child
          cube.GetComponent<Renderer>().material.color = def.color;

          //letter.color = def.color;   //We could colorize the letter too
          letter.text = def.letter;     //Set the letter that is shown
          type = wt;                    //Finally actually set the type

     }//public void SetType


     public void AbsorbedBy(GameObject target)
     {
          //This function is called by the Hero class when a PowerUp is collected
          //We could tween into the target and shrink in size, but for now just destroy this gameObject
          Destroy(gameObject);

     }//public void AbsorbedBy


     void CheckOffscreen()
     {
          //If the PowerUp has drifted entirely off screen...
          if (Utils.ScreenBoundsCheck(cube.GetComponent<Collider>().bounds, BoundsTest.offScreen) != Vector3.zero)
               //...Then destroy this gameObject
               Destroy(gameObject);

     }//void CheckOffscreen


}//public class Power_Up
