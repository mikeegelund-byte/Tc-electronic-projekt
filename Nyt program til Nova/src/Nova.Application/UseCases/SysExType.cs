namespace Nova.Application.UseCases;

/// <summary>
/// Enum representing the type of SysEx data in a .syx file.
/// </summary>
public enum SysExType
{
    /// <summary>
    /// Unknown or invalid SysEx format
    /// </summary>
    Unknown,
    
    /// <summary>
    /// Single preset (521 bytes)
    /// </summary>
    Preset,
    
    /// <summary>
    /// System settings dump (527 bytes)
    /// </summary>
    SystemDump,
    
    /// <summary>
    /// User bank with 60 presets (31,260 bytes or partial)
    /// </summary>
    UserBank
}
