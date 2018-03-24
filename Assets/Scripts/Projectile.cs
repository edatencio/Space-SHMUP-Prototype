using UnityEngine;
using System.Collections;


public class Projectile : MonoBehaviour
{
     /*************************************************************************************************
     *** Variables
     *************************************************************************************************/
     private WeaponType _type;


     /*************************************************************************************************
     *** Awake
     *************************************************************************************************/
     void Awake()
     {
          //Test to see whether this has passed offscreen every 2 seconds
          InvokeRepeating("CheckOffscreen", 2f, 2f);

     }//void Awake


     /*************************************************************************************************
     *** Update
     *************************************************************************************************/
     //void Update()
     //{


     //}//void Update


     /*************************************************************************************************
     *** Methods
     *************************************************************************************************/
     public void SetType(WeaponType eType)
     {
          //Set the type
          _type = eType;
          WeaponDefinition def = Main.GetWeaponDefinition(_type);
          gameObject.GetComponent<Renderer>().material.color = def.projectileColor;

     }//public void SetType


     void CheckOffscreen()
     {
          if (Utils.ScreenBoundsCheck(gameObject.GetComponent<BoxCollider>().bounds, BoundsTest.offScreen) != Vector3.zero)
               Destroy(gameObject);

     }//void CheckOffscreen


     /*************************************************************************************************
     *** Properties
     *************************************************************************************************/
     //This public property masks the field _type and takes action when it is set
     public WeaponType type
     {
          get { return (_type); }
          set { SetType(value); }

     }//public WeaponType type


}//public class Projectile