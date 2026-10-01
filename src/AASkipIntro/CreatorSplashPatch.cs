using HarmonyLib;
using UnityEngine;

namespace AASkipIntro;

[HarmonyPatch(typeof(CreatorSplash), nameof(CreatorSplash.Awake))]
internal static class CreatorSplashPatch
{
    // Run after the native Awake has stored its input state. Disabling this
    // one presenter invokes its native OnDisable, which restores input and
    // sets IsComplete. Resource initialization observes completion normally.
    // No loading/scene/animation coroutine or global timescale is patched.
    internal static void Postfix(CreatorSplash __instance)
    {
        if (__instance == null) return;
        try
        {
            var presenter = __instance.gameObject;
            if (presenter == null || !presenter.activeSelf) return;
            presenter.SetActive(false);
            Debug.Log("极速启动: startup presentation skipped.");
        }
        catch (Exception error)
        {
            // Leave the native startup path in control on an incompatible host.
            Debug.LogWarning("极速启动: could not skip startup (" + error.GetType().Name + ").");
        }
    }
}
