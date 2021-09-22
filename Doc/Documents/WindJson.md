# WindJson parsing library
* The lexical analysis algorithm in the WindJson parsing library adopts the lexical state transition matrix algorithm. The code of the algorithm is very concise. The analysis of each category of words is highly abstracted into a state transition matrix for representation. At the same time, the lexical and grammatical analysis of Json is abstracted There are two levels, so the logical structure of the code becomes very simple. Friends who are interested can read LexicalAnalysis.cs and JsonParser.cs files.

## Features supported by WindJson
* Support standard Json format analysis
* Conveniently convert between object/jsonstring/jsonnode
* Compatible with repeated commas and semicolons
* Support the recognition of enumerated types and true/false keywords
* Support // /**/ Recognition of comments
* The Json library can be used on the hot update side

## WindJson API
* jsonstring ==> jsonnode
```C#
JsonParser rJsonParser = new JsonParser(rText);
JsonNode rJsonNode = rJsonParser.Parser();
```
* jsonnode ==> object
```C#
var rObjet = rJsonNode.ToObject(rObjectType);
var rObjet1 = rJsonNode.ToObject<ObjectType>(); // To object

var rObjectList = rJsonNode.ToList<ObjectType>(); // To list

var rObjectDict = rJsonNode.ToDict<TKey, TObject>(); // To dictionary
```
* object ==> jsonnode
```C#
JsonNode rJsonNode = JsonParser.ToJsonNode(rObject);
```

* jsonnode ==> jsonstring
```C#
string rJsonString = rJsonNode.ToString();
```

## Test case
![JsonParser1](/Doc/res/images/json_1.png)
![JsonParser1](/Doc/res/images/json_2.png)

## Expansion of lexical analysis algorithm
* This lexical analysis algorithm can parse out numbers, identifiers, strings, comments, some special characters, etc., so we can use it to analyze other formats of configuration.
* A configuration in a special format is parsed in the framework. This is a skill configuration. For details on the algorithm, please refer to GamePlayComponentParser.cs in the hot change DLL.
* ![JsonParser1](/Doc/res/images/json_3.png)