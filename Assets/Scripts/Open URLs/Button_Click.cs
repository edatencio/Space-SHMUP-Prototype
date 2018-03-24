//Source: http://va.lent.in/opening-links-in-a-unity-webgl-project/

using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;
using System.Runtime.InteropServices;


public class Button_Click : MonoBehaviour, IPointerDownHandler
{
     /*************************************************************************************************
     *** Variables
     *************************************************************************************************/
     public string URL;


     /*************************************************************************************************
     *** Methods
     *************************************************************************************************/
     public void Open_URL()
     {
          #if UNITY_WEBGL && !UNITY_EDITOR
               openWindow(URL);
          #else
               Application.OpenURL(URL);
          #endif

     }//public void OpenLinkJSPlugin

     #if UNITY_WEBGL && !UNITY_EDITOR
          [DllImport("__Internal")]
          private static extern void openWindow(string url);
     #endif


     /*************************************************************************************************
     *** Event Handler
     *************************************************************************************************/
     [Serializable]
     public class ButtonPressEvent : UnityEvent { }

     public ButtonPressEvent OnPress = new ButtonPressEvent();

     public void OnPointerDown(PointerEventData eventData)
     {
          OnPress.Invoke();

     }//public void OnPointerDown


}//public class PressHandler
