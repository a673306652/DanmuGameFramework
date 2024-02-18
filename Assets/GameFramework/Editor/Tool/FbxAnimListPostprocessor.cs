// FbxAnimListPostprocessor.cs : Use an external text file to import a list of 
// splitted animations for FBX 3D models.
//
// Put this script in your "Assets/Editor" directory. When Importing or 
// Reimporting a FBX file, the script will search a text file with the 
// same name and the ".txt" extension.
// File format: one line per animation clip "firstFrame-lastFrame loopFlag animationName"
// The keyworks "loop" or "noloop" are optional.
// Example:
// 0-50 loop Move forward
// 100-190 die
// 0-50 loop Move 回调方法|触发时间|参数

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Daan;
public class FbxAnimListPostprocessor : AssetPostprocessor
{
    public void OnPreprocessModel()
    {
        if (System.IO.Path.GetExtension(assetPath).ToLower() == ".fbx"
            && !assetPath.Contains("@"))
        {
            try
            {
                // Remove 6 chars because dataPath and assetPath both contain "assets" directory
                string fileAnim = Application.dataPath + System.IO.Path.ChangeExtension(assetPath, ".txt").Substring(6);
                StreamReader file = new StreamReader(fileAnim);

                string sAnimList = file.ReadToEnd();
                file.Close();

                //if (EditorUtility.DisplayDialog("FBX Animation Import from file",
                //    fileAnim, "Import", "Cancel"))
                //{


                //    EditorUtility.DisplayDialog("Imported animations",
                //        "Number of imported clips: "
                //        + modelImporter.clipAnimations.GetLength(0).ToString(), "OK");
                //}


                System.Collections.ArrayList List = new ArrayList();
                ParseAnimFile(sAnimList, ref List);

                ModelImporter modelImporter = assetImporter as ModelImporter;
                //modelImporter.splitAnimations = true;
                modelImporter.clipAnimations = (ModelImporterClipAnimation[])
                    List.ToArray(typeof(ModelImporterClipAnimation));

                if (modelImporter.animationType == ModelImporterAnimationType.Legacy)
                {
                    ParseClipFile(sAnimList);
                }
            }
            catch { }
            // (Exception e) { EditorUtility.DisplayDialog("Imported animations", e.Message, "OK"); }
        }
    }

    void ParseAnimFile(string sAnimList, ref ArrayList List)
    {
        Regex regexString = new Regex(" *(?<firstFrame>[0-9]+) *- *(?<lastFrame>[0-9]+) *(?<loop>(loop|noloop| )) *(?<name>[^\r^\n]*[^\r^\n^ ])",
            /*RegexOptions.Compiled |*/ RegexOptions.ExplicitCapture);
        Match match = regexString.Match(sAnimList, 0);

        while (match.Success)
        {
            ModelImporterClipAnimation clip = new ModelImporterClipAnimation();

            if (match.Groups["firstFrame"].Success)
                clip.firstFrame = System.Convert.ToInt32(match.Groups["firstFrame"].Value, 10);

            if (match.Groups["lastFrame"].Success)
                clip.lastFrame = System.Convert.ToInt32(match.Groups["lastFrame"].Value, 10);

            if (match.Groups["loop"].Success)
            {
                string loop = match.Groups["loop"].Value;
                clip.loopTime = loop == "loop";
            }
                

            if (match.Groups["name"].Success)
            {
                Debug.Log("name:" + match.Groups["name"].Value);
                


                //clip.name = match.Groups["name"].Value;
                string[] dataArr = match.Groups["name"].Value.Split('|');
                clip.name = dataArr[0];

                AnimationRootEditor are = MyIO.JsonDeserialize<AnimationRootEditor>(dataArr[1]);
                clip.loopTime = are.isLoop;
                
                List<AnimationEvent> aeArr = new List<AnimationEvent>();
                for (int nI = 0;nI < are.AnimationEvent.Count;nI++)
                {
                    AnimationEvent ae = new AnimationEvent();
                    ae.functionName = are.AnimationEvent[nI].func;
                    ae.time = are.AnimationEvent[nI].time;
                    ae.stringParameter = are.AnimationEvent[nI].parameter;
                    aeArr.Add(ae);
                }

                clip.lockRootRotation = are.RootRotation.islock;
                clip.keepOriginalOrientation = are.RootRotation.upon;
                clip.rotationOffset = are.RootRotation.offset;

                clip.lockRootHeightY = are.RootPositionY.islock;
                clip.keepOriginalPositionY = are.RootPositionY.upon;
                clip.heightOffset = are.RootPositionY.offset;

                clip.lockRootPositionXZ = are.RootPositionXZ.islock;
                clip.keepOriginalPositionXZ = are.RootPositionXZ.upon;
                //clip.cycleOffset = are.RootPositionXZ.offset;

                clip.events = aeArr.ToArray();
            }

            List.Add(clip);
            match = regexString.Match(sAnimList, match.Index + match.Length);
        }
    }

    void ParseClipFile(string sAnimList)
    {
        Regex regexString = new Regex(" *(?<firstFrame>[0-9]+) *- *(?<lastFrame>[0-9]+) *(?<loop>(loop|noloop| )) *(?<name>[^\r^\n]*[^\r^\n^ ])",
            /*RegexOptions.Compiled |*/ RegexOptions.ExplicitCapture);
        Match match = regexString.Match(sAnimList, 0);

        Object[] tmObj = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        List<AnimationClip> clipList = new List<AnimationClip>();
        for (int i = 0; i < tmObj.Length; i++)
        {
            if (tmObj[i] != null && tmObj[i] is AnimationClip)
            {
                AnimationClip tmClip = tmObj[i] as AnimationClip;

                AnimationClip newClip = new AnimationClip();
                try
                {
                    EditorUtility.CopySerialized(tmClip, newClip);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                }

                string targetPath = System.IO.Path.GetDirectoryName(assetPath);
                targetPath += "/" + tmClip.name + ".anim";
                AssetDatabase.CreateAsset(newClip, targetPath);
                clipList.Add(newClip);
            }
        }
        //AssetDatabase.Refresh();

        while (match.Success)
        {
            if (match.Groups["name"].Success)
            {
                string[] dataArr = match.Groups["name"].Value.Split(' ');
                string clipName = dataArr[0];

                for (int i = 0; i < clipList.Count; i++)
                {
                    AnimationClip clip = clipList[i] as AnimationClip;
                    if (clipName == clip.name)
                    {
                        float firstFrame = 0f;
                        float lastFrame = 0f;
                        if (match.Groups["firstFrame"].Success)
                            firstFrame = System.Convert.ToInt32(match.Groups["firstFrame"].Value, 10);

                        if (match.Groups["lastFrame"].Success)
                            lastFrame = System.Convert.ToInt32(match.Groups["lastFrame"].Value, 10);

                        if (match.Groups["loop"].Success)
                        {
                            if (match.Groups["loop"].Value == "loop")
                            {
                                clip.wrapMode = WrapMode.Loop;
                            }
                        }

                        AnimationEvent[] aeArr = new AnimationEvent[dataArr.Length - 1];
                        for (int nI = 1; nI < dataArr.Length; nI++)
                        {
                            string[] eventDataArr = dataArr[nI].Split('|');
                            AnimationEvent ae = new AnimationEvent();
                            ae.functionName = eventDataArr[0];
                            float time = (float)System.Convert.ToDouble(eventDataArr[1]);
                            float lenght = (lastFrame - firstFrame) / clip.frameRate;
                            ae.time = lenght * time;
                            if (eventDataArr.Length > 2) ae.stringParameter = eventDataArr[2];
                            aeArr[nI - 1] = ae;
                        }

                        AnimationUtility.SetAnimationEvents(clip, aeArr);
                    }
                }
            }
            match = regexString.Match(sAnimList, match.Index + match.Length);
        }
    }
}

#region 动画设置配置文件
    [System.Serializable]
    public class RootRotation
    {
        /// <summary>
        /// 
        /// </summary>
        public bool islock;
        /// <summary>
        /// 
        /// </summary>
        public bool upon;
        /// <summary>
        /// 
        /// </summary>
        public float offset;
    }

    [System.Serializable]
    public class RootPositionY
    {
        /// <summary>
        /// 
        /// </summary>
        public bool islock;
        /// <summary>
        /// 
        /// </summary>
        public bool upon;
        /// <summary>
        /// 
        /// </summary>
        public float offset;
}

    [System.Serializable]
    public class RootPositionXZ
    {
        /// <summary>
        /// 
        /// </summary>
        public bool islock;
        /// <summary>
        /// 
        /// </summary>
        public bool upon;
        /// <summary>
        /// 
        /// </summary>
        public float offset;
}

    [System.Serializable]
    public class AnimationEventItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string func;
        /// <summary>
        /// 
        /// </summary>
        public float time;
        /// <summary>
        /// 
        /// </summary>
        public string parameter;
    }

    public class AnimationRootEditor
    {
        public bool isLoop;

        public RootRotation RootRotation;
            /// <summary>
            /// 
            /// </summary>
        public RootPositionY RootPositionY;
            /// <summary>
            /// 
            /// </summary>
        public RootPositionXZ RootPositionXZ;
            /// <summary>
            /// 
            /// </summary>
        public List<AnimationEventItem> AnimationEvent;
    }
#endregion
