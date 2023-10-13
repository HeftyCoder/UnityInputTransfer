using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

[CustomPropertyDrawer(typeof(Input))]
public class SensorTypeDrawer : PropertyDrawer
{
    static Type[] sensorTypes;
    static string[] classNames;
    [InitializeOnLoadMethod]
    private static void FindSensorTypes()
    {
        var sensorType = typeof(Sensor);
        var assembly = sensorType.Assembly;
        sensorTypes = (from t in assembly.GetTypes()
                       where t != sensorType && sensorType.IsAssignableFrom(t)
                       select t).ToArray();

        classNames = new string[sensorTypes.Length];
        for (int i = 0; i < classNames.Length; i++)
            classNames[i] = sensorTypes[i].Name;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var typeProp = property.FindPropertyRelative("sensorType");

        int index = 0;
        for (int i = 0; i < classNames.Length; i++)
        {
            if (typeProp.stringValue == classNames[i])
            {
                index = i;
                break;
            }
        }

        index = EditorGUI.Popup(position, label.text, index, classNames);
        typeProp.stringValue = classNames[index];
    }
}
