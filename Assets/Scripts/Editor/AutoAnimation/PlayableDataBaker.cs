using System.Collections.Generic;
using System.IO;
using Firearms;
using UnityEditor;
using UnityEngine;

namespace Editor.AutoAnimation
{
    public class PlayableDataBaker : EditorWindow
    {
        public GameObject characterPrefab;
        public string savePath = "Assets/WeaponData"; 
        public List<AnimationClip> clipsToBake = new List<AnimationClip>();

        private SerializedObject so;
        private SerializedProperty clipsProp;

        [MenuItem("Tools/武器数据烘焙器")]
        public static void ShowWindow() => GetWindow<PlayableDataBaker>("武器烘焙器");

        private void OnEnable()
        {
            so = new SerializedObject(this);
            clipsProp = so.FindProperty("clipsToBake");
        }

        void OnGUI()
        {
            so.Update();
            characterPrefab = (GameObject)EditorGUILayout.ObjectField("角色原型", characterPrefab, typeof(GameObject), true);
        
            EditorGUILayout.BeginHorizontal();
            savePath = EditorGUILayout.TextField("存储路径", savePath);
            if (GUILayout.Button("选择", GUILayout.Width(50)))
            {
                string folder = EditorUtility.OpenFolderPanel("选择保存文件夹", "Assets", "");
                if (!string.IsNullOrEmpty(folder))
                {
                    if (folder.Contains(Application.dataPath))
                        savePath = "Assets" + folder.Replace(Application.dataPath, "");
                    else
                        Debug.LogWarning("请选择工程目录(Assets)内的文件夹！");
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(clipsProp, new GUIContent("待烘焙动画列表"), true);

            if (GUILayout.Button("一键批量烘焙到文件夹", GUILayout.Height(40)))
            {
                ExecuteBake();
            }
            so.ApplyModifiedProperties();
        }

        void ExecuteBake()
        {
            if (characterPrefab == null) { EditorUtility.DisplayDialog("错误", "请指定角色原型！", "确定"); return; }
            // 检查文件夹是否存在
            if (!AssetDatabase.IsValidFolder(savePath)) {
                EditorUtility.DisplayDialog("警告", $"路径不存在或无效: {savePath}\n请手动创建文件夹或重新选择。", "确定");
                return;
            }
            if (clipsToBake.Count == 0) { EditorUtility.DisplayDialog("警告", "动画列表为空！", "确定"); return; }

            Animator anim = characterPrefab.GetComponent<Animator>();
            Transform handR = anim.GetBoneTransform(HumanBodyBones.RightHand);
            Transform handL = anim.GetBoneTransform(HumanBodyBones.LeftHand);
            Transform chest = anim.GetBoneTransform(HumanBodyBones.Chest);

            int count = 0;
            foreach (AnimationClip clip in clipsToBake)
            {
                if (clip == null) continue;

                string assetName = clip.name + "Asset.asset";
                string fullPath = Path.Combine(savePath, assetName).Replace("\\", "/");
            
                // 修正：从磁盘加载现有资源
                WeaponClipAsset asset = AssetDatabase.LoadAssetAtPath<WeaponClipAsset>(fullPath);
            
                if (asset == null)
                {
                    // 修正：创建新实例并保存
                    asset = ScriptableObject.CreateInstance<WeaponClipAsset>();
                    asset.handWeight = AnimationCurve.Constant(0, 1, 1);
                    asset.followWeight = AnimationCurve.Constant(0, 1, 1);
                    AssetDatabase.CreateAsset(asset, fullPath);
                }

                asset.clipName = clip.name;
                InitCurves(asset);

                float frameRate = clip.frameRate;
                int frameCount = Mathf.CeilToInt(clip.length * frameRate);

                for (int i = 0; i <= frameCount; i++)
                {
                    float time = i / frameRate;
                    clip.SampleAnimation(characterPrefab, time);

                    Vector3 rPos = chest.InverseTransformPoint(handR.position);
                    Vector3 rRot = (Quaternion.Inverse(chest.rotation) * handR.rotation).eulerAngles;
                    AddKeyToPos(asset.rPosX, asset.rPosY, asset.rPosZ, time, rPos);
                    AddKeyToRot(asset.rRotX, asset.rRotY, asset.rRotZ, time, rRot);

                    Vector3 lPos = chest.InverseTransformPoint(handL.position);
                    Vector3 lRot = (Quaternion.Inverse(chest.rotation) * handL.rotation).eulerAngles;
                    AddKeyToPos(asset.lPosX, asset.lPosY, asset.lPosZ, time, lPos);
                    AddKeyToRot(asset.lRotX, asset.lRotY, asset.lRotZ, time, lRot);
                }

                EditorUtility.SetDirty(asset); // 标记文件已被修改，需要保存
                count++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("成功", $"已成功烘焙 {count} 个动画资源文件！", "确定");
        }

        void InitCurves(WeaponClipAsset asset)
        {
            asset.rPosX = new AnimationCurve(); asset.rPosY = new AnimationCurve(); asset.rPosZ = new AnimationCurve();
            asset.rRotX = new AnimationCurve(); asset.rRotY = new AnimationCurve(); asset.rRotZ = new AnimationCurve();
            asset.lPosX = new AnimationCurve(); asset.lPosY = new AnimationCurve(); asset.lPosZ = new AnimationCurve();
            asset.lRotX = new AnimationCurve(); asset.lRotY = new AnimationCurve(); asset.lRotZ = new AnimationCurve();
        }

        void AddKeyToPos(AnimationCurve x, AnimationCurve y, AnimationCurve z, float t, Vector3 pos)
        {
            x.AddKey(t, pos.x); y.AddKey(t, pos.y); z.AddKey(t, pos.z);
        }

        void AddKeyToRot(AnimationCurve x, AnimationCurve y, AnimationCurve z, float t, Vector3 rot)
        {
            x.AddKey(t, rot.x); y.AddKey(t, rot.y); z.AddKey(t, rot.z);
        }
    }
}