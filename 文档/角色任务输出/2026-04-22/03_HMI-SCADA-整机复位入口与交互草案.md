# 03_HMI-SCADA-整机复位入口与交互草案

## 1. 现状入口

基于当前代码，最合适的主入口已经存在：

- 页面：`SubOPManual`
- 按钮：`btn整机初始化`
- 文案资源：`Strings.Get("Manual.Action.MachineReset")`

现状触发方式：

- `MouseDown -> Hmi_bArray[41] = true -> AdsWritePlc()`
- `MouseUp -> Hmi_bArray[41] = false -> AdsWritePlc()`

结论：

- 当前不是“没有入口”，而是“入口太原始、太容易误触”。

## 2. 交互建议

首批建议不新增导航页，不改页面组织，只优化交互：

1. 仍在 `SubOPManual` 左侧大按钮上发起整机复位。
2. 点击后先弹 `DialogAsk` 做确认。
3. 确认后再发复位请求，不再直接用按住即执行。
4. 执行中锁定整机复位按钮和相关单工位按钮。
5. 反馈优先复用 `FrmTips` 与底部 `richMsgInfo / lbPLC消息`。

## 3. 误触与权限

建议首批先做轻量防护：

- 自动/半自动运行中阻断整机复位按钮
- 执行中禁二次触发
- 如需权限，优先复用现有 `plload / tbload / btn登录` 挡板，不另起一套页面

## 4. 与现有 WinForms 风格保持一致的约束

1. 不改 `MainFrm + SubWindows + gpbSubWin` 架构。
2. 若后续加控件，仍按 `.Designer.cs + InitializeComponent() + Controls.Add(...)` 方式落。
3. 不引入 Web 式抽屉、Toast、卡片式流程页。
4. 文案继续走 `Strings.resx / Strings.en-US.resx`。

## 5. 建议涉及文件

- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPManual.cs`
- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPManual.Designer.cs`
- `HMI/JSZW1000A/JSZW1000A/MainFrm.cs`
- `HMI/JSZW1000A/JSZW1000A/Strings.resx`
- `HMI/JSZW1000A/JSZW1000A/Strings.en-US.resx`
- `HMI/JSZW1000A/JSZW1000A/DialogAsk.cs`
- `HMI/JSZW1000A/JSZW1000A/FrmTips.cs`

## 6. 关键结论

1. 主入口保留在 `SubOPManual` 最合适。
2. 当前 `MouseDown/MouseUp` 对整机级动作过于粗糙，应升级为“确认后启动 + 执行锁定 + 状态反馈”。
3. `SubCheckItem` 更适合做开机前条件提示，不适合直接承载整机复位执行。
