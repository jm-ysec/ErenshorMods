---
type: always
description: Coding standards and style guidelines for ErenshorMods
---

# Coding Standards

## C# Code Style

### Plugin.cs Pattern
- **Keep Plugin.cs thin** - only initialization code
- Delegate all logic to separate classes
- Single responsibility: load Harmony patches, initialize config

```csharp
// Good: Thin Plugin.cs
public class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        Logger.LogInfo("ModName: Initializing...");
        ModNameConfig.Initialize(Config);  // Delegate config
        new Harmony("com.author.modname").PatchAll();  // Load patches
    }
}
```

### Harmony Patch Naming
- Use clear naming convention: `TargetClass_MethodName_Patch`
- Group related patches in nested classes

```csharp
// Good: Clear patch naming
[HarmonyPatch(typeof(PlayerController), "Move")]
public class PlayerController_Move_Patch
{
    private static void Prefix(ref float speed) { }
    private static void Postfix() { }
}
```

### Namespace Convention
- Use mod name as namespace: `namespace MyModName`
- Templates use placeholder: `__MOD_NAMESPACE__`

### MonoBehaviour Pattern
- Use singleton pattern for persistent controllers
- Call `DontDestroyOnLoad()` for cross-scene persistence

```csharp
public class ModController : MonoBehaviour
{
    public static ModController Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
```

## Configuration Style

### BepInEx Config
- Group settings by category
- Use `AcceptableValueRange` for numeric limits
- Provide clear descriptions

```csharp
EnableFeature = config.Bind(
    "General",           // Category
    "EnableFeature",     // Key
    true,                // Default
    "Enable the main feature"  // Description
);

SpeedMultiplier = config.Bind(
    "Movement",
    "SpeedMultiplier",
    1.5f,
    new ConfigDescription(
        "Speed multiplier for player movement",
        new AcceptableValueRange<float>(0.1f, 10f)
    )
);
```

## File Management

### Never Manually Edit
- **Package files** (`.csproj` dependencies) - use `dotnet add package`
- **mods.json** for complex changes - use `mod_registry.py`

### Always Use Automation
- Version bumping → `python version.py`
- Building → `python build.py`
- Packaging → `python package.py`
- Health checks → `python doctor.py`

## Template Placeholders

When creating mods from templates, these placeholders are replaced:

| Placeholder | Replaced With |
|-------------|---------------|
| `__MOD_NAMESPACE__` | Mod name (PascalCase) |
| `__MOD_CLASS_NAME__` | Mod class name |
| `__mod_id__` | Mod ID (lowercase) |
| `__Mod Display Name__` | Human-readable name |

