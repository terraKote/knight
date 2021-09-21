//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Knight.Framework;
using Knight.Core;
using Knight.Framework.Hotfix;
using System.Threading.Tasks;
using UnityEngine.UI;
using Knight.Framework.AssetBundles;
using Knight.Framework.Net;
using UnityFx.Async;

namespace Game
{
    /// <summary>
    /// Game initialization
    /// </summary>
    internal class Init : MonoBehaviour
    {
        public string HotfixABPath = "";
        public string HotfixModule = "";

        async void Start()
        {
            // Frame limit
            Application.targetFrameRate = 30;

            // Initialize the event manager
            EventManager.Instance.Initialize();

            // Initialize the coroutine manager
            CoroutineManager.Instance.Initialize();

            TypeResolveManager.Instance.Initialize();

            TypeResolveManager.Instance.AddAssembly("Game");
            TypeResolveManager.Instance.AddAssembly("Game.Hotfix" , true);

            // Initialize the hot update module
            HotfixRegister.Register();
            HotfixManager.Instance.Initialize();

            // Initialize the UI module
            UIRoot.Instance.Initialize();

            // Initialize loading progress bar
            GameLoading.Instance.LoadingView = LoadingView_Knight.Instance;
            GameLoading.Instance.StartLoading(0.5f, "In the initial stage of the game, start loading resources...");

            // Asynchronous initialization code
            await Start_Async();
        }


        private async Task Start_Async()
        {
            // Platform initialization
            await ABPlatform.Instance.Initialize();

            // Resource download module initialization
            await ABUpdater.Instance.Initialize();

            // Initialize the resource loading module
            AssetLoader.Instance = new ABLoader();
            AssetLoader.Instance.Initialize();

            GameLoading.Instance.Hide();
            GameLoading.Instance.StartLoading(1.0f, "In the initial stage of the game, start loading resources...");

            // Load hot update code resources
            await HotfixManager.Instance.Load(this.HotfixABPath, this.HotfixModule);

            // Start the main game logic of the hot update terminal
            await HotfixGameMainLogic.Instance.Initialize();

            Debug.Log("End init..");
        }
    }
}
