using UnityEngine.UI;

namespace Editor
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(SudokuSO))]
    public class SudokuSOEditor : Editor
    {
        private SerializedProperty _rowsProp;
        private const int GridSize = 9;

        private void OnEnable()
        {
            _rowsProp = serializedObject.FindProperty("rows");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EnsureArraySizes();

            EditorGUILayout.LabelField("Sudoku Grid (0 = empty)", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Optionally clamp to 0–9; set to true if you want that behavior
            bool clampToDigit = true;

            // Draw 9x9 grid
            for (int r = 0; r < GridSize; r++)
            {
                EditorGUILayout.BeginHorizontal();

                SerializedProperty rowProp = _rowsProp.GetArrayElementAtIndex(r).FindPropertyRelative("cells");

                for (int c = 0; c < GridSize; c++)
                {
                    SerializedProperty cellProp = rowProp.GetArrayElementAtIndex(c);

                    // Show current value as a string (0 becomes empty)
                    string current = cellProp.intValue == 0 ? "" : cellProp.intValue.ToString();

                    // Save GUI changes
                    EditorGUI.BeginChangeCheck();

                    // Draw as a 1-character text field (force width)
                    string input = EditorGUILayout.TextField(current, GUILayout.MaxWidth(15));

                    if (EditorGUI.EndChangeCheck())
                    {
                        // 1) Keep only the first character typed
                        if (!string.IsNullOrEmpty(input))
                        {
                            input = input.Substring(0, 1);

                            // 2) If it's a digit, convert it
                            if (char.IsDigit(input[0]))
                            {
                                int parsed = input[0] - '0';

                                // If you still want clamp behavior (optional)
                                if (clampToDigit)
                                    parsed = Mathf.Clamp(parsed, 0, 9);

                                cellProp.intValue = parsed;
                            }
                            else
                            {
                                // Non-digit = treat as empty
                                cellProp.intValue = 0;
                            }
                        }
                        else
                        {
                            // Empty = 0
                            cellProp.intValue = 0;
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Clear Grid (Set All To 0)"))
            {
                ClearGrid();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void EnsureArraySizes()
        {
            if (_rowsProp.arraySize != GridSize)
            {
                _rowsProp.arraySize = GridSize;
            }

            for (int r = 0; r < GridSize; r++)
            {
                SerializedProperty rowProp = _rowsProp.GetArrayElementAtIndex(r);
                SerializedProperty cellsProp = rowProp.FindPropertyRelative("cells");

                if (cellsProp.arraySize != GridSize)
                {
                    cellsProp.arraySize = GridSize;
                }

                // Ensure defaults are 0 (Unity default, but explicit is fine)
                for (int c = 0; c < GridSize; c++)
                {
                    SerializedProperty cellProp = cellsProp.GetArrayElementAtIndex(c);
                    if (cellProp.intValue < 0)
                    {
                        cellProp.intValue = 0;
                    }
                }
            }
        }

        private void ClearGrid()
        {
            for (int r = 0; r < GridSize; r++)
            {
                SerializedProperty rowProp = _rowsProp.GetArrayElementAtIndex(r).FindPropertyRelative("cells");
                for (int c = 0; c < GridSize; c++)
                {
                    rowProp.GetArrayElementAtIndex(c).intValue = 0;
                }
            }
        }
    }
}
