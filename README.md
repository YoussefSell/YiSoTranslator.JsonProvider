# YiSoTranslator Json Provider
is a translation provider for the [YiSoTranslator](https://github.com/YoussefSell/YiSoTranslator) Plugin, which will give you the ability to store and retrieve the translations using JSON files.

## How it works
the JSON Provider give you a way for storing and retrieving translation from JSON Files and that is base on implementing the functions described by [`IYiSoTranslationProvider`](https://github.com/YoussefSell/YiSoTranslator/blob/master/YiSoTranslator/Interfaces/IYiSoTranslationProvider.cs) interface:
* Add
* AddRange
* Update
* Clear
* Remove
* Reload
* SaveChanges
* SaveToFile
* ReadFromFile
* Find
* IsExist
* Contains
* IndexOf

we also expose two events :
* `TranslationsGroupsListChanged` : Raised when a change is made to the translations Groups List.
* `DataSourceChanged` : raised when a change is made to the file that contain the translation , in case of deletion of the file or the file has been modified.

in order to use `DataSourceChanged` and be able to lessen on changes on the file you need to activate that by setting the `AllowDataSourceChangedEvent` to true : 
```
var provider = new YiSoTranslatorJsonProvider("fileName"){
  AllowDataSourceChangedEvent = true
}
```

in case the file that hold the translation is deleted you can create a backup for the file by setting the `CreateBackupOnFileDeletion` to true
```
var provider = new YiSoTranslatorJsonProvider("fileName"){
  AllowDataSourceChangedEvent = true,
  CreateBackupOnFileDeletion = true
}
```

the provider will store the translation inside folder called Translations that you should create in your project, than you place your json files inside the folder, than you can refer to that file by creating a `JsonTranslationFile` like so : 
```
var JsonFile = new JsonTranslationFile("filename");
```

note : you don't supply the extension, just give the filename without extension. and we will look for the file inside the Translation Folder

if you want to work with a file outside the translation folder you can do that by supplying the full path of the file and set the `IsFullPath` flag to true :
```
var filePath = @"YourFullPathToTheFile\file.json";
var JsonFile = new JsonTranslationFile("filePath", true);
```

to start using the provider you need to provide a `JsonTranslationFile`
or you just pass the file name if the file is included in the project inside the Translation folder:
```
var provider = new YiSoTranslatorJsonProvider("fileName");
```
or
```
var JsonFile = new JsonTranslationFile("filename");
var provider = new YiSoTranslatorJsonProvider(JsonFile);
```
or
```
var filePath = @"YourFullPathToTheFile\file.json";
var JsonFile = new JsonTranslationFile("filePath", true);
var provider = new YiSoTranslatorJsonProvider(JsonFile);
```

all the three methods are valid you just need to choose the right one for you.

## Getting Started
add the YiSoTranslator.JsonProvider package to your project by using he nuget package manager or nuget package manager console:  
`PM> Install-Package YiSoTranslator.JsonProvider`  
