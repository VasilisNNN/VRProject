using UnityEditor;
using UnityEngine;

namespace Nav3D.API.Editor
{
    [CustomEditor(typeof(Nav3DAgentDescription))]
    public class Nav3DAgentDescriptionInspector : UnityEditor.Editor
    {
        #region Constants

        const string RADIUS_OBTAIN_MODE_VALUE = "Specific value";
        const string RADIUS_OBTAIN_MODE_RANDOM_RANGE = "Random ranged value";

        const string RADIUS_RANDOM_MODE_UNIFORM = "Default";
        const string RADIUS_RANDOM_MODE_NORMAL = "Gaussian";

        const string MAX_SPEED_OBTAIN_METHOD_VALUE = "Absolute value";
        const string MAX_SPEED_OBTAIN_METHOD_MULTIPLIER = "Speed multiplier";

        GUIStyle BOLD_WRAPPED_WHITE_STYLE;

        #endregion

        #region Attributes

        Nav3DAgentDescription m_Description;

        bool m_RadiusFoldout;
        bool m_SpeedFoldout;
        bool m_BehaviorFoldout;
        bool m_LocalAvoidanceFoldout;
        bool m_PathfindingFoldout;
        bool m_MotionFoldout;
        bool m_VelocityFoldout;
        bool m_DebugFoldout;

        SerializedProperty m_BehaviorType;
        SerializedProperty m_MotionNavigationType;
        SerializedProperty m_Radius;
        SerializedProperty m_RadiusObtainMode;
        SerializedProperty m_RadiusRandomRangeMode;
        SerializedProperty m_RadiusMin;
        SerializedProperty m_RadiusMax;

        SerializedProperty m_Speed;
        SerializedProperty m_SpeedObtainMode;
        SerializedProperty m_SpeedRandomRangeMode;
        SerializedProperty m_SpeedMin;
        SerializedProperty m_SpeedMax;
        SerializedProperty m_MaxSpeed;
        SerializedProperty m_MaxSpeedObtainMode;
        SerializedProperty m_SpeedToMaxSpeedMultiplier;
        SerializedProperty m_UseConsideredAgentsNumberLimit;
        SerializedProperty m_ConsideredAgentsNumberLimit;
        SerializedProperty m_ORCATau;

        SerializedProperty m_PathfindingTimeout;
        SerializedProperty m_SmoothPath;
        SerializedProperty m_SmoothRatio;
        SerializedProperty m_AutoUpdatePath;
        SerializedProperty m_PathAutoUpdateCooldown;

        SerializedProperty m_TargetReachDistance;
        SerializedProperty m_MaxAgentDegreesRotationPerTick;

        SerializedProperty m_PathVelocityWeight;
        SerializedProperty m_PathVelocityWeight1;
        SerializedProperty m_PathVelocityWeight2;
        SerializedProperty m_AgentsAvoidanceVelocityWeight;
        SerializedProperty m_AgentsAvoidanceVelocityWeight1;
        SerializedProperty m_AgentsAvoidanceVelocityWeight2;
        SerializedProperty m_ObstacleAvoidanceVelocityWeight;
        SerializedProperty m_ObstacleAvoidanceVelocityWeight1;
        SerializedProperty m_ObstacleAvoidanceVelocityWeight2;

        SerializedProperty m_UseLog;
        SerializedProperty m_LogSize;

        #endregion

        #region Unity methods

        void OnEnable()
        {
            BOLD_WRAPPED_WHITE_STYLE = new GUIStyle
            {
                wordWrap = true,
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState { textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black }
            };

            m_Radius = serializedObject.FindProperty("m_Radius");
            m_RadiusObtainMode = serializedObject.FindProperty("m_RadiusObtainMode");
            m_RadiusRandomRangeMode = serializedObject.FindProperty("m_RadiusRandomRangeMode");
            m_RadiusMin = serializedObject.FindProperty("m_RadiusMin");
            m_RadiusMax = serializedObject.FindProperty("m_RadiusMax");

            m_Speed = serializedObject.FindProperty("m_Speed");
            m_SpeedObtainMode = serializedObject.FindProperty("m_SpeedObtainMode");
            m_SpeedRandomRangeMode = serializedObject.FindProperty("m_SpeedRandomRangeMode");
            m_SpeedMin = serializedObject.FindProperty("m_SpeedMin");
            m_SpeedMax = serializedObject.FindProperty("m_SpeedMax");
            m_MaxSpeed = serializedObject.FindProperty("m_MaxSpeed");
            m_MaxSpeedObtainMode = serializedObject.FindProperty("m_MaxSpeedObtainMode");
            m_SpeedToMaxSpeedMultiplier = serializedObject.FindProperty("m_SpeedToMaxSpeedMultiplier");

            m_UseConsideredAgentsNumberLimit = serializedObject.FindProperty("m_UseConsideredAgentsNumberLimit");
            m_ConsideredAgentsNumberLimit = serializedObject.FindProperty("m_ConsideredAgentsNumberLimit");
            m_ORCATau = serializedObject.FindProperty("m_ORCATau");

            m_BehaviorType = serializedObject.FindProperty("m_BehaviorType");
            m_MotionNavigationType = serializedObject.FindProperty("m_MotionNavigationType");

            m_PathfindingTimeout = serializedObject.FindProperty("m_PathfindingTimeout");
            m_SmoothPath = serializedObject.FindProperty("m_SmoothPath");
            m_SmoothRatio = serializedObject.FindProperty("m_SmoothRatio");
            m_AutoUpdatePath = serializedObject.FindProperty("m_AutoUpdatePath");
            m_PathAutoUpdateCooldown = serializedObject.FindProperty("m_PathAutoUpdateCooldown");

            m_TargetReachDistance = serializedObject.FindProperty("m_TargetReachDistance");
            m_MaxAgentDegreesRotationPerTick = serializedObject.FindProperty("m_MaxAgentDegreesRotationPerTick");

            m_PathVelocityWeight = serializedObject.FindProperty("m_PathVelocityWeight");
            m_PathVelocityWeight1 = serializedObject.FindProperty("m_PathVelocityWeight1");
            m_PathVelocityWeight2 = serializedObject.FindProperty("m_PathVelocityWeight2");
            m_AgentsAvoidanceVelocityWeight = serializedObject.FindProperty("m_AgentsAvoidanceVelocityWeight");
            m_AgentsAvoidanceVelocityWeight1 = serializedObject.FindProperty("m_AgentsAvoidanceVelocityWeight1");
            m_AgentsAvoidanceVelocityWeight2 = serializedObject.FindProperty("m_AgentsAvoidanceVelocityWeight2");
            m_ObstacleAvoidanceVelocityWeight = serializedObject.FindProperty("m_ObstacleAvoidanceVelocityWeight");
            m_ObstacleAvoidanceVelocityWeight1 = serializedObject.FindProperty("m_ObstacleAvoidanceVelocityWeight1");
            m_ObstacleAvoidanceVelocityWeight2 = serializedObject.FindProperty("m_ObstacleAvoidanceVelocityWeight2");

            m_UseLog = serializedObject.FindProperty("m_UseLog");
            m_LogSize = serializedObject.FindProperty("m_LogSize");
        }

        public override void OnInspectorGUI()
        {
            m_Description = (Nav3DAgentDescription)target;

            serializedObject.Update();

            HeaderControls();

            //Behavior setup
            BehaviorFoldout();

            //Radius setup
            RadiusFoldout();

            //Speed setup 
            SpeedFoldout();

            //Local avoidance setup
            if (m_MotionNavigationType.enumValueIndex == 1 || m_MotionNavigationType.enumValueIndex == 2)
                LocalAvoidanceFoldout();

            //Pathfinding setup
            if (m_BehaviorType.enumValueIndex == 0)
                PathfindingFoldout();

            //Motion setup
            MotionFoldout();

            if (m_MotionNavigationType.enumValueIndex == 1)
                VelocityBlendingFoldout();

            DebugFoldout();

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Service methods

        void HeaderControls()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Expand All"))
            {
                m_RadiusFoldout = true;
                m_SpeedFoldout = true;
                m_BehaviorFoldout = true;
                m_LocalAvoidanceFoldout = true;
                m_PathfindingFoldout = true;
                m_MotionFoldout = true;
                m_VelocityFoldout = true;
                m_DebugFoldout = true;
            }

            if (GUILayout.Button("Collapse All"))
            {
                m_RadiusFoldout = false;
                m_SpeedFoldout = false;
                m_BehaviorFoldout = false;
                m_LocalAvoidanceFoldout = false;
                m_PathfindingFoldout = false;
                m_MotionFoldout = false;
                m_VelocityFoldout = false;
                m_DebugFoldout = false;
            }

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Set default parameters") && EditorUtility.DisplayDialog("Set default description parameters?", "Are you sure?", "Yes", "Cancel"))
                ((Nav3DAgentDescription)target).SetDefaultAttributes();

            if (GUILayout.Button("Save changes"))
                Save();
        }

        void Save()
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }

        void BehaviorFoldout()
        {
            // ReSharper disable once AssignmentInConditionalExpression
            if (m_BehaviorFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_BehaviorFoldout, "Behavior"))
            {
                EditorGUILayout.PropertyField(m_BehaviorType);

                //Set LOCAL motion nav type, if behavior type is set to YIELDING
                if (m_BehaviorType.enumValueIndex == 1)
                    m_MotionNavigationType.enumValueIndex = 2;

                if (m_BehaviorType.enumValueIndex == 0)
                    EditorGUILayout.PropertyField(m_MotionNavigationType);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void RadiusFoldout()
        {
            // ReSharper disable once AssignmentInConditionalExpression
            if (m_RadiusFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_RadiusFoldout, "Radius"))
            {
                EditorGUILayout.LabelField("Select radius value type:");
                m_RadiusObtainMode.intValue = GUILayout.SelectionGrid(m_RadiusObtainMode.intValue, new[] { RADIUS_OBTAIN_MODE_VALUE, RADIUS_OBTAIN_MODE_RANDOM_RANGE }, 2);

                if (m_RadiusObtainMode.intValue == 0)
                {
                    EditorGUILayout.PropertyField(m_Radius);
                }
                else
                {
                    EditorGUILayout.LabelField("Select random distribution mode:");
                    m_RadiusRandomRangeMode.intValue = GUILayout.SelectionGrid(m_RadiusRandomRangeMode.intValue, new[] { RADIUS_RANDOM_MODE_UNIFORM, RADIUS_RANDOM_MODE_NORMAL }, 2);

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.PrefixLabel("Radius [Min, Max]: ");
                    m_RadiusMin.floatValue = EditorGUILayout.FloatField(m_RadiusMin.floatValue);
                    m_RadiusMax.floatValue = EditorGUILayout.FloatField(m_RadiusMax.floatValue);

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void SpeedFoldout()
        {
            // ReSharper disable once AssignmentInConditionalExpression
            if (m_SpeedFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_SpeedFoldout, "Speed"))
            {
                EditorGUILayout.LabelField("Select speed value type:");
                m_SpeedObtainMode.intValue = GUILayout.SelectionGrid(m_SpeedObtainMode.intValue, new[] { RADIUS_OBTAIN_MODE_VALUE, RADIUS_OBTAIN_MODE_RANDOM_RANGE }, 2);

                if (m_SpeedObtainMode.intValue == 0)
                {
                    EditorGUILayout.PropertyField(m_Speed);

                    if (m_MaxSpeedObtainMode.intValue == 0)
                    {
                        m_MaxSpeed.floatValue = m_Speed.floatValue * m_SpeedToMaxSpeedMultiplier.floatValue;
                    }
                    else if (m_MaxSpeedObtainMode.intValue == 1)
                    {
                        m_Speed.floatValue = Mathf.Clamp(m_Speed.floatValue, 0, Mathf.Max(m_Speed.floatValue, m_MaxSpeed.floatValue));
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Select random distribution mode:");
                    m_SpeedRandomRangeMode.intValue = GUILayout.SelectionGrid(m_SpeedRandomRangeMode.intValue, new[] { RADIUS_RANDOM_MODE_UNIFORM, RADIUS_RANDOM_MODE_NORMAL }, 2);

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.PrefixLabel("Speed [Min, Max]: ");
                    m_SpeedMin.floatValue = EditorGUILayout.FloatField(m_SpeedMin.floatValue);
                    m_SpeedMax.floatValue = EditorGUILayout.FloatField(m_SpeedMax.floatValue);

                    EditorGUILayout.EndHorizontal();
                }

                if (m_MotionNavigationType.enumValueIndex == 1 || m_MotionNavigationType.enumValueIndex == 2)
                {
                    EditorGUILayout.LabelField("Select how to obtain max speed value:");
                    m_MaxSpeedObtainMode.intValue = GUILayout.SelectionGrid(m_MaxSpeedObtainMode.intValue, new[] { MAX_SPEED_OBTAIN_METHOD_MULTIPLIER, MAX_SPEED_OBTAIN_METHOD_VALUE }, 2);

                    if (m_MaxSpeedObtainMode.intValue == 0)
                    {
                        EditorGUILayout.PropertyField(m_SpeedToMaxSpeedMultiplier);
                        if (m_SpeedObtainMode.intValue == 0)
                        {
                            EditorGUILayout.LabelField($"Max speed: {m_SpeedToMaxSpeedMultiplier.floatValue * m_Speed.floatValue}");
                        }
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(m_MaxSpeed);
                    }
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void LocalAvoidanceFoldout()
        {
            // ReSharper disable once AssignmentInConditionalExpression
            if (m_LocalAvoidanceFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_LocalAvoidanceFoldout, "Local avoidance"))
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(m_ORCATau);

                if (GUILayout.Button("Set default"))
                {
                    m_Description.SetDefaultORCATau();

                    Save();
                }

                EditorGUILayout.EndHorizontal();

                //Agents limit
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PrefixLabel("Agents considered number limit");
                m_UseConsideredAgentsNumberLimit.boolValue = EditorGUILayout.Toggle(m_UseConsideredAgentsNumberLimit.boolValue);

                EditorGUILayout.EndHorizontal();

                if (m_UseConsideredAgentsNumberLimit.boolValue)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.PrefixLabel("Agents number:");
                    m_ConsideredAgentsNumberLimit.intValue = Mathf.Max(1, EditorGUILayout.IntField(m_ConsideredAgentsNumberLimit.intValue));

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void PathfindingFoldout()
        {
            // ReSharper disable once AssignmentInConditionalExpression
            if (m_PathfindingFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_PathfindingFoldout, "Pathfinding"))
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PrefixLabel("Pathfinding timeout (ms)");
                m_PathfindingTimeout.intValue = EditorGUILayout.IntField(m_PathfindingTimeout.intValue);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PrefixLabel("Smooth the path");
                m_SmoothPath.boolValue = EditorGUILayout.Toggle(m_SmoothPath.boolValue);

                EditorGUILayout.EndHorizontal();

                if (m_SmoothPath.boolValue)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.PrefixLabel("Samples per min bucket volume");
                    m_SmoothRatio.intValue = Mathf.Max(1, EditorGUILayout.IntField(m_SmoothRatio.intValue));

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PrefixLabel("Auto-update path on stagnant behavior");
                m_AutoUpdatePath.boolValue = EditorGUILayout.Toggle(m_AutoUpdatePath.boolValue);

                EditorGUILayout.EndHorizontal();


                if (m_AutoUpdatePath.boolValue)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.PrefixLabel("Auto-update cooldown (ms)");
                    m_PathAutoUpdateCooldown.intValue = EditorGUILayout.IntField(m_PathAutoUpdateCooldown.intValue);

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void MotionFoldout()
        {
            // ReSharper disable once AssignmentInConditionalExpression
            if (m_MotionFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_MotionFoldout, "Motion"))
            {
                if (m_BehaviorType.enumValueIndex == 0)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.PrefixLabel("Target reach distance");
                    m_TargetReachDistance.floatValue = EditorGUILayout.FloatField(m_TargetReachDistance.floatValue);

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PrefixLabel("Max rotation in degrees per fixed update tick");
                m_MaxAgentDegreesRotationPerTick.floatValue = EditorGUILayout.FloatField(m_MaxAgentDegreesRotationPerTick.floatValue);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void VelocityBlendingFoldout()
        {
            // ReSharper disable once AssignmentInConditionalExpression
            if (m_VelocityFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_VelocityFoldout, "*Velocity blending"))
            {
                if (GUILayout.Button("Set default blending weights"))
                    m_Description.SetDefaultVelocitiesBlendingWeights();

                EditorGUILayout.HelpBox("Here you need to set the weights that will be used to blend the velocity vectors in different situations.", MessageType.Info);

                GUILayout.Label("Agent follows global path, and there are both other agents and obstacles near.", BOLD_WRAPPED_WHITE_STYLE);

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_PathVelocityWeight);
                EditorGUILayout.PropertyField(m_AgentsAvoidanceVelocityWeight);
                EditorGUILayout.PropertyField(m_ObstacleAvoidanceVelocityWeight);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();

                GUILayout.Label("Agent follows global path, and there are only obstacles near.", BOLD_WRAPPED_WHITE_STYLE);

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_PathVelocityWeight1);
                EditorGUILayout.PropertyField(m_ObstacleAvoidanceVelocityWeight1);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();

                GUILayout.Label("Agent follows global path, and there are only other agents near.", BOLD_WRAPPED_WHITE_STYLE);

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_PathVelocityWeight2);
                EditorGUILayout.PropertyField(m_AgentsAvoidanceVelocityWeight1);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();

                GUILayout.Label("Agent preform only local avoidance, and there are both other agents and obstacles near.", BOLD_WRAPPED_WHITE_STYLE);

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_AgentsAvoidanceVelocityWeight2);
                EditorGUILayout.PropertyField(m_ObstacleAvoidanceVelocityWeight2);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void DebugFoldout()
        {
            // ReSharper disable once AssignmentInConditionalExpression
            if (m_DebugFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_DebugFoldout, "Debug"))
            {
                EditorGUILayout.PropertyField(m_UseLog);

                if (m_UseLog.boolValue)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.PrefixLabel("Log records count:");
                    m_LogSize.intValue = EditorGUILayout.IntField(m_LogSize.intValue);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        #endregion
    }
}
