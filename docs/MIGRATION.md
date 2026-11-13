# 图标拆分准备

来源：Probably-Stolen-ChemAssistPlus，版本 0.4.9，提交 10397795bf7473d661a45ba6b77b9590fa99309f；复制日期 2026-11-13（香港时间）。本轮只准备独立仓库，原仓库不作修改。

## 已迁入

- common/build/validate/query-api/deploy/release 脚本、NativeScan 与 AssemblyQuery 源码，已替换路径、DLL、ID、版本文案和部署目录。
- Directory.Build.props、NuGet.Config 和适用的忽略规则。
- MaterialIcons.cs 原样快照及原 DEVELOPMENT.md，位于 Research/SourceBaseline/；sources.json 记录来源文件和 SHA256。
- 图标接口短结论摘入 NATIVE_API.md。containericons-*、icon-refresh-* 原生 IL 复制到本地 Research/，忽略提交且不进入源码包。
- 独立空生命周期入口，仅用于验证编译和扫描链路，不表示图标功能完成。

## 原迁移清单（2026-11-13 已完成实现，游戏内验收待办）

1. MaterialIcons.cs 属于 ChemAssistPlus.Plus 分部类。提取独立显示管理器，更换 ChemAssistPlus.MaterialIcon 节点名称，接入独立启用、更新、禁用和切档清理。
2. Scan() 依赖 Plus/SceneSnapshot 的玩家物品递归；只迁入只读扫描，保留拥有物品筛选、去重及循环/深度保护。
3. Core.MachineView.Walk(...).Container 依赖原生机器槽位遍历；只迁入最小槽位适配层，不迁入加工按钮或其他业务。
4. LiveLiquidRecipe 依赖 Core.RecipeCatalog，且被化学助手蒸发器分工使用。图标只需原材料映射，不应迁入整套加工配方；原助手移除图标时须保留其业务仍需的实际成分读取能力。
5. MachineMaterialMask 的空蒸发器标记依赖 EvaporatorRoute 和 CHEMASSISTPLUS_EVAPORATOR_RECIPE_V1 标签。后续明确是否只读已有标签、无助手时如何显示；不得为显示写入分工标签。准备阶段不擅自改变该行为。
6. InvalidateMaterialIcons 由 MixingGuard 原生倒液 Postfix 调用。已有节点逐帧比较掩码与工作瓶，0.25 秒发现新对象；独立后自行刷新，不依赖另一 mod 的补丁。
7. 保留原布局：容器可见瓶图中心纵排，机器可见本体左上横排、右上留给状态 mod；窗口槽位容器覆盖，实际图/拖影显隐与 alpha 同步。改布局前先展示预览。
8. 实现后原生扫描，再由玩家核查空瓶、单/多原料、替换瓶子、倒液、干湿变化、旋转拖动、窗口裁切、缩放、禁用和切档。

## 并行开发交接

独立运行时已落地：IconRuntime、ItemReader、MaterialIcons。只读槽位适配仅提取 container，未迁入加工入口、RecipeCatalog 或完整 Core。原布局绘制算法保留，刷新由独立 LateUpdate 驱动，切档事件与 PlayerStore 身份变化清理，不依赖 MixingGuard。

空蒸发器兼容策略已确定：保留对 CHEMASSISTPLUS_EVAPORATOR_RECIPE_V1 的 GetTagReadonly 读取，静态映射 caffeine_pill、dream_dust、pink_alt_injector、nightmare_dust；只在工作瓶为空或无瓶时回退。无标签不显示记忆，不创建/更改标签。其他产品标签不映射。这维持旧绘制的空设备行为而不带入设备分配业务。

化学助手后续移除旧绘制入口、节点和专用刷新调用；共享成分读取、配方判断及蒸发器分工仍属原 mod 业务，须检查调用者后保留。独立图标替代前须协调停用旧实现，避免重复绘制。本轮未修改原源码、版本或工坊配置。

暂不迁入加工文档、完整历史更新记录、发布回执、截图和全部研究资料，独立 mod 不沿用原助手发布历史。未来工坊使用新条目和独立工作区。
