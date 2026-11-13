# 开发操作

当前发布版本为 0.1.0，独立图标运行时已接入并通过构建和原生扫描，游戏内验收待完成。GitHub Release 提供安装包与白名单源码包；本次不部署游戏、不上传工坊。脚本从化学助手迁入后已改用本 mod 标识。

## 环境与路径

运行时源码在 BottleAndMachineIcon/，文档 docs/，脚本 scripts/，工具 Tools/，研究资料 Research/，日常输出 build/。

仓库根目录 GameRoot.local.txt 填写游戏根目录，单行无引号且不提交；PROBABLY_STOLEN_GAME_ROOT 优先。项目引用与脚本共用配置，未配置时保留原脚本的父目录回退。

主 mod 目标 netstandard2.1，工具默认 net8.0。只有 .NET 9 时设置 `$env:PROBABLY_STOLEN_TOOL_FRAMEWORK = 'net9.0'`，主 mod 框架不变。脚本使用 UTF8 BOM，兼容 Windows PowerShell 5；NuGet.Config 清空外部包源，使用本机 SDK 引用包。

在仓库根目录使用前缀 `powershell -NoProfile -ExecutionPolicy Bypass -File`：

| 目标 | 接在前缀后的命令 | 输出与边界 |
| --- | --- | --- |
| 日常构建 | .\scripts\build.ps1 | 同步版本、编译、原生扫描；不部署/打包 |
| 重扫构建 | .\scripts\validate.ps1 | build/validation.json、验证结果.txt，记录游戏及运行文件哈希 |
| 查接口 | .\scripts\query-api.ps1 -Type GameItem -Method GetTagReadonly | 终端签名和 Research/ 完整结果 |
| 查 IL | .\scripts\query-api.ps1 -Type GameItem -Method GetTagReadonly -IL | 只读元数据，保存 Research/；类型/方法可单独用 * |
| 查参考程序集 | .\scripts\query-api.ps1 -Assembly <DLL路径> -Type <类型> -Method <方法> | 不执行目标方法 |
| 专项部署 | .\scripts\deploy.ps1 -WorkshopState Disabled | 确认同类模组禁用后，重扫并复制三个运行文件 |
| 同 ID 本地优先 | .\scripts\deploy.ps1 -WorkshopState SameIdLocalPriorityConfirmed | 仅在确认同 ID 优先级且无其他图标提供者冲突后使用 |
| 留档打包 | .\scripts\release.ps1 | releases/ 安装包和白名单源码包，不上传 |

运行文件为 BottleAndMachineIcon.dll、manifest.xml、使用说明.txt。部署到游戏 Mods/BottleAndMachineIcon。发布时显式运行 release.ps1 重新构建、扫描和打包；它不部署游戏或上传工坊。

VERSION 为唯一版本源，build 同步程序集、清单与使用说明。失败构建撤销旧验证凭据；扫描在 build/scan-input/ 的运行文件副本上执行，与工具二进制隔离，并核对原文件哈希。

## 图标开发

先读 MIGRATION.md。布局修改先展示预览并取得玩家确认，覆盖容器本体、机器本体、机器窗口及单种/多种原料；实现后检查裁切、缩放、旋转、拖动、禁用和切档。

原化学助手暂时保留图标实现，本轮不改该仓库。独立图标启用前须协调旧实现停用/移除，避免重复绘制。离线扫描通过只证明工程和扫描链路可用。

## 工坊

尚无独立条目。此次不迁入 prepare-workshop.ps1、publish-workshop.ps1 和 WorkshopPublisher，它们绑定化学助手条目、标题及工作区。后续配置新条目及独立工作区，不沿用 3836818025。Research/SourceBaseline/DEVELOPMENT.original.md 仅为来源存档，不能作为本仓库操作入口。
