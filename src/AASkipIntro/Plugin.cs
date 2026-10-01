using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace AASkipIntro;

[BepInPlugin(Id, "极速启动", "0.1.0")]
[BepInProcess("AzureArchive.exe")]
public sealed class Plugin : BasePlugin
{
    public const string Id = "halocue.aa.skipintro";
    private Harmony? _harmony;

    public override void Load()
    {
        _harmony = new Harmony(Id);
        try
        {
            _harmony.PatchAll(typeof(Plugin).Assembly);
            Log.LogInfo("极速启动 enabled: AA creator splash is skipped; native initialization is preserved.");
        }
        catch
        {
            _harmony.UnpatchSelf();
            throw;
        }
    }

    public override bool Unload()
    {
        _harmony?.UnpatchSelf();
        return true;
    }
}
