# knight
Support a wave of 996.ICU
<a href="https://996.icu"><img src="https://img.shields.io/badge/link-996.icu-red.svg" alt="996.icu"></a>

Knight is a game GamePlay framework based on the Unity engine. It provides some easy-to-use game framework interfaces to allow developers to focus more on the development of game content.

It contains a complete resource management module (package, download, load, version management), a C# hot update module based on ILRuntime, a UI framework based on MVVM (support hot update) and other basic functions.

The framework will continue to be updated, and the content in the framework will be revised and improved in the future. The current version of Unity is Unity2019.1.2f1.
At present, all modules in the Master branch have been moved to Packages, and PackageManager is used to manage them, so that they can be plugged and unplugged at any time during use.

![knight's frame structure](./Doc/res/images/img_1.png)

### Update log (2019/5/13)
* Update the Unity version to Unity2019.1.0f2.
* Refactoring of UI modules, ViewModel can be reused between UI modules, and pure data information can be accessed through ViewModelManager anywhere in the world.
* UI module refactoring, adding DatabindingConvert converter, can support the conversion between BindOneWay two different types of variables.
* Refactoring of the UI module, adding a DatabindingRelated associated tag, supporting a variable in the ViewModel is changed, and other associated variables will be updated to the UI along with it.
* UI module refactoring, through Mono.Ceil static injection method, so that ViewModel does not need to write this.PropChanged by hand.
* The UI module is refactored, and the resource loading method of the UI is changed to synchronous loading to avoid interface flicker.
* The Tweening module has been refactored, adding a multi-stage tween animation class, TweeningAnimator.

### Update log (2019/3/31)
* All modules in the framework are completely decoupled. Except for the Knight.Core module, which is a necessary public dependency module, each module is divided into a single Package. You can use these framework modules selectively.
* ILRuntime is updated to the latest version, supports Unity 2018.3 and above, and can debug asynchronous logic with breakpoints.
* The hot update logic is managed in Assets, and Unity itself is used to compile and automatically generate a .bytes DLL file. There is no need to open a VS and manually compile the hot update project.
* Assetbundle resource management module, which supports Editor to run the game directly without building an Assetbundle operation.
* Temporarily remove the ET server part, but the client network module continues to remain.

### run game
* Run the Tools/Assetbundle/Assetbundle Build command in the menu to build the Assetbundle resource package. If you check Tools/Develope Mode and Simulate Mode, you can skip this step.
* Open the Assets/Game/Scene/Game.unity scene and click Play to run the game Demo.

### Main function introduction
* [Framework](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/%E6%A1%86%E6%9E%B6%E7%BB%93%E6%9E%84.md)
* [Assetbundle resource module](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/Assetbundle%E8%B5%84%E6%BA%90%E6%A8%A1%E5%9D%97.md)
* [ILRuntime Hot Update Module](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/ILRuntime%E7%83%AD%E6%9B%B4%E6%96%B0%E6%A8%A1%E5%9D%97.md)
* [WindJson](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/WindJson.md)
* [WindUI](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/WindUI.md)
* [Coroutine Coroutine Module](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/Coroutine%E5%8D%8F%E7%A8%8B%E6%A8%A1%E5%9D%97.md)
* [Server Integration](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/%E6%9C%8D%E5%8A%A1%E5%99%A8%E9%9B%86%E6%88%90.md)
* [Configuration in the game](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/%E6%B8%B8%E6%88%8F%E4%B8%AD%E7%9A%84%E9%85%8D%E7%BD%AE.md)

### Plugins (thanks to the following plugins and frameworks for supporting the underlying functions of knight)
* ILRuntime: A library written in C# to explain and run C# IL code, used to implement the hot update mechanism, address: https://github.com/Ourpalm/ILRuntime
* ET: A two-terminal unity game framework that includes a distributed .Net Core server. Knight uses its server part. Address: https://github.com/egametang/ET
* NaughtyAttributes: A script Inspector UI extension library, Editor extensions implemented through Attribute tags. Address: https://github.com/dbrizov/NaughtyAttributes

### Contact information
* Email: hgplan@126.com
* QQ group: 651543479