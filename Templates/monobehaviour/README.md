# Erenshor BepInEx Mod Template

## Project Structure

```
MyModName/
├── MyModName.csproj    # Build configuration and references
├── MyModName.sln       # Solution file (optional, for IDE support)
├── Plugin.cs           # BepInEx entry point — thin loader only
└── MyModLogic.cs       # All mod functionality (Harmony patches, game logic)
```

### Design Pattern

**Plugin.cs** is the BepInEx entry point. It should only:
- Initialize Harmony and call `PatchAll()`
- Log startup messages

**ModName.cs** is a static class containing all Harmony patch classes. This keeps
mod logic cleanly separated from the loading framework.

## Setup

### 1. Create the project

Copy the template files and replace all placeholders:

| Placeholder | Example |
|---|---|
| `__MOD_NAMESPACE__` | `ErenshorMyMod` |
| `__MOD_ASSEMBLY_NAME__` | `MyModName` |
| `__MOD_DESCRIPTION__` | `My cool Erenshor mod` |
| `__MOD_CLASS_NAME__` | `MyModLogic` |
| `__mod_id__` | `my_mod` |
| `__Mod Display Name__` | `My Cool Mod` |

Rename `ModName.csproj` and `ModName.cs` to match your mod.

### 2. Create a solution file (optional)

```bash
dotnet new sln -n MyModName
dotnet sln add MyModName.csproj
```

### 3. Verify paths

The `.csproj` assumes:
- **Game install:** `~/.steam/steam/steamapps/common/Erenshor`
- **Gale profile:** `~/.local/share/com.kesomannen.gale/erenshor/profiles/Default/BepInEx`

Update `GamePath` and `GalePath` in the `.csproj` if your paths differ.

### 4. Add Unity module references

The template includes `UnityEngine` and `UnityEngine.CoreModule`. If your mod
needs additional modules (UI, IMGUI, etc.), add them to the `<ItemGroup>`:

```xml
<Reference Include="UnityEngine.UI">
  <HintPath>$(ManagedPath)/UnityEngine.UI.dll</HintPath>
  <Private>False</Private>
</Reference>
```

## Building

```bash
dotnet build
```

The PostBuild target automatically copies the DLL to your Gale profile's plugins
folder. Verify the `DestinationFolder` in the `.csproj` matches where BepInEx
expects to find your plugin.

## Harmony Patch Reference

### Patching a single method (no overloads)

```csharp
[HarmonyPatch(typeof(TargetClass), "MethodName")]
public class TargetClass_MethodName_Patch
{
    private static void Postfix() { }
}
```

### Patching a method with a specific signature

```csharp
[HarmonyPatch(typeof(TargetClass), "MethodName", new[] { typeof(string), typeof(int) })]
public class TargetClass_MethodName_Patch
{
    private static void Prefix(string param1, int param2) { }
}
```

### Patching all overloads of a method

Use `TargetMethods()` when a method has multiple overloads to avoid
`AmbiguousMatchException`:

```csharp
[HarmonyPatch]
public class TargetClass_MethodName_Patch
{
    static IEnumerable<MethodBase> TargetMethods()
    {
        return AccessTools.GetDeclaredMethods(typeof(TargetClass))
            .Where(m => m.Name == "MethodName");
    }

    private static void Prefix(/* first shared param */) { }
}
```

## Testing

BepInEx/Harmony mods require a full game restart to pick up a new DLL.
There is no hot-reload. After building, restart Erenshor completely.

Check the BepInEx log at:
```
<GalePath>/LogOutput.log
```

