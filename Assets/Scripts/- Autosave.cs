//Source: https://forum.unity3d.com/threads/we-need-auto-save-feature.483853/

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
 
[InitializeOnLoad]
public class Autosave
{
    static Autosave()
    {
        EditorApplication.playmodeStateChanged += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
            {
                Debug.Log("Auto-saving all open scenes...");
                EditorSceneManager.SaveOpenScenes();
                AssetDatabase.SaveAssets();
				
            }//if
			
        };//EditorApplication.playmodeStateChanged
		
    }//static Autosave
	
}//public Class Autosave

#endif