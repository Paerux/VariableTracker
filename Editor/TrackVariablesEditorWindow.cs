using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using _Project.Scripts.VariableTracker.Demo;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.VariableTracker.Editor
{
    public class TrackVariablesEditorWindow : EditorWindow
    {
        private List<TrackedVariableInfo> _exposedMembers = new();
        private GameObject[] allGameObjects;
        private List<GameObject> trackedObjects = new();
        private List<Type> containingTypes = new();
        [MenuItem("Paerux/Tools/Track Variables")]
        public static void Open()
        {
           CreateWindow<TrackVariablesEditorWindow>("Variable Tracker");
        }

        private void Refresh()
        {
            containingTypes = new List<Type>();
            trackedObjects = new List<GameObject>();
            _exposedMembers = new List<TrackedVariableInfo>();
            allGameObjects =  SceneManager.GetActiveScene().GetRootGameObjects();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
                    var members = type.GetMembers(flags);
                    var attributedMemberFound = false;
                    foreach (var member in members)
                    {
                        if (member.CustomAttributes.ToArray().Length <= 0) continue;
                        var attribute = member.GetCustomAttribute<TrackAttribute>();
                        if (attribute != null)
                        {
                            _exposedMembers.Add(new TrackedVariableInfo(member,attribute,type));
                            Debug.Log("Added " + member.Name + " to exposed members with type " + type.Name);
                            attributedMemberFound = true;
                        }
                    }

                    if (attributedMemberFound)
                    {
                        containingTypes.Add(type);
                        Debug.Log("Added " + type.Name + " to containing types");
                    }
                }
            }

            foreach (var gameObject in allGameObjects)
            {
                foreach (var type in containingTypes)
                {
                    if (gameObject.GetComponent(type) != null)
                    {
                        if (!trackedObjects.Contains(gameObject))
                        {
                            trackedObjects.Add(gameObject);
                            Debug.Log("Added " + gameObject.name + " to tracked objects");
                        }
                    }
                }
            }
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        public void OnGUI()
        {
            if (GUILayout.Button("Refresh"))
            {
                Refresh();
            }

            EditorGUILayout.LabelField("Tracked Variables", EditorStyles.boldLabel);

            foreach (var trackedObject in trackedObjects)
            {
                foreach (var trackerVariableInfo in _exposedMembers)
                {
                    var memberInfo = trackerVariableInfo.memberInfo;
                    var attribute = trackerVariableInfo.trackAttribute;
                    var type = trackerVariableInfo.type;

                    if (memberInfo.MemberType == MemberTypes.Field)
                    {
                        var fieldInfo = (FieldInfo) memberInfo;
                        string value;

                        if (trackedObject.GetComponent(type) != null)
                        {
                            var obj = fieldInfo.GetValue(trackedObject.GetComponent(type));
                            if (obj != null)
                            {
                                value = obj.ToString();
                                EditorGUILayout.LabelField($"{attribute.DisplayName} - {value} - {trackedObject.name}");
                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField(
                            $"{trackerVariableInfo.trackAttribute.DisplayName} - {trackedObject.name}");
                    }
                }
            }
        }
    }


    public struct TrackedVariableInfo
    {
        public readonly MemberInfo memberInfo;
        public readonly TrackAttribute trackAttribute;
        public readonly Type type;

        public TrackedVariableInfo(MemberInfo info, TrackAttribute attribute, Type type)
        {
            memberInfo = info;
            trackAttribute = attribute;
            this.type = type;
        }
    }
}