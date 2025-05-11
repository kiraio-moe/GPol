namespace GPol.Serialization;

/// <summary>
///     The Group Policy Object Editor `Registry.pol` files. Reside under `{drive}:\Windows\System32\GroupPolicy\` folder.
///     Ref: https://learn.microsoft.com/en-us/previous-versions/windows/desktop/policy/registry-policy-file-format
/// </summary>
public struct PolFile
{
    internal uint Signature;

    /// <summary>
    ///     The .pol file version. Initially defined as 1, then incremented each time the file format is changed.
    /// </summary>
    public uint Version;

    /// <summary>
    ///     List of Policies being applied to the system.
    /// </summary>
    public Policy[] Policies;
}
