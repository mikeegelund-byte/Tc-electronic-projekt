namespace Nova.Common;

/// <summary>
/// System verification class for testing infrastructure
/// </summary>
public class SystemInfo
{
    public string GetProjectName() => "Nova System MIDI Control";
    
    public string GetVersion() => "0.1.0";
    
    public bool IsSystemReady() => true;
}
