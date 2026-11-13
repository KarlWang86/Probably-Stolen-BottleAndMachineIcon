# 目录与边界

| 位置 | 职责 |
| --- | --- |
| BottleAndMachineIcon/ModEntry.cs | 独立 IMod 入口，转交启用/禁用 |
| BottleAndMachineIcon/IconRuntime.cs | 独立持久化更新节点、LateUpdate、切档/禁用清理、异常退避 |
| BottleAndMachineIcon/ItemReader.cs | 玩家拥有物品只读扫描、机器 container 槽定位，去重及深度保护 |
| BottleAndMachineIcon/MaterialIcons.cs | 实际成分映射、原生图集、自建图标节点、原布局和拖影同步 |
| BottleAndMachineIcon/VERSION、manifest.xml、csproj | 独立版本、加载标识与显式编译清单 |
| scripts/ | 构建、扫描、查询、专项部署、白名单打包 |
| Tools/NativeScan | 调用游戏原生 ModScanner，扫描本模组 DLL |
| Tools/AssemblyQuery | 只读程序集元数据与 IL |
| Research/SourceBaseline/ | 图标源码及原开发说明快照，不编译 |
| docs/MIGRATION.md | 耦合点、迁移顺序和并行开发边界 |

运行时不依赖 ChemAssistPlus.dll 或其静态 Plus 类，不带入加工和设备分配流程。仅引用游戏、Unity Core/UI 程序集，无 Harmony 补丁。

每 0.25 秒发现玩家物品；已显示对象逐帧比较成分和工作瓶，变化时立即重扫。LateUpdate 按原生图片最终边界绘制，保留中心纵排、机器左上横排、机器右上留白、祖先裁切交集和拖影透明度算法。仅设置自建节点，布局数值未变。

成分读取 state.dict 中已有 CURRENT_PART，不触发 TagSystem.GetTag 或 ValidateShapeState。空蒸发器仅用 GetTagReadonly 读取旧助手标签，并将已知产品 ID 映射为三类图标；无标签、未知标签、非空的纯溶剂均不回退为记忆标记。不写标签，也不在本模组内建立分工记忆。

加载存档事件、PlayerStore 实例变化或消失、禁用时销毁节点并释放物品引用；异常后清理并等待 5 秒重试，避免逐帧刷日志。所有原生/Unity 访问在主线程执行。

独立 ID 为 bottle_and_machine_icon，程序集和部署目录为 BottleAndMachineIcon，初始开发版本 0.1.0。与化学助手标识不同，仍须协调重复图标提供者。
