## How to use:

Download the `CustomAppAPI.dll` file in the mod download. <br/>
Add the `CustomAppAPI.dll` to your references in Visual Studio along with your BepInEx Plugin. <br/>

Create a new class which inherits from the `CustomApp` class. <br/>
Implement the inherited abstract members, and you're ready to go! <br/>

## Implementation

The `DisplayName` will be the app name that shows up on the home screen of the phone. <br/>
The `Icon` will be the texture that shows up next to the app name. <br/>

A static `LoadTexture` function is provided in `CustomApp` to load a `Texture2D` from a file path. <br/>
To get the file path to your mod .dll, `Path.GetDirectoryName(BaseUnityPlugin.Info.Location)` will get you the folder your mod is located in, where `BaseUnityPlugin` is your main plugin class. You may need a static accessor to get the instance of it.

Example Code:
```
public class MyMod : BaseUnityPlugin {

    private static MyMod _instance;
    public static MyMod Instance => _instance;
    public string ModFolderPath => Path.GetDirectoryName(Info.Location);

    private void Awake() {
        _instance = this;
    }

}

public class MyCustomApp : CustomApp {

    public override string DisplayName => "My Custom App";
    public override Texture2D Icon => LoadTexture(Path.Combine(MyMod.Instance.ModFolderPath, "myAppIcon.png"));

}
```