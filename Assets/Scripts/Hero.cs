using UnityEngine;
using System.Collections;


public class Hero : MonoBehaviour
{
     /*************************************************************************************************
     *** Variables
     *************************************************************************************************/
     static public Hero S; //Singleton

     public float speed = 30f;
     public float rollMult = 45;
     public float pitchMult = 30;
     public float gameRestartDelay = 2f;
     public GameObject textGameOver;
     public Weapon[] weapons;           //Weapons fields

     private float _shieldLevel = 1;    //Ship status information
     private Bounds bounds;

     //Declare a new delegate type WeaponFireDelegate
     public delegate void WeaponFireDelegate();

     //Create a WeaponFireDelegate field named fireDelegate
     public WeaponFireDelegate fireDelegate;


     /*************************************************************************************************
     *** Awake
     *************************************************************************************************/
     void Awake()
     {
          S = this;
          bounds = Utils.CombineBoundsOfChildren(gameObject);
          textGameOver.SetActive(false);

     }//void Awake


     /*************************************************************************************************
     *** Start
     *************************************************************************************************/
     void Start()
     {
          //Reset the weapons to start Hero with 1 blaster
          ClearWeapons();
          weapons[0].SetType(WeaponType.blaster);

     }//void Start


     /*************************************************************************************************
     *** Update
     *************************************************************************************************/
     void Update()
     {
          //Pull information from the Imput class
          float xAxis = Input.GetAxis("Horizontal");
          float yAxis = Input.GetAxis("Vertical");

          //Change transform.position based on the axes
          Vector3 pos = gameObject.transform.position;
          pos.x += xAxis * speed * Time.deltaTime;
          pos.y += yAxis * speed * Time.deltaTime;
          gameObject.transform.position = pos;

          bounds.center = gameObject.transform.position;

          //Keep the ship constrained to the screen bounds
          Vector3 off = Utils.ScreenBoundsCheck(bounds, BoundsTest.onScreen);
          if (off != Vector3.zero)
          {
               pos -= off;
               gameObject.transform.position = pos;

          }//if

          //Rotate the ship to make it feel more dynamic
          gameObject.transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * -rollMult, 0);

          //Use the fireDelegate to fire Weapons
          //First, make sure the Axis("Jump") button is pressed
          //Then ensure that fireDelegate isnt null to avoid an error
          if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
               fireDelegate();

     }//void Update


     /*************************************************************************************************
     *** OnTriggerEnter
     *************************************************************************************************/
     //This variable holds a reference to the last triggering GameObject
     private GameObject lastTriggerGo = null;

     void OnTriggerEnter(Collider col)
     {
          //Find the tag of col.gameObject or its parent GameObjects
          GameObject go = Utils.FindTaggedParent(col.gameObject);

          //If there is a parent with tag
          if (go != null)
          {
               //Make sure its not the same triggering go as last time
               if (go == lastTriggerGo)
               {
                    return;

               }//if

               lastTriggerGo = go;

               if (go.tag == "Enemy")
               {
                    //If the shield was triggered by an enemy decrease the level of the shield by 1
                    shieldLevel--;

                    //Destroy the enemy
                    Destroy(go);

               }
               else if (go.tag == "Power_Up")
               {
                    //If the shield was triggeres by a PowerUp
                    AbsorbPowerUp(go);

               }//if else

          }
          else
          {
               //Otherwise announce the original col.gameObject
               Debug.Log("Triggered 2: " + go.gameObject.name);

          }//if else

     }//void OnTriggerEnter


     /*************************************************************************************************
     *** Properties
     *************************************************************************************************/
     public float shieldLevel
     {
          get { return (_shieldLevel); }
          set
          {
               _shieldLevel = Mathf.Min(value, 4);

               //If the shield is going to be set to less than zero
               if (value < 0)
               {
                    //Tell Main.S to restart the game after a delay
                    Main.S.DelayedRestart(gameRestartDelay);

                    //Show the GameOver text
                    textGameOver.SetActive(true);

                    Destroy(gameObject);

               }//if

          }//set

     }//public float shieldLevel


     /*************************************************************************************************
     *** Methods
     *************************************************************************************************/
     public void AbsorbPowerUp(GameObject go)
     {
          Power_Up pu = go.GetComponent<Power_Up>();

          switch (pu.type)
          {
               case WeaponType.shield:
                    shieldLevel++;
                    break;

               default: //If its any Weapon PowerUp
                    //Check the current weapon type
                    if (pu.type == weapons[0].type)
                    {
                         //Then increase the number of weapons of this type
                         Weapon w = GetEmptyWeaponSlot();   //Find an available weapon

                         if (w != null)
                              //Set it to pu.type
                              w.SetType(pu.type);

                    }
                    else
                    {
                         //If this is a different weapon
                         ClearWeapons();
                         weapons[0].SetType(pu.type);

                    }//if else

                    break;

          }//switch

          pu.AbsorbedBy(gameObject);

     }//public void AbsorbPowerUp


     Weapon GetEmptyWeaponSlot()
     {
          for (int i = 0; i < weapons.Length; i++)
               if (weapons[i].type == WeaponType.none)
                    return (weapons[i]);

          return (null);

     }//Weapon GetEmptyWeaponSlot


     void ClearWeapons()
     {
          foreach (Weapon w in weapons)
               w.SetType(WeaponType.none);

     }//void ClearWeapons


}//public class Hero