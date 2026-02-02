using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Nova.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Nova.Presentation.Views;

public partial class SavePresetDialog : Window
{
    private readonly UserBankDump? _bankDump;
    public int? SelectedSlot { get; private set; }

    public SavePresetDialog()
    {
        InitializeComponent();
    }

    public SavePresetDialog(Preset preset, UserBankDump? bankDump = null)
    {
        InitializeComponent();
        _bankDump = bankDump;

        // Set preset name (read-only)
        var presetNameText = this.FindControl<TextBox>("PresetNameText");
        if (presetNameText != null)
        {
            presetNameText.Text = preset.Name;
        }

        // Populate slot combo box (1-60)
        var slotCombo = this.FindControl<ComboBox>("SlotComboBox");
        if (slotCombo != null)
        {
            var slots = Enumerable.Range(1, 60).Select(i => new SlotItem
            {
                Number = i,
                DisplayText = GetSlotDisplayText(i)
            }).ToList();

            slotCombo.ItemsSource = slots;
            slotCombo.SelectedIndex = 0; // Default to slot 1
            slotCombo.SelectionChanged += OnSlotSelectionChanged;
        }

        // Wire up buttons
        var saveButton = this.FindControl<Button>("SaveButton");
        if (saveButton != null)
        {
            saveButton.Click += OnSaveClicked;
        }

        var cancelButton = this.FindControl<Button>("CancelButton");
        if (cancelButton != null)
        {
            cancelButton.Click += OnCancelClicked;
        }
    }

    private void OnSlotSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var slotCombo = sender as ComboBox;
        var overwriteWarning = this.FindControl<Border>("OverwriteWarning");

        if (slotCombo?.SelectedItem is SlotItem slotItem && overwriteWarning != null)
        {
            // Check if slot already has a preset
            var slotHasPreset = _bankDump?.Presets[slotItem.Number - 1] != null;
            overwriteWarning.IsVisible = slotHasPreset;
        }
    }

    private void OnSaveClicked(object? sender, RoutedEventArgs e)
    {
        var slotCombo = this.FindControl<ComboBox>("SlotComboBox");
        if (slotCombo?.SelectedItem is SlotItem slotItem)
        {
            SelectedSlot = slotItem.Number;
            Close(true);
        }
    }

    private void OnCancelClicked(object? sender, RoutedEventArgs e)
    {
        SelectedSlot = null;
        Close(false);
    }

    private string GetSlotDisplayText(int slotNumber)
    {
        if (_bankDump == null)
        {
            return $"Slot {slotNumber}";
        }

        var preset = _bankDump.Presets[slotNumber - 1];
        if (preset != null)
        {
            return $"Slot {slotNumber} - {preset.Name} (will overwrite)";
        }

        return $"Slot {slotNumber} (empty)";
    }

    private class SlotItem
    {
        public int Number { get; set; }
        public string DisplayText { get; set; } = string.Empty;

        public override string ToString() => DisplayText;
    }
}
