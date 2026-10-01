# “极速启动”边界验证

The only hook is a postfix on CreatorSplash.Awake. The native Awake runs first,
then the mod disables that presentation object. Native OnDisable owns input
restoration and completion. No settings are written and no asset is removed.

Current binary evidence (read-only):

| Native method | RVA | Observed boundary |
|---|---|---|
| CreatorSplash.Awake | 0x992A10 | Hides both pages and saves/blocks UI input |
| CreatorSplash.OnDisable | 0x992CC0 | Marks complete, restores saved input state |
| CreatorSplash.Start.MoveNext | 0x997310 | Awaits engine splash, displays AA pages, finally disables itself |
| CreatorSplash.ShowPage.MoveNext | 0x997100 | Presentation fade/hold only |
| ScenarioResourceManager.InitializeAfterSplash.MoveNext | 0x7CC280 | Waits until presenter completes or becomes inactive, then initializes |

Implementation uses methods/properties, never RVA patches or hardcoded offsets.
GameAssembly SHA256:
`BD45C2DFBA4EE59A3A3007E34B53B401985B838D66FFDEBAC863C8527948A80F`.
Assembly-CSharp interop SHA256:
`AFA74D22354E75803E63E8D39F48C4FDE4F13379BA4400BC12C0E880EAEBB11C`.

Four tests link the actual production postfix against host-boundary doubles:
restoring both previous input states via the disable callback, one-time completion,
null presenter and failure fallback. Build-only checks confirm the beta bindings.
These do not prove native Unity callback dispatch or startup visuals.

Manual acceptance remaining: enable only through AA's mod manager, restart,
confirm author pages are absent, catalog appears after normal resource loading,
mouse input works, and disabling the mod restores pages on the next restart.
AA was already running during development; no desktop interaction or forced restart
was used. Unity's own engine splash is outside this mod's scope.
