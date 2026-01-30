# TC Electronic Nova System - Effect Reference Guide

**Based on:** TC Nova Manual.pdf (complete user manual analysis)

## Overview

Nova System features **15 effect types** organized into **7 effect blocks**:
1. Drive (NDT™ analog section)
2. Compressor
3. EQ + Noise Gate
4. Modulation (6 types)
5. Pitch (5 types)
6. Delay (6 types)
7. Reverb (4 types)

All effects can run simultaneously with true spillover on delay/reverb.

---

## Signal Chain & Routing

### Serial Routing
```
Input → Drive → Boost → Comp → Gate → EQ → Modulation → Pitch → Delay → Reverb → Output
```

### Semi-Parallel Routing
```
Input → Drive → Boost → Comp → Gate → EQ → Modulation → Pitch → ┬─→ Delay ───┐
                                                                 └─→ Reverb ─→ Mix → Output
```

### Parallel Routing
```
Input → Drive → Boost → Comp → Gate → EQ → ┬─→ Modulation ──┐
                                            ├─→ Pitch ───────┤
                                            ├─→ Delay ────────┤
                                            └─→ Reverb ───────┴─→ Mix → Output
```

---

## 1. DRIVE BLOCK

### NDT™ Technology (Nova Drive Technology)
- **100% analog circuit** with digital parameter control
- **NOT modeling** - real analog tubes/transistors simulation
- Physically separated from digital effects section
- Wide gain range: light breakup → heavy distortion

### Overdrive Type
**Character:** Classic tube amp overdrive  
**Use Cases:** Blues, classic rock, sustaining leads

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Gain** | 0-100% | Amount of overdrive (low = warm clean, high = singing overdrive) |
| **Tone** | 0-100% | High-frequency content control |
| **Level** | -100 to 0 dB | Output level compensation |
| **Boost Level** | 0-10 dB | Assignable boost (within Boost Max) |

**Frequency Response:** Preserves low-end dynamics, natural compression at high gain

---

### Distortion Type
**Character:** Fat, aggressive, compressed  
**Use Cases:** Metal, hard rock, massive chords, sustained leads

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Gain** | 0-100% | Amount of distortion (fat/beefy → screaming/compressed) |
| **Tone** | 0-100% | High-frequency content control |
| **Level** | -100 to 0 dB | Output level compensation |
| **Boost Level** | 0-10 dB | Assignable boost (within Boost Max) |

**Frequency Response:** Less dynamic than overdrive, more saturation

---

## 2. COMPRESSOR BLOCK

### Percussive Type
**Character:** Hard, obvious, recognizable (stomp box style)  
**Use Cases:** Funky rhythm, clean picking with punch

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Drive** | 1-20 | Combined Threshold+Ratio (higher = more compression) |
| **Response** | 1-10 | Release time (lower = more audible, higher = transparent) |
| **Level** | -12 to +12 dB | Output level adjustment |

**Behavior:** Fast attack, medium release, noticeable "pumping" effect

---

### Sustaining Type
**Character:** Subtle, transparent, musical  
**Use Cases:** Gentle sustain, evening out dynamics without obvious compression

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Drive** | 1-20 | Combined Threshold+Ratio (higher = more compression) |
| **Response** | 1-10 | Release time (lower = more audible, higher = transparent) |
| **Level** | -12 to +12 dB | Output level adjustment |

**Behavior:** Slower attack, longer release, auto make-up gain applied

---

### Advanced Type
**Character:** Full parametric control  
**Use Cases:** Precise dynamics shaping, limiting, creative effects

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Threshold** | -30 to 0 dB | Compression activation point |
| **Ratio** | Off, 1.12:1 to Infinite:1 | Compression amount (Infinite = Limiter) |
| **Attack** | 0.3 to 140 ms | Response time after threshold exceeded |
| **Release** | 50 to 2000 ms | Time to release gain reduction |
| **Level** | -12 to +12 dB | Output level adjustment |

**Ratio Guide:**
- **1.5:1 - 2:1:** Subtle, transparent
- **3:1 - 4:1:** Musical, noticeable
- **6:1 - 10:1:** Heavy compression
- **Infinite:1:** Limiter (prevents peaks above threshold)

**Attack Guide:**
- **<5ms:** Fast transient control (drums, percussive picking)
- **5-30ms:** General use (balanced)
- **>50ms:** Preserves attack, sustains body

---

## 3. EQ + NOISE GATE BLOCK

### 3-Band Parametric EQ
**Type:** True parametric (adjustable frequency, gain, bandwidth)  
**Can be locked globally:** EQ Lock in Utility menu

#### Band 1, 2, 3 (Identical Parameters)
| Parameter | Range | Description |
|-----------|-------|-------------|
| **Frequency** | 41 Hz - 20 kHz | Center frequency of band |
| **Gain** | -12 to +12 dB | Boost or cut amount |
| **Width** | 0.3 - 1.6 octaves | Bandwidth (Q factor) |

**EQ Strategies:**
- **Band 1 (Low):** 80-200 Hz for body/muddiness control
- **Band 2 (Mid):** 400-2000 Hz for presence/nasal tone
- **Band 3 (High):** 3-8 kHz for bite/harshness control

**Global EQ Mode:**  
Press and hold EQ button → Select Line/Drive input-specific EQ

---

### Noise Gate
**Type:** Dynamic gate with soft/hard modes  
**Purpose:** Attenuate noise when no signal present

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Mode** | Hard / Soft | Gate response speed |
| **Threshold** | -60 to 0 dB | Activation point (signal below = gate closes) |
| **Damp** | 0 to 90 dB | Attenuation amount when gated |
| **Release** | 0 to 200 dB/sec | Speed of gate opening |

**Setup Guide:**
1. Set Damp to 50 dB
2. Play guitar and increase Threshold until gate activates in silence
3. Adjust Damp for smooth transition (30-50 dB typical)
4. Set Release fairly high (>100 dB/sec) to avoid cutting attack

**Mode Comparison:**
- **Hard:** Fast gate, abrupt cutoff (metal, high-gain)
- **Soft:** Gradual gate, natural fade (clean, blues)

---

## 4. MODULATION BLOCK (6 Types)

### Common Parameters (All Modulation Types)
| Parameter | Range | Description |
|-----------|-------|-------------|
| **Speed** | 0.050 - 20 Hz | LFO rate (modulation speed) |
| **Tempo** | Ignore, 2 to 1/32T | Global tempo sync subdivision |
| **Depth** | 0-100% | Modulation intensity |

---

### 4.1 Phaser
**Character:** Notch-swept, swooshing  
**Signal Path:** Split → All-pass filter (phase shift) → Mix

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Feedback** | -100 to +100% | Resonance amount (negative = phase inversion) |
| **Range** | Low / High | Low-end or high-end frequency focus |
| **Mix** | 0-100% | Dry/wet balance |

**Classic Settings:**
- **Slow Sweep:** Speed 0.2 Hz, Depth 50%, Feedback 30%
- **Fast Leslie:** Speed 4 Hz, Depth 80%, Feedback -40%

---

### 4.2 Tremolo
**Character:** Volume modulation (classic amp tremolo)  
**Waveform:** Soft (sine) or Hard (square)

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Type** | Soft / Hard | Sine wave (smooth) or Square wave (choppy) |
| **Width** | 0-100% | Pulse width (Hard mode: duty cycle) |
| **Hi-Cut** | 20 Hz - 20 kHz | Attenuate high frequencies in tremolo |

**Width Behavior (Hard Mode):**
- **20%:** On for 80% of cycle (short pulses)
- **50%:** Equal on/off (square wave)
- **80%:** On for 20% of cycle (gated effect)

**Classic Settings:**
- **Surf:** Speed 4 Hz, Depth 60%, Type Soft
- **Helicopter:** Speed 8 Hz, Depth 100%, Type Hard, Width 20%

---

### 4.3 Panner
**Character:** Auto-pan (signal sweeps left/right)

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Depth** | 0-100% | Sweep width (100% = full L/R, 50% = subtle) |

**Use Cases:**
- Ambient soundscapes
- Stereo widening
- Rhythmic panning (sync to tempo)

---

### 4.4 Chorus
**Character:** Pitch modulation, thickening, shimmer  
**Signal Path:** Split → Delay (modulated by LFO) → Mix

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Cho Delay** | 0-50 ms | Delay length (10ms typical for chorus) |
| **Hi-Cut** | 20 Hz - 20 kHz | Tame bright chorus |
| **Mix** | 0-100% | Dry/wet balance |

**Chorus vs Flanger:**
- **Chorus:** ~10ms delay, no feedback, smooth modulation
- **Flanger:** ~0.8ms delay, feedback, metallic sweep

**Classic Settings:**
- **Subtle Thicken:** Speed 0.5 Hz, Depth 30%, Delay 12ms
- **80s Shimmer:** Speed 2 Hz, Depth 60%, Delay 15ms, Hi-Cut 8kHz

---

### 4.5 Flanger
**Character:** Metallic sweep, jet plane, resonant  
**Signal Path:** Split → Short delay (modulated) → Feedback → Mix

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Feedback** | -100 to +100% | Resonance (caution: >90% can squeal) |
| **FB Cut** | 20 Hz - 20 kHz | Tame harsh feedback resonance |
| **Fla Delay** | 0-50 ms | Delay length (0.8ms typical for flanger) |
| **Hi-Cut** | 20 Hz - 20 kHz | Reduce high-end dominance |
| **Mix** | 0-100% | Dry/wet balance |

**Feedback Behavior:**
- **Positive:** Reinforces sweep (in-phase)
- **Negative:** Inverses sweep (phase-inverted, thinner)

**Classic Settings:**
- **Jet Flanger:** Speed 0.3 Hz, Depth 80%, Feedback 70%, Delay 1ms
- **Barber Pole:** Speed 0.1 Hz, Depth 100%, Feedback 85% (extreme)

---

### 4.6 Vibrato
**Character:** Pitch wobble (no dry signal)  
**Difference from Chorus:** Pure pitch modulation, no dry mix

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Hi-Cut** | 20 Hz - 17.8 kHz / Off | Reduce dominance |

**Use Cases:**
- Vocal-style vibrato
- Retro organ sounds
- Extreme modulation effects

**Classic Settings:**
- **Subtle Vocal:** Speed 5 Hz, Depth 30%
- **Warped:** Speed 0.2 Hz, Depth 100%

---

## 5. PITCH BLOCK (5 Types)

### 5.1 Pitch Shifter
**Character:** Two independent pitch-shifted voices with delay/feedback  
**Range:** ±1 octave per voice

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Voice 1** | -1200 to +1200 cents | Voice 1 pitch shift (100 cents = 1 semitone) |
| **Voice 2** | -1200 to +1200 cents | Voice 2 pitch shift |
| **Pan 1** | -50 to +50 | Voice 1 stereo position |
| **Pan 2** | -50 to +50 | Voice 2 stereo position |
| **Delay 1** | 0-350 ms | Voice 1 delay time |
| **Delay 2** | 0-350 ms | Voice 2 delay time |
| **Feedback 1/2** | 0-100% | Voice repeat count |
| **Level 1/2** | -100 to 0 dB | Voice output level |
| **Mix** | 0-100% | Dry/wet balance |

**Fast Tracking:** Nova System's DSP eliminates "note searching" (instant pitch lock)

**Classic Settings:**
- **Octave Down + Fifth Up:** V1=-1200, V2=+700
- **Detune Shimmer:** V1=+12, V2=-8, Delay1=15ms, Delay2=22ms

---

### 5.2 Octaver
**Character:** Simple one-voice octave up/down  
**Polyphonic:** No (monophonic tracking)

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Direction** | Up / Down | Octave above or below |
| **Range** | 1 / 2 octaves | One or two octaves shift |
| **Mix** | 0-100% | Dry/wet balance |

**Use Cases:**
- Bass doubling (1 or 2 octaves down)
- High shimmer (1 octave up)
- Organ sounds (1 octave up + dry)

---

### 5.3 Whammy
**Character:** Expression pedal-controlled pitch shift  
**Polyphonic:** No (monophonic tracking)

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Pitch** | 0-100% | Expression pedal position (auto-mapped) |
| **Direction** | Up / Down | Pedal toe-down = pitch up/down |
| **Range** | 1 / 2 octaves | Pedal range |

**Factory Preset Behavior:**  
Pedal Input auto-controls Pitch parameter (acts as Whammy pedal)

**Classic Settings:**
- **Dive Bomb:** Direction Down, Range 2-Oct
- **Harmonic Squeal:** Direction Up, Range 1-Oct

---

### 5.4 Detune
**Character:** Static pitch offset (chorus-like but fixed)  
**Voices:** Two independent voices

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Voice 1** | -100 to +100 cents | Voice 1 detune amount |
| **Voice 2** | -100 to +100 cents | Voice 2 detune amount |
| **Delay 1** | 0-50 ms | Voice 1 delay (natural feel) |
| **Delay 2** | 0-50 ms | Voice 2 delay |
| **Mix** | 0-100% | Dry/wet balance |

**Use Cases:**
- 12-string guitar emulation
- Thickening (subtle detune)
- Extreme detuned pads

**Classic Settings:**
- **12-String:** V1=+10 cents, V2=+5 cents, Delay1=12ms, Delay2=8ms
- **Subtle Thicken:** V1=+2, V2=-3 (minimal delay)

---

### 5.5 Intelligent Pitch Shifter
**Character:** Automatic harmonies in selected key/scale  
**Voices:** Two scale-degree-based voices

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Key** | C, C#, D, ..., B | Musical key |
| **Scale** | 13 types | Ionian, Dorian, Pentatonic, Blues, etc. |
| **Voice 1** | -13 to +13 degrees | Harmony interval (scale degrees) |
| **Voice 2** | -13 to +13 degrees | Harmony interval |
| **Pan 1/2** | -50 to +50 | Voice stereo positioning |
| **Delay 1/2** | 0-50 ms | Voice delay (10-20ms for natural feel) |
| **Level 1/2** | -100 to 0 dB | Voice output level |
| **Mix** | 0-100% | Dry/wet balance |
| **Out Level** | -100 to 0 dB | Overall block output |

**Scale Degree Examples (Key of C Major):**
- **Unison (0):** C (no shift)
- **+2:** E (major third)
- **+4:** G (perfect fifth)
- **+6:** B (major seventh)
- **-3:** A (major sixth down)

**Classic Settings:**
- **3 Guitars in C:** Key=C, Scale=Ionian, V1=+2, V2=+4 (thirds+fifths)
- **Country Harmony:** Key=G, Scale=Major Pentatonic, V1=+2, Delay1=15ms

---

## 6. DELAY BLOCK (6 Types)

### Common Parameters (All Delays)
| Parameter | Range | Description |
|-----------|-------|-------------|
| **Delay Time** | 0-1800 ms | Repeat interval |
| **Tempo** | Ignore, 2 to 1/32T | Global tempo sync |
| **Feedback** | 0-120% | Repeat count (>100% = infinite) |
| **Hi-Cut** | 20 Hz - 20 kHz | Dampen bright repeats |
| **Lo-Cut** | 20 Hz - 20 kHz | Thin out muddy repeats |
| **Mix** | 0-100% | Dry/wet balance |

**Spillover Feature:**  
FX Mute (Utility menu):
- **Soft:** Delays ring out when changing presets
- **Hard:** Delays mute immediately

---

### 6.1 Clean Delay
**Character:** Pristine digital repeats  
**Use Cases:** Rhythmic delays, slapback, transparent doubling

**Settings:**  
No additional parameters beyond common delay parameters.

**Classic Settings:**
- **Slapback:** 100-120ms, Feedback 0%, Mix 30%
- **Dotted Eighth:** Tempo 1/8D, Feedback 40%

---

### 6.2 Analog Delay
**Character:** Warm, deteriorating repeats with optional clipping  
**Emulates:** Bucket-brigade delay pedals

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Drive** | 0-24 dB | Saturation/clipping on repeats |

**Classic Settings:**
- **Warm Slapback:** 140ms, Drive 6dB, Hi-Cut 6kHz, Feedback 20%
- **Self-Oscillation:** Feedback 105%, Drive 12dB (caution!)

---

### 6.3 Tape Delay
**Character:** Vintage tape echo saturation  
**Emulates:** Echoplex, Space Echo

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Drive** | 0-24 dB | Tape saturation on repeats |

**Difference from Analog:**  
More "analog feel," smoother saturation character

**Classic Settings:**
- **Echoplex:** 380ms, Drive 8dB, Hi-Cut 5kHz, Feedback 50%

---

### 6.4 Dynamic Delay
**Character:** Delays duck when you play, resurface in silence  
**Origin:** TC 2290 feature

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Offset** | -200 to +200 ms | Right channel delay offset (stereo width) |
| **Sense** | -50 to 0 dB | Threshold for delay ducking |
| **Damp** | 0-100 dB | Attenuation amount when playing |
| **Release** | 20-1000 ms | Speed of delay return |

**Use Cases:**
- Clean lead with ambient delays (delays don't muddy playing)
- Verse/chorus dynamics (delays emphasize gaps)

**Classic Settings:**
- **Subtle Duck:** Sense -20dB, Damp 30dB, Release 200ms

---

### 6.5 Dual Delay
**Character:** Two independent delay taps with separate controls

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Delay 1** | 0-1800 ms | First tap time |
| **Delay 2** | 0-1800 ms | Second tap time |
| **Tempo 1** | Ignore, 2 to 1/32T | First tap tempo sync |
| **Tempo 2** | Ignore, 2 to 1/32T | Second tap tempo sync |
| **Feedback 1** | 0-120% | First tap repeats |
| **Feedback 2** | 0-120% | Second tap repeats |
| **Pan 1** | -50 to +50 | First tap stereo position |
| **Pan 2** | -50 to +50 | Second tap stereo position |

**Use Cases:**
- Polyrhythmic delays (different tempo subdivisions)
- Wide stereo delays (pan hard L/R)

**Classic Settings:**
- **Dotted + Quarter:** Tempo1=1/4, Tempo2=1/8D, Pan1=-40, Pan2=+40

---

### 6.6 Ping Pong Delay
**Character:** Delays alternate left/right channels

| Parameter | Range | Description |
|-----------|-------|-------------|
| **Width** | 0-100% | Stereo spread (0% = center, 100% = full L/R) |

**Use Cases:**
- Wide stereo delays
- Rhythmic panning effects

**Classic Settings:**
- **Classic Ping Pong:** 300ms, Width 80%, Feedback 40%

---

## 7. REVERB BLOCK (4 Types)

### Common Parameters (All Reverbs)
| Parameter | Range | Description |
|-----------|-------|-------------|
| **Decay** | 0.1-20 seconds | Reverb length (60 dB decay time) |
| **Pre-Delay** | 0-100 ms | Initial gap before reverb |
| **Shape** | Round / Curved / Square | Room geometry |
| **Size** | Box to Huge (8 sizes) | Room size preset |
| **Hi Color** | Wool to Glass (7 types) | High-frequency character |
| **Hi Level** | -25 to +25 dB | High color emphasis |
| **Lo Color** | Thick to NoBass (7 types) | Low-frequency character |
| **Lo Level** | -25 to +25 dB | Low color emphasis |
| **Early** | -100 to 0 dB | Early reflections level |
| **RevLev** | -100 to 0 dB | Diffuse field level |
| **Diffuse** | -25 to +25 dB | Density adjustment |
| **Mix** | 0-100% | Dry/wet balance |

---

### 7.1 Spring Reverb
**Character:** Vintage amp spring tank (boingy, metallic)  
**Emulates:** Fender, Ampeg spring reverbs

**Use Cases:**
- Surf guitar
- Vintage blues/rockabilly
- Amp-in-room sound

**Classic Settings:**
- **Surf:** Decay 1.5s, Shape Round, Size Medium, Hi Color Bright

---

### 7.2 Hall Reverb
**Character:** Large concert hall, natural, spacious  
**Use Cases:** Ballads, ambient leads, lush soundscapes

**Classic Settings:**
- **Cathedral:** Decay 4s, Size Large, Shape Round, Hi Color Real
- **Arena:** Decay 2.5s, Size XL, Pre-Delay 50ms

---

### 7.3 Room Reverb
**Character:** Small furnished room, warm, intimate  
**Use Cases:** Natural ambience, studio feel

**Classic Settings:**
- **Vocal Booth:** Decay 0.8s, Size Small, Lo Color Warm
- **Living Room:** Decay 1.2s, Size Medium, Hi Color Wool

---

### 7.4 Plate Reverb
**Character:** Vintage plate reverb (bright, diffuse, "stands out")  
**Emulates:** EMT 140 plate

**Use Cases:**
- Leads that need to cut through
- Bright vocal-style reverb
- 80s sounds

**Classic Settings:**
- **Classic Plate:** Decay 2s, Hi Color Bright, Hi Level +10dB

---

## Global Parameters

### Tap Tempo
- **Range:** 100-3000 ms (displayed as BPM on device)
- **Tap Master (Utility):**
  - **Preset:** Uses stored delay time until tapped
  - **Global:** Uses tapped tempo immediately on preset change

### Boost Function
- **Boost Max:** Set in Levels menu (0-10 dB)
- **Per-Preset Boost Level:** Set in Drive/Levels menu
- **Boost Lock:** When ON, boost applies across all presets

### Expression Pedal Mapping
- **Assignable to:** Any parameter
- **Response Curve:** Min / Mid / Max points (0-100%)
- **Master Mode:**
  - **Preset:** Uses stored value on preset change
  - **Pedal:** Uses current pedal position on preset change

---

## Effect Variations

Each effect block can store **4 instant variations** (per effect type):
1. Edit effect settings
2. Press and hold Variation button (1-4)
3. Variation is saved (independent of preset)
4. Recall variation in any preset by pressing effect + variation

**Use Case:**  
Store favorite reverb settings, then apply to multiple presets instantly.

---

## Technical Specifications

### Audio Performance
- **A/D & D/A:** 24-bit, 128x oversampling
- **Dynamic Range:** >104 dB
- **Frequency Response:** +0/-0.3 dB, 20 Hz - 20 kHz
- **THD:** <-98 dB (0.0013%) @ 1 kHz
- **Latency:** 0.63 ms @ 48 kHz

### Inputs
- **Drive Input:** Hi-Z, 18 dBu to -6 dBu @ 0 dBFS
- **Line Input:** Balanced, 24 dBu to 0 dBu @ 0 dBFS

### Outputs
- **Balanced Stereo:** 1/4" TRS
- **Max Output Level:** 20 dBu balanced / 14 dBu unbalanced
- **Output Range:** Selectable 2/8/14/20 dBu

---

**Document Version:** 1.0  
**Last Updated:** January 30, 2026  
**Source:** TC Nova Manual.pdf (complete 46-page manual)
