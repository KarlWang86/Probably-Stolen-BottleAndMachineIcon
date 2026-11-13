# Probably Stolen 容器与机器图标

从化学助手拆分的独立 mod，便于并行开发容器与机器的原材料标识。

当前发布版本为 **0.1.0**：独立图标实现已接入并通过编译与原生扫描，游戏内验收待完成。在 [Steam 创意工坊](https://steamcommunity.com/sharedfiles/filedetails/?id=3838955514)订阅，下载后在游戏模组列表启用。已有 GitHub Release 保留，后续仅发布工坊。

沿用原有显示：容器本体中心纵排，机器本体左上横排，操作窗口容器槽同样覆盖；咖啡豆、梦帽菇和梦魇帽菇按实际成分显示。同步实际图片与拖影的显隐和透明度，不修改液体、形状或存档。

空蒸发器只读兼容化学助手已有的原料记忆标签；无标签时不生成记忆。独立运行不依赖 ChemAssistPlus.dll。若旧版化学助手或其他模组仍提供同类图标，启用前须停用其同类绘制，避免重复图标。

在仓库根目录的 GameRoot.local.txt 填写游戏根目录（单行、无引号，不提交），或设置 PROBABLY_STOLEN_GAME_ROOT，再运行：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\build.ps1
```

见 [开发操作](docs/DEVELOPMENT.md)、[当前交接](docs/HANDOFF.md)、[迁移清单](docs/MIGRATION.md)和[架构](docs/ARCHITECTURE.md)。代码使用 [MIT License](LICENSE)，游戏素材及原生程序集不随源码分发。
