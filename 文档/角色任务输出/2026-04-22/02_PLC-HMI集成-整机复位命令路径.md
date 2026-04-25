# 02_PLC-HMI集成-整机复位命令路径

## 1. HMI 触发入口

文件：

- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPManual.cs`

现状：

- `btn整机初始化_MouseDown()` 将 `MainFrm.Hmi_bArray[41] = true` 后调用 `mf.AdsWritePlc()`
- `btn整机初始化_MouseUp()` 将 `MainFrm.Hmi_bArray[41] = false` 后调用 `mf.AdsWritePlc()`

结论：

- 当前整机复位是典型的“按钮按下置位、松开清位”模式。
- HMI 没有独立“整机复位命令对象”或结构化命令，仅使用布尔位触发。

## 2. ADS 写入边界

文件：

- `HMI/JSZW1000A/JSZW1000A/MainFrm.cs`

现状：

- `AdsWritePlc()` 会整体写出 `Hmi_bArray / Hmi_iArray / Hmi_rArray / Hmi_iSemiAuto / Hmi_iAuto / Hmi_rSlitter ...`
- 整机复位没有专用单点写入接口，而是随整包布尔数组一起写入 PLC

结论：

- 整机复位当前路径是 `Hmi_bArray[41]` 随整包数组下发。
- 这条路径足够简单，首批不建议再加新 HMI 通信层。

## 3. PLC 接收点

文件：

- `26.4/Tc_JSZW400/JSZW/Untitled1/POUs/fbMachine.TcPOU`

现状：

- `bReset := Hmi_bArray[41]`
- `rtReset(CLK := bReset)`
- `rtReset.Q` 时立即清空：
  - `TaskClamp`
  - `TaskBackGauge`
  - `TaskApron`
  - `TaskSlitter`
  - `TaskTable`
  - `TaskFeed`
- `CASE iWholeRst OF` 已存在，但目前只有空框架

结论：

- PLC 已经具备整机复位的统一接收点和状态机变量。
- 真正缺的是 `iWholeRst` 内部顺序，不是入口缺失。

## 4. 相关分工位链路

### 4.1 后挡/台面/送料

- `ST_FolderCom.TcDUT` 已有 `TaskBackGauge / TaskTable / TaskFeed`
- `fb_BackGauge.TcPOU` 已能消化这些任务并在完成后清空任务位

### 4.2 翻板

- `ST_FolderCom.TcDUT` 已有 `TaskApron`
- `fb_Apron.TcPOU` 已能消化 `ApronDaulSlide / ApronTopFold / ApronBtmFold` 任务

### 4.3 钳口

- `ST_FolderCom.TcDUT` 已有 `TaskClamp`
- `fb_Clamp.TcPOU` 已能消化 `ClampUnlock / ClampLock / ClampJog / ClampMove`

### 4.4 分条

- `ST_FolderCom.TcDUT` 已有 `TaskSlitter`
- 但 `E_UnitTask` 只有 `eTask_SlitterAct`
- `FB_SlitterAct.TcPOU` 的回原位仍直接走 `Hmi_bArray[44]`

结论：

- PLC-HMI 边界里最大的不对称点是分条回原位。
- 其他单元基本都已经“任务化”，分条还保留明显的 HMI 直驱痕迹。

## 5. 物理复位按钮边界

文件：

- `26.4/Tc_JSZW400/JSZW/Untitled1/GVLs/GVL_IO.TcGVL`

现状：

- 存在 `In_btnReset AT %I* : BOOL`

当前问题：

- 在当前可见源码中，未定位到该物理输入被直接消费到整机复位主链路

结论：

- 必须现场确认物理复位按钮是否已通过其他路径接入。
- 在确认前，不能默认“物理复位按钮”和“HMI 整机复位”完全等价。

## 6. 建议的最小收口位置

首批建议只在 PLC 内部收口，不扩 HMI 通信协议：

1. 保留 `SubOPManual -> Hmi_bArray[41] -> AdsWritePlc()` 作为触发入口。
2. 在 `fbMachine.TcPOU` 的 `iWholeRst` 内完成整机复位顺序。
3. 在 `E_UnitTask` 中增加 `eTask_SlitterHome`，让分条回原位进入任务总线。
4. 先不扩 `ST_FolderCom` 结构，因为任务位已经够用。

## 7. 边界风险

1. `MouseDown/MouseUp` 模式意味着复位命令是脉冲，不是自保持；`iWholeRst` 必须在 PLC 内部自保持。
2. `rtReset.Q` 当前会先清空所有任务位，后续状态机写入必须避免被同周期后续逻辑再覆盖。
3. 分条若不任务化，整机复位状态机将被迫回写 HMI 位 `44`，边界会变脏。
4. 物理复位按钮路径未确认前，不能把现场复位行为直接判定为与 HMI 一致。

## 8. 需要和自动化总工确认的点

1. 整机“初始状态”的唯一口径，尤其是翻板滑台最终安全位。
2. 出料平台或其他现场机构是否纳入整机复位。
3. 复位中遇到急停/报警时，是终止并等待人工重发，还是允许从中断点恢复。
4. 分条回原位在现场是否必须先于翻板滑台动作。
