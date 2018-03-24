using UnityEngine;
using System.Collections;


public class Enemy : MonoBehaviour
{
     /*************************************************************************************************
     *** Variables
     *************************************************************************************************/
     public float speed = 10f;                    //The speed in m/s
     public float fireRate = 0.3f;                //Seconds/shot (Unused)
     public float health = 10f;
     public int score = 100;                      //Points earned for destroying this
     public int showDamageForFrames = 2;          //# frames to show damage
     public float powerUpDropChance = 1f;         //Chance to drop a PowerUp

     protected Bounds bounds;                       //The bounds of this and its children
     protected Vector3 boundsCenterOffset;          //Dist of bounds.center from position
     protected Color[] originalColors;
     protected Material[] materials;                //All the materials of this and its children
     protected int remainingDamageFrames = 0;       //Damage frames left


     /*************************************************************************************************
     *** Awake
     *************************************************************************************************/
     void Awake()
     {
          materials = Utils.GetAllMaterials(gameObject);
          originalColors = new Color[materials.Length];

          for (int i = 0; i < materials.Length; i++)
               originalColors[i] = materials[i].color;

          InvokeRepeating("CheckOffscreen", 0f, 2f);

     }//void Awake


     /*************************************************************************************************
     *** Update
     *************************************************************************************************/
     void Update()
     {
          Move();

          if (remainingDamageFrames > 0)
          {
               remainingDamageFrames--;
               if (remainingDamageFrames == 0)
                    UnShowDamage();

          }//if

     }//void Update


     /*************************************************************************************************
     *** OnCollisionEnter
     *************************************************************************************************/
     void OnCollisionEnter(Collision col)
     {
          GameObject other = col.gameObject;

          switch (other.tag)
          {
               case "Projectile_Hero":
                    Projectile p = other.GetComponent<Projectile>();

                    //Enemies dont take damage unless they are onscreen
                    //This stops the player from shooting them before they are visible
                    bounds.center = gameObject.transform.position + boundsCenterOffset;

                    if (bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen) != Vector3.zero)
                    {
                         Destroy(other);
                         break;

                    }//if

                    //Hurt this Enemy
                    ShowDamage();

                    //Get the damage amount from the Projectile.type & Main.W_DEFS
                    health -= Main.W_DEFS[p.type].damageOnHit;
                    if (health <= 0)
                    {
                         //Tell the Main singleton that this ship has been destroyed
                         Main.S.ShipDestroyed(this);

                         //Destroy this Enemy
                         Destroy(gameObject);

                    }//if

                    Destroy(other);
                    break;

          }//switch

     }//void OnCollisionEnter


     /*************************************************************************************************
     *** Methods
     *************************************************************************************************/
     public virtual void Move()
     {
          Vector3 tempPos = pos;
          tempPos.y -= speed * Time.deltaTime;
          pos = tempPos;

     }//public virtual void Move


     void CheckOffscreen()
     {
          //If bounds are still their default value
          if (bounds.size == Vector3.zero)
          {
               //Then set them
               bounds = Utils.CombineBoundsOfChildren(gameObject);

               //Also find the diff between bounds.center and transform.position
               boundsCenterOffset = bounds.center - gameObject.transform.position;

          }//if

          //Every time, update the bounds to the current position
          bounds.center = gameObject.transform.position + boundsCenterOffset;

          //Check to see whether the bounds are completely offscreen
          Vector3 off = Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen);
          if (off != Vector3.zero)
          {
               //If this enemy has gone off the bottom edge of the screen
               if (off.y < 0)
                    //Then destroy it
                    Destroy(gameObject);

          }//if

     }//void CheckOffscreen


     void ShowDamage()
     {
          foreach (Material m in materials)
               m.color = Color.red;

          remainingDamageFrames = showDamageForFrames;

     }//void ShowDamage


     void UnShowDamage()
     {
          for (int i = 0; i < materials.Length; i++)
               materials[i].color = originalColors[i];

     }//void UnShowDamage


     /*************************************************************************************************
     *** Properties
     *************************************************************************************************/
     public Vector3 pos
     {
          get { return (gameObject.transform.position); }
          set { gameObject.transform.position = value; }

     }//public Vector3 pos


}//public class Enemy