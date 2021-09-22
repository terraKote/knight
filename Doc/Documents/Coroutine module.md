# Coroutine Management
* (Off-topic) Although many big guys on the Internet do not recommend using coroutines in Unity, it is undeniable that using coroutines in U3D for asynchronous processing can make the code very concise and beautiful. Especially when it is used in conjunction with U3D resource loading, it is really convenient. So I still highly recommend the use of coroutines for asynchronous processing in U3D.
* At present, all asynchronous operations in the project have been replaced with async await mode, which supports Unity Coroutine and async await to be mixed with each other, and free to change.

## Functions supported by the coroutine module
* Coroutine uses unified management, so that the startup of Ctrip no longer depends on MonoBehaviour.
* Realize the coroutine object returned with custom parameters like WWW/AssetBundleRequest to simplify the code structure of the coroutine.
* Support the use of coroutines on the hot update side.

## API provided by the module
* ![coroutine_1](/Doc/res/images/coroutine_1.png)
* The coroutine management provided in the framework can start a coroutine operation at any position without being restricted by MonoBehaviour.
```C#
CoroutineManager.cs
public CoroutineHandler StartHandler(IEnumerator rIEnum) // Start a controllable (stopped) coroutine
public Coroutine Start(IEnumerator rIEnum) // Start a common coroutine
public void Stop(CoroutineHandler rCoroutineHandler) // Stop a coroutine
```
* Ctrip object returned by custom parameters
```C#
public class LoaderRequest: CoroutineRequest<LoaderRequest>
{
    public Object obj;
    public string path;

    public LoaderRequest(string rPath)
    {
        this.path = rPath;
    }
}

public class CoroutineLoadTest
{
    public LoaderRequest mRequest;

    private LoaderRequest Load_Async(string rPath)
    {
        LoaderRequest rRequest = new LoaderRequest(rPath);
        mRequest = rRequest;
        rRequest.Start(Load(rRequest));
        return rRequest;
    }

    private IEnumerator Load(LoaderRequest rRequest)
    {
        yield return new WaitForSeconds(1.0f);
        rRequest.obj = new GameObject(rRequest.path);
        Debug.LogFormat("Create GameObject: {0}", rRequest.path);
    }

    public IEnumerator Loading(string rPath)
    {
        yield return Load_Async(rPath);
    }
}

public class CoroutineTest: MonoBehaviour
{
    IEnumerator Start()
    {
        CoroutineManager.Instance.Initialize();
        yield return CoroutineManager.Instance.Start(Start_Async());
    }

    IEnumerator Start_Async()
    {
        CoroutineLoadTest rTest = new CoroutineLoadTest();
        yield return CoroutineManager.Instance.Start(rTest.Loading("Test1"));
        yield return CoroutineManager.Instance.Start(rTest.Loading("Test2"));
        Debug.Log("Done.");
    }
}
```

* async await asynchronous mode
```C#
public class Init: MonoBehaviour
{
    public string HotfixABPath = "";
    public string HotfixModule = "";
    
    async void Start()
    {
        CoroutineManager.Instance.Initialize();
        wait Start_Async();
    }

    private async Task Start_Async()
    {
        await ABPlatform.Instance.Initialize();
        await ABUpdater.Instance.Initialize();

        await HotfixManager.Instance.Load(this.HotfixABPath, this.HotfixModule);

        await HotfixGameMainLogic.Instance.Initialize();

        Debug.Log("End init..");
    }
}

// Directly await Assetbundle.LoadFromFileAsync API
rAssetLoadEntry.CacheAsset = await AssetBundle.LoadFromFileAsync(rAssetLoadUrl);

// Directly await Assetbundle.LoadAssetAsync API
rRequest.Asset = await rAssetLoadEntry.CacheAsset.LoadAssetAsync(rRequest.AssetName);
```

## Issues that need attention
* ILRuntime library currently does not support the use of yield return xxx; operation in foreach.
```C#
// wrong usage
foreach (var rPair in rGameStageList)
{
    yield return rPair.Value.Run_Async();
}

// Correct usage: currently it can only be converted into a for loop to execute the coroutine
var rGameStageList = new List<KeyValuePair<int, GameStage>>(this.gameStages);
for (int i = 0; i <rGameStageList.Count; i++)
{
    yield return rGameStageList[i].Value.Run_Async();
}
```
* Currently, the hot update terminal does not support direct inheritance of the CoroutineRequest class, and the inheritance of template classes is not well supported in ILRuntime. On the hot update side, you can directly create a class to cache the CoroutineHandler object to simulate the implementation of the coroutine object with parameters. For specific usage, please refer to the ActorCreater.cs file in the hot update DLL.