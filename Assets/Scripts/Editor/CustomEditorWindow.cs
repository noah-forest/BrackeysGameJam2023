using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomEditorWindow : EditorWindow
{
	[MenuItem("Tools/CustomWindow")]
	public static void ShowWindow()
	{
		GetWindow<CustomEditorWindow>("CustomerEditorWindow");
	}

	private void OnGUI()
	{
		GUILayout.Label("Reload Unit Stats", EditorStyles.boldLabel);
		if(GUILayout.Button("Reload Stats"))
		{
			GameObject.Find("UnitManager").GetComponent<LoadExcel>().LoadUnitData();
		}
	}
}
