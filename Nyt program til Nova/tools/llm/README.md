# LLM Preset Builder

Dette er en lille, manuelt styret pipeline til at lave .syx presets via chat.
Intet kører automatisk. Du beder mig analysere lyd, jeg foreslår parametre,
og vi genererer en .syx fra en base-preset.

## Arbejdsgang
1. Eksporter en base-preset fra appen (en eksisterende preset der lyder tæt på).
2. Opret/tilpas et JSON med parametre (se `examples/clean_lead.json`).
3. Kør `preset_builder.py` for at generere en ny .syx.
4. Importer .syx i appen og finjustér.

## Eksempel
```
python tools/llm/preset_builder.py \
  --base "C:\path\to\base.syx" \
  --out "C:\path\to\LLM-Lead.syx" \
  --preset-number 45 \
  --name "LLM Lead" \
  --params-json tools/llm/examples/clean_lead.json
```

Du kan også bruge direkte overrides:
```
python tools/llm/preset_builder.py \
  --out "C:\path\to\LLM-Quick.syx" \
  --preset-number 32 \
  --name "LLM Quick" \
  --set DriveEnabled=1 --set DriveGain=35 --set ReverbMix=18
```

## Noter
- User presets er **31-90**. (Preset 31 = slot 1 i UI).
- Parameterkort ligger i `tools/llm/preset_map.json`.
- Dette er bevidst simpelt og chat-drevet.
