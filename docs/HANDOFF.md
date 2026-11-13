# 当前交接

## 2026-11-13 · 0.1.0 已发布独立工坊条目

玩家澄清“发布”为上传工坊、订阅使用；保留已误发的 GitHub Release，后续仅发布工坊。已在 AppID 4348910 下创建新公开条目 3838955514：https://steamcommunity.com/sharedfiles/filedetails/?id=3838955514 。english 运行文件/封面/标题简介及 schinese 标题简介均收到 k_EResultOK，均不要求接受新协议，结果为 submitted；未做发布后远端复查。

独立上传工作区为仓库内 WorkshopUpload-BottleAndMachineIcon/（Git 忽略），未使用化学助手条目或其上传目录。运行文件只有 BottleAndMachineIcon.dll、manifest.xml、使用说明.txt。自动回执为 upload-result-0.1.0.json；条目编号保存在 docs/WorkshopItemId.txt，准备脚本会沿用该编号避免重复建项。新增 Tools/WorkshopPublisher 与 prepare-workshop/publish-workshop 脚本，使用当前 Steam 客户端会话，不启动游戏。

按玩家指定读取会话 01a1cfef-8fc1-7f72-a193-d2260fc1a121，采用已确认的 material-icon-layout.html 布局图作为工坊封面。docs/workshop-layout-source.html 保存来源 HTML，docs/workshop-preview.png 为渲染封面；明确标注非游戏截图，后续玩家补实图。没有修改游戏 UI 布局或运行时源码。

运行说明改为工坊订阅使用，随后重新 build.ps1：0 警告、0 错误，原生 ModScanner PASS（43 methods，capabilities none）。工坊 DLL SHA256 为 0B390E1F0A29E9942D2BADC950D2BE582C63B12F06CEEF2D0DEB451F997E30E4；三个上传文件与 build/validation.json 哈希一致。未新增或运行测试、部署、启动游戏、修改材料/存档/订阅缓存，也未修改原化学助手。游戏内验收仍待玩家完成；启用前确认其他模组同类绘制已停用。

## 2026-11-13 · 0.1.0 已发布 GitHub Release

用户手动推送提交 3bc0477b79d8ee41a5caaa5ed70de18275fa0f0c 后，明确要求继续发布。已创建并发布 v0.1.0，标签指向该提交，安装包与白名单源码包均已上传并核对附件状态和大小。

发布地址：https://github.com/KarlWang86/Probably-Stolen-BottleAndMachineIcon/releases/tag/v0.1.0 。本地附件及发布回执位于 releases/0.1.0-20261113-214101-596/，publish-receipt.json 记录 Release ID、附件 ID、大小及 SHA256。安装包 SHA256：6E4E04165958D1B65A164E779BA682672BB1D08DB3062A5D6A0260C2BD116BE7；源码包 SHA256：A1A91D3E81F3E1579854BFB05A095F993C45B7A6C6D34AFBF28CAFF542092D1F。

VERSION 保持 0.1.0。scripts/release.ps1 构建 0 警告、0 错误，原生 ModScanner PASS（43 methods，capabilities none）。DLL SHA256：ED4826A81078885B0201BEF43F0B1E4F5AE66FBE99B69022E52E9CDCE2DD6FDA；安装包内容已逐项对照扫描凭据核验。源码包不含游戏程序集、原生 IL、路径配置或旧助手二进制。

未新增或运行测试，未部署、启动游戏、修改存档/材料/订阅缓存或原化学助手，未上传工坊。尚无独立工坊条目，不沿用 3836818025 或其上传工作区。离线扫描不代表游戏内验收；玩家仍需按下方清单验收，启用前须确认其他模组同类绘制已停用。

## 2026-11-13 · 0.1.0 独立实现可构建，未发布

本轮按用户要求完成独立 mod 制作至可构建，发布留给新会话。VERSION 保持 0.1.0；开始时准备工作尚未提交，本轮未推进版本、提交或推送 Git。

运行时已完成：ModEntry 转交独立生命周期；IconRuntime 建立主线程 LateUpdate 节点，启用、禁用、加载存档及 PlayerStore 实例变化时清理；ItemReader 递归扫描玩家拥有物品、去重/深度保护并定位机器 container 槽；MaterialIcons 提取三类成分映射和图集绘制，无 ChemAssistPlus.dll 或 Harmony 依赖。

原布局算法和数值保留：容器中心纵排、机器本体左上横排、机器窗口容器覆盖、右上状态留白，读取原生图片边界/祖先裁切，实际图与拖影显隐及 alpha 同步。每 0.25 秒发现新对象，已显示对象逐帧检查成分和工作瓶变化。本轮未更改位置、方向、间距或缩放；尚未进入游戏检查裁切、缩放、旋转、拖动。

空蒸发器策略：只读 CHEMASSISTPLUS_EVAPORATOR_RECIPE_V1 已有标签；caffeine_pill → 咖啡豆，dream_dust/pink_alt_injector → 梦帽菇，nightmare_dust → 梦魇帽菇。仅无瓶或零液体时使用，未知/无标签不显示记忆，纯溶剂不回退。不创建/修改/清除标签，不迁入配方、自动加工、耗料、补瓶或混液确认。

最终 build.ps1 完成：0 警告、0 错误，原生 ModScanner PASS（43 methods，capabilities none）。构建位于 build/，运行文件 BottleAndMachineIcon.dll、manifest.xml、使用说明.txt；凭据 build/validation.json。DLL SHA256：ED4826A81078885B0201BEF43F0B1E4F5AE66FBE99B69022E52E9CDCE2DD6FDA。游戏程序集哈希仍为 725CB999D019DFF1FF31BED353C7F9BB8D81A7605A15DE1F80F5A3A6CF7B1D05。两个 SourceBaseline 快照哈希未变。初次沙箱构建无法读取用户 NuGet.Config，经权限复核运行标准构建成功。

未新增或运行测试、部署、打包、上传、启动游戏、操作材料、修改存档或订阅缓存；原化学助手仓库只读查阅，未修改。离线 PASS 不代表游戏内验收。

新会话从这里继续：

1. 本 mod 尚无工坊条目，不得沿用化学助手条目 3836818025 或上传工作区。发布需新条目和独立工作区。
2. 若要专项部署/游戏内验收，先记录同类模组状态，协调停用或移除旧助手图标；旧助手目前仍带旧绘制，本轮未处理。
3. 由玩家验收容器本体、机器本体、机器窗口中单种/多种原料，空瓶/溶剂、倒液、换瓶、干湿变化、旋转拖动、窗口裁切/缩放、禁用重启和切档。若要调整布局，按 AGENTS.md 先展示预览并获玩家确认。
4. Git 工作区仍含准备阶段及本轮未提交文件；推进新版本前先完成 0.1.0 提交。

## 2026-11-13 · 0.1.0 开发准备

用户要求将化学助手的容器与机器图标拆成独立 mod，以便并行开发。本轮先迁移所需文档、脚本和工具，原化学助手后续再移除实现，本轮未修改该仓库。

BottleAndMachineIcon/ 为独立骨架，ID bottle_and_machine_icon，版本 0.1.0。ModEntry 生命周期为空，图标尚未接入。构建、扫描、查询、部署和打包脚本已适配本项目，部署目标 Mods/BottleAndMachineIcon。未迁入绑定原条目的工坊脚本。

MaterialIcons.cs 和原开发说明在 Research/SourceBaseline/，已核对 SHA256；图标原生 IL 只保留本地，保持忽略。接口短结论在 NATIVE_API.md，来源及耦合点见 MIGRATION.md。

下一步按迁移清单提取只读物品扫描、槽位、成分映射及独立生命周期，明确空蒸发器配方记忆兼容策略，再接入现有布局。替代前协调原助手停用/移除旧绘制，避免重复。

已完成 build.ps1：0 警告、0 错误，原生 ModScanner PASS（6 methods，capabilities none）。DLL SHA256 为 C2760433198820164488E6B1CC9C30164105F3AB935C07CDE245B79F51CD5F3D。query-api.ps1 成功查询 GameItem.GetTagReadonly；所有脚本语法解析与 UTF8 BOM 检查通过。

本机游戏目录从 Steam 库配置确认位于 E:\SteamLibrary\steamapps\common\Probably Stolen，已写入忽略的 GameRoot.local.txt；游戏程序集 SHA256 为 725CB999D019DFF1FF31BED353C7F9BB8D81A7605A15DE1F80F5A3A6CF7B1D05。首次构建因沙箱无法读取用户 NuGet.Config 失败，标准构建与只读查询经权限复核后重试成功。

未新增或运行测试、部署、打包、上传、启动游戏、操作材料或修改存档/订阅缓存。未提交或推送 Git。离线 PASS 仅验证骨架与工具链，不代表图标游戏内验收。
