# 折弯预览功能规划

> 文档版本: 1.0
> 最后更新: 2026-05-10
> 对应 CONTEXT.md 领域术语版本: 1.0

## 概述

折弯预览（Fold Preview）是 JSZW400 HMI 端的核心功能之一，运行在 `SubOPAutoView` 子窗口中。它将工件的几何折弯数据 (`OrderType.lengAngle[]`) 和半自动执行步骤序列 (`OrderType.lstSemiAuto[]`) 以 2D 图形动画的形式实时展示给操作员，同时提供碰撞危险警示、操作提示、步骤导航和方案切换功能。

预览是"折弯候选方案浏览 → 选定正式方案 → 发送到 PLC"工作流中的关键验证环节。预览所见即发送给 PLC 执行的真实步骤序列。

---

## 1. 功能范围与定位

### 1.1 预览在系统中的位置

```
┌──────────────┐    ┌──────────────────────┐    ┌─────────────────┐
│  工件定义     │ → │  序列生成             │ → │  预览验证        │
│  (lengAngle) │    │  (lstSemiAuto)       │    │  (SubOPAutoView)│
└──────────────┘    └──────────────────────┘    └────────┬────────┘
                                                          │
                     ┌──────────────────────────────────────┘
                     ↓
         ┌─────────────────────────┐
         │  候选方案浏览            │
         │  Next Plan / Cycle     │
         │  (TryPreviewNext...)   │
         └───────────┬─────────────┘
                     ↓
         ┌─────────────────────────┐
         │  手动编辑 (SubOPAutoSet)│
         │  布局确认验证           │
         └───────────┬─────────────┘
                     ↓
         ┌─────────────────────────┐
         │  发送到 PLC             │
         │  (Hmi_iSemiAuto 数组)  │
         └─────────────────────────┘
```

### 1.2 预览职责边界

| 预览负责 | 预览不负责 |
|---------|-----------|
| 渲染板料几何轮廓（feed-phase / fold-phase） | 生成折弯候选方案（`MainFrm.SemiAuto.cs` 负责） |
| 播放折弯动画（逐步骤推进） | 编辑步骤顺序和参数（`SubOPAutoSet` 负责） |
| 碰撞危险红色警示（碰撞检测算法） | 布局验证可行性（`ValidateLayoutConfirmation` 负责） |
| 翻面 (FLIP) 过渡动画 | PLC 实际执行折弯 |
| 挤压 (Squash / OpenSquash) 可视化 | 生成/删除步骤行 |
| 文字步骤列表和操作提示 | 文件 I/O、订单保存/加载 |
| 方案切换（Next Plan） | 回弹补偿应用（手动模式单独处理） |
| 预览偏好切换（正/逆序、颜色面） | |

### 1.3 与 CONTEXT.md 的对应关系

预览功能严格遵循 CONTEXT.md 中定义的领域规则：

| CONTEXT.md 规则 | 预览实现 |
|----------------|---------|
| **折弯预览以折弯点为中心** (`Fold-point centered preview geometry`) | `TransformPreviewProfileToStepFrame` — 以 `坐标序号` 指定的节点为旋转支点，不使用后挡位置 |
| **后挡料喂送为刚体平移** (`Backgauge feed rigid translation`) | 喂送阶段 (`IsFeedPhase`) 不旋转板料，仅改变坐标系原点 |
| **翻转参考中心线固定** (`Flip reference centerline`) | `GetPreviewFlipReferencePoint` — 返回板料宽度的中点，不随折弯过程变化 |
| **颜色面影响初始放置方向** (`Color-side placement effect`) | `ResolveSemiAutoPreviewColorDown` — 跟踪颜色面状态，影响折弯方向 |
| **A-B / B-A 侧模式不直接决定上/下** (`A-B side-mode non-directionality`) | 折弯上下方向由 `ResolveEffectivePreviewDirection` 结合颜色面决定 |
| **组合碰撞区域** (`Combined collision zone`) | `BuildPreviewCollisionPolygons` — 由上/下折刀边界 + 夹钳区域半平面组装 |
| **预览碰撞候选点规则** | `GetPreviewCollisionSegmentDetails` — 排除当前折弯点，在 screen-right 分支上寻找 |
| **预览红色警示规则** | `RecalculatePreviewCollisionSegmentsForCurrentDraw` — 碰撞的板料段变红色 |
| **Squash / OpenSquash 几何区别** | `DrawSinglePreviewSquash` — 两种挤压有不同可见间隙渲染 |
| **OpenSquash 间隙取配置值的一半** | `GetPreviewOpenSquashGapPixels` — `Hmi_rArray[129] * 0.5` |
| **Implicit flip insertion** | `ShouldFlipAfterCurrentPreviewStep` — 相邻步骤侧模式变化时自动插入 FLIP 过渡 |
| **重置视图动作** (`Reset view action`) | `btnSetZero_Click` — 返回第 0 步；方案失效时触发完整重生成 |

---

## 2. 模块架构

预览功能跨越 4 个源代码文件：

```
SubOPAutoView.cs                    →  UI 交互/事件/刷新入口
SubOPAutoView.PreviewTimeline.cs   →  核心渲染管线 (构建分段 → 变换 → 碰撞检测 → 绘制)
SubOPAutoView.PreviewText.cs       →  步骤列表文字渲染 (RichTextBox 输出)
MainFrm.SemiAuto.DerivedState.cs   →  几何状态构建 (profile 构建/翻转/折叠变换)
MainFrm.SemiAuto.cs                →  碰撞检测算法/碰撞多边形生成/配置管理
```

### 2.1 文件职责

| 文件 | 职责 |
|------|------|
| `SubOPAutoView.cs` | 页面生命周期 (`Load`/`RefreshPreviewState`)、定时器 (`timer1`/`tmr预览`)、导航按钮事件、偏好开关 (正逆序/颜色面)、绘图初始化 (`InitDraw`/`redrawPreView`) |
| `SubOPAutoView.PreviewTimeline.cs` | **核心渲染引擎**：每帧数据构建 (`BuildPreviewSegmentsForDrawStep`)、几何变换 (`TransformPreviewProfileToStepFrame`)、翻面动画 (`BuildPreviewFlipTransitionFrame`)、挤压渲染 (`DrawPreviewSquash`)、碰撞多边形绘制 (`DrawPreviewCollisionBasis`) |
| `SubOPAutoView.PreviewText.cs` | 右侧步骤列表 RichTextBox 渲染 (`RenderFoldListText`)、操作提示文本 (`UpdatePreviewStepInfo`)、结构说明模式 (`showStructureExplanationMode`) |
| `MainFrm.SemiAuto.DerivedState.cs` | 派生状态计算：`BuildSemiAutoPreviewStageProfile`（逐步构建已折弯轮廓）、`ResolveSemiAutoPreviewColorDown`（颜色面状态跟踪）、`ApplyPreviewFoldStep`（单步折叠变换） |
| `MainFrm.SemiAuto.cs` | 碰撞检测引擎：`GetPreviewCollisionSegmentSeverities`、`BuildPreviewCollisionPolygons`、`PreviewCollisionBoundary` 参数化边界配置、`PreviewCollisionConfig` 持久化 |

---

## 3. 数据流与渲染管线

### 3.1 每帧渲染流程

```
┌──────────────────────────────────────────────────────────────────────────┐
│                         折弯预览每帧渲染流程                               │
├──────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  1.  PreViewSt() [PreviewTimeline.cs:29]                                 │
│      ├─ 检查是否需要插入 FLIP 过渡                                       │
│      │   └─ ShouldFlipAfterCurrentPreviewStep() → Flip_DataProc()        │
│      └─ iDrawStep++                                                      │
│                                                                          │
│  2.  BuildPreviewSegmentsForDrawStep(iDrawStep) [PreviewTimeline.cs:100] │
│      ├─ 确定当前步骤索引: GetCurrentPreviewStepIndex()                    │
│      │   · feed phase: iDrawStep=0 或 iDrawStep%2==1                     │
│      │   · fold phase: iDrawStep>0 && iDrawStep%2==0                     │
│      ├─ 获取显示步骤: GetDisplayedPreviewStepIndex()                      │
│      │   · flip 完成后 → 显示下一侧模式的第一个步骤                       │
│      ├─ 构建分阶段轮廓: BuildSemiAutoPreviewStageProfile()                │
│      │   · 从初始 pxList 开始，逐步应用前 N 个已执行步骤                  │
│      │   · 对每个折弯步骤：旋转 screen-right 分支                        │
│      │   · 对翻转点：整体旋转 180°                                       │
│      ├─ 坐标系变换: TransformPreviewProfileToStepFrame()                 │
│      │   · 以 坐标序号 为折弯支点                                         │
│      │   · 将 screen-right 端点旋转至水平朝左                             │
│      ├─ FLIP 过渡帧: BuildPreviewFlipTransitionFrame()                   │
│      │   · 绕固定中心线旋转 -90° + 透视收缩                               │
│      └─ 挤压状态更新: UpdateAppliedSquashStates()                        │
│                                                                          │
│  3.  RecalculatePreviewCollisionSegmentsForCurrentDraw()                  │
│      └─ GetPreviewCollisionSegmentSeverities()                            │
│          · 对 screen-right 候选点逐一做半平面碰撞测试                     │
│          · 根据 SoftCollisionRatio 分为 Soft/Hard 两级                   │
│                                                                          │
│  4.  redrawPreView() [SubOPAutoView.cs:179]                              │
│      ├─ 清空画布，绘制背景图                                             │
│      ├─ 绘制虚线中心线                                                   │
│      ├─ 绘制碰撞基准区域 (DrawPreviewCollisionBasis)                      │
│      │   · 上/下折刀边界半平面                                           │
│      │   · 夹钳碰撞带 (upper clamp band)                                │
│      │   · 送料区域 (feed area)                                         │
│      ├─ 绘制翻面运动覆盖层 (DrawPreviewFlipMotionOverlay)                  │
│      ├─ 绘制板料轮廓 (DrawLine + 碰撞颜色)                               │
│      │   · 白色=正常段, 橙色=Soft碰撞, 红色=Hard碰撞                    │
│      │   · 颜色面指示线 (+5px/-5px)                                     │
│      └─ 绘制挤压效果 (DrawPreviewSquash)                                 │
│                                                                          │
│  5.  UpdatePreviewStepInfo() [PreviewText.cs:5]                          │
│      ├─ 确定操作提示类型 (Feed/Flip/Fold Up/Fold Down/Squash)           │
│      └─ RenderFoldListText() → 右侧 RichTextBox                         │
│                                                                          │
└──────────────────────────────────────────────────────────────────────────┘
```

### 3.2 iDrawStep 步进机制

预览使用 `iDrawStep` 整数步进器，每帧 +1：

| iDrawStep | 阶段 | 说明 |
|-----------|------|------|
| `0` | Feed (初始) | 显示未折弯的平板，`BuildPreviewSegmentsForDrawStep` 不做任何折叠 |
| `1, 3, 5, ...` | Feed (每步前) | 后挡料喂送到下一折弯点，`BuildSemiAutoPreviewStageProfile` 应用了 N 步折弯，`afterCurrentFold=false` |
| `2, 4, 6, ...` | Fold | 正在执行第 N/2 步折弯，`afterCurrentFold=true`，此时 geometry 已包含当前折弯结果 |
| 奇数 + `showingFlipCompletionState=true` | Flip 过渡 | A/B 侧切换时的 180° 旋转动画，`BuildPreviewFlipTransitionFrame` 变换 |
| `> count*2` | 结束 | 自动停止 `tmr预览` |

### 3.3 几何变换链

```
pxList (原始图形点, List<PointF>)
    │
    │ BuildSemiAutoPreviewStageProfile(order, appliedStepCount, flipAfterLast)
    │
    ├─ for each fold step:
    │   ApplyPreviewFoldStep()
    │   · 旋转 angle = 180° ± foldAngle (取决于折弯方向)
    │   · 旋转范围由 内外选择 决定 (lower/higher index 侧)
    │
    ├─ for each flip point:
    │   RotateWholePreviewProfile(180°)
    │   · 绕固定翻转中心线旋转 (width/2, 0)
    │
    ▼
stagedProfile (分阶段几何, List<PointF>)
    │
    │ TransformPreviewProfileToStepFrame(displayStep)
    │
    ├─ 确定锚点: displayStep.坐标序号
    ├─ 确定邻居点: 内外选择 决定左/右邻居
    ├─ 计算当前朝向角: atan2(neighbor - anchor)
    └─ 整体旋转到水平朝左 (PI 弧度)
        │
        ▼
transformed (变换后屏幕坐标, List<PointF>)
    │
    ├─ +cx, -cy (屏幕坐标系转换)
    │
    ▼
pxDraw (屏幕坐标点, List<Point>) → GDI+ DrawLine
```

### 3.4 碰撞检测流程

```
BuildPreviewCollisionPolygons(step, boundary)
    │
    ├─ 下 flap 工作状态:
    │   danger = left of lower flap outer edge
    │         AND (left of upper clamp boundary
    │              OR left of upper parked flap outer edge)
    ├─ 上 flap 工作状态:
    │   danger = left of upper flap outer edge
    │         AND (left of lower clamp boundary
    │              OR left of lower parked flap outer edge)
    │
    ▼
collisionPolygons (危险区域多边形列表)
    │
    │ GetPreviewCollisionSegmentSeverities(machineProfile, ...)
    │
    ├─ 候选点: screen-right 分支端点 + 拐角点 (排除当前折弯点)
    ├─ 对每个候选点做多边形内测试 (point-in-polygon)
    ├─ 碰撞段: 候选点相邻的两段标记
    └─ severity = Hard (ratio > SoftCollisionRatio) : Soft
```

---

## 4. 核心数据结构

### 4.1 预览状态字段 (SubOPAutoView)

```csharp
// 步进与阶段
int iDrawStep;                    // 当前绘制步 (0=初始, 奇数=喂送, 偶数=折弯)
bool showingFlipCompletionState;   // 当前是否处于 FLIP 过渡动画状态

// 几何渲染
List<PointF> currentPreviewPolyline;      // 当前帧的分阶段轮廓 (机器坐标)
Dictionary<int, PreviewCollisionSeverity>  // 碰撞段 → 严重程度映射
    currentPreviewCollisionSegments;
bool currentPreviewIsFoldCompletionState;  // 是否处于折弯完成状态
bool currentPreviewComparesWorkingFlap;    // 是否对比工作 flap 区域
int currentPreviewAppliedStepCount;        // 已应用的步骤数

// 挤压状态
Dictionary<int, SquashDisplayState> currentPreviewAppliedSquashStates;
HashSet<int> currentPreviewSuppressedSegments;

// 显示参数
bool currentPreviewColorDown;      // 当前颜色面状态 (影响折弯方向文字)
MainFrm.SemiAutoType currentPreviewDisplayStep;  // 当前显示的步骤

// UI 模式
bool showStructureExplanationMode; // true=结构说明模式, false=步骤列表模式
```

### 4.2 碰撞边界参数 (PreviewCollisionBoundary)

所有参数可通过 `Config.ini` 的 `[PreviewCollision]` 节持久化：

```csharp
readonly struct PreviewCollisionBoundary
{
    double ClampArmWidth;           // 夹钳臂宽 (mm)
    double TopBoundaryAngle;        // 上折刀边界角度 (°)
    double UpperClampThickness;     // 上夹钳厚度 (mm)
    double UpperClampVerticalOffset; // 上夹钳垂直偏移 (mm)
    double BottomBoundaryAngle;     // 下折刀边界角度 (°)
    double UpperFlapAngle;          // 上折刀角度 (°)
    double UpperFlapHeight;         // 上折刀高度 (mm)
    double LowerFlapAngle;          // 下折刀角度 (°)
    double LowerFlapHeight;         // 下折刀高度 (mm)
    double ParkedFlapRetreatUnit;   // 停驻折刀后撤单位 (mm)
    double FlapLength;              // 折刀长度 (mm)
    double SoftCollisionRatio;      // Soft/Hard 分界比例 (默认 0.8)
}
```

### 4.3 挤压显示状态

```csharp
readonly record struct SquashDisplayState(
    int EdgeIndex,     // 0=头部挤压, 99=尾部挤压
    int ActionType,    // Squash(1) / OpenSquash(2)
    int Direction,     // 折弯方向
    bool ColorDown     // 颜色面状态
);
```

---

## 5. 关键函数说明

### 5.1 入口与刷新

| 函数 | 文件:行 | 说明 |
|------|---------|------|
| `RefreshPreviewState()` | SubOPAutoView.cs:98 | 主刷新入口。判断是否有手动编辑→规范化；否则兜底重建→刷新视图 |
| `InitDraw(bool)` | SubOPAutoView.cs:150 | 初始化绘图：排序步骤表、设置画布中心点、重绘初始帧 |
| `btnPreViewSt_Click()` | SubOPAutoView.cs:146 | 单步前进按钮：调用 `PreViewSt()` |
| `btnSetZero_Click()` | SubOPAutoView.cs:235 | 重置到第 0 步，停止自动播放 |
| `btnNextPlan_Click()` | SubOPAutoView.cs:298 | 切换到下一个候选方案 (`mf.TryPreviewNextSemiAutoPlan()`) |

### 5.2 核心渲染管线

| 函数 | 文件:行 | 说明 |
|------|---------|------|
| `BuildPreviewSegmentsForDrawStep(int)` | PreviewTimeline.cs:100 | **核心数据构建**。输入 `iDrawStep`，输出 `pxDraw`（屏幕坐标点列表）|
| `PreViewSt()` | PreviewTimeline.cs:29 | 步进器推进逻辑。检查 FLIP 需求、递增步、调用 `refshPoint()` 和 `redrawPreView()` |
| `redrawPreView(bool)` | SubOPAutoView.cs:179 | GDI+ 渲染主函数。遍历 `pxDraw` 画线，根据 `currentPreviewCollisionSegments` 选择颜色 |
| `RecalculatePreviewCollisionSegmentsForCurrentDraw()` | PreviewTimeline.cs:181 | 碰撞检测入口。构建碰撞多边形，对候选点做半平面测试 |
| `TransformPreviewProfileToStepFrame(...)` | PreviewTimeline.cs:419 | 坐标系变换。以折弯支点为原点，将工作侧旋转至水平 |
| `BuildPreviewFlipTransitionFrame(...)` | PreviewTimeline.cs:474 | 构建 FLIP 过渡帧：绕中心线 -90° + 透视收缩 |
| `DrawPreviewSquash(...)` | PreviewTimeline.cs:533 | 挤压效果绘制。遍历 `currentPreviewAppliedSquashStates`，画间隙线 |
| `DrawPreviewCollisionBasis(...)` | PreviewTimeline.cs:211 | 绘制机器碰撞基准区域（调试用，`PreviewCollisionAreaVisible` 控制） |

### 5.3 几何状态计算

| 函数 | 文件 | 说明 |
|------|------|------|
| `BuildSemiAutoPreviewStageProfile(...)` | DerivedState.cs:71 | 从初始 pxList 构建到指定步骤数的已折弯轮廓 |
| `ApplyPreviewFoldStep(...)` | DerivedState.cs:161 | 对一个折弯步骤执行 profile 侧旋转 |
| `RotateWholePreviewProfile(...)` | DerivedState.cs:193 | 整体旋转 profile 180°（用于 FLIP） |
| `ResolveSemiAutoPreviewColorDown(...)` | DerivedState.cs:80 | 跟踪颜色面状态：遇到 FLIP 取反 |
| `ResolveEffectivePreviewDirection(...)` | MainFrm.cs | 根据颜色面 + 折弯角度符号决定上/下方向 |
| `GetPreviewCollisionSegmentSeverities(...)` | SemiAuto.cs:2480 | 碰撞段检测，返回每个碰撞段的 Hard/Soft 等级 |
| `BuildPreviewCollisionPolygons(...)` | SemiAuto.cs:3164 | 构建上/下 flap 危险区域多边形 |

### 5.4 文字渲染

| 函数 | 文件:行 | 说明 |
|------|---------|------|
| `UpdatePreviewStepInfo()` | PreviewText.cs:5 | 更新操作提示标签 (`lb下一操作提示`) 和方案摘要 |
| `RefreshPreviewInfoText(int)` | PreviewText.cs:58 | 刷新右侧 RichTextBox（结构说明模式 / 步骤列表模式） |
| `RenderFoldListText(...)` | PreviewText.cs:103 | 渲染步骤列表，带 `> ` 高亮当前步骤 |
| `BuildStepTitle(...)` | PreviewText.cs:154 | 构建单步标题（如 "折弯 1"、"压死边"、"翻面"） |
| `BuildStepDetails(...)` | PreviewText.cs:170 | 构建单步详情（后挡位置、折弯角度、回弹值、压钳高度） |

---

## 6. UI 交互

### 6.1 预览控制按钮

| 控件 | 行为 |
|------|------|
| `btnPreViewSt` (单步) | 每点击一次推进一帧 (`PreViewSt()`) |
| `btn自动预览` (自动/点动切换) | 切换 `tmr预览` 定时器；自动模式下 `timer1_Tick` 每 `Interval` ms 触发一次 |
| `trackBar1` (速度调节) | 改变 `tmr预览.Interval = Maximum - Value` (快←→慢) |
| `btnSetZero` (重置步骤) | `iDrawStep=0; showingFlipCompletionState=false` → `InitDraw(true)` |
| `btnNextPlan` (下一个方案) | 调用 `mf.TryPreviewNextSemiAutoPlan()` → `RefreshPreviewState()` |

### 6.2 预览偏好开关

| 开关 | 点击行为 | 效果 |
|------|---------|------|
| `sw正逆序` | 切换 `CurtOrder.st逆序` | 调用 `mf.TryApplyPreviewPreferences()` 重排候选方案 |
| `sw颜色面` | 切换 `CurtOrder.st色下` | 同上；若无候选方案可用则规范化并重建派生状态 |
| `sw继续步骤` | 切换 `Hmi_bArray[71]` | 控制 PLC 端执行模式：继续/单步 |
| `sw分条开关` | 切换 `Hmi_bArray[72]` | 启用/禁用边做边分切 |

### 6.3 预览模式切换

`btnPreviewTextMode` 在两种模式间切换：

| 模式 | 显示内容 |
|------|---------|
| 步骤列表模式 (默认) | 右侧 RichTextBox 显示 `折弯1`, `后挡位置...`, `折弯角度...` 等步骤详情 |
| 结构说明模式 | 显示 `mf.GetCurrentFormalPlanStructureExplanation()` 返回的结构化说明文本 |

---

## 7. 碰撞检测详细规则

### 7.1 危险区域组合规则 (来自 CONTEXT.md)

```
上 flap 工作状态:
  danger = left of upper flap outer edge
         AND (left of lower clamp boundary OR left of lower parked flap outer edge)

下 flap 工作状态:
  danger = left of lower flap outer edge
         AND (left of upper clamp boundary OR left of upper parked flap outer edge)
```

### 7.2 碰撞严重程度分类

```
collisionRatio ∈ [0.0, 1.0]:  候选点落入危险区域的比例
    ·
    · > SoftCollisionRatio (默认 0.8)  →  Hard (红色)
    ·
    · ≤ SoftCollisionRatio              →  Soft (橙色)
```

### 7.3 红色警示输出规则 (CONTEXT.md)

- 碰撞段 = 碰撞候选点直接相邻的两段
- 当前折弯点本身不参与碰撞测试
- 头/尾端点仅在属于 screen-right 分支时才标记其唯一相邻段

### 7.4 碰撞区域调试多边形

当 `PreviewCollisionAreaVisible=true`（通过配置节 `[PreviewCollision]` 的 `ShowCollisionArea`）时，预览画面上会绘制红色半透明多边形（选中区域）和蓝色多边形（候选区域），用于视觉校准。**调试多边形编号不是业务逻辑的真实来源**，真值来源是每个区域的半平面签名。

---

## 8. 挤压 (Squash) 预览规则

### 8.1 Squash vs OpenSquash 几何区别

| 类型 | ActionType | 可见间隙 | 渲染方式 |
|------|-----------|---------|---------|
| `Squash` (压死边) | `1` | `PreviewClosedSquashGapPixels = 2.0` | 最小可见间隙（贴着折弯边）|
| `OpenSquash` (压开边) | `2` | `Hmi_rArray[129] * 0.5` | 配置值的半值 |

### 8.2 挤压渲染流程

```
UpdateAppliedSquashStates(appliedStepCount)
    · 遍历已执行步骤中的 Squash/OpenSquash
    · 按 edgeIndex (0=头, 99=尾) 记录到 currentPreviewAppliedSquashStates

DrawPreviewSquash(graphic, outlinePen)
    · 遍历 currentPreviewAppliedSquashStates
    · 获取屏幕线段端点 (TryGetScreenEdgeSegment)
    · 计算垂直间隙偏移 (GetPreviewOpenSquashGapPixels)
    · 绘制三条线: 两条间隙线 + 一条折弯边
```

---

## 9. 配置管理

### 9.1 预览碰撞配置

配置文件：`Config.ini` 或应用启动目录下的指定路径

```ini
[PreviewCollision]
ShowCollisionArea = false
ClampArmWidth = 30.0
TopBoundaryAngle = 85.0
UpperClampThickness = 20.0
UpperClampVerticalOffset = 8.0
BottomBoundaryAngle = 85.0
UpperFlapAngle = 45.0
UpperFlapHeight = 88.0
LowerFlapAngle = 45.0
LowerFlapHeight = 88.0
ParkedFlapRetreatUnit = 10.0
FlapLength = 350.0
SoftCollisionRatio = 0.8
```

配置加载流程：

```
InitAct() [MainFrm.cs:576]
    → LoadInitSet()
        → MainFrm.LoadPreviewCollisionConfig()
            → ReadPreviewCollisionConfig(path)
                → Parse INI [PreviewCollision] section
                → PreviewCollisionConfig.WithValue(key, value)
            → EnsurePreviewCollisionConfigFile()
                → Auto-append missing keys to file
```

---

## 10. 依赖关系图

```
SubOPAutoView.cs
    │
    ├──► SubOPAutoView.PreviewTimeline.cs
    │         │
    │         ├──► MainFrm.BuildSemiAutoPreviewStageProfile()
    │         │           │
    │         │           └──► MainFrm.SemiAuto.DerivedState.cs
    │         │                      ├─ BuildFlatPreviewProfile()
    │         │                      ├─ ApplyPreviewFoldStep()
    │         │                      ├─ RotateWholePreviewProfile()
    │         │                      └─ ResolveSemiAutoPreviewColorDown()
    │         │
    │         ├──► TransformPreviewProfileToStepFrame()
    │         ├──► BuildPreviewFlipTransitionFrame()
    │         ├──► DrawPreviewSquash()
    │         └──► MainFrm.GetPreviewCollisionSegmentSeverities()
    │                     │
    │                     └──► MainFrm.SemiAuto.cs
    │                                ├─ BuildPreviewCollisionPolygons()
    │                                ├─ ClassifyPreviewCollisionSeverity()
    │                                └─ PreviewCollisionBoundary (配置)
    │
    ├──► SubOPAutoView.PreviewText.cs
    │         │
    │         ├──► MainFrm.GetCurrentFormalPlanSummaryText()
    │         ├──► MainFrm.GetRuntimeMessagesSnapshot()
    │         └──► MainFrm.GetCurrentFormalPlanStructureExplanation()
    │
    └──► MainFrm (父窗口引用 mf)
              │
              ├─ TryPreviewNextSemiAutoPlan()     ← 方案切换
              ├─ TryApplyPreviewPreferences()    ← 偏好应用
              ├─ HasManualSemiAutoEdits()         ← 手动编辑检测
              ├─ NormalizeGeneratedSemiAutoSequence()
              └─ AdsWritePlc1Bit() / AdsReadPlc1Bit()  ← PLC 通信
```

---

## 11. 当前实现状态

### 11.1 已实现

| 功能 | 状态 |
|------|------|
| 初始平板渲染 | ✅ 完成 |
| 逐步骤折弯动画（feed/fold 交替） | ✅ 完成 |
| A/B 侧模式切换的 FLIP 过渡动画 | ✅ 完成 |
| 颜色面状态跟踪（影响显示方向） | ✅ 完成 |
| 折弯碰撞检测（Hard/Soft 分级红色警示） | ✅ 完成 |
| 碰撞区域调试多边形（可选显示） | ✅ 完成 |
| Squash (压死边) 预览渲染 | ✅ 完成 |
| OpenSquash (压开边) 预览渲染 | ✅ 完成 |
| 方案切换 (Next Plan) | ✅ 完成 |
| 预览偏好（正/逆序、颜色面）切换 | ✅ 完成 |
| 步骤列表文字渲染（RichTextBox） | ✅ 完成 |
| 操作提示标签 (Feed/Flip/Fold Up/Down/Squash) | ✅ 完成 |
| 自动播放定时器 (`tmr预览`) | ✅ 完成 |
| 速度调节 (`trackBar1`) | ✅ 完成 |
| 配置持久化 (INI 文件) | ✅ 完成 |
| 回弹补偿值显示 | ✅ 完成 |

### 11.2 潜在改进方向

| 方向 | 说明 |
|------|------|
| **挤压动画过渡** | 当前 Squash/OpenSquash 在 `fold completion` 状态下直接渲染最终间隙；可增加从平直到间隙的渐进动画 |
| **后挡料喂送动画** | 当前 feed phase 是瞬时切换；可增加板料平移的连续动画 |
| **OpenGL 渲染** | 项目已引入 `OpenTK.GLControl` 但当前使用 GDI+；复杂轮廓和抗锯齿可受益于硬件加速 |
| **触摸操作** | 预览画面支持触摸拖拽、捏合缩放 |
| **3D 预览** | 增加第三个维度展示折弯深度 |
| **方案历史记录** | 记录浏览过的方案，方便回退比较 |
| **方案对比视图** | 并排显示两个候选方案的预览动画 |
| **碰撞热力图** | 对每个候选方案整体评分时可视化其碰撞风险分布 |

---

## 12. 测试

### 12.1 单元测试

```
HMI/JSZW1000A/JSZW1000A.Tests/
└── SemiAutoPreviewTests.cs
```

### 12.2 测试覆盖重点

- `BuildPreviewSegmentsForDrawStep` — 各种 `iDrawStep` 边界情况
- `TransformPreviewProfileToStepFrame` — 锚点边界、内外选择切换
- `BuildPreviewFlipTransitionFrame` — 180° 翻转后透视收缩正确性
- `GetPreviewCollisionSegmentSeverities` — Hard/Soft 分类阈值
- `ResolveSemiAutoPreviewColorDown` — 多步 FLIP 后的颜色面状态
- `DrawPreviewSquash` — Squash vs OpenSquash 间隙渲染差异
