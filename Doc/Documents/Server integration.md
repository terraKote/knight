# Server integration
## Pomelo server
* The Pomelo server used in the framework is not the latest version. Version 1.2.2 used.
* In order to run the framework Demo conveniently, the login process of the server has been blocked in the framework.
* Pomelo's client plug-in is not the original version. I have modified it and replaced the Json parsing library inside with WindJson, so that it can perfectly fit with the other logic of the framework.
* At present, Pomelo's server only has the login function. Since the server is nodejs and this open source library is not maintained now, the framework does not plan to continue the development of game logic on this server in the later period. It has been connected to a server Egametang based on .Net Core to realize dual-end development using C#.
* The version of Pomelo server is backed up on the unity5.x branch. Address: https://github.com/winddyhe/knight/tree/unity5.x

### Build Pomelo server locally
* Due to the version, the node_modules in the Pomelo server game-server is not downloaded directly with npm. There is a node_modules.7z compressed package in the knight/Server/pomelo-knight folder, unzip it to the game-server directory That's it.
* Mongodb is used as the database in the framework. Click the start-mongodb-knight.bat file in the knight/Server/mongodb-win32-i386-2.0.4 folder to run the mongodb database.
* Use vscode to open game-server, you can debug and run pomelo server. You can also run the server directly with the command line.

* If you want to open the server login process, open the server login code 1 in LoginView.cs of the hot change DLL, and block the code 2 that directly creates the role.
* ![Pomelo_1](/Doc/res/images/pomelo_1.png)

## Egametang server
* The Egametang server is a .Net Core server in the Egametang open source framework. I extracted the client network module in the ET framework and integrated it into knight.
* Independent ET network module library: https://github.com/winddyhe/egametang-network-client
* The address of the Egametang framework: https://github.com/egametang/Egametang
* The operation of the Egametang server is very simple, just open server.sln with vs2017 and run it directly.