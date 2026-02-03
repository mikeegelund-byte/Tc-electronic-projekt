# Task: MIDI CC Learn Mode (v1.1)

## Goal
Implement MIDI Learn functionality for CC (Control Change) mapping - user clicks "Learn" button, sends a MIDI CC message from their controller, and the app automatically captures and assigns it to the selected parameter.

## User Story
As a Nova System user, I want to quickly assign MIDI CC controllers to parameters by sending a CC message, so that I don't have to manually look up CC numbers and can configure my MIDI controller more efficiently.

---

## Requirements

### Functional Requirements
- [ ] Add "Learn" button next to each CC mapping row in MIDI Mapping tab
- [ ] Implement MIDI input listening mode
- [ ] Capture incoming MIDI CC messages (CC number 0-127)
- [ ] Display captured CC number in real-time
- [ ] Confirm mapping assignment with visual feedback
- [ ] Cancel learn mode with Escape key
- [ ] Prevent conflicts (warn if CC already assigned)
- [ ] Save learned mappings to Nova System hardware

### Non-Functional Requirements
- [ ] Learn mode timeout after 10 seconds if no CC received
- [ ] Clear visual indication when in learn mode (pulsing button, modal overlay)
- [ ] Accessible via keyboard (Tab to Learn button, Enter to activate)
- [ ] Screen reader announces "Listening for MIDI CC message"

---

## Implementation Tasks

### Backend (Application Layer)
- [ ] Create `LearnCCMappingUseCase`
- [ ] Implement MIDI input stream listener
- [ ] Add CC message parser and validator
- [ ] Implement conflict detection logic
- [ ] Add timeout mechanism (10 seconds)

### Frontend (Presentation Layer)
- [ ] Add "Learn" button to `MidiMappingView.axaml`
- [ ] Create `LearnModeOverlay` user control
- [ ] Implement visual feedback (pulsing animation, modal)
- [ ] Add confirmation dialog for conflicts
- [ ] Update `MidiMappingViewModel` with learn mode state

### Testing
- [ ] Unit tests for `LearnCCMappingUseCase`
- [ ] Mock MIDI input stream tests
- [ ] Integration tests with hardware (manual)
- [ ] Accessibility testing (keyboard nav, screen reader)

---

## UI Design

### Learn Button
```xaml
<Button Content="Learn"
        Command="{Binding StartLearnModeCommand}"
        CommandParameter="{Binding SelectedParameter}"
        Width="60"
        Height="28"
        ToolTip.Tip="Click to learn MIDI CC (or press Enter)"/>
```

### Learn Mode Overlay
```
+--------------------------------+
|  Listening for MIDI CC...     |
|                                |
|  [Animated waveform]           |
|                                |
|  Move a controller or press    |
|  a button on your MIDI device  |
|                                |
|  [Cancel]                      |
+--------------------------------+
```

### Captured CC Feedback
```
+--------------------------------+
|  CC #24 Received!             |
|                                |
|  Assign to: [Parameter Name]  |
|                                |
|  [Assign]  [Try Again]         |
+--------------------------------+
```

---

## Files to Modify

| File | Changes |
|------|---------|
| `Nova.Application/UseCases/LearnCCMappingUseCase.cs` | New file - learn logic |
| `Nova.Midi/IMidiInputListener.cs` | New interface - MIDI input stream |
| `Nova.Presentation/Views/MidiMappingView.axaml` | Add Learn buttons |
| `Nova.Presentation/ViewModels/MidiMappingViewModel.cs` | Add learn mode state |
| `Nova.Presentation/Controls/LearnModeOverlay.axaml` | New control - overlay UI |
| `Nova.Application.Tests/UseCases/LearnCCMappingUseCaseTests.cs` | New test file |

---

## Testing Checklist

### Unit Tests
- [ ] LearnCCMappingUseCase starts listening
- [ ] LearnCCMappingUseCase captures CC message
- [ ] LearnCCMappingUseCase detects conflicts
- [ ] LearnCCMappingUseCase times out after 10s
- [ ] LearnCCMappingUseCase cancels on user request

### Integration Tests
- [ ] User clicks Learn button
- [ ] Overlay appears with "Listening..." message
- [ ] Send CC #24 from MIDI controller
- [ ] App captures CC #24 and shows confirmation
- [ ] User clicks Assign
- [ ] Mapping saved to hardware
- [ ] Verify CC #24 controls the parameter

### Accessibility Tests
- [ ] Tab to Learn button
- [ ] Press Enter to activate
- [ ] Screen reader announces "Listening for MIDI CC"
- [ ] Escape cancels learn mode
- [ ] Focus returns to Learn button after cancel

### Edge Cases
- [ ] Multiple CC messages received simultaneously (use first)
- [ ] Invalid CC number (ignore)
- [ ] MIDI device disconnected during learn (show error)
- [ ] User clicks multiple Learn buttons (cancel previous)

---

## Documentation Updates

### User Manual
Add new section: **"MIDI CC Learn Mode"**

```markdown
## Learning MIDI CC Mappings

1. Navigate to **MIDI Mapping** tab
2. Find the parameter you want to control
3. Click the **Learn** button next to it
4. A dialog appears: "Listening for MIDI CC..."
5. Move a knob/slider on your MIDI controller
6. The app captures the CC number
7. Click **Assign** to save the mapping

**Tips:**
- Use Escape to cancel learn mode
- If the CC is already assigned, you'll see a conflict warning
- Learn mode times out after 10 seconds
```

### CHANGELOG.md
```markdown
## [1.1.0] - TBD

### Added
- MIDI CC Learn Mode - quickly assign controllers by sending CC messages
- Visual feedback during learn mode with animated overlay
- Conflict detection prevents duplicate CC assignments
```

---

## Acceptance Criteria

**Definition of Done:**
- [ ] User can click Learn button and capture CC messages
- [ ] Visual feedback shows listening state and captured CC
- [ ] Conflicts are detected and user is warned
- [ ] Mappings are saved to hardware correctly
- [ ] 15+ unit tests pass
- [ ] Manual testing with hardware confirms functionality
- [ ] Accessibility requirements met (WCAG AA)
- [ ] User manual updated with Learn Mode section
- [ ] CHANGELOG.md updated

---

## Estimated: 3 days
## Priority: HIGH
## Depends on: v1.0.0 released
