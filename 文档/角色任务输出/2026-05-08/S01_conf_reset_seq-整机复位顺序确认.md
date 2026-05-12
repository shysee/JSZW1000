# S01_conf_reset_seq 整机复位顺序确认

日期：2026-05-08

## 1. 任务范围

本次只做离线源码核查和复位顺序文档确认，不修改 PLC/HMI 源码，不变更安全互锁，不变更 I/O 映射。

任务目标来自 `.hermes/task.yaml`：

- 确认 `iWholeRst` 当前复位顺序是否适合实际设备。
- 检查翻板、夹钳、分条、出料平台等工位的复位安全互锁。
- 输出复位顺序、启动条件、安全检测项、失败分支和超时保护的确认材料。

## 2. 当前代码事实

### 2.1 复位入口

| 项目 | 当前证据 | 结论 |
| --- | --- | --- |
| HMI 入口 | `Hmi_bArray[41]` 写入 PLC 后，在 `fbMachine.TcPOU` 中赋给 `bReset` | HMI 整机复位入口存在 |
| PLC 上升沿 | `rtReset(CLK := bReset)` | 复位请求由上升沿触发 |
| 物理按钮 | `GVL_IO.In_btnReset AT %I* : BOOL` | 物理按钮映射存在，但本次未发现它直接接入 `iWholeRst` |
| 状态回传 | `Hmi_iArray[23] := TO_UINT(iWholeRstStatus)` | PLC 已回传整机复位状态码 |

状态码当前定义在 `fbMachine.TcPOU`：

- `0`: idle
- `1`: accepted
- `2`: running
- `3`: done
- `4`: failed/canceled
- `5`: timeout

### 2.2 复位状态机

`fbMachine.TcPOU` 中 `A_ManualRun` 已实现 `CASE iWholeRst OF`，当前顺序如下：

| 步骤 | 动作 | 完成条件/检测项 |
| --- | --- | --- |
| `10 -> 11` | `TaskBackGauge := eTask_BackGauge_BackPos` | `TaskBackGauge = eTask_NULL AND BackGauge_AtHome` |
| `20 -> 21` | `TaskTable := eTask_Table_AbsPos`, `paraTablePos := 0` | `TaskTable = eTask_NULL AND Table_AtHome` |
| `30 -> 31` | `TaskFeed := eTask_Feed_AbsPos`, `paraFeedPos := 0` | `TaskFeed = eTask_NULL AND Feed_AtHome` |
| `40 -> 41` | 必要时 `TaskSlitter := eTask_SlitterHome` | 仅在 `Clamp_AtZeroPoint`、上下翻板原点/滑台姿态满足时允许回分条 |
| `50 -> 52` | 必要时切换滑台并回下翻板原点 | `TaskApron = eTask_NULL AND ApronBtmFold_AtHome` |
| `60 -> 62` | 必要时切换滑台并回上翻板原点 | `TaskApron = eTask_NULL AND ApronTopFold_AtHome` |
| `70 -> 71` | 滑台回默认安全姿态 | `TaskApron = eTask_NULL` |
| `80 -> 81` | `TaskClamp := eTask_ClampUnlock`, `paraClamp := 2` | `TaskClamp = eTask_NULL` |
| `90` | 汇总确认 | 分条、后挡、台面、送料、上下翻板均在 home 后置 `iWholeRstStatus := 3` |

### 2.3 失败和超时分支

复位请求触发时会先清空以下任务：

- `TaskClamp`
- `TaskBackGauge`
- `TaskApron`
- `TaskSlitter`
- `TaskTable`
- `TaskFeed`

当前失败/中断逻辑：

- `ton181(IN := iWholeRst <> 0, PT := T#30000MS)` 作为整机复位总超时。
- 进入半自动、分条、手动模式，或总超时触发时，清空任务并置状态：
  - 超时：`iWholeRstStatus := 5`
  - 非超时但复位未完成：`iWholeRstStatus := 4`
  - 无活动复位：`iWholeRstStatus := 0`
- 完成、失败、超时状态保持 3 秒后回到 `0`。

### 2.4 工位状态来源

| 工位 | 状态位 | 来源 |
| --- | --- | --- |
| 分条 | `Slitter_AtHome` | `fb_Slitter.TcPOU`: `In_sigSlitLmt0` |
| 后挡 | `BackGauge_AtHome` | `fb_BackGauge.TcPOU`: 后挡实际位置与 `BackgaugeMaxPos` 窗口 |
| 台面 | `Table_AtHome` | `fb_BackGauge.TcPOU`: `Table_ActPos < 5.0` |
| 送料 | `Feed_AtHome` | `fb_BackGauge.TcPOU`: `Feed_ActPos > -100.0 AND Feed_ActPos < 5.0` |
| 翻板滑台 | `ApronTopSlide_AtWork`, `ApronBtmSlide_AtWork` | `fb_Apron.TcPOU`: 实际位置 + 到位开关 |
| 翻板折弯 | `ApronTopFold_AtHome`, `ApronBtmFold_AtHome` | `fb_Apron.TcPOU`: 实际角度 + 原点开关 |
| 夹钳 | `Clamp_AtZeroPoint` | `fb_Clamp.TcPOU`: 夹钳高度实际位置 |

## 3. 安全互锁核查

本次确认到的代码互锁：

- 分条不在原位时，翻板滑动/折弯动作不能直接放行。
- 分条回原位前，`iWholeRst` 要求夹钳和翻板处于可复位姿态。
- 夹钳动作依赖分条、翻板、台面、送料等状态边界。
- 整机复位总超时不会继续推进后续步骤。
- 复位中切换到半自动、分条或手动模式会中断当前复位并清任务。

未在本次离线源码中完成确认的边界：

- `GVL_IO.In_btnReset` 物理按钮是否在现场通过其它链路接入整机复位。
- 出料平台是否存在、是否需要纳入整机复位。
- 急停、断气、断压恢复后的现场策略是否与当前“中断后需重新发起”口径一致。
- 各原点/到位信号在真实机构上的可靠性。

## 4. 与 2026-04-22 前置文档的差异

2026-04-22 文档曾记录 `iWholeRst` 为空、`eTask_SlitterHome` 未任务化。当前源码已经不同：

- `iWholeRst` 状态机已实现。
- `E_UnitTask` 已存在 `eTask_SlitterHome`。
- `fb_Slitter.TcPOU` 已能消费 `TaskSlitter = eTask_SlitterHome` 并在完成后清空任务。
- `Hmi_iArray[23]` 已回传整机复位状态码。

因此，当前 S01 的离线结论应以 2026-05-08 源码为准；旧文档只作为历史前置背景。

## 5. 当前结论

离线源码层面，`iWholeRst` 已形成一个可追踪的整机复位顺序：

1. 后挡回原位。
2. 台面回原位。
3. 送料回原位。
4. 分条在可复位姿态下回原位。
5. 下翻板回原点。
6. 上翻板回原点。
7. 翻板滑台回默认安全姿态。
8. 夹钳解锁到安全位。
9. 汇总确认分条、后挡、台面、送料、上下翻板 home 后完成。

但 S01 的“现场确认/签字确认”尚未完成。下一步需要现场人员确认：

- 当前顺序是否符合真实机构安全节拍。
- 出料平台是否存在并纳入复位。
- 物理复位按钮是否必须与 HMI 复位共用同一状态机入口。
- 异常恢复是否接受当前超时/中断策略。

## 6. 验证记录

已执行：

```powershell
$root='Tc_JSZW400\JSZW'
$files=Get-ChildItem -Path $root -Recurse -File -Include *.TcPOU,*.TcDUT,*.TcGVL,*.plcproj,*.tsproj
$bad=@()
foreach($f in $files){
  try { [xml](Get-Content -LiteralPath $f.FullName -Raw -Encoding UTF8) | Out-Null }
  catch { $bad += $f.FullName }
}
if($bad.Count -eq 0){ "XML_OK $($files.Count)" } else { "XML_FAIL $($bad.Count)"; $bad }
```

结果：

```text
XML_OK 104
```

验证边界：

- 该检查只证明 TwinCAT XML 文件可解析。
- 未运行 TwinCAT 编译。
- 未做现场实机复位。
- 未确认物理按钮和出料平台。

## 7. 签字确认

| 角色 | 结论 | 签字/日期 |
| --- | --- | --- |
| 电气/机械现场负责人 | 待确认复位顺序与真实机构安全节拍一致 | 待签 |
| PLC 工程师 | 待确认当前状态机可进入现场联调 | 待签 |
| HMI/操作负责人 | 待确认状态码和提示文案满足操作员识别 | 待签 |
