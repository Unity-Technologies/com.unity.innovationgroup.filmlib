﻿using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace MWU.Shared.Utilities
{

    [InitializeOnLoad]
    public class Shortcuts
    {

        // [MenuItem ("Edit/Deselect _d")]
        // static void DeselectAll () {
        // 	Selection.activeObject = null;
        // }

        static Shortcuts()
        {
            SceneView.onSceneGUIDelegate += CheckSceneShortcuts;
        }

        static void CheckSceneShortcuts(SceneView view)
        {
            Event evt = Event.current;
            if (evt.type == EventType.KeyDown)
            {
                switch (evt.keyCode)
                {
                    case KeyCode.Keypad7:
                        view.LookAt(view.pivot, Quaternion.LookRotation(Vector3.down, Vector3.forward), view.size, true);
                        evt.Use();
                        break;
                    case KeyCode.Keypad1:
                        view.LookAt(view.pivot, Quaternion.LookRotation(Vector3.right, Vector3.up), view.size, true);
                        evt.Use();
                        break;
                    case KeyCode.Keypad3:
                        view.LookAt(view.pivot, Quaternion.LookRotation(Vector3.forward, Vector3.up), view.size, true);
                        evt.Use();
                        break;
                    case KeyCode.Keypad5:
                        view.LookAt(view.pivot, view.rotation, view.size, !view.orthographic);
                        evt.Use();
                        break;
                    case KeyCode.Keypad0:
                        if (Camera.main)
                        {
                            view.LookAt(view.pivot, Camera.main.transform.rotation, view.size, false);
                            evt.Use();
                        }
                        break;
                }

            }
        }

        [MenuItem("Tools/Edit/Find In Project %g", false, 3)]
        public static void ProjectSearch()
        {
            // Get the internal UnityEditor.ObjectBrowser window
            System.Type t = typeof(EditorWindow).Assembly.GetType("UnityEditor.ProjectBrowser");

            // Get the window & focus it
            EditorWindow win = EditorWindow.GetWindow(t);
            win.Focus();

            // Send a find command
            Event e = new Event();
            e.type = EventType.ExecuteCommand;
            e.commandName = "Find";
            win.SendEvent(e);
        }

        [MenuItem("Tools/Edit/Create Group %#g", false, 4)]
        public static void CreateGroup()
        {
            GameObject newGO = new GameObject("Group");
            foreach (GameObject go in Selection.gameObjects)
                go.transform.parent = newGO.transform;
            Selection.activeGameObject = newGO;
            CenterOnChildren();
        }

        [MenuItem("Tools/Edit/Center Group on Children", false, 5)]
        public static void CenterOnChildren()
        {
            foreach (Transform root in Selection.GetFiltered(typeof(Transform), SelectionMode.TopLevel | SelectionMode.ExcludePrefab | SelectionMode.Editable))
            {
                Vector3 min = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
                Vector3 max = new Vector3(-Mathf.Infinity, -Mathf.Infinity, -Mathf.Infinity);
                List<Vector3> origPos = new List<Vector3>();
                bool found = false;

                foreach (Transform t in root)
                {
                    found = true;
                    min = Vector3.Min(min, t.position);
                    max = Vector3.Max(max, t.position);
                    origPos.Add(t.position);
                }

                if (found)
                {
                    Vector3 centerPoint = (max + min) / 2f;
                    root.position = centerPoint;

                    int idx = 0;
                    foreach (Transform t in root)
                        t.position = origPos[idx++];
                }
            }
        }

        [MenuItem("Tools/Edit/Disable All Gizmos")]
        static void DisableAllGizmosMenu()
        {
            ToggleGizmos(0);
        }

        [MenuItem("Tools/Edit/Enable All Gizmos")]
        static void EnableAllGizmos()
        {
            ToggleGizmos(1);
        }

        static void ToggleGizmos(int state)
        {
            var Annotation = Type.GetType("UnityEditor.Annotation, UnityEditor");
            var ClassId = Annotation.GetField("classID");
            var ScriptClass = Annotation.GetField("scriptClass");

            Type AnnotationUtility = Type.GetType("UnityEditor.AnnotationUtility, UnityEditor");
            var GetAnnotations = AnnotationUtility.GetMethod("GetAnnotations", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            var SetGizmoEnabled = AnnotationUtility.GetMethod("SetGizmoEnabled", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            var SetIconEnabled = AnnotationUtility.GetMethod("SetIconEnabled", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

            Array annotations = (Array)GetAnnotations.Invoke(null, null);

            foreach (var a in annotations)
            {
                int classId = (int)ClassId.GetValue(a);
                string scriptClass = (string)ScriptClass.GetValue(a);

                SetGizmoEnabled.Invoke(null, new object[] { classId, scriptClass, state });
                SetIconEnabled.Invoke(null, new object[] { classId, scriptClass, state });
            }
        }
    }
}