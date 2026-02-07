# UI Guidelines (Apple-agtig design)

## Design principles
1. **Minimalism** – Remove clutter, use whitespace
2. **Clarity** – Labels are clear, buttons obvious
3. **Consistency** – Same interaction everywhere
4. **Responsiveness** – Sub-100ms feedback
5. **Accessibility** – High contrast, keyboard shortcuts

---

## Visual language

### Colors
- **Background:** Light gray (#F5F5F5 light mode, #1E1E1E dark mode)
- **Accent:** Soft blue (#0078D4 Windows native, or #007AFF Apple)
- **Text:** Dark gray (#333333 light), light gray (#EEEEEE dark)
- **Danger:** Soft red (#D92E1B)
- **Success:** Soft green (#107C10)

### Typography
- **Title:** Segoe UI 18pt bold (Windows) or System 18pt bold (Mac later)
- **Body:** Segoe UI 11pt regular
- **Mono:** Courier New 10pt (for MIDI data)

### Spacing
- Default padding: 12px
- Large padding: 24px
- Small padding: 6px

---

## Component guidelines

### Buttons
- Primary button: blue background, white text, rounded corners (4px)
- Secondary button: light gray background, dark text
- Disabled: 50% opacity
- Hover: Slight shadow + color lighten

### Input fields
- Border: light gray, 1px
- Focus: blue border + subtle glow
- Placeholder: light gray italic text

### Presets list
- Row height: 44px (touch-friendly)
- Alternating background (subtle striping)
- Hover: light blue background
- Selected: darker blue background

### Status messages
- Success: green icon + "✓ Completed"
- Error: red icon + "✗ Failed: reason"
- Info: blue icon + "ℹ Processing..."

---

## Layout for MVP

### Main window
```
┌─────────────────────────────────────┐
│  Nova System Control                │
├─────────────────────────────────────┤
│ Port: [Dropdown▼] [Connect]   [🔴] │
├─────────────────────────────────────┤
│                                     │
│ [Download Bank]  [Upload Bank]      │
│                                     │
│ Status: Ready                       │
├─────────────────────────────────────┤
│ Preset list:                        │
│ ┌─────────────────────────────────┐ │
│ │ 00-1 | Clean Lead              │ │
│ │ 00-2 | Overdrive               │ │
│ │ 00-3 | Ambient                 │ │
│ │ ...                             │ │
│ └─────────────────────────────────┘ │
└─────────────────────────────────────┘
```

### Port selector
- Dropdown lists all input + output ports
- Filters for "Nova" in name
- Auto-select if only one device found
- Manual re-scan button

### Status bar (bottom)
- Current state (Connected/Disconnected)
- Last action result
- Progress bar for long ops (downloads)

---

## Interactions

### Connect flow
1. User selects port from dropdown
2. Clicks "Connect" button
3. Button shows "Connecting..." (disabled)
4. Status shows "Connecting to Nova System"
5. On success: button shows "Connected" (disabled), status "Ready"
6. On failure: status shows red error message, user can retry

### Download Bank flow
1. User clicks "Download Bank"
2. Button becomes disabled, shows spinner
3. Status: "Downloading..." with progress (0/60 presets)
4. On success: presets appear in list, status "Downloaded 60 presets"
5. On failure: status shows error, offer "Retry?"

---

## Themes

### Light theme (default)
- Background: #F5F5F5
- Text: #333333
- Borders: #CCCCCC

### Dark theme (settings → toggle)
- Background: #1E1E1E
- Text: #EEEEEE
- Borders: #444444

**Implementation:** Avalonia `ResourceDictionary` with theme switching

---

## Accessibility
- Minimum text size: 11pt
- Color contrast ratio: 4.5:1 (WCAG AA)
- Keyboard shortcuts: Alt+key for buttons
- Screen reader support (XAML AutomationProperties)

---

## Animation (subtle)
- Button hover: 100ms fade
- Status transitions: 200ms slide
- Loading spinners: smooth rotation
- No flashy transitions (keeps focus on data)

---

## Responsive behavior
- Window min size: 600x400
- Presets list scales with window
- Port selector expands on focus
- Status bar text truncates with ellipsis if window too narrow

---

## Error UX
- Errors appear in status bar (red background)
- Include reason: "Connection failed: Port not found"
- Offer recovery: "Retry" button or "Select different port"
- Never crash silently

