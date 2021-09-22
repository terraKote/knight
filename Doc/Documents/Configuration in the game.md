# Configuration in the game
* The processing of the game configuration file in the framework is to finally process the Excel table into a serialized binary file. Then package it into an Assetbundle resource package for game use.
* The data type corresponding to the configuration in the game is on the hot update side.

## ExcelReader
* The framework provides a tool to parse Excel into Json strings, ExcelReader, and the menu path is Tools/AssetBudle/Export Excel Config.
* ![gameconfig_1](/Doc/res/images/gameconfig_1.png)
* At present, the path of the Excel table is fixed: it can only be in the Assets/Game/Knight/GameAsset/Config/Excel directory. Of course, you can also modify the code of the export tool to modify the path.
* There is a tabular data format configuration file excel_format_config.json under the Assets/Editor/ExcelReader folder, which needs to be configured for the exported data.
* ![gameconfig_2](/Doc/res/images/gameconfig_2.png)
* ExcelName: The file name of the Excel table
* SheetName: Sheet name of the table
* ClassName: The class name in the corresponding code of the table data
* PrimaryKey: The configuration will be uniformly exported as a json string in Dictionary format, so you need to find a column in the table to make the Dict Key value

* The tool also supports the export of a single table.
* ![gameconfig_3](/Doc/res/images/gameconfig_3.png)

## Serialization of Json data
* There is a GameConfig class in the hot update DLL, which is used to store all game configuration data.
* ![gameconfig_4](/Doc/res/images/gameconfig_4.png)

* The serialization code of a single data is automatically generated by a serialization tool, and the configuration only needs to inherit the HotfixSerializerBinary class. The HotfixSBGroup tag is used to group configurations.
```C#
    [HotfixSBGroup("GameConfig")]
    public partial class ActorHero: HotfixSerializerBinary
    {
        public int ID;
        public string Name;
        public int AvatarID;
        public int SkillID;
        public float Scale;
        public float Height;
        public float Radius;
    }
```
* In the hot update project, a SerializerBinaryEditor tool is provided to automatically generate the serialization and deserialization code of the configuration class, eliminating the need to write serialization code yourself.
* ![gameconfig_5](/Doc/res/images/gameconfig_5.png)
* The generated serialization code is in knight\HotfixModule\KnightHotfixModule\Knight\Generate\SerializerBinary, as shown below.
```C#
using System.IO;
using Core;
using WindHotfix.Core;

/// <summary>
/// Automatic file generation does not need to be done again! If a compilation error occurs, it will be automatically generated after deleting the file
/// </summary>
namespace Game.Knight
{
	public partial class ActorHero
	{
		public override void Serialize(BinaryWriter rWriter)
		{
			base.Serialize(rWriter);
			rWriter.Serialize(this.ID);
			rWriter.Serialize(this.Name);
			rWriter.Serialize(this.AvatarID);
			rWriter.Serialize(this.SkillID);
			rWriter.Serialize(this.Scale);
			rWriter.Serialize(this.Height);
			rWriter.Serialize(this.Radius);
		}
		public override void Deserialize(BinaryReader rReader)
		{
			base.Deserialize(rReader);
			this.ID = rReader.Deserialize(this.ID);
			this.Name = rReader.Deserialize(this.Name);
			this.AvatarID = rReader.Deserialize(this.AvatarID);
			this.SkillID = rReader.Deserialize(this.SkillID);
			this.Scale = rReader.Deserialize(this.Scale);
			this.Height = rReader.Deserialize(this.Height);
			this.Radius = rReader.Deserialize(this.Radius);
		}
	}
}
```

* Before resource packaging, the packaging tool will first re-serialize the configured json data into a binary data file. The file path is Assets\Game\Knight\GameAsset\Config\GameConfig\Binary\GameConfig.bytes. Pack the binary file again.

## TODO
* The above method is suitable for games with less configuration. For games with more configuration data, such a configuration processing method takes up Mono memory. A solution for storing configuration data in sqlite will be added later.