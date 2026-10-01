# 极速启动（AA 开场跳过）

独立的 AA 开场动画跳过 Mod。启用“极速启动”（内部 ID：`AASkipIntro`）后，跳过启动时的 Logo / 作者展示页；
停用并重启 AA 后恢复原样。没有额外设置开关。

保留 AA 的数据库、字体、素材和用户设置初始化，不跳过作品内的片头、标题或转场。
本 Mod 针对 AA 自己的 CreatorSplash，不修改 Unity 引擎 Logo 或程序文件。

## 安装

推荐直接在 AA 的 `mods` 目录执行：

```powershell
git clone https://github.com/Jianmiao/aa-skip-intro.git AASkipIntro
```

仓库根目录中的 `0.1.0` 就是 AA 要读取的版本目录，不需要再移动文件。
也可以下载 Release 压缩包，将其中的 `mods/AASkipIntro/0.1.0` 放入 AA 的
`mods/AASkipIntro`。在 AA 模组管理中启用“极速启动”，然后按 AA 提示重启。
停用后下一次启动会恢复动画。安装本身不会改变其他 Mod 的启用状态。

## 验证范围

目标为本机 `AzureArchive_Win_1.0_beta` 的 BepInEx 6 IL2CPP 环境。
已核对原生 CreatorSplash.Awake/OnDisable 及资源初始化等待边界。
编译与四项生产补丁逻辑测试通过；尚未以启用/停用两次实际启动验证视觉效果。

```powershell
dotnet build src/AASkipIntro -c Release -p:AAInstallPath='你的AA目录'
dotnet run --project tests/AASkipIntro.Tests -c Release
```

源码与测试保存在独立目录；不更改 HaloCue、StoryForge 或视频导出 Mod 的实现。
不分发 AA 程序、数据库、素材或生成的 interop DLL。第三方引用均由本机 AA 提供。
