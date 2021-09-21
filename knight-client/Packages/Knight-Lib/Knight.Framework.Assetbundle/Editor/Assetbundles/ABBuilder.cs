//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Knight.Core;
using Knight.Core.Editor;
using UnityEditor;

namespace Knight.Framework.AssetBundles.Editor
{
    /// <summary>
    /// Auxiliary class for resource packaging
    /// </summary>
    public class ABBuilder : TSingleton<ABBuilder>
    {
        public enum BuildPlatform
        {
            Windows     = BuildTarget.StandaloneWindows,        //Windows
            Windows64   = BuildTarget.StandaloneWindows64,      //Windows64
            OSX         = BuildTarget.StandaloneOSX,            //OSX
            IOS         = BuildTarget.iOS,                      //IOS
            Android     = BuildTarget.Android,                  //Android
        };

        /// <summary>
        /// The directory of the output Assetbundle
        /// </summary>
        public static string AssetbundlePath        = "Assets/../Assetbundles";

        /// <summary>
        /// Resource package configuration file path
        /// </summary>
        public static string ABEntryConfigPath      = "Assets/Game.Editor/Editor/Assetbundle_Settings.json";

        /// <summary>
        /// Resource pack prefix
        /// </summary>
        public static string ABPrefixRoot           = "game/";

        /// <summary>
        /// Resource path prefix
        /// </summary>
        public static string ABAssetPrefixRoot      = "Assets/Game/GameAsset/";

        /// <summary>
        /// The platform of the current project
        /// </summary>
        public BuildPlatform CurBuildPlatform       = BuildPlatform.Windows;

        /// <summary>
        /// Configuration cache of resource items
        /// </summary>
        public List<ABEntry> ABEntries;
    
        private ABBuilder()
        {
            CurBuildPlatform = GetCurrentBuildPlatform();
        }

        /// <summary>
        /// Get the path prefix of Assetbundle, choose according to different platforms
        /// </summary>
        public string GetPathPrefix_Assetbundle()
        {
            return Path.Combine(AssetbundlePath, GetManifestName()).Replace("\\", "/");
        }

        /// <summary>
        /// Get the name of the Manifest
        /// </summary>
        public string GetManifestName()
        {
            return GetCurrentBuildPlatformName() + "_Assetbundles";
        }

        /// <summary>
        /// Package resources
        /// </summary>
        public void BuildAssetbundles(BuildAssetBundleOptions rOptions)
        {
            List<AssetBundleBuild> rABBList = AssetbundleEntry_Building();
            
            string rABPath = GetPathPrefix_Assetbundle();
            DirectoryInfo rDirInfo = new DirectoryInfo(rABPath);
            if (!rDirInfo.Exists) rDirInfo.Create();

            var rOldABVersion = ABVersionEditor.Load(rABPath);
            var rOldMD5 = ABVersionEditor.GetMD5ForABVersion(rABPath);

            // Start packing
            var rNewABManifest = BuildPipeline.BuildAssetBundles(rABPath, rABBList.ToArray(), rOptions, (BuildTarget)CurBuildPlatform);
            if (rNewABManifest == null)
            {
                Debug.Log("BuildPipeline.BuildAssetBundles() return null, " + rABPath);
                return;
            }
            // Generate a new version file
            var rNewABVersion = ABVersionEditor.CreateVersion(rABPath, rOldABVersion, rNewABManifest);
            rNewABVersion.SaveInEditor(rABPath);

            var rNewMD5 = ABVersionEditor.GetMD5ForABVersion(rABPath);

            // Save historical version records
            if (!string.IsNullOrEmpty(rOldMD5) && !rOldMD5.Equals(rNewMD5))
            {
                rOldABVersion.SaveHistory(rABPath);
            }
            Debug.Log("Resource packaging is complete!");
        }

        /// <summary>
        /// Configuration items for generating AB package
        /// </summary>
        public List<ABEntry> GenerateABEntries()
        {
            var rABEntryConfig = EditorAssists.ReceiveAsset<ABEntryConfig>(ABEntryConfigPath);
            if (rABEntryConfig == null) return new List<ABEntry>();
            return rABEntryConfig.ABEntries;
        }

        /// <summary>
        /// Build the path, package name, and package suffix of the resources that need to be packaged
        /// </summary>
        public List<AssetBundleBuild> AssetbundleEntry_Building()
        {
            this.ABEntries = this.GenerateABEntries();
            if (ABEntries == null) ABEntries = new List<ABEntry>();

            // Preprocessing atlas configuration
            ABUIAtlasTools.GenerateAtlas();

            // Resource preprocessing
            List<ABEntryProcessor> rABEntryProcessors = new List<ABEntryProcessor>();
            foreach (var rEntry in ABEntries)
            {
                ABEntryProcessor rProcessor = ABEntryProcessor.Create(rEntry);
                rProcessor.PreprocessAssets();
                rProcessor.ProcessAssetBundleLabel();
                rABEntryProcessors.Add(rProcessor);
            }
            // Pack
            List<AssetBundleBuild> rABBList = new List<AssetBundleBuild>();
            foreach (var rProcessor in rABEntryProcessors)
            {
                rABBList.AddRange(rProcessor.ToABBuild());
            }
            return rABBList;
        }

        public void UpdateAllAssetsABLabels(string aBEntryConfigPath)
        {
            this.ABEntries = this.GenerateABEntries();
            if (ABEntries == null) ABEntries = new List<ABEntry>();

            // Resource preprocessing
            List<ABEntryProcessor> rABEntryProcessors = new List<ABEntryProcessor>();
            foreach (var rEntry in ABEntries)
            {
                ABEntryProcessor rProcessor = ABEntryProcessor.Create(rEntry);
                rProcessor.PreprocessAssets();
                rProcessor.ProcessAssetBundleLabel();
                rABEntryProcessors.Add(rProcessor);
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }
        
        public static BuildPlatform GetCurrentBuildPlatform()
        {
            return (BuildPlatform)EditorUserBuildSettings.activeBuildTarget;
        }

        public static string GetCurrentBuildPlatformName()
        {
            string rPlatformName = GetCurrentBuildPlatform().ToString();
            if (rPlatformName.Equals("Windows64"))
            {
                rPlatformName = "Windows";
            }
            return rPlatformName;
        }
    }
}