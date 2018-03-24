using UnityEngine;
using System.Collections;


//This is an enum of the various possible weapo types
//It also includes a "shield" type to allow a shield power-up
//Items marked [NI] below are Not Implemented in this book
public enum WeaponType
{
     none,           //The default / no weapon
     blaster,   //A simple blaster
     spread,    //Two shots simultaneously
     phaser,    //Shots that move in waves [NI]
     missile,   //Homing missiles [NI]
     laser,     //Damage over time [NI]
     shield	 //Raise shieldLevel

}//public enum WeaponType


/* The WeaponDefinition class allows you to set the properties of a specific weapon in the Inspector.
 * Main has an array of WeaponDefinitions that makes this possible.
 * [System.Serializable] tells Unity to try to view WeaponDefinition in the inspector pane.
 * It doesn't work for everything, but it will work for simple classes like this. */
[System.Serializable]
public class WeaponDefinition
{
     //Note: Weapon prefabs, colors and so on are set in the class Main.

     public WeaponType type = WeaponType.none;
     public string letter;                  //The letter to show on the power-up
     public Color color = Color.white;      //Color of Collar and power-up
     public GameObject projectilePrefab;     //Prefab for projectiles
     public Color projectileColor = Color.white;
     public float damageOnHit = 0f;           //Amount of damage caused
     public float continuousDamage = 0f;      //Damage per second (Laser)
     public float delayBetweenShots = 0f;
     public float velocity = 20f;

}//public class WeaponDefinition


public class Weapon : MonoBehaviour
{
     /*************************************************************************************************
     *** Variables
     *************************************************************************************************/
     static public Transform PROJECTILE_ANCHOR;

     [SerializeField]
     private WeaponType _type = WeaponType.none;
     private WeaponDefinition def;
     private GameObject collar;
     private float lastShot; //Time last shot was fired


     /*************************************************************************************************
     *** Awake
     *************************************************************************************************/
     void Awake()
     {
          collar = transform.Find("Collar").gameObject;

     }//void Awake


     /*************************************************************************************************
     *** Start
     *************************************************************************************************/
     void Start()
     {
          //Call SetType() properly for the default _type
          SetType(_type);

          if (PROJECTILE_ANCHOR == null)
          {
               GameObject go = new GameObject("_Projectile_Anchor");
               PROJECTILE_ANCHOR = go.transform;

          }//if

          //Find the fireDelegate of the parent
          GameObject parentGO = gameObject.transform.transform.parent.gameObject;
          if (parentGO.tag == "Hero")
               Hero.S.fireDelegate += Fire;

     }//void Start


     /*************************************************************************************************
     *** Update
     *************************************************************************************************/
     //void Update ()
     //{


     //}//void Update


     /*************************************************************************************************
     *** Properties
     *************************************************************************************************/
     public WeaponType type
     {
          get { return (_type); }
          set { SetType(value); }

     }//public WeaponType type


     /*************************************************************************************************
     *** Methods
     *************************************************************************************************/
     public void SetType(WeaponType wt)
     {
          _type = wt;
          if (type == WeaponType.none)
          {
               gameObject.SetActive(false);
               return;

          }
          else
          {
               gameObject.SetActive(true);

          }//if else

          def = Main.GetWeaponDefinition(_type);
          collar.GetComponent<Renderer>().material.color = def.color;

          //You can always fire inmediately after _type is set
          lastShot = 0;

     }//public void SetType


     public void Fire()
     {
          //If this gameObject is inactive, return
          if (!gameObject.activeInHierarchy)
               return;

          //If it hasnt been enough time between shots, return
          if (Time.time - lastShot < def.delayBetweenShots)
               return;

          Projectile p;
          switch (type)
          {
               case WeaponType.blaster:
                    p = MakeProjectile();
                    p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;
                    break;

               case WeaponType.spread:
                    p = MakeProjectile();
                    p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;

                    p = MakeProjectile();
                    p.GetComponent<Rigidbody>().velocity = new Vector3(-0.2f, 0.9f, 0f) * def.velocity;

                    p = MakeProjectile();
                    p.GetComponent<Rigidbody>().velocity = new Vector3(0.2f, 0.9f, 0f) * def.velocity;
                    break;

          }//switch

     }//public void Fire


     public Projectile MakeProjectile()
     {
          GameObject go = Instantiate(def.projectilePrefab) as GameObject;

          if (gameObject.transform.parent.tag == "Hero")
          {
               go.tag = "Projectile_Hero";
               go.layer = LayerMask.NameToLayer("Projectile_Hero");

          }
          else
          {
               go.tag = "Projectile_Enemy";
               go.layer = LayerMask.NameToLayer("Projectile_Enemy");

          }//if else

          go.transform.position = collar.transform.position;
          go.transform.parent = PROJECTILE_ANCHOR;

          Projectile p = go.GetComponent<Projectile>();
          p.type = type;
          lastShot = Time.time;
          return (p);

     }//public Projectile MakeProjectile


}//public class Weapon