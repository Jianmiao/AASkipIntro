using AASkipIntro;
using UnityEngine;

int passed = 0;
foreach (bool inputPreviouslyBlocked in new[] { false, true })
{
    var splash = new CreatorSplash(inputPreviouslyBlocked);
    CreatorSplashPatch.Postfix(splash);
    Check(!splash.gameObject.activeSelf, "startup presenter was not hidden");
    Check(splash.IsComplete, "native disable completion was not invoked");
    Check(splash.InputBlocked == inputPreviouslyBlocked, "native input restoration was bypassed");
    Check(splash.NativeDisableCalls == 1, "native disable path must run exactly once");
    CreatorSplashPatch.Postfix(splash);
    Check(splash.NativeDisableCalls == 1, "repeat calls must not repeat completion");
    passed++;
}
var unavailable = new CreatorSplash(false);
unavailable.gameObject.ThrowOnDisable = true;
CreatorSplashPatch.Postfix(unavailable);
Check(unavailable.gameObject.activeSelf && !unavailable.IsComplete, "failure must leave startup in control");
Check(Debug.WarningCount == 1, "failure should emit one non-content diagnostic");
passed++;
CreatorSplashPatch.Postfix(null!);
passed++;
Console.WriteLine($"{passed}/4 passed: production patch dispatch, input restoration callback, idempotence and failure fallback; native Unity dispatch requires AA acceptance.");

static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }

// Only the host boundary is simulated. The linked production postfix remains
// unchanged. Native OnDisable's body is not copied into the distributable mod.
public sealed class CreatorSplash
{
    public GameObject gameObject { get; }
    public bool IsComplete { get; private set; }
    public bool InputBlocked { get; private set; }
    public int NativeDisableCalls { get; private set; }
    public CreatorSplash(bool previousInput)
    {
        InputBlocked = true;
        gameObject = new GameObject(() => { NativeDisableCalls++; InputBlocked = previousInput; IsComplete = true; });
    }
    public void Awake() { }
}

namespace UnityEngine
{
    public sealed class GameObject
    {
        private readonly Action _disable;
        public bool activeSelf { get; private set; } = true;
        public bool ThrowOnDisable { get; set; }
        public GameObject(Action disable) => _disable = disable;
        public void SetActive(bool active)
        {
            if (ThrowOnDisable) throw new InvalidOperationException("test unavailable host");
            if (active == activeSelf) return;
            activeSelf = active;
            if (!active) _disable();
        }
    }
    public static class Debug
    {
        public static int WarningCount;
        public static void Log(string value) { }
        public static void LogWarning(string value) { WarningCount++; }
    }
}

namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class HarmonyPatch : Attribute
    {
        public HarmonyPatch(Type type, string name) { }
    }
}
