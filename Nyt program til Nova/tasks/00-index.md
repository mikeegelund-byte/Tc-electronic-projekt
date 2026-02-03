# Task Index — Nova Manager

## 🎉 v1.0.0 Complete!

All core features are implemented and tested. The application is production-ready.

**Status**: ✅ All 10 modules complete (342 tests passing)  
**Release Date**: February 3, 2026  
**Next Version**: v1.1 (planned features below)

📂 **Historical Tasks**: See `Arkiv/completed-tasks/` for all completed module implementations.

---

## v1.1 Planned Features

### HIGH Priority (User-Requested)

| Feature | Description | Estimated | Task File |
|---------|-------------|-----------|-----------|
| **MIDI CC Learn Mode** | Click "Learn" button, send CC message, app captures and assigns it | 3 days | `16-v11-cc-learn.md` |
| **Editable System Settings** | Allow editing MIDI channel, global bypass, device ID | 2 days | `17-v11-system-editor.md` |
| **Pedal Calibration** | Wizard to calibrate expression pedal min/max positions | 2 days | `18-v11-pedal-calibration.md` |

### MEDIUM Priority (Enhanced UX)

| Feature | Description | Estimated | Task File |
|---------|-------------|-----------|-----------|
| **Preset Library Browser** | Browse, search, and organize preset collections | 4 days | `19-v11-preset-library.md` |
| **Undo/Redo History** | Visual history panel showing all changes | 2 days | `20-v11-undo-history.md` |
| **Preset Comparison** | Side-by-side comparison of two presets | 3 days | `21-v11-preset-compare.md` |

### LOW Priority (Nice-to-Have)

| Feature | Description | Estimated | Task File |
|---------|-------------|-----------|-----------|
| **Export to PDF** | Generate printable preset sheets with all parameters | 2 days | `22-v11-pdf-export.md` |
| **Preset Tags** | Add custom tags to presets for better organization | 2 days | `23-v11-preset-tags.md` |
| **Dark/Light Auto-Switch** | Follow Windows theme automatically | 1 day | `24-v11-theme-auto.md` |

---

## v1.2 Future Roadmap

### Platform Expansion

| Feature | Description | Estimated |
|---------|-------------|-----------|
| **macOS Support** | Port application to macOS (Avalonia supports it) | 2 weeks |
| **Linux Support** | Port application to Linux | 2 weeks |

### Advanced Features

| Feature | Description | Estimated |
|---------|-------------|-----------|
| **Cloud Preset Sync** | Sync presets across devices via cloud storage | 1 week |
| **Preset Sharing** | Share presets with community via URL/QR code | 1 week |
| **Batch Operations** | Bulk edit, copy, rename multiple presets | 3 days |

### AI-Powered Features (Experimental)

| Feature | Description | Estimated |
|---------|-------------|-----------|
| **AI Preset Generator** | Generate presets based on text description | 3 weeks |
| **Similar Preset Finder** | Find presets with similar sound characteristics | 2 weeks |
| **Auto-Parameter Tuning** | AI-assisted parameter optimization | 4 weeks |

---

## v1.3+ Ideas (Backlog)

- **MIDI Program Change Sequencer**: Automate preset changes during performance
- **Audio Spectrum Analyzer**: Visualize output frequency spectrum
- **Preset Morphing**: Smooth transition between two presets
- **Multi-Pedal Support**: Manage multiple Nova System units
- **VST Plugin**: Use Nova System as VST3/AU plugin in DAW
- **Mobile App**: iOS/Android companion app for remote control

---

## Contributing New Features

Want to implement a feature? Follow these steps:

1. **Check** if a task file exists in `tasks/` directory
2. **Read** the task file for requirements and implementation details
3. **Create** a feature branch: `git checkout -b feature/cc-learn-mode`
4. **Implement** with tests (target: 80%+ coverage)
5. **Update** CHANGELOG.md and USER_MANUAL.md
6. **Submit** a Pull Request

### Development Guidelines

- **Architecture**: Follow Clean Architecture patterns
- **Testing**: Write unit tests for all new code
- **Documentation**: Update user manual for user-facing features
- **Code Style**: Follow C# conventions and existing patterns
- **Accessibility**: Maintain WCAG AA compliance

---

## Task File Template

When creating a new task file, use this template:

```markdown
# Task: [Feature Name] (v1.x)

## Goal
[Brief description of what this feature does]

## User Story
As a [user type], I want [goal] so that [benefit].

## Requirements
- [ ] Requirement 1
- [ ] Requirement 2
- [ ] Requirement 3

## Implementation Tasks
- [ ] Task 1
- [ ] Task 2
- [ ] Task 3

## Testing Checklist
- [ ] Unit tests for core logic
- [ ] Integration tests for UI
- [ ] Manual testing on hardware
- [ ] Accessibility testing

## Documentation Updates
- [ ] Update USER_MANUAL.md
- [ ] Update CHANGELOG.md
- [ ] Add example screenshots

## Estimated: X days
## Priority: HIGH|MEDIUM|LOW
```

---

## Version History

### v1.0.0 (February 3, 2026) ✅
- Initial release
- Full SysEx protocol implementation
- 342 passing tests
- WCAG AA accessible
- Complete user manual
- WiX installer
- CI/CD pipeline

**Completed Modules** (all in `Arkiv/completed-tasks/`):
- Module 0: Environment Setup
- Module 1: Foundation (MIDI + Domain)
- Module 2: Preset Viewer
- Module 3: System Viewer
- Module 4: System Editor
- Module 5: Preset Detail
- Module 6: Preset Editor
- Module 7: Preset Management
- Module 8: File I/O
- Module 9: MIDI Mapping
- Module 10: Release & Polish

---

## Development Resources

### Documentation
- [Architecture Guide](../docs/03-architecture.md)
- [MIDI Protocol](../MIDI_PROTOCOL.md)
- [Effect Reference](../EFFECT_REFERENCE.md)
- [Testing Strategy](../TESTING_STRATEGY.md)

### External Resources
- [TC Electronic Nova System Manual](https://www.tcelectronic.com/brand/tcelectronic/nova-system)
- [DryWetMIDI Documentation](https://melanchall.github.io/drywetmidi/)
- [Avalonia UI Documentation](https://docs.avaloniaui.net/)

---

**Last Updated**: February 3, 2026  
**Status**: v1.0.0 Released, v1.1 Planning Phase
