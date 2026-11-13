# 开发约定

- 每轮先读本文件与 docs/HANDOFF.md，再按问题查 docs/ARCHITECTURE.md、docs/NATIVE_API.md 或 docs/MIGRATION.md。
- 本仓库独立开发容器与机器图标；运行时源码放 BottleAndMachineIcon/，文档放 docs/，脚本放 scripts/，工具放 Tools/，研究资料放 Research/。
- 不修改原化学助手仓库。Research/SourceBaseline/ 为迁移前快照，不参与编译。
- 显示仅读取成分、机器槽位和原生图集；不写液体、原生形状、存档或化学助手设备分工标签，不引入自动加工、耗料、补瓶和混液确认。
- 先用 rg 定位接口，不足或游戏程序集哈希改变时再定向查询；原生 IL 仅存本地 Research/，短结论写 docs/NATIVE_API.md。不新增或运行测试。
- 修改位置、方向、间距或缩放前先展示预览，标明容器本体、机器本体、机器窗口及单种/多种原料，玩家确认后再改 UI。原布局迁移仍需检查裁切、缩放、旋转和拖动。
- PowerShell 脚本用 UTF8 BOM，兼容 Windows PowerShell 5；build.ps1 只构建和扫描，离线 PASS 不代表游戏内验收。
- 不自行启动游戏、部署、上传、改存档、操作材料或修改订阅缓存。专项部署须显式执行并记录同类模组状态；旧助手仍带图标时须避免重复绘制。
- VERSION 为唯一版本源；变更记 docs/CHANGELOG.md，当前状态记 docs/HANDOFF.md。推进版本前检查 Git，已有旧版未提交工作先完成旧版提交。
- 尚无本模组工坊条目，不得复用化学助手条目 3836818025 或其上传工作区。
