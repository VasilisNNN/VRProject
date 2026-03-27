using UnityEditor;
using UnityEngine;

namespace Nav3D.API.Editor
{
    [CustomEditor(typeof(Nav3DAgent), true), CanEditMultipleObjects]
    public class Nav3DAgentInspector : UnityEditor.Editor
    {
        #region Attributes

        SerializedProperty m_DrawRadius;
        SerializedProperty m_DrawVelocities;
        SerializedProperty m_DrawCurrentPath;

        SerializedProperty m_Description;

        SerializedProperty m_ShowAgents;
        SerializedProperty m_ShowStaticTriangles;

        Nav3DAgent m_Agent;

        #endregion

        #region Unity methods

        void OnEnable()
        {
            m_DrawRadius = serializedObject.FindProperty("m_DrawRadius");
            m_DrawVelocities = serializedObject.FindProperty("m_DrawVelocities");
            m_DrawCurrentPath = serializedObject.FindProperty("m_DrawCurrentPath");

            m_Description = serializedObject.FindProperty("m_Description");

            m_ShowAgents = serializedObject.FindProperty("m_ShowAgents");
            m_ShowStaticTriangles = serializedObject.FindProperty("m_ShowStaticTriangles");

            m_Agent = (Nav3DAgent)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DebugDrawContent(m_Agent);

            DescriptionContent(m_Agent.Description);

            if (m_Agent.Description?.UseLog ?? false)
                LogContent(m_Agent);

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Service methods

        void DebugDrawContent(Nav3DAgent _Agent)
        {
            if (EditorUtility.IsPersistent(m_Agent))
                return;

            EditorGUILayout.LabelField("Debug drawing", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Draw radius: ");
            m_DrawRadius.boolValue = EditorGUILayout.Toggle(m_DrawRadius.boolValue);

            EditorGUILayout.EndHorizontal();

            if (!Application.isPlaying)
                return;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Draw velocities: ");
            m_DrawVelocities.boolValue = EditorGUILayout.Toggle(m_DrawVelocities.boolValue);

            EditorGUILayout.EndHorizontal();

            if (_Agent.Description != null && _Agent.Description.MotionNavigationType != MotionNavigationType.LOCAL)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Draw path: ");
                m_DrawCurrentPath.boolValue = EditorGUILayout.Toggle(m_DrawCurrentPath.boolValue);

                EditorGUILayout.EndHorizontal();
            }

            if (_Agent.Description != null && _Agent.Description.MotionNavigationType != MotionNavigationType.GLOBAL)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Highligh nearest agents: ");
                m_ShowAgents.boolValue = EditorGUILayout.Toggle(m_ShowAgents.boolValue);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Highlight nearest triangles: ");
                m_ShowStaticTriangles.boolValue = EditorGUILayout.Toggle(m_ShowStaticTriangles.boolValue);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
        }

        void LogContent(Nav3DAgent _Agent)
        {
            EditorGUILayout.LabelField("Log", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Log is enabled for an agent.");
            EditorGUILayout.LabelField("Press the button below to copy log to the clipboard.");

            if (GUILayout.Button("Copy"))
                EditorGUIUtility.systemCopyBuffer = _Agent.GetLogText();
        }

        void DescriptionContent(Nav3DAgentDescription _Description)
        {
            EditorGUILayout.LabelField("Description", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_Description);

            if (Application.isPlaying && !EditorUtility.IsPersistent(m_Agent))
            {
                EditorGUILayout.LabelField("Press the button below to copy Description instance to the clipboard.");

                if (GUILayout.Button("Copy"))
                    EditorGUIUtility.systemCopyBuffer = _Description.ToString();
            }
        }

        #endregion
    }
}
