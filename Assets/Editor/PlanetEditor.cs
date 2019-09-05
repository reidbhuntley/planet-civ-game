using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor {

    Planet planet;
    Editor tileEditor, shapeEditor, mapEditor;

	public override void OnInspectorGUI() {
        using (var check = new EditorGUI.ChangeCheckScope()) {
            base.OnInspectorGUI();
            // if (check.changed) {
            //     planet.GeneratePlanet();
            // }
        }

        if (GUILayout.Button("Generate Planet")) {
            planet.GeneratePlanet();
        }

        DrawSettingsEditor(planet.tileSettings, planet.OnTileSettingsUpdated, ref planet.tileSettingsFoldout, ref tileEditor);
        DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(planet.mapSettings, planet.OnMapSettingsUpdated, ref planet.mapSettingsFoldout, ref mapEditor);
	}

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor) {
        if (settings != null) {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            using (var check = new EditorGUI.ChangeCheckScope()) {
                if (foldout) {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed) {
                        if (onSettingsUpdated != null) {
                            onSettingsUpdated();
                        }
                    }
                }
            }
        }
    }

	private void OnEnable() {
        planet = (Planet)target;
	}
}