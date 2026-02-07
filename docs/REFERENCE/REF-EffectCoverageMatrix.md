# Nova System Manager - Effect Coverage Matrix

Purpose: map `REF-EffectReference.md` + `REF-SysexMapTables.md` to Domain + ViewModel coverage.

## Global Parameters
| Parameter | Ref Range | Domain Property | ViewModel Property | Status |
| --- | --- | --- | --- | --- |
| Tap Tempo | 100-3000 ms | `Preset.TapTempo` | `PresetDetailViewModel.TapTempo` | OK |
| Routing | 0-2 | `Preset.Routing` | `PresetDetailViewModel.Routing` | OK |
| Level Out L | -100..0 dB | `Preset.LevelOutLeft` | `PresetDetailViewModel.LevelOutLeft` | OK |
| Level Out R | -100..0 dB | `Preset.LevelOutRight` | `PresetDetailViewModel.LevelOutRight` | OK |
| Map Parameter | 0-127 | `Preset.MapParameter` | `PresetDetailViewModel.MapParameter` | OK |
| Map Min | 0-100% | `Preset.MapMin` | `PresetDetailViewModel.MapMin` | OK |
| Map Mid | 0-100% | `Preset.MapMid` | `PresetDetailViewModel.MapMid` | OK |
| Map Max | 0-100% | `Preset.MapMax` | `PresetDetailViewModel.MapMax` | OK |

## Compressor (Comp)
| Parameter | Ref Range | Domain Property | ViewModel Property | Status |
| --- | --- | --- | --- | --- |
| Type | 0-2 | `Preset.CompType` | `CompressorBlockViewModel.Type/TypeId` | OK |
| Threshold (adv) | -30..0 dB | `Preset.CompThreshold` | `CompressorBlockViewModel.Threshold` | OK |
| Ratio (adv) | 0-15 | `Preset.CompRatio` | `CompressorBlockViewModel.Ratio` | OK |
| Attack (adv) | 0-16 table | `Preset.CompAttack` | `CompressorBlockViewModel.Attack` | OK |
| Release (adv) | 13-23 table | `Preset.CompRelease` | `CompressorBlockViewModel.Release` | OK |
| Response (perc/sus) | 1-10 | `Preset.CompResponse` | `CompressorBlockViewModel.Response` | OK |
| Drive (perc/sus) | 1-20 | `Preset.CompDrive` | `CompressorBlockViewModel.Drive` | OK |
| Level | -12..+12 dB | `Preset.CompLevel` | `CompressorBlockViewModel.Level` | OK |
| Enabled | 0/1 | `Preset.CompressorEnabled` | `CompressorBlockViewModel.IsEnabled` | OK |

## Drive
| Parameter | Ref Range | Domain Property | ViewModel Property | Status |
| --- | --- | --- | --- | --- |
| Type | 0-1 | `Preset.DriveType` | `DriveBlockViewModel.Type/TypeId` | OK |
| Gain | 0-100% | `Preset.DriveGain` | `DriveBlockViewModel.Gain` | OK |
| Tone | 0-100% | `Preset.DriveTone` | `DriveBlockViewModel.Tone` | OK |
| Level | -100..0 dB | `Preset.DriveLevel` | `DriveBlockViewModel.Level` | OK |
| Boost Level | 0-10 dB | `Preset.BoostLevel` | `DriveBlockViewModel.BoostLevel` | OK |
| Boost Enabled | 0/1 | `Preset.BoostEnabled` | `DriveBlockViewModel.BoostEnabled` | OK |
| Enabled | 0/1 | `Preset.DriveEnabled` | `DriveBlockViewModel.IsEnabled` | OK |

## Modulation
| Parameter | Ref Range | Domain Property | ViewModel Property | Status |
| --- | --- | --- | --- | --- |
| Type | 0-5 | `Preset.ModType` | `ModulationBlockViewModel.Type/TypeId` | OK |
| Speed | 0-81 table | `Preset.ModSpeed` | `ModulationBlockViewModel.Speed` | OK |
| Depth | 0-100% | `Preset.ModDepth` | `ModulationBlockViewModel.Depth` | OK |
| Tempo | 0-16 table | `Preset.ModTempo` | `ModulationBlockViewModel.Tempo` | OK |
| Hi Cut | 0-61 table | `Preset.ModHiCut` | `ModulationBlockViewModel.HiCut` | OK |
| Feedback | -100..+100% | `Preset.ModFeedback` | `ModulationBlockViewModel.Feedback` | OK |
| Delay/Range/Type | Type-dependent | `Preset.ModDelayOrRange` | `ModulationBlockViewModel.DelayOrRange` | OK |
| Width (trem) | 0-100% | `Preset.ModWidth` | `ModulationBlockViewModel.Width` | OK |
| Mix | 0-100% | `Preset.ModMix` | `ModulationBlockViewModel.Mix` | OK |
| Enabled | 0/1 | `Preset.ModulationEnabled` | `ModulationBlockViewModel.IsEnabled` | OK |

## Delay
| Parameter | Ref Range | Domain Property | ViewModel Property | Status |
| --- | --- | --- | --- | --- |
| Type | 0-5 | `Preset.DelayType` | `DelayBlockViewModel.Type/TypeId` | OK |
| Time | 0-1800 ms | `Preset.DelayTime` | `DelayBlockViewModel.Time` | OK |
| Time2 (dual) | 0-1800 ms | `Preset.DelayTime2` | `DelayBlockViewModel.Time2` | OK |
| Tempo | 0-16 | `Preset.DelayTempo` | `DelayBlockViewModel.Tempo` | OK |
| Tempo2/Width | Type-dependent | `Preset.DelayTempo2OrWidth` | `DelayBlockViewModel.Tempo2OrWidth` | OK |
| Feedback | 0-120% | `Preset.DelayFeedback` | `DelayBlockViewModel.Feedback` | OK |
| Clip/Feedback2 | Type-dependent | `Preset.DelayClipOrFeedback2` | `DelayBlockViewModel.ClipOrFeedback2` | OK |
| Hi Cut | 0-61 table | `Preset.DelayHiCut` | `DelayBlockViewModel.HiCut` | OK |
| Lo Cut | 0-61 table | `Preset.DelayLoCut` | `DelayBlockViewModel.LoCut` | OK |
| Offset/Pan1 | Type-dependent | `Preset.DelayOffsetOrPan1` | `DelayBlockViewModel.OffsetOrPan1` | OK |
| Sense/Pan2 | Type-dependent | `Preset.DelaySenseOrPan2` | `DelayBlockViewModel.SenseOrPan2` | OK |
| Damp (dynamic) | 0-100 dB | `Preset.DelayDamp` | `DelayBlockViewModel.Damp` | OK |
| Release (dynamic) | 11-21 table | `Preset.DelayRelease` | `DelayBlockViewModel.Release` | OK |
| Mix | 0-100% | `Preset.DelayMix` | `DelayBlockViewModel.Mix` | OK |
| Enabled | 0/1 | `Preset.DelayEnabled` | `DelayBlockViewModel.IsEnabled` | OK |

## Reverb
| Parameter | Ref Range | Domain Property | ViewModel Property | Status |
| --- | --- | --- | --- | --- |
| Type | 0-3 | `Preset.ReverbType` | `ReverbBlockViewModel.Type/TypeId` | OK |
| Decay | 1-200 (0.1-20s) | `Preset.ReverbDecay` | `ReverbBlockViewModel.Decay` | OK |
| Pre Delay | 0-100 ms | `Preset.ReverbPreDelay` | `ReverbBlockViewModel.PreDelay` | OK |
| Shape | 0-2 | `Preset.ReverbShape` | `ReverbBlockViewModel.Shape` | OK |
| Size | 0-7 | `Preset.ReverbSize` | `ReverbBlockViewModel.Size` | OK |
| Hi Color | 0-6 | `Preset.ReverbHiColor` | `ReverbBlockViewModel.HiColor` | OK |
| Hi Level | -25..+25 dB | `Preset.ReverbHiLevel` | `ReverbBlockViewModel.HiLevel` | OK |
| Lo Color | 0-6 | `Preset.ReverbLoColor` | `ReverbBlockViewModel.LoColor` | OK |
| Lo Level | -25..+25 dB | `Preset.ReverbLoLevel` | `ReverbBlockViewModel.LoLevel` | OK |
| Room Level | -100..0 dB | `Preset.ReverbRoomLevel` | `ReverbBlockViewModel.RoomLevel` | OK |
| Reverb Level | -100..0 dB | `Preset.ReverbLevel` | `ReverbBlockViewModel.ReverbLevel` | OK |
| Diffuse | -25..+25 dB | `Preset.ReverbDiffuse` | `ReverbBlockViewModel.Diffuse` | OK |
| Mix | 0-100% | `Preset.ReverbMix` | `ReverbBlockViewModel.Mix` | OK |
| Enabled | 0/1 | `Preset.ReverbEnabled` | `ReverbBlockViewModel.IsEnabled` | OK |

## EQ / Gate
| Parameter | Ref Range | Domain Property | ViewModel Property | Status |
| --- | --- | --- | --- | --- |
| Gate Type | 0-1 | `Preset.GateType` | `EqGateBlockViewModel.Type` | OK |
| Gate Threshold | -60..0 dB | `Preset.GateThreshold` | `EqGateBlockViewModel.GateThreshold` | OK |
| Gate Damp | 0-90 dB | `Preset.GateDamp` | `EqGateBlockViewModel.GateDamp` | OK |
| Gate Release | 0-200 dB/s | `Preset.GateRelease` | `EqGateBlockViewModel.GateRelease` | OK |
| Gate Enabled | 0/1 | `Preset.GateEnabled` | `EqGateBlockViewModel.GateEnabled` | OK |
| EQ Enabled | 0/1 | `Preset.EqEnabled` | `EqGateBlockViewModel.EqEnabled` | OK |
| EQ Freq 1 | table 25-113 | `Preset.EqFreq1` | `EqGateBlockViewModel.EqBand1Freq` | OK |
| EQ Gain 1 | -12..+12 dB | `Preset.EqGain1` | `EqGateBlockViewModel.EqBand1Gain` | OK |
| EQ Width 1 | table 5-12 | `Preset.EqWidth1` | `EqGateBlockViewModel.EqBand1Width` | OK |
| EQ Freq 2 | table 25-113 | `Preset.EqFreq2` | `EqGateBlockViewModel.EqBand2Freq` | OK |
| EQ Gain 2 | -12..+12 dB | `Preset.EqGain2` | `EqGateBlockViewModel.EqBand2Gain` | OK |
| EQ Width 2 | table 5-12 | `Preset.EqWidth2` | `EqGateBlockViewModel.EqBand2Width` | OK |
| EQ Freq 3 | table 25-113 | `Preset.EqFreq3` | `EqGateBlockViewModel.EqBand3Freq` | OK |
| EQ Gain 3 | -12..+12 dB | `Preset.EqGain3` | `EqGateBlockViewModel.EqBand3Gain` | OK |
| EQ Width 3 | table 5-12 | `Preset.EqWidth3` | `EqGateBlockViewModel.EqBand3Width` | OK |

## Pitch
| Parameter | Ref Range | Domain Property | ViewModel Property | Status |
| --- | --- | --- | --- | --- |
| Type | 0-4 | `Preset.PitchType` | `PitchBlockViewModel.Type` | OK |
| Voice 1 | -100..+100 cents or -13..+13 degrees | `Preset.PitchVoice1` | `PitchBlockViewModel.Voice1` | OK |
| Voice 2 | -100..+100 cents or -13..+13 degrees | `Preset.PitchVoice2` | `PitchBlockViewModel.Voice2` | OK |
| Pan 1 | -50..+50 | `Preset.PitchPan1` | `PitchBlockViewModel.Pan1` | OK |
| Pan 2 | -50..+50 | `Preset.PitchPan2` | `PitchBlockViewModel.Pan2` | OK |
| Delay 1 | 0-50 ms | `Preset.PitchDelay1` | `PitchBlockViewModel.Delay1` | OK |
| Delay 2 | 0-50 ms | `Preset.PitchDelay2` | `PitchBlockViewModel.Delay2` | OK |
| Feedback1 or Key | 0-100% or 0-12 | `Preset.PitchFeedback1OrKey` | `PitchBlockViewModel.Feedback1OrKey` | OK |
| Feedback2 or Scale | 0-100% or 0-13 | `Preset.PitchFeedback2OrScale` | `PitchBlockViewModel.Feedback2OrScale` | OK |
| Level 1 | -100..0 dB | `Preset.PitchLevel1` | `PitchBlockViewModel.Level1` | OK |
| Level 2 or Direction | -100..0 dB or 0/1 | `Preset.PitchLevel2` / `Preset.PitchDirection` | `PitchBlockViewModel.Level2` / `Direction` | OK |
| Range | 1-2 | `Preset.PitchRange` | `PitchBlockViewModel.Range` | OK |
| Mix | 0-100% | `Preset.PitchMix` | `PitchBlockViewModel.Mix` | OK |
| Enabled | 0/1 | `Preset.PitchEnabled` | `PitchBlockViewModel.IsEnabled` | OK |

## Type-Dependent Fields
- Mod Delay/Range/Type: handled in `Preset.FromSysEx` + validation + `ModulationBlockViewModel` type flags.
- Delay Tempo2/Width, Clip/Feedback2, Offset/Pan1, Sense/Pan2: handled in `Preset.FromSysEx` + validation + `DelayBlockViewModel` type flags.
- Pitch Voice 1/2, Feedback/Key, Feedback/Scale, Level2/Direction, Range: handled in `Preset.FromSysEx` + validation + `PitchBlockViewModel` type flags.
