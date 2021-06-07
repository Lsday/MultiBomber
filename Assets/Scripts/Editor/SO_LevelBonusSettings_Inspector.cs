using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SO_LevelBonusSettings)), CanEditMultipleObjects]

public class SO_LevelBonusSettings_Inspector : Editor
{
    SO_LevelBonusSettings myTarget;

    SerializedProperty bonusList;
    SerializedObject GetTarget;
    int ListSize;

    private void OnEnable()
    {
        myTarget = (SO_LevelBonusSettings)target;
        bonusList = this.serializedObject.FindProperty("_bonusSettings");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // https://forum.unity.com/threads/display-a-list-class-with-a-custom-editor-script.227847/
        //EditorGUILayout.PropertyField(bonusList, true,null);

        int bonusCount = Mathf.CeilToInt(myTarget.boxesCount * myTarget.bonusPercentage);

        SO_LevelBonusSettings.BonusStock[] results =  myTarget.ComputeBonusList(bonusCount, myTarget.playersCount);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Total: "+ bonusCount.ToString(), GUILayout.Width(100));
        EditorGUILayout.Space();
        int sum = 0;

        for (int i=0;i< results.Length; i++)
        {
            var dataRef = results[i];
            if(dataRef.count > 0)
            {
                sum += dataRef.count;
                EditorGUILayout.LabelField(dataRef.name + " " + dataRef.count.ToString(), GUILayout.Width(100));
            }
            
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Sum: " + sum.ToString(), GUILayout.Width(100));

        if (GUI.changed) { EditorUtility.SetDirty(myTarget); }
    }
}
