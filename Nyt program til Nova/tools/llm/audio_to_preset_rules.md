# Audio → Preset (heuristik)

Dette er en enkel oversættelsestabel fra audio-features til presetvalg.
Bruges som støtte når vi analyserer en lydfil via Python.

- Høj spectral centroid (lys/skarpt): øg `EqGain3`, sænk `EqGain1`.
- Lav spectral centroid (mørk/tyk): øg `EqGain1`, sænk `EqGain3`.
- Høj RMS + lav dynamik: aktiver `CompressorEnabled`, øg `CompRatio`.
- Høj harmonisk indhold: øg `DriveGain`, sæt `DriveType` til distortion.
- Lang sustain/rumklang i lyd: øg `ReverbDecay`, `ReverbMix`.
- Tydelige repeats: aktiver `DelayEnabled`, sæt `DelayTime` og `DelayMix`.

Denne tabel er bevidst grov. Vi finjusterer altid i appen bagefter.
