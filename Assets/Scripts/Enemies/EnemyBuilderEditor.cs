using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Germinator
{
    [CustomEditor(typeof(EnemyBuilder))]
    public class EnemyBuilderEditor : Editor
    {
        private List<Type> specieTypes;
        private int selectedSpecieIndex = 0;

        private void OnEnable()
        {
            specieTypes = Assembly.GetAssembly(typeof(EnemySpecie))
                                  .GetTypes()
                                  .Where(t => t.IsSubclassOf(typeof(EnemySpecie)) && !t.IsAbstract)
                                  .ToList();
        }

        public override void OnInspectorGUI()
        {
            EnemyBuilder builder = (EnemyBuilder)target;

            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Enemy Behaviour", EditorStyles.boldLabel);

            if (builder.behaviour == null)
            {
                if (specieTypes.Count > 0)
                {
                    string[] specieNames = specieTypes.Select(t => t.Name).ToArray();
                    selectedSpecieIndex = EditorGUILayout.Popup("Select Specie", selectedSpecieIndex, specieNames);

                    if (GUILayout.Button("Add Specie"))
                    {
                        Type selectedType = specieTypes[selectedSpecieIndex];

                        // Mark the object as dirty so Unity saves the change
                        EditorUtility.SetDirty(builder);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No EnemySpecie subclasses found.", MessageType.Warning);
                }
            }
            else
            {
                Editor editor = CreateEditor(builder.behaviour);
                editor.OnInspectorGUI();

                if (GUILayout.Button("Remove Specie"))
                {
                    builder.behaviour = null;
                    EditorUtility.SetDirty(builder);
                }
            }
        }
    }
}