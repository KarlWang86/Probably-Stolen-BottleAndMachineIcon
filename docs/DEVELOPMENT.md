# 开发操作

当前发布版本为 0.1.0，独立图标运行时已接入并通过构建和原生扫描，游戏内验收待完成。发布指上传 Steam 工坊条目 3838955514，玩家订阅即可使用；已有 GitHub Release 保留，后续不再创建 GitHub Release。脚本从化学助手迁入后已改用本 mod 标识。

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

本模组独立条目为 [3838955514](https://steamcommunity.com/sharedfiles/filedetails/?id=3838955514)，AppID 4348910。编号保存在 docs/WorkshopItemId.txt，新机器准备工作区时沿用该编号；不得复用化学助手条目 3836818025 或其 WorkshopUpload 工作区。

本仓库 WorkshopUpload-BottleAndMachineIcon/ 为独立上传工作区，已忽略 Git。content/ 只含 BottleAndMachineIcon.dll、manifest.xml、使用说明.txt；配置、封面、简介、更新说明、备份与回执放 content/ 外。封面 docs/workshop-preview.png 来源于玩家指定会话已确认的布局图，原 HTML 保存在 docs/workshop-layout-source.html；属于布局示意，并非游戏截图，后续由玩家补实图。

显式发布步骤（在仓库根目录按上文 PowerShell 5 前缀执行）：

1. scripts/build.ps1：构建并原生扫描，不部署、不上传。
2. scripts/prepare-workshop.ps1：核对 VERSION、扫描凭据与三个运行文件哈希，备份既有准备目录，生成独立配置与上传内容。简介与更新说明源为 docs/WorkshopDescription.txt、docs/WorkshopChangelog.txt。
3. scripts/publish-workshop.ps1：复用在线 Steam 客户端会话，核对条目归属，提交 english 的运行内容/封面/简介，再同步 schinese 简介；不启动游戏。两次回调均成功且无需接受协议，即视为上传完成。简短结果保存在 WorkshopUpload-BottleAndMachineIcon/upload-result-<版本>.json。

可选 scripts/publish-workshop.ps1 -CheckOnly 仅作本地预检，不连接 Steam。仅中文提交失败时，可用 -SchineseOnly 重试；前提是已有同一批文件的 english 成功回执。首条创建成功后立即落盘编号，发生超时或失败时先检查配置/回执，不擅自重新创建条目。

不自动检查订阅缓存、启动游戏或部署本地副本；订阅下载和游戏内显示由玩家验收。原生扫描通过不代表游戏内验收。后续发布只上传工坊，不新增 GitHub Release；release.ps1 仍仅用于显式留档打包。
