/**
 * cesar: adapted from http://unifycommunity.com/wiki/index.php?title=Expose_properties_in_inspector
 */
using UnityEngine;
using System.Collections;
using UnityEditor;

//public class AZQuadGridEditor : MonoBehaviour


[CustomEditor( typeof( AZQuadGrid ) )]
public class AZQuadGridEditor : Editor {


    AZQuadGrid m_Instance;
    PropertyField[] m_fields;
   
   
    public void OnEnable()
    {
        m_Instance = target as AZQuadGrid;
        m_fields = ExposeProperties.GetProperties( m_Instance );
    }
   
    public override void OnInspectorGUI () {

        if ( m_Instance == null )
            return;
       
        this.DrawDefaultInspector();
       
        ExposeProperties.Expose( m_fields );
		
		if (GUI.changed) {  EditorUtility.SetDirty (target); }  
       
    }
}
