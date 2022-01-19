using Gaze;
using UnityEditor;
using UnityEngine;

namespace SpatialStories
{
    [CustomEditor(typeof(SC_Touch))]
    public class SC_TouchEditor : SC_CollisionEditor
    {
        private readonly string[] m_touchIndexesName = { "Touch", "Untouch" };

        private SC_Touch m_touchCondition;

        public override void OnEnable()
        {
            base.OnEnable();
            m_touchCondition = (SC_Touch)target;
            m_HUDButtonHeight = 60;
            m_enableDisplayHands = true;
        }

        protected override void ActionTypeToTriggerTheCondition()
        {
            S_EditorUtils.DrawSectionTitle("Hand");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(220));
            EditorGUILayout.LabelField("Mode", GUILayout.MaxWidth(100));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // display the OnEnter OR OnExit condition for the list
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(220));
            m_touchCondition.TouchIndexAction = EditorGUILayout.Popup("", m_touchCondition.TouchIndexAction, m_touchIndexesName, GUILayout.MaxWidth(100));
            switch (m_touchCondition.TouchIndexAction)
            {
                case 0:
                    m_Condition.ProximityMap.proximityStateIndex = (int)S_ProximityStates.ENTER;
                    break;

                default:
                    m_Condition.ProximityMap.proximityStateIndex = (int)S_ProximityStates.EXIT;
                    break;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        public override void OnEditorUI()
        {
            EditorGUILayout.Space();
            S_EditorUtils.DrawEditorHint("Checks if the user hand(s)' collide with an object (uses the Manipulate collider)");

            base.OnEditorUI();
        }

        public override void OnRuntimeUI()
        {
            base.OnRuntimeUI();

            EditorGUILayout.BeginHorizontal();
            if (m_Condition.IsValid)
            {
                RenderSatisfiedLabel("Proximities Validated:");
                RenderSatisfiedLabel("True");
            }
            else
            {
                RenderNonSatisfiedLabel("Proximities Validated:");
                RenderNonSatisfiedLabel("False");
            }
            EditorGUILayout.EndHorizontal();

            string objectsInProximity = "";

            foreach (S_ProximityEntry pe in m_Condition.ProximityMap.proximityEntryList)
            {
                //TODO @apelab modify this method because it displays multiple times the same object
                for (int i = 0; i < pe.CollidingObjects.Count; i++)
                {
                    objectsInProximity = string.Concat(objectsInProximity, ", ", pe.CollidingObjects[i].name);
                }
            }
            if (objectsInProximity.Length > 0)
            {
                RenderSatisfiedLabel("Objects in proximity:");
                RenderSatisfiedLabel(objectsInProximity.Substring(1, objectsInProximity.Length - 1));

            }
            else
            {
                RenderNonSatisfiedLabel("Objects in proximity:");
                RenderNonSatisfiedLabel("---");
            }
            EditorGUILayout.Space();
        }

        // This function is overriden from SC_CollisionEditor and has pretty much the same body
        // just to remove the logic to show a warning message when there is less than two objects
        // (which does not make sense in SC_Touch) and to remove the display of the Require all
        // toggle which actually needs a whole other logic for SC_Touch.
        // This is not ideal to have duplicated code like this, but the whole thing should be refactored
        // soon to remote that inheritance to SC_Collision
        protected override void DisplayTargetObjects()
        {
            // ++++ TARGETS OBJECTS ++++
            S_EditorUtils.DrawSectionTitle("Target Objects");

            // IN CASE IT'S EMPTY INITIALIZE WITH A NEW EMPTY INSTANCE
            if (m_Condition.ProximityMap.proximityEntryList.Count == 0)
            {
                m_Condition.ProximityMap.AddProximityEntry();
            }

            for (int i = 0; i < m_Condition.ProximityMap.proximityEntryList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(300));

                // Set the default collider for the gaze
                if (m_Condition.ProximityMap.proximityEntryList[i].DependentGameObject == null)
                {
                    m_Condition.ProximityMap.proximityEntryList[i].DependentGameObject = m_Condition.GetComponentInParent<S_InteractiveObject>();
                }

                var proximityObject = EditorGUILayout.ObjectField(m_Condition.ProximityMap.proximityEntryList[i].DependentGameObject, typeof(S_InteractiveObject), true);

                if (proximityObject != null)
                    m_Condition.ProximityMap.proximityEntryList[i].DependentGameObject = (S_InteractiveObject)proximityObject;

                if (m_Condition.ProximityMap.proximityEntryList.Count > 1)
                {
                    if (GUILayout.Button("-"))
                    {
                        m_Condition.ProximityMap.DeleteProximityEntry(m_Condition.ProximityMap.proximityEntryList[i]);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            // display 'add' button
            if (GUILayout.Button("+", GUILayout.MaxWidth(300)))
            {
                S_ProximityEntry d = m_Condition.ProximityMap.AddProximityEntry();
                EditorGUILayout.BeginHorizontal();

                // assign the first interactive object in the hierarchy list by default (0)
                if (m_HierarchyProximities != null && m_HierarchyProximities.Count > 0)
                {
                    if (d.DependentGameObject == null)
                    {
                        d.DependentGameObject = m_Condition.GetComponentInParent<S_InteractiveObject>();
                    }

                    var proximityObject = EditorGUILayout.ObjectField(d.DependentGameObject, typeof(S_InteractiveObject), true);

                    if (proximityObject != null)
                        d.DependentGameObject = (S_InteractiveObject)proximityObject;

                    if (GUILayout.Button("-", GUILayout.MaxWidth(300)))
                    {
                        m_Condition.ProximityMap.DeleteProximityEntry(d);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}