using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;



[CustomEditor(typeof(CommandTrigger))]
public class CommandTriggerEditor : Editor {

	ReorderableList commandListProp;

	// We don't use this other than to make sure it has at least
	// one entry for the default Command.
	//ReorderableList commandListStatusProp;

	public SerializedProperty
		startArmedProp,
		fireOnlyOnceProp,
		printDebugMessagesProp;

	void OnEnable() {
		startArmedProp = serializedObject.FindProperty("startArmed");
		fireOnlyOnceProp = serializedObject.FindProperty("fireOnlyOnce");
		printDebugMessagesProp = serializedObject.FindProperty("printDebugMessages");

		commandListProp = new ReorderableList(serializedObject, 
                serializedObject.FindProperty("commandList"),
                true, true, true, true);


		// does this actually init list?
		if (commandListProp.list == null) {
			commandListProp.list = new List<Command>();
		}

		if (commandListProp.serializedProperty.arraySize < 1) {
			commandListProp.serializedProperty.arraySize++;
		}

		//Debug.Log(commandListProp.serializedProperty.arraySize);
		serializedObject.ApplyModifiedProperties();
		//Debug.Log(commandListProp.serializedProperty.arraySize);

		var lineHeight = EditorGUIUtility.singleLineHeight * 1.1f;

		commandListProp.elementHeightCallback = (int index) => {
			float elementHeight = 80.0f;
			var commandProp = commandListProp.serializedProperty.GetArrayElementAtIndex(index);
			bool isClaimCamera = commandProp.FindPropertyRelative("type").enumValueIndex == 0;
			bool isActivateTrigger = commandProp.FindPropertyRelative("type").enumValueIndex == 1;
			bool isTriggeredByPlayer = commandProp.FindPropertyRelative("triggerMode").enumValueIndex == 1;
			bool isTriggeredByNpc = commandProp.FindPropertyRelative("triggerMode").enumValueIndex == 2;


			// Account for "target trigger" or "target NPC" field.
			if (isActivateTrigger || isTriggeredByNpc || isTriggeredByPlayer) {
				elementHeight += lineHeight;
			}

			if (isClaimCamera)
				elementHeight += 2 * lineHeight;

			return elementHeight;
		};

		commandListProp.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {	
			SerializedProperty commandProp = commandListProp.serializedProperty.GetArrayElementAtIndex(index);
			rect.y += 5;

			Rect adjustedRect;
			int linesAdded = 0;

			// Command Type
			EditorGUI.PropertyField(
				new Rect(rect.x, rect.y, rect.width, lineHeight),
				commandProp.FindPropertyRelative("type"),
				GUIContent.none);
			linesAdded++;

			bool isClaimCamera = commandProp.FindPropertyRelative("type").enumValueIndex == 0;
			bool isArmTrigger = commandProp.FindPropertyRelative("type").enumValueIndex == 1;

			if (isClaimCamera) {
				EditorGUI.indentLevel++;
				adjustedRect = EditorGUI.PrefixLabel(new Rect(rect.x, rect.y + linesAdded * lineHeight, rect.width, lineHeight),
				new GUIContent("Camera focus target:    "));

				EditorGUI.PropertyField(
					adjustedRect,
					commandProp.FindPropertyRelative("cameraFocusTarget"),
					GUIContent.none);

				//rect.y += lineHeight;
				linesAdded++;

				// "Transition Duration" label
				adjustedRect = EditorGUI.PrefixLabel(
					new Rect(rect.x, rect.y + linesAdded * lineHeight, rect.width, lineHeight),
					new GUIContent("Transition Duration:      "));
				adjustedRect.x = rect.x + rect.width - 50;
				adjustedRect.width = 50;

				// Transition Duration field
				EditorGUI.PropertyField(
					adjustedRect,
					commandProp.FindPropertyRelative("transitionDuration"),
					GUIContent.none);
				linesAdded++;

				EditorGUI.indentLevel--;
			}
			else if (isArmTrigger) {
				EditorGUI.indentLevel++;
				adjustedRect = EditorGUI.PrefixLabel(new Rect(rect.x, rect.y + linesAdded * lineHeight, rect.width, lineHeight),
				new GUIContent("Target trigger:    "));

				EditorGUI.PropertyField(
				adjustedRect,
				commandProp.FindPropertyRelative("targetTrigger"),
				GUIContent.none);

				rect.y += lineHeight;
				EditorGUI.indentLevel--;
			}

			// Trigger mode
			EditorGUI.PropertyField(
				new Rect(rect.x, rect.y + linesAdded * lineHeight, rect.width, lineHeight),
				commandProp.FindPropertyRelative("triggerMode"),
				GUIContent.none);
			linesAdded++;

			bool isTriggeredByNpc = commandProp.FindPropertyRelative("triggerMode").enumValueIndex == 0;

			if (!isClaimCamera && !isArmTrigger && !isTriggeredByNpc) {
				EditorGUI.indentLevel++;
				adjustedRect = EditorGUI.PrefixLabel(new Rect(rect.x, rect.y + linesAdded * lineHeight, rect.width, lineHeight),
				new GUIContent("Target NPC:    "));

				EditorGUI.PropertyField(
				adjustedRect,
				commandProp.FindPropertyRelative("targetNpc"),
				GUIContent.none);

				rect.y += lineHeight;
				EditorGUI.indentLevel--;
			}

			// "Wait before firing" label
			adjustedRect = EditorGUI.PrefixLabel(
				new Rect(rect.x, rect.y + linesAdded * lineHeight, rect.width, lineHeight),
				new GUIContent("Wait before firing:      "));
			adjustedRect.x = rect.x + rect.width - 50;
			adjustedRect.width = 50;

			// Wait seconds field
			EditorGUI.PropertyField(
				adjustedRect,
				commandProp.FindPropertyRelative("waitSecondsBeforeFiring"),
				GUIContent.none);
			linesAdded++;

			// "Wait after firing" label
			adjustedRect = EditorGUI.PrefixLabel(
				new Rect(rect.x, rect.y + linesAdded * lineHeight, rect.width, lineHeight),
				new GUIContent("Wait after firing:       "));
			adjustedRect.x = rect.x + rect.width - 50;
			adjustedRect.width = 50;

			// Wait seconds field
			EditorGUI.PropertyField(
				adjustedRect,
				commandProp.FindPropertyRelative("waitSecondsAfterFiring"),
				GUIContent.none);
			linesAdded++;
		};

		commandListProp.drawHeaderCallback = (Rect rect) => {
			EditorGUI.LabelField(rect, "Commands");
		};

		commandListProp.onCanRemoveCallback = (ReorderableList l) => {
    		return l.serializedProperty.arraySize > 1;
		};
	}



	public override void OnInspectorGUI() {
		serializedObject.Update();

		EditorGUILayout.PropertyField(startArmedProp);
		EditorGUILayout.PropertyField(fireOnlyOnceProp);
		EditorGUILayout.PropertyField(printDebugMessagesProp);
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		commandListProp.DoLayoutList();

		serializedObject.ApplyModifiedProperties();
	}
}
