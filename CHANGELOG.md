# Changelog

All notable changes to Nova System Manager will be documented here.

Format based on [Keep a Changelog](https://keepachangelog.com/).

## [Unreleased]

### Added
- Professional development tooling (.editorconfig, git hooks, VS Code config)
- Structured documentation with PREFIX naming convention
- Archived historical planning docs for reference
- CONTRIBUTING.md with development guidelines
- PROJECT.md with project metadata

### Changed
- Reorganized docs/ into categorized subdirectories (DESIGN/, REFERENCE/, PROCESS/, etc.)
- Renamed CLAUDE.md → PROJECT_MEMORY.md for clarity
- Renamed documentation files with PREFIX pattern for better discoverability

### Fixed
- MIDI memory leaks (commit 8e37993)
- Event handler cleanup in DisconnectAsync
- InvalidCastException in XAML (commit 2ae4c72)
- Domain validation for all 42 preset parameters

## [0.1.0] - 2026-02-07

### Added
- Initial MIDI communication
- Preset domain model
- Domain validation (all 42 parameters)
- Clean Architecture implementation
- 458 tests (453 passing = 98.9%)

### Changed
- Project migration to clean structure
- Moved from "C:\Projekter\TC Electronic\Tc electronic projekt\Nyt program til Nova"
- To "C:\Projekter\Mikes preset app"

### Migration Details
- ✅ 296 C# filer (kildekode + tests)
- ✅ 26 dokumentations-filer
- ✅ Git repository (fuld historik bevaret - 2214 filer i .git/)
- ✅ Build configuration
- ✅ Scripts

## [0.0.1] - Initial Development

### Added
- Basic MIDI port abstraction (IMidiPort)
- DryWetMIDI implementation
- Preset SysEx parsing
- Avalonia UI framework setup
- xUnit test infrastructure
