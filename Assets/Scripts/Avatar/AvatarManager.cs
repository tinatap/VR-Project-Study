using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AvatarManager : MonoBehaviour
{
    [Serializable]
    public class AvatarEntry
    {
        [Tooltip("Name shown in the Avatar dropdown")]
        public string avatarName;

        [Tooltip("Avatar GameObject inside Geometry")]
        public GameObject avatarObject;

        [Tooltip("Avatar used by the main Animator")]
        public Avatar animatorAvatar;
    }

    [Header("Main Animator")]
    [SerializeField] private Animator animator;

    [Header("Available Avatars")]
    [SerializeField]
    private List<AvatarEntry> availableAvatars =
        new List<AvatarEntry>();

    [Header("Selected Avatar")]
    [SerializeField] private int selectedAvatarIndex = 0;


    // =========================================================
    // START
    // =========================================================

    private void Awake()
    {
        ApplySelectedAvatar();
    }

    private void Start()
    {
        ApplySelectedAvatar();
    }


    // =========================================================
    // APPLY AVATAR
    // =========================================================

    public void ApplySelectedAvatar()
    {
        if (animator == null)
        {
            Debug.LogWarning(
                "AvatarManager: Main Animator is not assigned."
            );

            return;
        }

        if (availableAvatars == null ||
            availableAvatars.Count == 0)
        {
            Debug.LogWarning(
                "AvatarManager: No avatars have been added."
            );

            return;
        }

        // Make sure index is valid
        selectedAvatarIndex = Mathf.Clamp(
            selectedAvatarIndex,
            0,
            availableAvatars.Count - 1
        );

        AvatarEntry selected =
            availableAvatars[selectedAvatarIndex];


        // =====================================================
        // 1. SET ANIMATOR AVATAR
        // =====================================================

        if (selected.animatorAvatar != null)
        {
            animator.avatar = selected.animatorAvatar;
        }
        else
        {
            Debug.LogWarning(
                "AvatarManager: Animator Avatar is missing for " +
                selected.avatarName
            );
        }


        // =====================================================
        // 2. DISABLE ALL AVATARS
        // =====================================================

        foreach (AvatarEntry entry in availableAvatars)
        {
            if (entry.avatarObject != null)
            {
                entry.avatarObject.SetActive(false);
            }
        }


        // =====================================================
        // 3. ENABLE SELECTED AVATAR
        // =====================================================

        if (selected.avatarObject != null)
        {
            selected.avatarObject.SetActive(true);
        }


        Debug.Log(
            "AvatarManager: Selected Avatar = " +
            selected.avatarName
        );
    }


    // =========================================================
    // SELECT AVATAR BY NAME
    // =========================================================

    public bool SelectAvatarByName(string avatarName)
    {
        if (string.IsNullOrEmpty(avatarName))
        {
            Debug.LogWarning(
                "AvatarManager: Avatar name is empty."
            );

            return false;
        }

        if (availableAvatars == null ||
            availableAvatars.Count == 0)
        {
            Debug.LogWarning(
                "AvatarManager: No avatars are available."
            );

            return false;
        }

        for (int i = 0; i < availableAvatars.Count; i++)
        {
            if (string.Equals(
                availableAvatars[i].avatarName,
                avatarName,
                StringComparison.OrdinalIgnoreCase))
            {
                selectedAvatarIndex = i;

                ApplySelectedAvatar();

                Debug.Log(
                    "AvatarManager: Avatar selected from TCP = " +
                    avatarName
                );

                return true;
            }
        }

        Debug.LogError(
            "AvatarManager: Avatar not found: " +
            avatarName
        );

        return false;
    }


    // =========================================================
    // GET CURRENT AVATAR
    // =========================================================

    public string GetSelectedAvatarName()
    {
        if (availableAvatars == null ||
            availableAvatars.Count == 0)
        {
            return "";
        }

        if (selectedAvatarIndex < 0 ||
            selectedAvatarIndex >= availableAvatars.Count)
        {
            return "";
        }

        return availableAvatars[selectedAvatarIndex].avatarName;
    }


#if UNITY_EDITOR

    // =========================================================
    // CUSTOM INSPECTOR
    // =========================================================

    [CustomEditor(typeof(AvatarManager))]
    public class AvatarManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            AvatarManager manager =
                (AvatarManager)target;

            serializedObject.Update();


            // =================================================
            // MAIN ANIMATOR
            // =================================================

            SerializedProperty animatorProperty =
                serializedObject.FindProperty("animator");

            EditorGUILayout.PropertyField(
                animatorProperty
            );


            EditorGUILayout.Space(10);


            // =================================================
            // AVAILABLE AVATARS
            // =================================================

            EditorGUILayout.LabelField(
                "Available Avatars",
                EditorStyles.boldLabel
            );

            SerializedProperty avatarsProperty =
                serializedObject.FindProperty(
                    "availableAvatars"
                );

            EditorGUILayout.PropertyField(
                avatarsProperty,
                true
            );


            EditorGUILayout.Space(10);


            // =================================================
            // SELECTED AVATAR DROPDOWN
            // =================================================

            if (manager.availableAvatars != null &&
                manager.availableAvatars.Count > 0)
            {
                SerializedProperty indexProperty =
                    serializedObject.FindProperty(
                        "selectedAvatarIndex"
                    );

                string[] avatarNames =
                    new string[
                        manager.availableAvatars.Count
                    ];


                for (int i = 0;
                     i < manager.availableAvatars.Count;
                     i++)
                {
                    string name =
                        manager.availableAvatars[i].avatarName;

                    if (string.IsNullOrEmpty(name))
                    {
                        name = "Avatar " + i;
                    }

                    avatarNames[i] = name;
                }


                int currentIndex =
                    Mathf.Clamp(
                        indexProperty.intValue,
                        0,
                        avatarNames.Length - 1
                    );


                int newIndex =
                    EditorGUILayout.Popup(
                        "Selected Avatar",
                        currentIndex,
                        avatarNames
                    );


                if (newIndex != currentIndex)
                {
                    indexProperty.intValue =
                        newIndex;

                    serializedObject.ApplyModifiedProperties();

                    manager.ApplySelectedAvatar();

                    EditorUtility.SetDirty(manager);

                    return;
                }


                EditorGUILayout.Space(5);


                // Show selected avatar information

                AvatarEntry selected =
                    manager.availableAvatars[currentIndex];


                EditorGUILayout.HelpBox(
                    "Selected: " +
                    selected.avatarName,
                    MessageType.Info
                );
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Add avatars to Available Avatars first.",
                    MessageType.Info
                );
            }


            // =================================================
            // APPLY BUTTON
            // =================================================

            EditorGUILayout.Space(5);

            if (GUILayout.Button(
                "Apply Selected Avatar",
                GUILayout.Height(30)))
            {
                serializedObject.ApplyModifiedProperties();

                manager.ApplySelectedAvatar();

                EditorUtility.SetDirty(manager);
            }


            serializedObject.ApplyModifiedProperties();
        }
    }

#endif
}