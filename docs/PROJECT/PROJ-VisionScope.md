# Vision & Scope

**STATUS:** MVP implementeret (453/458 tests passing). Dette dokument beskriver den oprindelige vision.

---

## Vision
Byg en moderne, native Windows 11‑app til TC Electronic Nova System, med Apple‑agtig UI og stabil MIDI, designet til at udvides modulært over tid.

**Kerneværdier:**
- Stabilitet over features (disciplineret udvikling)
- Modulært design (enkelt at udvide)
- Testdrevet (høj betryggelse)
- Cross-platform forberedt (Win11 nu, macOS senere)

---

## Primære mål (i prioritet)
1. **Stabil MIDI forbindelse** uden eksterne tools (MIDI‑OX ikke påkrævet)
2. **Minimal MVP der virker 100%** – Modul 1 (Connection + Bank Dump roundtrip)
3. **Modulær arkitektur** – lettest at udvide til macOS senere
4. **Høj test coverage** – sikkerhed for at der virker

---

## Ikke‑mål (for nu)
- Browser‑baseret UI
- Audio streaming (MIDI er kontrol‑kun, ikke audio)
- Nye effekter (hardware‑låst)
- Direkte LLM i app (AI kommer via agent/service senere)
- Linux support (Windows 11 primær target)

---

## Success Criteria (MVP)
### Brugerniveau
- App åbner fra desktop ikon uden fejl
- MIDI port picker viser tilgængelige enheder
- "Connect" knap forbinder til Nova System
- "Download Bank" modtager alle 60 presets uden fejl
- "Upload Bank" sender dump retur uden fejl

### Teknisk
- Ingen MIDI buffer overflows
- SysEx checksum validering 100% korrekt
- Timeout på 5 sek, retry max 3 gange
- Fejlmeldinger er brugergenvejledende
- Alle unit tests passerer

---

## Fremtidsvisioner (fase 2+)
- Fuld preset editor (alle 15 effekt typer)
- System settings editor
- Preset bibliotek med tags
- AI‑hjælp via agent (tone matching fra musik)
- macOS support
- Preset sharing/cloud sync (senere)

---

## Begrænsninger (hardware‑låst)
- Max delay: 1800 ms
- Kun eksisterende effekt‑typer (kan ikke tilføje nye)
- Routing: Serial / Semi‑Parallel / Parallel (3 muligheder)
- MIDI protokol: fast format (520 bytes preset SysEx)

---

## Brugergruppe
- Guitar‑musikere (professionel + amatør)
- Primært Windows 11 brugere
- MIDI‑erfaring varierer (UI skal være intuitiv)
