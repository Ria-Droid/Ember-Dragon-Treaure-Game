#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

using AnimatorController = UnityEditor.Animations.AnimatorController;

public static class BuildEnemyAnimations
{
    private class Setup
    {
        public string name;
        public string sourcePrefab;
        public float forwardSign = 1f;
        public float walkPeriod;
        public float bobHeight;
        public float swayDegrees;
        public float leanDegrees;
        public float lungeDistance;
        public float lungePitch;
        public float attackLength;
        public float dieLength;
        public float dieSink;
    }

    private static readonly Setup[] Setups =
    {
        new Setup
        {
            name = "Troll",
            sourcePrefab = "Assets/Prefabs/Troll Variant.prefab",
            forwardSign = 1f,
            walkPeriod = 1.2f,
            bobHeight = 0.18f,
            swayDegrees = 4f,
            leanDegrees = 0f,
            lungeDistance = 0.8f,
            lungePitch = 18f,
            attackLength = 0.7f,
            dieLength = 1.0f,
            dieSink = 0.3f
        },
        new Setup
        {
            name = "Skinwalker",
            sourcePrefab = "Assets/Prefabs/Skinwalker Variant.prefab",
            forwardSign = 1f,
            walkPeriod = 0.7f,
            bobHeight = 0.14f,
            swayDegrees = 3f,
            leanDegrees = 10f,
            lungeDistance = 1.0f,
            lungePitch = 22f,
            attackLength = 0.5f,
            dieLength = 1.0f,
            dieSink = 0.3f
        }
    };

    [MenuItem("Ember/Build Troll + Skinwalker Animations")]
    public static void BuildAll()
    {
        int built = 0;

        foreach (Setup setup in Setups)
        {
            if (Build(setup))
                built++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"BuildEnemyAnimations: built {built} of {Setups.Length} enemies.");
    }

    private static bool Build(Setup s)
    {
        GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(s.sourcePrefab);

        if (source == null)
        {
            Debug.LogError($"BuildEnemyAnimations: could not find {s.sourcePrefab}");
            return false;
        }

        string animFolder = "Assets/Animation/" + s.name;
        string prefabPath = "Assets/Prefabs/" + s.name + " Animated.prefab";

        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            bool rebuild = EditorUtility.DisplayDialog(
                "Build enemy animations",
                $"\"{s.name} Animated\" already exists. Rebuild it? This replaces its clips and controller.",
                "Rebuild",
                "Skip"
            );

            if (!rebuild)
                return false;

            if (AssetDatabase.IsValidFolder(animFolder))
                AssetDatabase.DeleteAsset(animFolder);
        }

        EnsureFolder("Assets/Animation");
        EnsureFolder(animFolder);

        AnimationClip walk = MakeWalkClip(s, animFolder);
        AnimationClip attack = MakeAttackClip(s, animFolder);
        AnimationClip die = MakeDieClip(s, animFolder);

        AnimatorController controller =
            MakeController(s, animFolder, walk, attack, die);

        Scene scratch = EditorSceneManager.NewPreviewScene();

        try
        {
            GameObject model =
                (GameObject)PrefabUtility.InstantiatePrefab(source, scratch);

            PrefabUtility.UnpackPrefabInstance(
                model,
                PrefabUnpackMode.Completely,
                InteractionMode.AutomatedAction
            );

            GameObject root = new GameObject(s.name);
            SceneManager.MoveGameObjectToScene(root, scratch);

            root.transform.SetPositionAndRotation(
                Vector3.zero,
                Quaternion.identity
            );

            root.transform.localScale = model.transform.localScale;

            Component[] components = model.GetComponents<Component>();
            System.Collections.Generic.List<Component> moved =
                new System.Collections.Generic.List<Component>();

            foreach (Component c in components)
            {
                if (c == null)
                    continue;

                if (c is Transform ||
                    c is MeshFilter ||
                    c is MeshRenderer ||
                    c is SkinnedMeshRenderer ||
                    c is Animator)
                    continue;

                if (ComponentUtility.CopyComponent(c) &&
                    ComponentUtility.PasteComponentAsNew(root))
                {
                    moved.Add(c);
                }
                else
                {
                    Debug.LogWarning(
                        $"{s.name}: could not copy {c.GetType().Name} to the new root."
                    );
                }
            }

            foreach (Component c in moved)
                Object.DestroyImmediate(c);

            model.name = "Model";
            model.transform.SetParent(root.transform, false);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;

            Animator animator = model.GetComponent<Animator>();

            if (animator == null)
                animator = model.AddComponent<Animator>();

            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;

            EnemyBase enemy = root.GetComponent<EnemyBase>();

            if (enemy != null)
            {
                enemy.animator = animator;
            }
            else
            {
                Debug.LogWarning(
                    $"{s.name}: no EnemyBase found on the new root, assign the Animator by hand."
                );
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            EditorSceneManager.ClosePreviewScene(scratch);
        }

        GameObject newPrefab =
            AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        int swapped = SwapInWaves(source, newPrefab);

        Debug.Log(
            $"BuildEnemyAnimations: {s.name} -> {prefabPath} " +
            $"(replaced {swapped} wave slot(s) in the open scene's WaveManager)."
        );

        return true;
    }

    private static AnimationClip MakeWalkClip(Setup s, string folder)
    {
        float p = s.walkPeriod;
        float h = s.bobHeight;
        float sway = s.swayDegrees;
        float lean = s.leanDegrees * s.forwardSign;

        return MakeClip(
            folder + "/" + s.name + "_Walk.anim",
            true,
            p,
            Curve(
                (0f, 0f),
                (p * 0.25f, h),
                (p * 0.5f, 0f),
                (p * 0.75f, h),
                (p, 0f)
            ),
            Curve((0f, 0f), (p, 0f)),
            Curve((0f, lean), (p, lean)),
            Curve(
                (0f, -sway),
                (p * 0.5f, sway),
                (p, -sway)
            )
        );
    }

    private static AnimationClip MakeAttackClip(Setup s, string folder)
    {
        float a = s.attackLength;
        float lean = s.leanDegrees * s.forwardSign;
        float pitch = s.lungePitch * s.forwardSign;
        float lunge = s.lungeDistance * s.forwardSign;

        return MakeClip(
            folder + "/" + s.name + "_Attack.anim",
            false,
            a,
            Curve(
                (0f, 0f),
                (a * 0.4f, 0f),
                (a, 0f)
            ),
            Curve(
                (0f, 0f),
                (a * 0.4f, lunge),
                (a, 0f)
            ),
            Curve(
                (0f, lean),
                (a * 0.4f, lean + pitch),
                (a, lean)
            ),
            Curve((0f, 0f), (a, 0f))
        );
    }

    private static AnimationClip MakeDieClip(Setup s, string folder)
    {
        float d = s.dieLength;
        float lean = s.leanDegrees * s.forwardSign;
        float fall = -90f * s.forwardSign;

        return MakeClip(
            folder + "/" + s.name + "_Die.anim",
            false,
            d,
            Curve(
                (0f, 0f),
                (d, -s.dieSink)
            ),
            Curve((0f, 0f), (d, 0f)),
            Curve(
                (0f, lean),
                (d * 0.8f, fall),
                (d, fall)
            ),
            Curve((0f, 0f), (d, 0f))
        );
    }

    private static AnimationClip MakeClip(
        string path,
        bool loop,
        float length,
        AnimationCurve posY,
        AnimationCurve posZ,
        AnimationCurve rotX,
        AnimationCurve rotZ)
    {
        AnimationClip clip = new AnimationClip();
        clip.frameRate = 30f;

        AnimationCurve flat =
            Curve((0f, 0f), (length, 0f));

        SetCurve(
            clip,
            "m_LocalPosition.x",
            flat
        );

        SetCurve(
            clip,
            "m_LocalPosition.y",
            posY
        );

        SetCurve(
            clip,
            "m_LocalPosition.z",
            posZ
        );

        SetCurve(
            clip,
            "localEulerAnglesRaw.x",
            rotX
        );

        SetCurve(
            clip,
            "localEulerAnglesRaw.y",
            flat
        );

        SetCurve(
            clip,
            "localEulerAnglesRaw.z",
            rotZ
        );

        AnimationClipSettings settings =
            AnimationUtility.GetAnimationClipSettings(clip);

        settings.loopTime = loop;

        AnimationUtility.SetAnimationClipSettings(
            clip,
            settings
        );

        AssetDatabase.CreateAsset(clip, path);

        return clip;
    }

    private static void SetCurve(
        AnimationClip clip,
        string property,
        AnimationCurve curve)
    {
        AnimationUtility.SetEditorCurve(
            clip,
            EditorCurveBinding.FloatCurve(
                "",
                typeof(Transform),
                property
            ),
            curve
        );
    }

    private static AnimationCurve Curve(
        params (float t, float v)[] keys)
    {
        AnimationCurve curve = new AnimationCurve();

        foreach ((float t, float v) in keys)
            curve.AddKey(new Keyframe(t, v));

        for (int i = 0; i < curve.length; i++)
            curve.SmoothTangents(i, 0f);

        return curve;
    }

    private static AnimatorController MakeController(
        Setup s,
        string folder,
        AnimationClip walk,
        AnimationClip attack,
        AnimationClip die)
    {
        AnimatorController controller =
            AnimatorController.CreateAnimatorControllerAtPath(
                folder + "/" + s.name + "Controller.controller"
            );

        controller.AddParameter(
            "Attack",
            UnityEngine.AnimatorControllerParameterType.Trigger
        );

        controller.AddParameter(
            "Die",
            UnityEngine.AnimatorControllerParameterType.Trigger
        );

        AnimatorStateMachine machine =
            controller.layers[0].stateMachine;

        AnimatorState walkState =
            machine.AddState("Walk");

        walkState.motion = walk;

        AnimatorState attackState =
            machine.AddState("Attack");

        attackState.motion = attack;

        AnimatorState dieState =
            machine.AddState("Die");

        dieState.motion = die;

        machine.defaultState = walkState;

        AnimatorStateTransition toAttack =
            machine.AddAnyStateTransition(attackState);

        toAttack.AddCondition(
            AnimatorConditionMode.If,
            0f,
            "Attack"
        );

        toAttack.hasExitTime = false;
        toAttack.duration = 0.05f;
        toAttack.canTransitionToSelf = false;

        AnimatorStateTransition backToWalk =
            attackState.AddTransition(walkState);

        backToWalk.hasExitTime = true;
        backToWalk.exitTime = 0.95f;
        backToWalk.duration = 0.1f;

        AnimatorStateTransition toDie =
            machine.AddAnyStateTransition(dieState);

        toDie.AddCondition(
            AnimatorConditionMode.If,
            0f,
            "Die"
        );

        toDie.hasExitTime = false;
        toDie.duration = 0.1f;
        toDie.canTransitionToSelf = false;

        return controller;
    }

    private static int SwapInWaves(
        GameObject oldPrefab,
        GameObject newPrefab)
    {
        WaveManager manager =
            Object.FindFirstObjectByType<WaveManager>();

        if (manager == null || manager.waves == null)
            return 0;

        int swapped = 0;

        Undo.RecordObject(
            manager,
            "Swap enemy prefabs"
        );

        foreach (WaveManager.Wave wave in manager.waves)
        {
            if (wave.enemyPrefabs == null)
                continue;

            for (int i = 0; i < wave.enemyPrefabs.Length; i++)
            {
                if (wave.enemyPrefabs[i] == oldPrefab)
                {
                    wave.enemyPrefabs[i] = newPrefab;
                    swapped++;
                }
            }
        }

        if (swapped > 0)
        {
            EditorUtility.SetDirty(manager);

            EditorSceneManager.MarkSceneDirty(
                manager.gameObject.scene
            );
        }

        return swapped;
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent =
            Path.GetDirectoryName(path).Replace('\\', '/');

        EnsureFolder(parent);

        AssetDatabase.CreateFolder(
            parent,
            Path.GetFileName(path)
        );
    }
}
#endif