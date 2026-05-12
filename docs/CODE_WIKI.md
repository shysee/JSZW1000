# JSZW400 Code Wiki

## 项目概述

JSZW400 是一个**双向智能折边机 HMI/SCADA 系统**，基于 WinForms (HMI端) + Beckhoff TwinCAT (PLC控制端) 的双层架构，实现钣金折弯生产的半自动/全自动控制。

项目名称中的 "JSZW" 代表 "精工折弯"，"400" 为设备型号。系统支持手动操作、半自动作业、全自动生产、订单管理、工艺库管理、分条作业等功能。

---

## 1. 项目整体架构

```
┌─────────────────────────────────────────────────────────────────┐
│                         JSZW400 系统架构                          │
├───────────────────────────────┬─────────────────────────────────┤
│         HMI 层 (C# WinForms)   │       PLC 层 (TwinCAT 3)        │
│   e:\_Work\JSZW双向智能折边机\   │  e:\_Work\JSZW双向智能折边机\     │
│   JSZW400\HMI\JSZW1000A\       │  JSZW400\Tc_JSZW400\            │
├───────────────────────────────┼─────────────────────────────────┤
│  ┌─────────────────────────┐  │  ┌─────────────────────────────┐│
│  │  MainFrm.cs             │  │  │  MAIN.TcPOU                 ││
│  │  (主窗口/状态机/ADS通信)  │  │  │  (PLC主程序/模式切换)        ││
│  └─────────────────────────┘  │  └─────────────────────────────┘│
│  ┌─────────────────────────┐  │  ┌─────────────────────────────┐│
│  │  SubWindows/            │  │  │  UNIT/fb_Apron/             ││
│  │  - SubOPManual          │  │  │  - fb_Apron.TcPOU          ││
│  │  - SubOPAuto            │  │  │  - fb_BackGauge            ││
│  │  - SubOPAutoView        │  │  │  - fb_Clamp                ││
│  │  - SubOPSetting         │  │  │  - fb_Slitter              ││
│  │  - SubOPLibrary         │  │  │  - FEED                    ││
│  │  - SubOPSlitter         │  │  └─────────────────────────────┘│
│  └─────────────────────────┘  │  ┌─────────────────────────────┐│
│  ┌─────────────────────────┐  │  │  PTP/                      ││
│  │  Domain Types           │  │  │  - Axis_PTPV3_CoE          ││
│  │  - OrderType            │  │  │  - Ax_MotionCtrl           ││
│  │  - SemiAutoType         │  │  │  - Ax_HomingCtrl           ││
│  │  - LengAngle            │  │  └─────────────────────────────┘│
│  │  - AngleAddit           │  │  ┌─────────────────────────────┐│
│  └─────────────────────────┘  │  │  Actuators/                 ││
│  ┌─────────────────────────┐  │  │  - IOMotor                  ││
│  │  Core Logic             │  │  │  - CYLINDER                 ││
│  │  - SemiAuto Generation  │  │  │  - EcFcCom                  ││
│  │  - Preview Rendering    │  │  └─────────────────────────────┘│
│  │  - Collision Detection  │  │  ┌─────────────────────────────┐│
│  │  - Plan Validation      │  │  │  EventLogger/              ││
│  └─────────────────────────┘  │  │  - EventCtrl               ││
│  ┌─────────────────────────┐  │  │  - EventsWriteToLogbook     ││
│  │  ADS Communication      │  │  └─────────────────────────────┘│
│  │  TcAdsClient           │  │                                  │
│  └─────────────────────────┘  │                                  │
└───────────────────────────────┴─────────────────────────────────┘
        ↕ ADS/RTU 协议 (TwinCAT.Ads.dll)
```

### 通信架构

HMI 与 PLC 通过 **Beckhoff ADS (Automation Device Specification)** 协议通信：

- **ADS Port**: 851 (PLC任务端口)
- **通信库**: `TwinCAT.Ads.dll` (引用 `bin\Debug\net6.0-windows7.0\TwinCAT.Ads.dll`)
- **通信模式**: 轮询 + 通知 (Notifications) 混合
- **变量映射**: HMI 端维护 `Hmi_bArray[bool[300]]`、`Hmi_iArray[Int16[300]]`、`Hmi_rArray[float[300]]` 三组数组，分别对应 PLC 中的 BOOL/INT/REAL 类型的变量块

### 目录结构

```
JSZW400/
├── HMI/
│   └── JSZW1000A/
│       ├── JSZW1000A.sln
│       └── JSZW1000A/
│           ├── JSZW1000A.csproj              ← HMI 项目文件
│           ├── Program.cs                     ← 应用入口
│           ├── MainFrm.cs                     ← 主窗口/核心状态机
│           ├── MainFrm.SemiAuto.cs            ← 半自动序列生成
│           ├── MainFrm.SemiAuto.DerivedState.cs
│           ├── MainFrm.ManualSemiAuto.cs      ← 手动模式逻辑
│           ├── MainFrm.Unit.cs                ← 单位转换工具
│           ├── AppLanguage.cs
│           ├── LocalizationManager.cs         ← 多语言管理
│           ├── LocalizationText.cs
│           ├── DisplayUnitManager.cs          ← mm/inch 单位管理
│           ├── Strings.Designer.cs            ← 本地化字符串资源
│           ├── SubWindows/
│           │   ├── SubOPManual.*              ← 手动操作页
│           │   ├── SubOPAuto.*                ← 自动作业主容器
│           │   ├── SubOPAutoView.*            ← 折弯预览/时间线
│           │   ├── SubOPAutoView.PreviewText.cs
│           │   ├── SubOPAutoView.PreviewTimeline.cs
│           │   ├── SubOPAutoSet.*             ← 布局/参数设置页
│           │   ├── SubOPAutoDraw.*            ← 自动绘图页
│           │   ├── SubOPSetting.*             ← 系统设置页
│           │   ├── SubOPLibrary.*             ← 工艺库管理
│           │   ├── SubOPSlitter.*             ← 分条作业页
│           │   └── SubCheckItem.*             ← 启动检查项
│           ├── Resources/                      ← 图像资源(中/英)
│           └── myPic/                         ← UI 图标资源
├── Tc_JSZW400/
│   └── JSZW/
│       ├── JSZW400.tsproj                    ← TwinCAT 项目
│       ├── JSZW.sln
│       └── Untitled1/
│           ├── Untitled1.tmc                 ← 类型管理器缓存
│           ├── Untitled1.plcproj
│           ├── GVLs/                         ← 全局变量列表
│           │   ├── GVL.TcGVL
│           │   ├── GVL_CONST.TcGVL
│           │   ├── GVL_Com.TcGVL
│           │   ├── GVL_IO.TcGVL
│           │   └── APP_Com.TcDUT
│           ├── DUTs/                         ← 用户自定义数据类型
│           │   └── DUTs/
│           │       ├── ST_Folder.TcDUT        ← 折弯机状态结构
│           │       ├── ST_FolderPara.TcDUT
│           │       ├── ST_FolderCom.TcDUT
│           │       ├── ST_FolderStatus.TcDUT
│           │       ├── ST_OneJob.TcDUT
│           │       ├── ST_UnitInfo.TcDUT
│           │       ├── ST_AxFunctions_*.TcDUT
│           │       ├── E_UnitName.TcDUT
│           │       ├── E_ActWorkUnits.TcDUT
│           │       └── E_PMLUserUnitMode.TcDUT
│           ├── POUs/                         ← 程序组织单元
│           │   ├── MAIN.TcPOU                ← PLC 主程序
│           │   ├── fbMachine.TcPOU           ← 机器状态机(PML)
│           │   ├── Axis_PTPV3_CoE.TcPOU      ← 轴控(CoE CANopen over EtherCAT)
│           │   ├── Ax_HMI.TcPOU              ← HMI 轴控接口
│           │   ├── FB_Visu.TcPOU             ← 可视化管理
│           │   ├── FB_Generic.TcPOU
│           │   ├── ERROR_HDL.TcPOU
│           │   ├── INPUT.TcPOU
│           │   ├── UNIT/
│           │   │   ├── FEED.TcPOU
│           │   │   ├── fb_Apron/             ← 折弯板(上下折刀)控制
│           │   │   ├── fb_BackGauge/         ← 后挡料控制
│           │   │   ├── fb_Clamp/             ← 夹钳控制
│           │   │   └── fb_Slitter/           ← 分条刀控制
│           │   ├── PTP/                      ← 点位运动库
│           │   │   ├── 01_Base/
│           │   │   │   ├── Ax_MotionCtrl.TcPOU
│           │   │   │   ├── Ax_HomingCtrl.TcPOU
│           │   │   │   └── Ax_AdminCtrl.TcPOU
│           │   │   ├── 02_Options/
│           │   │   │   ├── Drive/Ethercat/
│           │   │   │   └── Parameter/
│           │   │   └── 03_Utilities/
│           │   ├── Actuators/                ← 执行器驱动库
│           │   │   ├── Motor/
│           │   │   ├── Cylinder/
│           │   │   ├── FreqConv/
│           │   │   ├── OUTPUT.TcPOU
│           │   │   ├── AIN_EL3004.TcPOU
│           │   │   └── ANALOG_OUT.TcPOU
│           │   └── EventLogger/              ← 事件日志系统
│           └── VISUs/
│               └── VIS_Main.TcVIS
├── AGENTS.md                                ← Agent 工具配置
├── CONTEXT.md                               ← 领域语言/业务规则定义
├── 370.ini / 3525.ini / Fold_*.ini         ← 配置文件/订单数据文件
└── 更新日志.txt
```

---

## 2. HMI 端模块详解

### 2.1 入口与初始化 — `Program.cs`

```csharp
ApplicationConfiguration.Initialize();
LocalizationManager.InitializeFromConfig();    // 从配置加载语言
DisplayUnitManager.InitializeFromConfig();       // 从配置加载单位制(mm/inch)
Application.Run(new MainFrm());
```

### 2.2 主窗口 — `MainFrm.cs`

`MainFrm` 是 HMI 端的中央枢纽，继承自 `Form`，负责：

| 职责 | 说明 |
|------|------|
| 页面导航 | 根据导航按钮切换 `gpbSubWin` 中的子窗口控件 |
| ADS 通信 | 管理 `TcAdsClient` 连接，维护 HMI ↔ PLC 变量映射 |
| 订单管理 | 管理 `GblOrder`（订单列表）和 `CurtOrder`（当前订单） |
| 折弯角度补偿 | 管理 `angleAddit[]`（400行×50列的补偿表） |
| 配置管理 | 加载/保存 `ConfigData[]`、`ConfigStr[]` |
| 布局状态 | 跟踪 `AutoWorksheetQuickDraw`/`Setup`/`Preview`/`Production` 四个工作区 |
| 语言/单位 | 通过 `Lang` 和 `DisplayUnitManager` 支持中/英/法/俄 + mm/inch |

#### 核心数据结构

**`OrderType`** — 工件/订单定义：

```csharp
public struct OrderType
{
    public double TopSpring, BtmSpring;          // 上下折弯回弹补偿
    public bool isSlitter;                       // 分条模式
    public bool 边做边分切启用;                   // 边做边分切
    public bool st逆序, st色下;                   // 正/逆序, 颜色面朝下
    public double SlitterWid;                    // 分条宽度
    public string Name, Customer, MaterialName, Remark;
    public double Width, Thickness, SheetLength;  // 板料尺寸
    public LengAngle[] lengAngle;                // 折弯几何数据[100]
    public List<PointF> pxList;                   // 图形轮廓点列表
    public List<SemiAutoType> lstSemiAuto;       // 半自动执行步骤序列
    public bool isTaper;                         // 锥度模式
    public double TaperWidth, TaperLength;
    public bool 半自动步骤已手动编辑;
    public string SemiAutoPlanOrigin;            // "generated-selected" | "custom-manual"
}
```

**`LengAngle`** — 单个折弯角的定义：

```csharp
public struct LengAngle
{
    public double Length;         // 折弯边长度
    public double Angle;          // 折弯角度 (+=上翻, -=下翻)
    public double TaperWidth;     // 锥度宽度
    public bool YinYang;          // 阴阳折标记
    public bool isRayAngle;      // 射线角度
    public double RayAngle_R, RayAngle_Num;
}
```

**`SemiAutoType`** — 半自动执行步骤：

```csharp
public struct SemiAutoType
{
    public int 折弯序号;           // 折弯步骤序号
    public int 行动类型;           // 0=折弯, 1=Squash, 2=OpenSquash, 3=Slit, 8=FLIP
    public double 折弯角度;         // 目标折弯角度
    public int 折弯方向;           // 1=上翻(Up), 0=下翻(Down)
    public double 后挡位置;
    public int 坐标序号;           // 几何坐标索引(折弯旋转支点)
    public int 长角序号;           // 0=无挤压, 1=首挤压, 99=尾挤压
    public int 内外选择;           // 0=A-B(A外侧), 1=B-A(B外侧)
    public int 重新抓取;           // 是否需要重新抓取
    public double 回弹值;
    public int 松开高度;
    public int 抓取类型;
    public bool is色下;
    public double 锥度斜率;
    public int 操作提示;           // 0=无需操作, 1=翻面
}
```

**`AngleAddit`** — 折弯角度补偿表条目：

```csharp
public struct AngleAddit
{
    public string Type, Material, Strength, Thickness, MachingGauging;
    public float[] AngleRange;     // [50]: 前15个=下翻补偿, 后15个=上翻补偿
}
```

#### 关键字段/数组

| 变量 | 类型 | 说明 |
|------|------|------|
| `Hmi_bArray[300]` | `bool[]` | HMI→PLC 布尔变量映射 |
| `Hmi_iArray[300]` | `Int16[]` | HMI→PLC 整数变量映射 |
| `Hmi_rArray[300]` | `float[]` | HMI→PLC 实数变量映射 |
| `Hmi_iSemiAuto[300]` | `Int16[]` | 半自动控制数组 |
| `Hmi_iAuto[300]` | `Int16[]` | 全自动控制数组 |
| `Hmi_rSlitter[30]` | `float[]` | 分条参数数组 |
| `Hmi_iAngleMapTop/Btm[200]` | `Int16[]` | 角度映射表 |
| `Hmi_iHeightMap[400]` | `Int16[]` | 高度映射表 |
| `Hmi_rAdvPara[100]` | `float[]` | 进阶参数 |
| `ConfigData[200]` | `float[]` | 系统配置参数 |
| `ConfigStr[200]` | `string[]` | 系统字符串配置 |
| `angleAddit[400]` | `AngleAddit[]` | 折弯角度补偿表 |
| `GblOrder` | `List<OrderType>` | 订单列表 |
| `CurtOrder` | `OrderType` | 当前活动订单 |

#### 导航函数

| 函数 | 目标页面 |
|------|---------|
| `fun导航_手动()` | SubOPManual — 手动折弯操作 |
| `btn导航_自动_Click()` / `切入自动1()` | SubOPAuto — 自动作业入口 |
| `btn导航_库_Click()` | SubOPLibrary — 工艺库管理 |
| `btn导航_设置_Click()` | SubOPSetting — 系统设置 |
| `btn导航_分条_Click()` | SubOPSlitter — 分条作业 |
| `导航_CheckItem()` | SubCheckItem — 启动检查 |

### 2.3 半自动序列生成 — `MainFrm.SemiAuto.cs`

这是 HMI 端最核心的业务逻辑模块，负责将工件的折弯几何数据 (`LengAngle[]`) 自动生成为可执行的半自动步骤序列 (`lstSemiAuto`)。

#### 核心流程

1. **`create生产序列()`** — 入口函数，根据 `CurtOrder.lengAngle` 重建 `lstSemiAuto`
2. **候选方案生成** — `GenerateSemiAutoCandidates()` 生成多个可行的折弯方案
3. **碰撞检测** — `DetectPreviewCollisions()` 检测折弯候选点是否进入危险区域
4. **方案排序/评分** — 根据碰撞风险、操作次数等指标给候选方案打分
5. **布局验证** — `ValidateLayoutConfirmation()` 在操作员确认布局编辑前验证可行性

#### 关键结构

- **`SemiAutoGenerationContext`** — 生成上下文，封装 order + reverse/color 偏好/生产模式选项
- **`SemiAutoGenerationResult`** — 生成结果，包含 `Success`、`StrategyName`、`Steps`、`FailureCode`
- **`FoldShapeProfile`** — 板料形状画像（折弯数量、正向/负向折数、宽/厚、是否有头/尾挤压）
- **`FoldStepSnapshot`** — 折弯步骤快照，用于方案间比较
- **`FoldSequenceCandidate`** — 候选方案，含步骤列表、评分、策略名
- **`InlineSlitPlan`** — 边做边分切方案

#### 核心常量

```csharp
public const int SemiAutoActionFold = 0;       // 普通折弯
public const int SemiAutoActionSquash = 1;     // 挤压折弯
public const int SemiAutoActionOpenSquash = 2;  // 开式挤压折弯
public const int SemiAutoActionSlit = 3;       // 分条
public const int SemiAutoActionFlip = 8;        // 翻面

public const string SemiAutoPlanOriginGeneratedSelected = "generated-selected";
public const string SemiAutoPlanOriginCustomManual = "custom-manual";
```

### 2.4 手动模式 — `MainFrm.ManualSemiAuto.cs`

处理手动折弯操作的逻辑，包括回弹补偿应用、夹钳/折刀位置控制、参数下发等。

### 2.5 单位管理 — `MainFrm.Unit.cs`

```csharp
public static double MmToDisplayLength(double mm)
    // 当前单位制为 mm 时返回 mm，否则转换为英寸

public static string FormatDisplayLength(double mm, int decimals = 3)
    // 格式化长度值，自动加单位后缀

public static bool TryParseDisplayLength(string? text, out double mm)
    // 解析带单位或不带单位的输入文本，支持 "mm"/"in"/"英寸"/"inch"/'"'

public static float ParseDisplayLengthFloatOrZero(string? text)
    // 安全解析，失败返回 0
```

### 2.6 子窗口模块

| 模块 | 文件 | 职责 |
|------|------|------|
| **手动操作** | `SubOPManual.*` | 手动折弯的实时控制 UI，发送操作命令到 PLC |
| **自动作业容器** | `SubOPAuto.*` | 包含自动模式的子页面容器，管理子页面切换 |
| **折弯预览** | `SubOPAutoView.*` | 核心预览渲染 — 绘制板料几何、折弯动画、碰撞红色警示 |
| **预览文本** | `SubOPAutoView.PreviewText.cs` | 预览步骤的文字说明（操作提示、回弹值、后挡位置等） |
| **预览时间线** | `SubOPAutoView.PreviewTimeline.cs` | 预览步骤的时间线/分步控制 |
| **布局/设置** | `SubOPAutoSet.*` | 编辑折弯步骤顺序和参数，确认后验证可行性 |
| **自动绘图** | `SubOPAutoDraw.*` | 板料图形的绘制/编辑 |
| **系统设置** | `SubOPSetting.*` | 机器参数、补偿表、语言、单位等配置 |
| **工艺库** | `SubOPLibrary.*` | 保存/加载标准工艺参数 |
| **分条作业** | `SubOPSlitter.*` | 分条模式下的边做边分切控制 |
| **启动检查** | `SubCheckItem.*` | 开机前的安全检查项确认 |

### 2.7 国际化/本地化

- **`LocalizationManager`** — 管理应用语言（zh-CN/en-US/fr-FR/ru-RU），从配置文件读取
- **`DisplayUnitManager`** — 管理显示单位（mm / inch）
- **`Strings.Designer.cs`** — RESX 字符串资源文件，通过 `Strings.Get(key)` 访问
- **`LocalizationText.cs`** — 程序化文本（如折弯方向、正/逆序标签的文字生成）

---

## 3. PLC 端模块详解

### 3.1 PLC 主程序 — `MAIN.TcPOU`

PLC 启动后执行 `Boot()` → `ModeSw()` → `fbMachine()` 状态机循环。

关键逻辑：
- **模式切换** (`ModeSw`): 根据 `PackTags.Command.UnitMode` 切换机器模式
- **命令分发** (`nStateCommandCheck`): 将 HMI 发来的 PackML 命令 (Start/Hold/Stop/Abort...) 路由到 `fbMachine.eCommand`
- **持久化存储** (`fbWritePersistentValue`): 定时将数据写回持久存储

### 3.2 机器状态机 — `fbMachine.TcPOU`

基于 **PackML (ISA TR88.01)** 标准的状态机，实现以下状态：

```
Undefined → Idle ↔ Execute ↔ Complete → Idle
                  ↕ Held ↔ Suspended
                  ↘ Aborted → Clearing → Stopped → Idle
```

| 模式 | 值 | 说明 |
|------|----|------|
| `ePMLState_Undefined` | 0 | 未定义 |
| `ePMLState_Idle` | 1 | 待机 |
| `ePMLState_Execute` | 2 | 执行中 |
| `ePMLState_Held` | 3 | 暂停 |
| `ePMLState_Suspended` | 4 | 悬挂 |
| `ePMLState_Complete` | 5 | 完成 |
| `ePMLState_Aborted` | 6 | 中止 |
| `ePMLState_Clearing` | 7 | 清理 |
| `ePMLState_Stopped` | 8 | 停止 |

### 3.3 折弯机数据结构 — `ST_Folder.TcDUT`

```pascal
ST_Folder :
STRUCT
    iMode         :INT;                          // 当前模式
    Status        :ST_FolderStatus;              // 折弯机状态
    ApronTop_Fold_ActVal  :DINT;                 // 上折刀折弯实际值
    ApronBtm_Fold_ActVal  :DINT;                 // 下折刀折弯实际值
    ApronTop_Slide_ActVal :DINT;                 // 上折刀滑动实际值
    ApronBtm_Slide_ActVal :DINT;                // 下折刀滑动实际值
    ApronTop_Fold_ActAngle:REAL;                // 上折刀实际角度
    ApronBtm_Fold_ActAngle:REAL;                // 下折刀实际角度
    ClampHt_ActPos:REAL;                        // 夹钳高度实际位置
    PumpPress_ActVal:INT;                        // 液压泵压力
    BackGauge_T1~T4_ActPos:REAL;                // 四个后挡料位置
    Feed_ActPos   :REAL;                        // 送料实际位置
    Table_ActPos   :REAL;                        // 工作台实际位置
    Taper_SW       :BOOL;                        // 锥度开关
    Taper_Type     :INT;                        // 锥度类型
    ActWorkUnitCom :E_ActWorkUnits;              // 实时工作单元通信状态
END_STRUCT
```

### 3.4 单元控制 (UNIT/)

| POU | 职责 |
|-----|------|
| `fb_Apron.TcPOU` | 上下折刀的运动控制（折弯角度、滑动位置、模拟量↔角度转换） |
| `fb_BackGauge.TcPOU` | 四个后挡料轴的位置控制 |
| `fb_Clamp.TcPOU` | 夹钳的夹紧/松开/高度控制 |
| `fb_Slitter.TcPOU` | 分条刀的执行控制 |
| `FEED.TcPOU` | 板料进给控制 |

### 3.5 轴控制 (PTP/)

| POU | 职责 |
|-----|------|
| `Axis_PTPV3_CoE.TcPOU` | 基于 CoE (CANopen over EtherCAT) 的点位运动控制，功能块封装 |
| `Ax_MotionCtrl.TcPOU` | 基础运动控制（使能、回零、点动、绝对/相对定位） |
| `Ax_HomingCtrl.TcPOU` | 回零/找原点控制 |
| `Ax_AdminCtrl.TcPOU` | 轴管理（故障复位、急停、安全限位） |
| `Ax_Para.TcPOU` | 轴参数管理 |
| `Ax_AutoFctCtrl.TcPOU` | 自动功能块控制 |
| `Ax_TouchProbeCtrl.TcPOU` | 触摸探头/位置捕获 |

### 3.6 执行器驱动 (Actuators/)

| POU | 设备类型 |
|-----|---------|
| `IOMotor.TcPOU` | 普通电机 (正/反转) |
| `IOMotor2Dir.TcPOU` | 双向电机 |
| `IOMotorStarDelta.TcPOU` | 星-三角电机启动 |
| `CYLINDER.TcPOU` | 气缸/液压缸控制 |
| `EcFcCom.TcPOU` | EtherCAT 变频器通信 |
| `IOFcCom.TcPOU` | 普通 IO 变频器通信 |
| `OUTPUT.TcPOU` | 数字量输出 |
| `ANALOG_OUT.TcPOU` | 模拟量输出 |
| `AIN_EL3004.TcPOU` | EL3004 模拟量输入模块 |

### 3.7 事件日志 (EventLogger/)

| POU | 职责 |
|-----|------|
| `EventCtrl.TcPOU` | 事件触发控制器（Class 10/25/50） |
| `EventStatistik.TcPOU` | 事件统计 |
| `EventsWriteToLogbook.TcPOU` | 写入日志本 |
| `ReadAdsEvents.TcPOU` | 通过 ADS 读取事件数据 |

---

## 4. 关键业务流程

### 4.1 半自动作业流程

```
┌─────────────┐    ┌──────────────┐    ┌─────────────┐    ┌──────────────┐
│ 工件定义     │ → │ 序列生成     │ → │ 预览验证    │ → │ 发送到PLC    │
│ (lengAngle)  │    │ (lstSemiAuto) │    │ (SubOPAutoView)│  │ (Hmi_iSemiAuto)│
└─────────────┘    └──────────────┘    └─────────────┘    └──────────────┘
                         ↓
              ┌─────────────────────┐
              │  候选方案浏览        │
              │  NextPlan/Cycle    │
              │  评分排序/碰撞检测  │
              └─────────────────────┘
                         ↓
              ┌─────────────────────┐
              │  手动编辑 (SubOPAutoSet) │
              │  布局确认验证       │
              └─────────────────────┘
```

### 4.2 全自动作业流程

```
┌─────────────┐    ┌──────────────┐    ┌─────────────┐    ┌──────────────┐
│ 启动检查     │ → │ 半自动发送    │ → │ 全自动启动  │ → │ PackML状态机 │
│ (SubCheckItem)│  │ (Hmi_iAuto)   │    │ (ePMLCommand_Start)│  │ fbMachine循环 │
└─────────────┘    └──────────────┘    └─────────────┘    └──────────────┘
```

### 4.3 预览碰撞检测逻辑

根据 CONTEXT.md 中定义的业务规则：

1. **组合碰撞区域** (`Combined Collision Zone`) — 由参数化机器边界半平面组装，从折弯点（原点）出发
2. **下 flap 工作状态**：`danger = left of lower flap outer edge AND (left of upper clamp boundary OR left of upper parked flap outer edge)`
3. **上 flap 工作状态**：`danger = left of upper flap outer edge AND (left of lower clamp boundary OR left of lower parked flap outer edge)`
4. **预览碰撞候选点**：当前折弯点排除；在当前 screen-right 形成的分支上；全局头尾端点仅在属于 screen-right 分支时参与
5. **红色警示输出**：当候选点进入危险区域时，其直接相邻的板料段被标记为红色

---

## 5. 依赖关系与第三方库

### 5.1 NuGet 包

| 包名 | 版本 | 用途 |
|------|------|------|
| `TwinCAT.Ads` | 引用本地 DLL | ADS 通信协议 |
| `ClosedXML` | 0.105.0 | Excel 导入/导出（订单文件） |
| `netDxf` | 2023.11.10 | DXF 文件读写（图形导入） |
| `Newtonsoft.Json` | 13.0.3 | JSON 序列化/反序列化（配置/订单） |
| `OpenTK.GLControl` | 4.0.2 | OpenGL 渲染（预留，当前预览主要用 GDI+） |

### 5.2 系统依赖

- **.NET 10** (Windows Desktop)
- **TwinCAT 3.1** (Build 4024+)
- **Beckhoff TwinCAT.Ads.dll** — ADS 通信库（本地引用）

### 5.3 配置文件

| 文件 | 位置 | 用途 |
|------|------|------|
| `370.ini` / `3525.ini` | 项目根目录 | 机器配置参数 |
| `Fold_*.ini` | 项目根目录 | 折弯订单/工艺文件 |
| `ConfigStr[1]` | 内存/Config.ini | 当前加载的订单文件路径 |

---

## 6. 项目运行方式

### 6.1 HMI 运行

```bash
# 使用 Visual Studio 打开
HMI/JSZW1000A/JSZW1000A.sln

# 或命令行构建
dotnet build HMI/JSZW1000A/JSZW1000A/JSZW1000A.csproj

# 发布
dotnet publish HMI/JSZW1000A/JSZW1000A/JSZW1000A.csproj -c Release
```

运行前提：
- TwinCAT 运行时已启动并处于 Run 模式
- ADS Route 已配置（HMI 的 `AdsConnEx()` 连接到 PLC 的 AMS NetId）

### 6.2 PLC 端

使用 **TwinCAT 3 XAE (eXtended Automation Engineering)** 打开：

```
Tc_JSZW400/JSZW/JSZW400.tsproj
```

构建并激活配置后，PLC 程序在 TwinCAT 运行时中执行。

### 6.3 启动流程

```
1. TwinCAT Runtime 启动
2. PLC MAIN 开始执行 (Boot → ModeSw → fbMachine)
3. HMI 程序启动
4. MainFrm.InitAct() 执行:
   - LoadParaFile(1)         加载机器参数
   - LoadInitSet()           加载初始设置
   - LoadOrderFile()         加载订单文件
   - setLang()               设置界面语言
   - AdsConnEx()             建立 ADS 连接
   - timer1s.Start()         启动1秒轮询定时器
5. 操作员在 SubCheckItem 完成启动检查
6. 进入手动/自动/库/设置等页面操作
```

---

## 7. 重要约定与设计模式

### 7.1 文件约定

- **所有 `.Designer.cs`、`.resx`、`.csproj` 文件不可删除或重命名** — Visual Studio Designer 依赖这些文件
- 订单/配置文件使用 `.ini` 格式，通过 `ClosedXML` (Excel) 或手写 INI 解析
- HMI 代码中的中文变量名（如 `折弯序号`、`后挡位置`）为历史遗留，保持兼容

### 7.2 HMI ↔ PLC 通信约定

- 所有 ADS 通信变量句柄在 `InitAct()` 后的 `AdsConnEx()` 中一次性获取
- 变量句柄缓存在 `H_*` 系列字段中（`H_bArray`、`H_iArray`、`H_rArray` 等）
- 写入 PLC：`adsClient.WriteAny(handle, value)`
- 读取 PLC（轮询）：`adsClient.ReadAny(handle, ref value)`
- 读取 PLC（通知）：通过 `AdsNotifications` 回调

### 7.3 核心设计模式

- **Partial Class 分拆**：`MainFrm` 按功能拆分为 `MainFrm.cs`、`MainFrm.SemiAuto.cs`、`MainFrm.ManualSemiAuto.cs`、`MainFrm.SemiAuto.DerivedState.cs`、`MainFrm.Unit.cs`
- **单例式全局状态**：`MainFrm` 中的 `static` 字段（`GblOrder`、`CurtOrder`、`Hmi_*Array`、`ConfigData`）作为全局单例状态
- **子窗口持有引用**：所有 `SubWindows` 子类通过构造函数或属性持有 `MainFrm` 引用 (`mf`)，直接访问全局状态

### 7.4 领域术语（来自 CONTEXT.md）

| 术语 | 含义 |
|------|------|
| **Fold Candidate Plan** | 从当前订单生成的多个机械可行折弯方案 |
| **Formal Semi-Auto Plan** | 选定的折弯方案，用于保存和发送到 PLC |
| **Plan Origin Marker** | 记录方案是"从候选中选择"还是"手动编辑" |
| **Regeneration Boundary** | 触发方案重新生成的条件（几何改变/折弯顺序改变） |
| **Preview Preference** | 不触发重生的调整（正/逆序、颜色面偏好） |
| **A-B / B-A Side Mode** | 决定哪边在外侧、哪边在内侧的模式标志 |
| **Flip Requirement** | A/B 侧切换时产生的翻面需求 |
| **Combined Collision Zone** | 由机器边界半平面组合的预览碰撞危险区域 |
| **Color-Side Placement Effect** | 颜色面选择影响预览的初始放置/方向 |

---

## 8. 测试

单元测试位于：

```
HMI/JSZW1000A/JSZW1000A.Tests/
├── JSZW1000A.Tests.csproj
└── SemiAutoPreviewTests.cs
```

运行测试：

```bash
dotnet test HMI/JSZW1000A/JSZW1000A.Tests/
```
