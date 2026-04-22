# JSZW400 改进优化执行计划

## 1. 目标概述

本轮计划聚焦 3 项改进：

1. 在折弯半自动流程中，将挤压步骤、分切步骤做成稳定可插入的标准工序。
2. 增加“手动设定折弯顺序后发送到半自动，按每个折弯角度设定生成步骤表”的能力。
当前主操作页面以 `SubOPAutoView` 为准，`SubOPAutoSet` 作为后续可接入的辅助设定页保留。
2. 优化上位机图形预览功能，提高预览准确性、交互流畅性和维护性。
3. 对液压运动控制的加减速做线性优化，降低冲击，提升动作一致性和可调性。

建议执行顺序：

1. 半自动工序改造
2. 上位机预览优化
3. 液压线性加减速优化

原因：

- 第 1 项决定半自动工艺数据结构和工序流。
- 第 2 项需要跟随第 1 项把“挤压/分切”展示到预览里。
- 第 3 项主要影响执行品质和现场调试，风险最高，适合放在前两项稳定后实施。

## 2. 当前基线判断

### 2.1 PLC 侧现状

当前 PLC 主线工程建议以 `26.4/Tc_JSZW400` 为准。

关键入口：

- `26.4/Tc_JSZW400/JSZW/Untitled1/POUs/fbMachine.TcPOU`
- `26.4/Tc_JSZW400/JSZW/Untitled1/DUTs/DUTs/E_UnitTask.TcDUT`
- `26.4/Tc_JSZW400/JSZW/Untitled1/DUTs/DUTs/ST_FolderCom.TcDUT`
- `26.4/Tc_JSZW400/JSZW/Untitled1/GVLs/GVL_CONST.TcGVL`

已确认的现有基础：

- `fbMachine.A_SemiAuto` 中已经存在半自动状态步逻辑。
- 挤压相关状态段已经存在，典型步骤位于 `350` 之后和 `380/3800` 分支。
- 分切相关状态段已经存在，典型步骤位于 `400-450` 分支。
- `SemiAuto_ActJob.ActType` 已经参与半自动工序分流：
  - `0`：普通折弯
  - `1`：挤压
  - `2`：开口挤压
  - `3`：分切
  - `8`：翻面
- `ST_FolderCom` 已经具备 `TaskClamp / TaskBackGauge / TaskApron / TaskSlitter / TaskTable / TaskFeed` 等任务通道。
- `E_UnitTask` 已经具备 `eTask_SlitterAct`、夹钳、后挡料、翻板、桌板、送料等任务定义。

结论：

- PLC 侧并不是从零开始。
- 后续重点不是“新增概念”，而是“把现有挤压/分切能力改造成稳定的半自动标准工序，并补全前后工艺衔接、异常处理和验收逻辑”。

### 2.2 HMI 侧现状

当前 HMI 主线工程建议以 `HMI/JSZW1000A` 为准。

关键入口：

- `HMI/JSZW1000A/JSZW1000A/MainFrm.cs`
- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPManual.cs`
- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPAutoView.cs`
- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPAutoDraw.cs`

已确认的现有基础：

- `MainFrm.SemiAutoType` 已经包含 `行动类型`、`折弯序号`、`折弯方向`、`后挡位置`、`抓取类型`、`松开高度` 等字段。
- `MainFrm` 中已有 `CurtOrder.lstSemiAuto` 作为半自动工序列表。
- `MainFrm` 中“发送到半自动”的主链路已经存在，当前入口是 `btn发送到半自动_Click -> create生产序列() -> Create生产数据() -> AdsWritePlc_SemiAuto()`。
- `MainFrm` 的生产序列生成逻辑已经会插入：
  - `行动类型 = 3` 的分切工序
  - `行动类型 = 1/2` 的挤压/开口挤压工序
- `SubOPManual.cs` 已经能显示普通折弯、挤压、开口挤压、分切、翻面。
- `SubOPAutoView.cs` 已经承担自动页面的基本功能主画面，包含：
  - `stSetting()` / `stPreView()` 两种工作状态
  - 基于 `lstSemiAuto` 的步骤按钮生成
  - 步骤上下移动
  - 当前步骤的抓取类型、松开高度、内外选择、折弯方向、回弹等参数查看与修改
  - 折弯图形预览和步骤位置联动
- `SubOPAutoDraw.cs` 已经支持快速手工绘制和生成图形点集。
- `SubOPAutoSet.Designer.cs` 已经有独立的自动设定页面骨架和控件布局，但 `SubOPAutoSet.cs` 目前基本为空，尚未接入现有自动页面主链路。

结论：

- HMI 侧已有工序建模基础，也已有挤压/分切的界面表达。
- “手动设定折弯顺序后再发送到半自动”这条需求，应优先基于 `SubOPAutoView` 现有能力扩展，因为它已经是当前的基本功能主画面。
- `SubOPAutoSet` 更适合作为后续补充的“辅助设定页”，承载全局设定项或生成前规则设定，而不是替代 `SubOPAutoView` 的主操作职责。
- 当前更大的问题在于：
  - 工序生成规则较硬编码
  - 主功能页中的手动调序能力与“发送后按设定重新生成步骤表”之间还没有完全闭环
  - `SubOPAutoSet` 预留页和现有发送链路尚未打通
  - 预览绘制方式不统一
  - 预览数据和界面状态耦合过深
  - 对“中间插入工序”的表达、校验和维护成本偏高

## 3. 任务一：半自动中间插入挤压步骤、分切步骤

### 3.1 目标

把“挤压”和“分切”从当前偏特例化处理，改造成可在半自动序列中稳定插入、稳定下发、稳定执行的标准工序节点。

目标效果：

- 能在任意指定折弯之间插入挤压工序。
- 能在任意指定折弯之间插入分切工序。
- HMI 列表、预览、PLC 执行顺序一致。
- 工序插入后，夹钳、后挡料、翻板、分切刀、台面动作衔接清晰。

### 3.2 主要改造入口

HMI：

- `HMI/JSZW1000A/JSZW1000A/MainFrm.cs`
- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPAutoSet.cs`
- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPManual.cs`
- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPAutoView.cs`

PLC：

- `26.4/Tc_JSZW400/JSZW/Untitled1/POUs/fbMachine.TcPOU`
- `26.4/Tc_JSZW400/JSZW/Untitled1/DUTs/DUTs/ST_FolderCom.TcDUT`
- `26.4/Tc_JSZW400/JSZW/Untitled1/DUTs/DUTs/E_UnitTask.TcDUT`

### 3.3 执行方案

#### 方案 A：先统一工序语义

先把半自动工序统一看成“标准工序节点”，至少保留以下类型：

- 普通折弯
- 挤压
- 开口挤压
- 分切
- 翻面

要求：

- HMI 中 `SemiAutoType.行动类型` 保持唯一含义，不再依赖角度特殊值去猜工序类型。
- 对当前还在使用的特殊角度值 `3.001 / 3.99 / 888`，作为兼容输入保留一段时间，但内部处理逐步转为显式动作类型优先。

#### 方案 B：重构半自动序列生成规则

在 `MainFrm` 的生产序列生成逻辑中，拆成 3 层：

1. 原始折弯工序生成
2. 插入型工序生成
3. 最终下发序列整理

建议做法：

- 先生成纯折弯主序列。
- 再按规则插入挤压、分切、翻面等辅助工序。
- 最后统一重排 `折弯序号`、显示顺序、预览顺序、下发顺序。

这样做的好处：

- 中间插入新工序不会打乱原有折弯计算。
- 后续如果增加“送料确认”“拍平”“回位”等工序，复用同样机制即可。

#### 方案 C：补齐 PLC 侧工序调度闭环

在 `fbMachine.A_SemiAuto` 中，把当前已存在的挤压和分切状态段整理成标准模式：

1. 进入工序
2. 下发 unit task
3. 等待任务完成
4. 二次确认关键状态
5. 清理状态并切到下一工序

建议检查点：

- 挤压前后：
  - 夹钳位置
  - 后挡料位置
  - 翻板缩回/工作位
  - 是否需要重新抓取
- 分切前后：
  - 夹钳是否压紧
  - 后挡料是否到位
  - 分切刀原点和慢速位
  - 分切后回原点确认

如现有 `TaskClamp / TaskBackGauge / TaskApron / TaskSlitter` 已够用，则优先复用，不新增 `E_UnitTask`。

#### 方案 D：补齐 HMI 编辑与下发闭环

需要保证以下 4 处一致：

- `SubOPAutoView` 中的手动顺序设定和当前步骤参数设定
- `SubOPManual` 中的工序编辑
- `SubOPAutoView` 中的工序顺序和预览
- `MainFrm` 中的 `lstSemiAuto`
- PLC 接收的 `Hmi_iSemiAuto`

建议把“工序列表转 PLC 数组”的逻辑单独收敛成一个出口，避免多个页面分别拼装。

#### 方案 E：以 `SubOPAutoView` 为主、`SubOPAutoSet` 为辅的页面分工

这项需求不建议把主功能迁到 `SubOPAutoSet`。
当前更合理的做法是保持 `SubOPAutoView` 作为主操作页，再把 `SubOPAutoSet` 作为可选的辅助设定页接入。

建议职责划分：

- `SubOPAutoView`
  - 负责步骤顺序调整
  - 负责当前步骤参数查看与编辑
  - 负责步骤预览与图形联动
  - 负责用户在自动页面上的主要交互
- `SubOPAutoSet`
  - 负责后续补充的全局设定项
  - 负责正逆序、颜色面、分条开关、回弹、材料宽度、生成规则开关等“生成前设定”
  - 不替代 `SubOPAutoView` 的步骤操作主界面
- `MainFrm`
  - 负责接收 `SubOPAutoView` 的调序结果和参数修改结果
  - 负责接收 `SubOPAutoSet` 的全局设定结果
  - 负责调用 `create生产序列()` 或新增的“按手动顺序生成”入口
  - 负责将生成后的 `lstSemiAuto` 打包到 `Hmi_iSemiAuto`
  - 负责 ADS 下发

建议数据流：

1. 在 `SubOPAutoView` 中完成折弯步骤顺序调整。
2. 在 `SubOPAutoView` 中完成当前步骤参数修改。
3. 如后续启用 `SubOPAutoSet`，再由它提供全局生成规则输入。
4. 点击发送按钮后，将“手动顺序 + 步骤参数 + 全局设定”汇总到 `MainFrm`。
5. `MainFrm` 基于这些人工设定重新生成 `lstSemiAuto`。
6. 生成完成后再走统一的 `PackSemiAutoStepsToPlc -> AdsWritePlc_SemiAuto` 下发链路。

建议生成规则：

- 手动设定的折弯顺序优先级高于自动推导顺序。
- 每个折弯步骤仍然允许根据角度和方向自动补出：
  - 挤压
  - 开口挤压
  - 分切
  - 翻面
- 也就是说，人工设定的是“主折弯序”和“折弯参数”，步骤表仍由程序按规则补全，而不是要求操作员手工把所有辅助工序逐条录入。

建议实现入口：

- 保留 `SubOPAutoView` 作为现有主页面，不重新迁移主功能。
- 在 `MainFrm` 新增一个类似 `create手动设定生产序列()` 的入口，避免直接把 `SubOPAutoSet` 的逻辑塞进现有 `create生产序列()` 里。
- 现有 `create生产序列()` 保留为默认自动生成入口；手动模式入口在内部优先读取 `SubOPAutoView` 当前人工调整结果。
- `btn发送到半自动_Click` 根据当前页面或当前模式，决定调用自动入口还是手动设定入口。

### 3.4 验收标准

- 订单不带挤压、不带分切时，原半自动流程行为不变。
- 在 `SubOPAutoView` 中手动调整折弯顺序后，点击发送到半自动，生成的步骤表顺序与人工设定一致。
- 在 `SubOPAutoView` 中修改当前步骤参数后，生成的 `lstSemiAuto` 和下发到 PLC 的 `Hmi_iSemiAuto` 正确反映该设定。
- 如启用 `SubOPAutoSet` 的全局设定项，这些设定能正确参与步骤表生成，但不覆盖 `SubOPAutoView` 已完成的人工调序结果。
- 订单带中间挤压时，HMI 列表、预览、PLC 执行顺序一致。
- 订单带中间分切时，HMI 列表、预览、PLC 执行顺序一致。
- 挤压和分切完成后，下一折弯能正确衔接，不出现夹钳未清、后挡料未回、翻板位置错误等问题。
- 半自动暂停、继续、急停恢复后，工序状态不乱序。

### 3.5 风险点

- 当前 PLC 状态步较长，直接改动 `fbMachine.A_SemiAuto` 容易引入回归。
- HMI 侧同时存在显式动作类型和特殊角度值两套表达，过渡期间要防止双重解释。
- 工序中插后，`折弯序号`、显示序号、真实动作序号可能不再完全相同，必须统一规则。
- 如果 `SubOPAutoView` 中已经调好顺序，但 `MainFrm.create生产序列()` 仍完全按原始订单重新生成，人工设定会被覆盖；因此必须明确“人工设定输入”和“最终步骤表输出”是两层数据。
- `SubOPAutoSet` 后续接入时，只能补充全局设定，不应反向覆盖 `SubOPAutoView` 的主操作结果。

### 3.6 当前已落地（2026-04-06）

- HMI 生产序列在生成完成后，已增加一次“显式工序归一化”，把首尾挤压的旧 `3.001 / 3.99` 占位折弯步合并回显式 `行动类型` 工序。
- `lstSemiAuto -> Hmi_iSemiAuto` 已收口为一个统一打包出口，避免 `MainFrm` 和 `SubOPManual` 各自拼装 PLC 数组。
- 挤压/开口挤压下发时，已保留显式 `折弯方向`，不再像旧逻辑那样在主流程里被强制写成 `0`。
- `SubOPAutoView` 的预览按钮和上下移动逻辑，已改为按显式工序顺序处理，为后续把分切/翻面也纳入同一预览模型打下基础。
- 当前落地范围仍以 HMI 侧工序标准化为主，PLC `A_SemiAuto` 的状态段整理属于下一轮承接项。

### 3.7 代码级落地拆解（当前主线）

这条“手动设定折弯顺序后，再按设定生成半自动步骤表”的需求，当前代码上的关键矛盾是：

- `SubOPAutoView` 现在操作的是最终步骤表 `CurtOrder.lstSemiAuto`。
- `btn发送到半自动_Click` 又会重新调用 `create生产序列()`，把 `lstSemiAuto` 按原始订单重新生成一遍。

所以如果直接在现有结构上继续堆逻辑，人工调序结果会被覆盖。

结论：

- 必须把“主折弯设定”和“最终半自动步骤表”拆成两层数据。
- `SubOPAutoView` 以后应优先编辑“主折弯设定层”，而不是直接编辑最终步骤表。

建议的数据层拆分：

1. 主折弯设定层
   - 只保存人工设定后的折弯顺序和每道折弯的人工参数。
   - 不直接包含分切、挤压、翻面等辅助工序。
2. 最终半自动步骤表
   - 由程序根据“主折弯设定层”自动补出挤压、分切、翻面、抓取、松开高度等完整步骤。
   - 仍然沿用 `CurtOrder.lstSemiAuto` 作为最终下发表。

建议新增一个轻量模型，例如：

- `ManualFoldPlan`
  - `原长角索引`
  - `手动顺序`
  - `折弯方向`
  - `折弯角度`
  - `回弹值`
  - `抓取类型`
  - `松开高度`
  - `内外选择`
  - `坐标序号`
  - `是否启用`

这层模型可以放在：

- `MainFrm.cs` 里作为 `OrderType` 的补充字段
- 或拆到新的 `MainFrm.ManualFold.cs`

从当前代码风格看，优先建议放在 `MainFrm` 体系内，不引入额外架构。

### 3.8 按文件的实施清单

#### A. `HMI/JSZW1000A/JSZW1000A/MainFrm.cs`

建议改造点：

1. 新增“主折弯设定层”的持有字段
   - 建议增加一个 `List<...>` 保存人工折弯顺序和参数。
   - 与 `CurtOrder.lstSemiAuto` 分开存放。
2. 新增“从订单初始化主折弯设定”的方法
   - 从 `lengAngle + pxList + 回弹默认值 + 正逆序/颜色面` 生成初始手动设定列表。
3. 新增“从主折弯设定生成最终步骤表”的方法
   - 由这层统一生成 `CurtOrder.lstSemiAuto`。
   - 作为 `create生产序列()` 的手动模式分支。
4. 调整 `btn发送到半自动_Click`
   - 当前是直接 `create生产序列()`。
   - 后续应改成：
     - 如果当前自动页存在人工设定，则走“按手动设定生成”；
     - 否则走默认自动生成。
5. 保留现有 ADS 下发链路
   - `Create生产数据()`
   - `PackSemiAutoStepsToPlc()`
   - `AdsWritePlc_SemiAuto()`
   - 这部分尽量不动。

建议新增方法名：

- `InitManualFoldPlanFromCurrentOrder()`
- `BuildSemiAutoStepsFromManualFoldPlan()`
- `Create手动设定生产序列()`
- `HasManualFoldPlanChanges()`

#### B. `HMI/JSZW1000A/JSZW1000A/MainFrm.SemiAuto.cs`

这是最适合承接生成规则的文件，建议把“手动设定生成步骤表”的核心算法放这里。

建议改造点：

1. 保留现有动作常量
   - `SemiAutoActionFold`
   - `SemiAutoActionSquash`
   - `SemiAutoActionOpenSquash`
   - `SemiAutoActionSlit`
   - `SemiAutoActionFlip`
2. 保留现有 `PackSemiAutoStepsToPlc()`
   - 它已经是稳定的最终打包出口。
3. 新增“从主折弯设定拼装辅助工序”的方法
   - 在每个折弯前后自动补：
     - 挤压
     - 开口挤压
     - 分切
     - 翻面
4. 新增“规范化最终步骤表”的方法
   - 复用或扩展现有 `NormalizeGeneratedSemiAutoSequence()`。
5. 如后续有更多生成规则，也优先继续放这里，不要再把算法散回 `SubOPAutoView`。

建议新增方法名：

- `BuildSemiAutoStepsFromManualFolds(...)`
- `AppendAuxiliaryStepsForFold(...)`
- `ApplyManualFoldOverrides(...)`
- `NormalizeManualFoldPlan(...)`

#### C. `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPAutoView.cs`

这是当前主功能页，后续改动重点在这里。

当前已有能力：

- `reCreateBtn()` 生成步骤按钮
- `btnMoveFront_Click()` / `btnMoveRear_Click()` 调整顺序
- `btn抓取类型_Click()` / `btn松开高度_Click()` / `btn折弯方向_Click()` / `btnPlus_Click()` / `btnMinus_Click()` 修改当前步骤参数

但这些能力当前直接作用在 `CurtOrder.lstSemiAuto`，后续需要改成作用于“主折弯设定层”。

建议改造点：

1. `stSetting()` 改为基于“主折弯设定层”刷新界面
   - 不再直接把最终下发步骤表作为唯一编辑源。
2. `reCreateBtn()` / `reViewMyButton()` 改为显示“主折弯设定项”
   - 每个按钮代表一道主折弯，而不是完整辅助工序表。
3. `btnMoveFront_Click()` / `btnMoveRear_Click()` 改为调整主折弯顺序
   - 调整后只更新手动设定列表，不直接重建 PLC 数组。
4. 参数编辑控件改为写回主折弯设定项
   - 抓取类型
   - 松开高度
   - 内外选择
   - 折弯方向
   - 回弹值
5. 提供一个“提交到 MainFrm”的出口
   - 可以是公开方法
   - 也可以由 `MainFrm` 主动读取当前页状态
6. 预览模式继续使用最终步骤表
   - 即：
     - 设定模式看“主折弯设定”
     - 预览模式看“按设定生成后的最终步骤表”

这一步是整个需求最关键的 HMI 改造点。

#### D. `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPAutoSet.cs`

这页当前代码基本为空，不建议把主功能搬过来。

建议作为第二阶段接入：

1. 放全局设定项
   - 正逆序
   - 颜色面
   - 分条开关
   - 顶/底回弹默认值
   - 板材宽度、计算总宽
2. 提供生成规则输入
   - 是否按手动顺序生成
   - 是否允许自动补挤压
   - 是否允许自动补分切
3. 不承载主步骤拖动和主步骤参数编辑
   - 这些仍保留在 `SubOPAutoView`

### 3.9 建议的第一批代码提交

第一批建议只做最小闭环，不碰 PLC：

1. 在 `MainFrm` / `MainFrm.SemiAuto.cs` 引入“主折弯设定层”。
2. 让 `SubOPAutoView` 的上下移动和参数编辑改为操作这层数据。
3. 点击“发送到半自动”时，用这层数据重新生成 `CurtOrder.lstSemiAuto`。
4. 保持 `PackSemiAutoStepsToPlc()` 和 ADS 下发逻辑不变。

这批完成后就能先验证：

- 手动调整顺序不会再被发送动作覆盖。
- 每个折弯角度设定会体现在最终半自动步骤表里。
- HMI 预览和 PLC 下发表仍走同一最终数据源。

### 3.10 第一批验收检查点

建议按下面顺序测：

1. 不做手动调整，直接发送
   - 结果应与现状一致。
2. 在 `SubOPAutoView` 中调整 2 个折弯前后顺序后发送
   - 最终步骤表顺序必须跟随变化。
3. 修改 1 个折弯的回弹、抓取、松开高度后发送
   - 对应步骤的 `Hmi_iSemiAuto` 值必须改变。
4. 含首尾挤压的订单做同样测试
   - 辅助工序仍应由程序自动补出。
5. 含分切订单做同样测试
   - 分切仍应出现在正确位置，不应被手动调序破坏。

## 4. 任务二：优化上位机侧图形预览功能

### 4.1 目标

让预览功能在以下方面可用且稳定：

- 图形显示准确
- 挤压/分切/翻面等特殊工序可视化
- 重绘不闪烁、不残影
- 顺序调整后预览及时更新
- 代码更容易维护

### 4.2 主要改造入口

- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPAutoView.cs`
- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPAutoDraw.cs`
- `HMI/JSZW1000A/JSZW1000A/MainFrm.cs`

### 4.3 当前问题判断

已观察到的典型问题：

- `SubOPAutoView.cs` 多处直接使用 `CreateGraphics()` 绘图。
- 预览计算、界面控件生成、当前步动画、工序解释混在一个文件里。
- `SubOPAutoDraw.cs` 和 `SubOPAutoView.cs` 都在处理点集和绘制，但模型不统一。
- 特殊工序的图形表达依赖较多局部判断，后续扩展工序容易继续堆逻辑。

### 4.4 执行方案

#### 方案 A：统一预览渲染入口

目标：

- 只保留 `Paint + Invalidate` 方式作为正式渲染入口。
- 去掉核心预览链路里长期持有 `CreateGraphics()` 的画法。

建议做法：

- 在 `SubOPAutoView` 中把“计算预览数据”和“执行绘图”拆开。
- 引入一个统一的 `RenderPreview(Graphics g, PreviewState state)` 风格方法。
- 特殊工序图标、折弯方向、当前步高亮都从 `PreviewState` 读取。

#### 方案 B：抽离预览数据模型

建议新增轻量预览模型，最少包含：

- 折线点集
- 当前折弯索引
- 当前工序类型
- 是否色下
- 是否翻面
- 是否挤压
- 是否分切
- 当前后挡位置
- 当前折弯方向

这样可以把：

- `MainFrm.CurtOrder`
- `lstSemiAuto`
- `pxList`
- 当前界面显示状态

从“直接混用”改为“先转换成预览模型，再绘制”。

#### 方案 C：增强工序可视化

预览里建议显式增加以下表达：

- 当前工序是折弯、挤压、开口挤压、分切还是翻面
- 当前工序对应的是哪一段边
- 当前工序前后的色面/内外侧变化
- 当前工序的后挡目标位置

优先级建议：

1. 工序类型图标/颜色区分
2. 当前步高亮
3. 关键参数标注

#### 方案 D：优化手工绘图与自动预览衔接

`SubOPAutoDraw` 当前已经可生成点集，后续建议补强：

- 像素坐标到物理坐标的统一映射
- 自动居中和自适应缩放
- 网格吸附和撤销保持不变
- 从手绘结果到 `lstSemiAuto` 的转换规则集中管理

### 4.5 验收标准

- 同一订单在切换页面、缩放、重绘、移动顺序后，预览结果一致。
- 挤压、分切、翻面在预览中都有明确视觉表达。
- 顺序调整后，预览与 `SubOPManual` 列表一致。
- 不再出现因窗口遮挡、切页、刷新导致的残影或显示丢失。

### 4.6 风险点

- `SubOPAutoView.cs` 现有逻辑较集中，直接大改容易引入显示回归。
- 如果预览模型和真实工序模型不同步，会出现“看起来对，执行不对”的问题。

### 4.7 任务二细化（承接任务一）

建议按下面 4 个子项继续推进：

1. 先补 `PreviewState` 数据模型
   - 最少收口 `行动类型 / 折弯方向 / 坐标序号 / 长角序号 / 后挡位置 / 内外选择 / is色下 / 当前选中步`
   - 输入只允许来自 `CurtOrder.lstSemiAuto + pxList`，不再直接从多个控件反向取值
2. 再收 `SubOPAutoView` 的按钮列表与绘图入口
   - 按“按钮数据生成”和“图形绘制”拆成两个方法
   - 让按钮文本、颜色、位置都只依赖 `PreviewState`
3. 单独处理特殊工序的视觉表达
   - `挤压/开口挤压`：使用统一的锚点偏移和图标
   - `分切`：显示独立刀位标识，不再混成普通折弯标签
   - `翻面`：显示切面方向切换或显式提示，不只靠大字提示
4. 最后替换 `CreateGraphics()` 热路径
   - 正式渲染只保留 `Paint/Invalidate`
   - 当前 `tmr预览` 驱动逻辑保持不变，先只替换绘图入口，避免一步改太大

建议下一轮的最小改动文件：

- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPAutoView.cs`
- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPAutoDraw.cs`
- 必要时新增 `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPAutoView.Preview.cs`

## 5. 任务三：液压运动控制加减速线性优化

### 5.1 目标

把当前液压动作从“高速/爬行速度切换 + 直接比例阀输出”优化为“目标速度线性变化 + 输出线性斜坡”，降低冲击并提升重复性。

优化对象优先级：

1. 翻板折弯
2. 翻板滑动
3. 夹钳开合
4. 必要时再扩展到泵频输出联动

### 5.2 主要改造入口

- `26.4/Tc_JSZW400/JSZW/Untitled1/POUs/fbMachine.TcPOU`
- `26.4/Tc_JSZW400/JSZW/Untitled1/GVLs/GVL_CONST.TcGVL`
- `26.4/Tc_JSZW400/JSZW/Untitled1/POUs/Lib/FB_HydLinearRamp.TcPOU`
- `26.4/Tc_JSZW400/JSZW/Untitled1/Untitled1.plcproj`（新增 POU 时同步挂载）

### 5.3 当前问题判断

从当前逻辑看：

- `GVL_CONST.TcGVL` 中已有大量速度、爬行速度、减速距离常量。
- `fbMachine` 的 `DualPump` 动作中，很多位置仍是：
  - 根据状态直接在“工作速度”和“爬行速度”之间切换
  - 再直接换算成比例阀模拟量输出
- 当前更像“两段式速度切换”，不是连续线性加减速。

这会带来：

- 起步和切换点冲击较大
- 同一动作在不同负载下手感不一致
- 现场调试时需要反复硬改速度常量

### 5.4 执行方案

#### 方案 A：引入统一斜坡层

建议新增一个液压斜坡功能块，输入/输出建议如下：

- 输入：
  - `Enable`
  - `TargetSpeed`
  - `AccStep` 或 `AccTime`
  - `DecStep` 或 `DecTime`
  - `MinOutput`
  - `MaxOutput`
- 输出：
  - `CmdSpeed`
  - `DoneToTarget`

实现原则：

- 每周期按固定步长向目标速度逼近。
- 加速和减速分开配置。
- 零速到起动速度要有最小起跳值，避免阀口开不动。

#### 方案 B：目标速度与输出量分层

建议把当前链路拆成两层：

1. 工艺层决定“目标速度”
2. 斜坡层决定“当前输出命令”

即：

- `CNST_APRON_*_SPD`
- `CNST_APRON_*_CREEP_SPD`
- `CNST_CLAMP_*_SPD`

不再直接等于阀输出，而是作为目标速度输入给斜坡层。

#### 方案 C：先从翻板和夹钳试点

建议先做 3 组动作：

- 上/下翻板折弯
- 上/下翻板滑动
- 夹钳开/合

理由：

- 这三类动作最容易感受到冲击和节拍变化。
- 都已经有明确的位置反馈或状态位，便于调参。

#### 方案 D：参数化而不是写死

建议把以下参数做成可调：

- 加速度
- 减速度
- 最小启动量
- 最大输出限幅
- 切入爬行区距离

短期可以先落在 `GVL_CONST`。
中期更建议转到 `stFolderPara` 或 HMI 可配置参数区，方便现场调机。

### 5.5 验收标准

- 翻板和夹钳动作启动、减速、停止明显更平顺。
- 同一动作连续运行时，末端冲击降低。
- 不影响原有限位、慢速位、急停、光幕联锁。
- 加减速参数调节后能稳定反映到实际动作。

### 5.6 风险点

- 该项强依赖现场液压响应，单靠离线代码阅读无法一次定准。
- 线性斜坡引入后，原来的慢速位和减速距离阈值可能需要一起重整。
- 如果泵频和比例阀同时加斜坡，动作延迟会叠加，必须按轴分别整定。

### 5.7 任务三细化（承接任务一）

建议把液压优化拆成“离线可完成”和“现场必须验证”两段：

#### 5.7.1 离线先完成

- 新增统一斜坡 FB，接口先固定为：
  - `Enable`
  - `TargetSpeed`
  - `AccStep`
  - `DecStep`
  - `MinOutput`
  - `MaxOutput`
  - `CmdSpeed`
  - `DoneToTarget`
- 在 `fbMachine` 内先只改 3 组试点动作：
  - 上/下翻板折弯
  - 上/下翻板滑动
  - 夹钳开/合
- 把原来“高速/爬行速度常量”改成“目标速度常量”，保留现有安全联锁与限位判断不动
- 泵频输出也只在 `DualPump` 尾部增加统一斜坡，不改前面的动作选择、慢速区判断和 idle/override 逻辑

#### 5.7.2 现场再验证

- 每组动作至少记录 3 个参数：
  - 启动斜率
  - 减速斜率
  - 最小启动量
- 每次调参只改 1 个轴、1 组动作，记录：
  - 空载体感
  - 带料体感
  - 末端冲击
  - 回零稳定性
- 现场通过后，再考虑把参数从 `GVL_CONST` 转入 `stFolderPara` 或 HMI 参数页

#### 5.7.3 当前已落地（2026-04-06）

- 已新增 `FB_HydLinearRamp`，接口固定为 `Enable / TargetSpeed / AccStep / DecStep / MinOutput / MaxOutput / CmdSpeed / DoneToTarget`，后续现场整定直接围绕这组参数开展。
- `fbMachine.DualPump` 已按“目标速度 -> 斜坡输出”改造，当前试点范围限定在上/下翻板折弯、上/下翻板滑动、夹钳开合。
- 翻板滑动/折弯的比例阀目标值不再固定取高速常量，而是和当前 `工作速 / 慢速` 目标保持一致，再进入斜坡层输出。
- 泵频输出保留原有动作优先级、光幕 `override`、idle 延时与 RopeStop 分支，仅在最终输出侧增加线性斜坡；`override=0` 和 RopeStop 仍走直接旁路，不把安全停机改成缓停。
- 当前参数先落在 `GVL_CONST`，属于离线初值；`CNST_HYD_*_STEP / MIN_OUTPUT` 必须在现场按空载、带料、末端冲击继续整定。
- 本轮未覆盖分切、后挡、送料，也未把参数迁到 `stFolderPara`/HMI，可作为下一轮承接项。

建议下一轮的最小改动文件：

- `26.4/Tc_JSZW400/JSZW/Untitled1/POUs/fbMachine.TcPOU`
- `26.4/Tc_JSZW400/JSZW/Untitled1/GVLs/GVL_CONST.TcGVL`
- `26.4/Tc_JSZW400/JSZW/Untitled1/POUs/Lib/FB_HydLinearRamp.TcPOU`
- `26.4/Tc_JSZW400/JSZW/Untitled1/Untitled1.plcproj`

## 6. 推荐实施阶段

### 阶段 1：方案固化

输出内容：

- 半自动工序标准定义
- 动作类型编码定义
- 插入规则清单
- 预览显示规则清单
- 液压线性控制参数表草案

### 阶段 2：半自动工序改造

优先完成：

- `MainFrm` 生产序列生成
- `SubOPManual` 编辑与显示
- `fbMachine.A_SemiAuto` 工序切换闭环

### 阶段 3：预览改造

优先完成：

- `SubOPAutoView` 渲染收口
- 特殊工序可视化
- 顺序调整后的预览一致性

### 阶段 4：液压线性优化

优先完成：

- 新增斜坡层
- 翻板/夹钳试点
- 现场调参与记录

### 阶段 5：联调与回归

建议准备 5 套标准测试件：

1. 纯折弯件
2. 首尾挤压件
3. 中间插入挤压件
4. 带分切件
5. 带锥度件

每套都验证：

- HMI 工序列表
- 预览
- PLC 实际执行顺序
- 结果件动作一致性

## 7. 建议的最小落地策略

为了符合“小步可评审”的改动方式，建议分 3 批提交：

### 批次 1

- 只整理半自动工序定义与生成逻辑
- 不碰液压控制

### 批次 2

- 只整理预览渲染和特殊工序显示
- 不改变 PLC 行为

### 批次 3

- 只上线液压线性斜坡
- 一次只试点 1 到 2 类动作

这样做的好处：

- 每批都能独立验证
- 问题定位更清晰
- 便于现场逐项回退

## 8. 结论

这三项优化都已经有现成基础，不需要推翻重做。

最合理的路线是：

1. 先把半自动里的“挤压/分切”标准工序化
2. 再把 HMI 预览做成围绕标准工序模型的稳定渲染
3. 最后对液压执行层做线性加减速优化和现场整定

如果按这个顺序推进，后续每一步都会更稳，也更容易在现场快速验证。

## 9. 后续任务四：长度单位从毫米切换到英寸

### 9.1 目标

将当前 HMI 中以毫米为主的长度相关输入、显示、编辑流程切换为英寸表达，同时保证第一阶段 PLC、ADS 协议、设备参数和运动执行层继续保持“毫米内核”不变。

目标效果：

- 操作员界面统一显示 `in` / `inch`
- 用户输入英寸后，程序内部自动换算成毫米再参与计算、生成、下发
- 半自动、手动、分条、送料、设备参数等功能在切换单位后行为保持一致
- 历史订单、配置、旧文本文件在未声明单位时，默认按毫米兼容

### 9.2 当前设备结构分析

#### 9.2.1 HMI 数据层

当前 HMI 长度相关核心数据主要集中在 `HMI/JSZW1000A/JSZW1000A/MainFrm.cs`：

- `LengAngle.Length / TaperWidth`
- `OrderType.Width / Thickness / SheetLength / TaperLength`
- `SemiAutoType.后挡位置`
- `CurtOrder.lstSemiAuto`

当前特点：

- 这些字段均为 `double` 裸值，没有单位字段或单位上下文
- 代码默认把这些值直接当作毫米参与计算
- `create生产序列()` 直接基于这些值生成半自动步骤表

结论：

- HMI 当前并不是“可切换单位”的模型，而是“毫米默认写死”的模型

#### 9.2.2 HMI 下发层

当前半自动下发主链路为：

1. `create生产序列()`
2. `CurtOrder.lstSemiAuto`
3. `PackSemiAutoStepsToPlc()`
4. `Hmi_iSemiAuto`
5. ADS 下发到 PLC

当前特点：

- `PackSemiAutoStepsToPlc()` 按每步 10 个槽位打包
- `折弯角度 / 回弹值 / 后挡位置` 会按 `*10` 编码
- `后挡位置` 的协议精度本质上是 `0.1mm`
- 除 `Hmi_iSemiAuto` 外，`Hmi_rArray`、`Hmi_rSlitter` 也承载了大量长度类设定值

结论：

- HMI 到 PLC 的边界协议当前也是按毫米约定的

#### 9.2.3 PLC 工艺层

当前 PLC 长度值进入设备执行的路径为：

1. `GVL.Hmi_iSemiAuto`
2. `fbMachine` 中还原为 `SemiAuto_ActJob : ST_OneJob`
3. 写入 `stFolderCom.Task* + para*`
4. 各 UNIT 功能块执行

关键结构：

- `GVLs/GVL.TcGVL`
- `DUTs/DUTs/ST_OneJob.TcDUT`
- `DUTs/DUTs/ST_FolderCom.TcDUT`
- `POUs/fbMachine.TcPOU`

当前特点：

- `SemiAuto_ActJob.FoldAngle / SpringBack / BackGaugePos` 都是 PLC 侧 `REAL`
- `fbMachine` 中对 `Hmi_iSemiAuto` 的角度、回弹、后挡值统一按 `/10` 还原
- `ST_FolderCom` 中 `paraBG_T1..T4 / paraTablePos / paraFeedPos / ParaApronTopFold / ParaApronBtmFold` 等都是毫米语义

结论：

- PLC 当前不是“显示单位层”，而是直接拿毫米工艺值驱动设备

#### 9.2.4 设备参数层

当前设备参数主要集中在 `DUTs/DUTs/ST_FolderPara.TcDUT`：

- `Clamp_LowPos / Clamp_MidPos / Clamp_HighPos / Clamp_MaxPos / ClampHalfPos`
- `BackgaugeMaxPos / BackgaugeMinPos`
- `SquashBackstop_Top / Btm / External / OpenTop / OpenBtm`
- `FeedOffset / FeedPosition`
- `SetMoveBackgauge / SetLoadingTable`

当前特点：

- 上述参数均为 `REAL`
- 现场已经按毫米整定
- `fbMachine`、`fb_BackGauge`、`fb_Clamp` 等逻辑里存在大量毫米常量和毫米阈值

结论：

- 如果直接把 PLC 原生单位改成英寸，将牵动参数表、公式、限位、偏移量和现场整定，不适合作为第一阶段方案

#### 9.2.5 界面文案与输入层

当前 HMI 中大量界面仍直接写死 `mm / 毫米`，典型页面包括：

- `SubOPAuto`
- `SubOPManual`
- `SubOPSetting`
- `FrmFeed`
- `FrmInlineSlit`
- `SubOPSlitter`

当前特点：

- 单位文案分散在 `.cs` 与 `.Designer.cs`
- 很多输入框直接 `double.TryParse()` 后写入毫米字段
- 很多显示文本直接 `ToString()` 拼接 `mm`

结论：

- 单位切换不能只改数据层，必须同时收口输入、显示和文案

### 9.3 总体判断

基于当前结构，推荐的第一阶段路线是：

- HMI 显示和输入切换为英寸
- HMI 内部订单、步骤、参数在参与业务计算前统一换算为毫米
- `Hmi_iSemiAuto / Hmi_rArray / Hmi_rSlitter` 继续按毫米下发
- PLC、TwinCAT 参数、现场整定值保持不变

不建议第一阶段直接做：

- PLC 原生单位改为英寸
- `ST_FolderPara` 全部重定义为英寸
- 现场参数整表重标定

原因：

- 风险大
- 回归面太广
- 需要现场重新整定，不能靠离线代码一次完成

### 9.4 实施原则

#### 原则 A：先建立统一单位层

建议在 HMI 侧新增统一长度单位工具层，至少收口以下能力：

- `MmToInch`
- `InchToMm`
- `FormatLengthForUi`
- `ParseLengthFromUi`
- `GetLengthUnitLabel`

要求：

- 所有长度类输入输出都经过这层
- 业务层不再直接在界面事件中裸写 `25.4`

#### 原则 B：内部计算先保持毫米

建议第一阶段内部仍以毫米为准，尤其是：

- `CurtOrder`
- `lstSemiAuto`
- `Hmi_iSemiAuto`
- `Hmi_rArray`
- `Hmi_rSlitter`

这样做的好处：

- 半自动生成公式不需要整体改写
- PLC 协议不变
- 现场设备行为不变

#### 原则 C：输入输出边界统一换算

边界定义建议如下：

- 用户输入框：英寸 -> 换算为毫米 -> 写入内部数据
- 界面显示文本：内部毫米 -> 换算为英寸 -> 显示
- PLC 下发：继续下发毫米
- PLC 回读：继续读取毫米，再换算成英寸显示

#### 原则 D：历史文件默认兼容毫米

当前订单、库文件、文本序列化内容没有明确单位标识，因此建议：

- 新格式允许增加 `LengthUnit`
- 旧文件未带单位字段时默认按毫米读取
- 读入后统一转换到内部毫米模型

### 9.5 主要改造入口

HMI：

- `HMI/JSZW1000A/JSZW1000A/MainFrm.cs`
- `HMI/JSZW1000A/JSZW1000A/MainFrm.SemiAuto.cs`
- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPAuto.cs`
- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPAutoView.cs`
- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPManual.cs`
- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPSetting.cs`
- `HMI/JSZW1000A/JSZW1000A/FrmFeed.cs`
- `HMI/JSZW1000A/JSZW1000A/FrmInlineSlit.cs`
- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPSlitter.cs`

PLC（第一阶段以核对边界为主，尽量不改协议）：

- `26.4/Tc_JSZW400/JSZW/Untitled1/GVLs/GVL.TcGVL`
- `26.4/Tc_JSZW400/JSZW/Untitled1/DUTs/DUTs/ST_OneJob.TcDUT`
- `26.4/Tc_JSZW400/JSZW/Untitled1/DUTs/DUTs/ST_FolderCom.TcDUT`
- `26.4/Tc_JSZW400/JSZW/Untitled1/DUTs/DUTs/ST_FolderPara.TcDUT`
- `26.4/Tc_JSZW400/JSZW/Untitled1/POUs/fbMachine.TcPOU`

### 9.6 执行方案

#### 方案 A：先补统一单位工具层

建议新增一个轻量长度单位模块，建议放在 HMI 工程内，例如：

- `MainFrm.Unit.cs`
- 或 `LengthUnitHelper.cs`

建议至少定义：

- `enum LengthUnitMode { Millimeter, Inch }`
- `const double MmPerInch = 25.4`
- `double ToInternalMm(double uiValue)`
- `double ToDisplayLength(double internalMm)`
- `string FormatDisplayLength(double internalMm, int digits = 3)`
- `bool TryParseDisplayLength(string text, out double internalMm)`

要求：

- 所有长度值都通过统一方法格式化和解析
- 不允许各页面自己重复写换算逻辑

#### 方案 B：收口订单与半自动核心链路

优先处理会影响主功能链路的字段：

- `OrderType.Width`
- `OrderType.Thickness`
- `OrderType.SheetLength`
- `OrderType.TaperLength`
- `LengAngle.Length`
- `LengAngle.TaperWidth`
- `SemiAutoType.后挡位置`

建议做法：

- UI 输入采用英寸
- 写回 `CurtOrder` 时立即换算成毫米
- `create生产序列()` 和 `PackSemiAutoStepsToPlc()` 继续按毫米工作

这样做可以保证：

- 半自动生成公式基本不动
- PLC 侧行为不变

#### 方案 C：收口 `Hmi_iSemiAuto` 下发边界

当前 `PackSemiAutoStepsToPlc()` 已经是稳定出口，建议保持：

- 内部 `step.后挡位置` 仍为毫米
- 打包时继续 `*10`
- PLC 继续 `/10` 还原

这一层原则上不改协议，只验证：

- 英寸界面输入后，最终打包出来的毫米值是否与人工换算一致

#### 方案 D：收口 `Hmi_rArray` 与 `Hmi_rSlitter`

当前很多手动、参数、送料、分条设定直接写入：

- `Hmi_rArray[50..54]`
- `Hmi_rArray[57]`
- `Hmi_rArray[90..93]`
- `Hmi_rArray[100..129]`
- `Hmi_rSlitter[...]`

建议做法：

- 所有页面输入框统一显示英寸
- 写入 `Hmi_rArray / Hmi_rSlitter` 前先换算到毫米
- 从 PLC 或配置回显时先按毫米读取，再转英寸显示

#### 方案 E：收口界面文案与 Designer 硬编码

建议统一替换以下两类内容：

- 直接写死的 `mm / 毫米`
- 直接拼接的 `" xxx mm"`

建议做法：

- 先把运行时可动态赋值的文案切到统一方法
- 再逐步清理 `.Designer.cs` 里的静态单位文本

#### 方案 F：补文件兼容与导入导出规则

当前文本化订单和库文件默认按毫米解释，建议后续补充：

- 新增 `LengthUnit:mm|in`
- 新文件写出时显式带单位
- 旧文件读入时默认 `mm`
- 内部统一落回毫米

### 9.7 验收标准

- HMI 所有长度类显示统一为英寸
- 输入 `1.000 in` 后，内部实际计算值等于 `25.4 mm`
- 同一订单分别用毫米旧文件和英寸新界面录入，生成的 `lstSemiAuto` 关键毫米值一致
- `Hmi_iSemiAuto` 下发到 PLC 的后挡位置、角度、回弹编码不因界面切换而变化
- `Hmi_rArray`、`Hmi_rSlitter` 对应的设备动作位置不变
- PLC 实际执行行为与毫米版本一致
- 旧订单、旧库文件、旧配置不因新增单位层而失效

### 9.8 风险点

- 当前 `mm / 毫米` 文案分散且硬编码，容易漏改
- 很多输入框直接 `TryParse()` 然后裸写内部字段，若不统一收口会出现局部仍按毫米解释
- `600 / 1200 / 10000` 这类长度校验阈值在界面切换后必须明确“显示单位变了，但内部比较仍按毫米”
- `Hmi_iSemiAuto` 是定长整型协议，英寸显示如果先四舍五入再换算，容易引入额外误差
- PLC 中存在大量毫米偏移常量与现场整定参数，第一阶段若误改到 PLC 原生单位，会引入大面积回归

### 9.9 建议的第一批代码提交

第一批建议只做最小闭环，不动 PLC 协议：

1. 新增统一长度单位工具层
2. 先覆盖 `OrderType / LengAngle / SemiAutoType` 相关主链路输入输出
3. 先覆盖 `create生产序列()`、`PackSemiAutoStepsToPlc()` 相关的 HMI 显示与输入边界
4. 保持 PLC、`Hmi_iSemiAuto`、`Hmi_rArray`、`Hmi_rSlitter` 内部仍为毫米

这批完成后优先验证：

- 英寸界面输入不会改变 PLC 最终收到的毫米值
- 半自动步骤生成结果与当前毫米版本一致

### 9.10 建议的后续批次

#### 批次 1：主链路单位层

- 新增统一单位工具
- 收口订单、折弯长度、锥度长度、半自动后挡位置
- 收口 `PackSemiAutoStepsToPlc()` 前的显示与输入

#### 批次 2：参数页与手动页

- `SubOPManual`
- `SubOPSetting`
- `FrmFeed`
- `FrmInlineSlit`
- `SubOPSlitter`

#### 批次 3：文件兼容与全面清理

- 订单/库文件单位标识
- `.Designer.cs` 静态单位文本清理
- 全量回归

### 9.11 建议的下一轮最小改动文件

- `HMI/JSZW1000A/JSZW1000A/MainFrm.cs`
- `HMI/JSZW1000A/JSZW1000A/MainFrm.SemiAuto.cs`
- 必要时新增 `HMI/JSZW1000A/JSZW1000A/MainFrm.Unit.cs`
- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPAuto.cs`
- `HMI/JSZW1000A/JSZW1000A/SubWindows/SubOPAutoView.cs`

### 9.12 本任务的推荐路线

本任务最稳妥的落地顺序是：

1. 先做 HMI 英寸显示与输入
2. 再做 HMI 内部边界统一换算
3. 保持 PLC 与设备参数继续使用毫米
4. 最后再决定是否有必要进入 PLC 原生英寸化阶段

如果按这个顺序推进，可以把“单位切换”控制在 HMI 边界内，最大程度降低对当前设备执行稳定性的影响。

## 10. 后续功能需求记录（2026-04-22）

本节先记录后续待增加功能，当前状态为需求池，不代表已经完成方案设计或排期。后续拆分落地时，应按 PLC 动作安全、HMI 操作闭环、数据结构兼容性分别细化。

1. 设备各工位复位至初始状态
   - 增加整机/分工位复位入口，明确夹钳、后挡、翻板、送料、出料、分切等单元的初始状态、复位顺序、互锁条件和异常恢复策略。
2. 角度表新增删减功能
   - 在 HMI 中补齐角度表的新增、删除、编辑、保存和校验流程，避免只能依赖固定表或手工改文件。
3. 出料平台
   - 增加出料平台相关控制、状态显示、自动流程衔接和安全联锁，后续需确认机械 IO、动作节拍和与折弯/送料的交接位置。
4. 程序自动功能：不下发到半自动，直接点击自动开始按钮
   - 增加从当前订单/步骤表直接进入自动运行的入口，减少“先下发到半自动再启动”的操作步骤，同时保留必要的数据校验和启动前确认。
5. 多种类的折弯步骤生成
   - 支持更多板型/工艺类型的步骤生成规则，后续应抽象生成策略，避免继续把特殊工艺写死在单一生成函数中。
6. 折弯碰撞计算
   - 增加折弯过程中板材、夹钳、翻板、后挡、出料平台等关键部件的干涉/碰撞判断，用于生成前校验和运行前提示。
7. 钳口下压高度控制
   - 增加钳口下压高度的参数设定、下发、显示和动作确认，需明确不同板厚、不同折弯类型下的默认值和安全范围。
8. 自动进料位置全尺寸适应以及锥度板进料位置
   - 优化自动进料位置计算，使不同长度、宽度、锥度板都能得到正确进料位置，并补齐锥度板进料基准和偏移规则。
9. 俄语、法语界面
   - 增加俄语、法语语言资源和切换机制，后续需整理现有中英文硬编码文本，统一到多语言资源表。
10. 3D 图形预览
    - 增加折弯过程 3D 预览能力，后续需明确预览数据源、步骤状态快照、板材厚度/方向表现，以及与现有 2D 预览规则的关系。
