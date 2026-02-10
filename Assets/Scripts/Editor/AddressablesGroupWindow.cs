using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Editor
{
    public class AddressableGroupGeneratorWindow : EditorWindow
    {
        private const string UtilityName = "AddressableEnumer";
        
        private readonly List<GroupEntry> _groups = new();

        private Vector2 _scrollPos;
  
        private void OnEnable()
        {
            RefreshGroups();
        }

        private void OnGUI()
        {
            DrawButtons();
            DrawScroll();
            DrawStatus();
        }

        [MenuItem("Tools/" + UtilityName)]
        public static void ShowWindow()
        {
            GetWindow<AddressableGroupGeneratorWindow>(UtilityName);
        }

        private void DrawButtons()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("REFRESH", GUILayout.Width(120), GUILayout.Height(30)))
            {
                RefreshGroups();
            }

            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(_groups.Any(group => group.IsSelected) == false);

            if (GUILayout.Button("GENERATE", GUILayout.Width(120), GUILayout.Height(30)))
            {
                GenerateSelected();
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawScroll()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUI.skin.box);

            if (_groups.Count == 0)
            {
                EditorGUILayout.HelpBox("No Addressable Groups", MessageType.Info);
                EditorGUILayout.EndScrollView();
                return;
            }

            for (int i = 0; i < _groups.Count; i++)
            {
                GroupEntry entry = _groups[i];
                bool isToggleEnabled = EditorGUILayout.ToggleLeft(entry.Name, entry.IsSelected);

                if (isToggleEnabled != entry.IsSelected)
                {
                    _groups[i] = new GroupEntry(entry.Name, isToggleEnabled);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawStatus()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUILayout.Label($"Groups: {_groups.Count}", EditorStyles.toolbarButton);
            GUILayout.Label($"Selected: {_groups.Count(g => g.IsSelected)}", EditorStyles.toolbarButton);

            EditorGUILayout.EndHorizontal();
        }

        private void GenerateSelected()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var selectedEntries = _groups.Where(groupEntry => groupEntry.IsSelected).ToList();
            
            if (selectedEntries.Count == 0)
            {
                return;
            }

            for (int i = 0; i < selectedEntries.Count; i++)
            {
                string name = selectedEntries[i].Name;
                AddressableAssetGroup group = settings.FindGroup(name);
                string[] names = group.entries.Select(entry => entry.address).ToArray();
                
                EnumGenerator.Generate(names, name);
            }
            
            EditorUtility.DisplayDialog("Ready!", $"Generated Count: {selectedEntries.Count}", "OK");
        }

        private void RefreshGroups()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

            if (settings == null)
            {
                _groups.Clear();
                return;
            }

            _groups.Clear();

            IEnumerable<GroupEntry> newGroups = settings.groups
                .Select(group => new GroupEntry(group.Name, false));

            _groups.AddRange(newGroups);
        }
    }
}