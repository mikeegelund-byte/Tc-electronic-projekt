# Overordnet fejlsøgningsplan (lokal-first)

Denne plan bruges efter oprydningen for systematisk at finde og løse fejl i Nova System Manager.

## 1) Baseline miljø-check
- Kør `dotnet --info` og bekræft .NET 8 SDK + runtime.
- Bekræft at repo-root er `C:\Users\mike_\Desktop\Tc electronic projekt`.

## 2) Ren build + tests
Kør i `Nyt program til Nova`:
```powershell
dotnet restore NovaApp.sln
dotnet build NovaApp.sln -c Release
dotnet test NovaApp.sln -c Release
```

## 3) Publish-verifikation (lokalt)
```powershell
dotnet publish src/Nova.Presentation/Nova.Presentation.csproj -c Release -r win-x64 --self-contained false -o publish
```
- Start `publish\Nova.Presentation.exe`.

## 4) Runtime-fejl (MIDI + UI)
Følg `MANUAL_TEST_GUIDE.md`:
- Start app
- Check MIDI-ports
- Connect
- Download Bank

Hvis fejl:
- Gem konsol-/log-output
- Sammenlign med `docs/MIDI_PROTOCOL.md`
- Isolér fejl i `Nova.Midi` og `Nova.Infrastructure`
- Bekræft MIDI IN/OUT ikke er byttet (MIDI OUT → pedal MIDI IN)

## 5) Hardware-edgecases
- Test med/uden device tilsluttet
- Timeout/retry-flow (5 sek timeout, max 3 retries)
- Verificér SysEx checksum (520 bytes format)

## 6) Hypoteser (prioriteret)
1. Forkert MIDI IN/OUT parring (MIDI 0/1 byttet)
2. MIDI port enumeration og timeouts
3. SysEx format mismatch (520 bytes)
4. UI state sync i `Nova.Presentation`

## Acceptkriterier
- Build og tests passerer lokalt
- App starter og viser MIDI-ports
- Download Bank uden crash
- Publish-output kører lokalt
