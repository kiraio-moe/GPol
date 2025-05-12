namespace GPol;

/// <summary>
///     The Policy data.
///     Ref: https://learn.microsoft.com/en-us/previous-versions/windows/desktop/policy/registry-policy-file-format
/// </summary>
public struct Policy
{
    /// <summary>
    ///     Path to the registry key.
    /// </summary>
    public string Key;

    /// <summary>
    ///     The name of the registry value.
    /// </summary>
    public string Name;

    /// <summary>
    ///     The data type. The field can contain any of the registry value types defined in WinNT.h.
    /// </summary>
    public RegistryType Type;

    /// <summary>
    ///     The size of the data field.
    /// </summary>
    public int DataSize;

    /// <summary>
    ///     The user-supplied data.
    /// </summary>
    public string Data;

    /// <summary>
    ///     User-defined comment.
    /// </summary>
    public string Comment;
}

/// <summary>
///     The registry value.
///     There's no "Not Configured" value because the whole Policy simply removed from the .pol file.
/// </summary>
public enum RegistryValue
{
    Disabled = 0,
    Enabled = 1
}

/// <summary>
///     Value types of the registry.
///     Refs: https://www.installsetupconfig.com/win32programming/windowsregistryapis6_6.htm,
///     https://github.com/Alexpux/mingw-w64/blob/master/mingw-w64-tools/widl/include/winnt.h#L4580
/// </summary>
public enum RegistryType
{
    /// <summary>
    ///     No defined value type.
    /// </summary>
    REG_NONE = 0,

    /// <summary>
    ///     A null-terminated string. This will be either a Unicode or an ANSI string, depending on whether you use
    ///     the Unicode or ANSI functions.
    /// </summary>
    REG_SZ = 1,

    /// <summary>
    ///     A null-terminated string that contains unexpanded references to environment variables (for example, %PATH%).
    ///     It will be a Unicode or ANSI string depending on whether you use the Unicode or ANSI functions.
    ///     To expand the environment variable references, use the ExpandEnvironmentStrings() function.
    /// </summary>
    REG_EXPAND_SZ = 2,

    /// <summary>
    ///     Binary data in any form.
    /// </summary>
    REG_BINARY = 3,

    /// <summary>
    ///     A 32-bit number.
    /// </summary>
    REG_DWORD = 4,

    /// <summary>
    ///     A 32-bit number in little-endian format. Windows is designed to run on little-endian computer architectures.
    ///     Therefore, this value is defined as REG_DWORD in the Windows header files.
    /// </summary>
    REG_DWORD_LITTLE_ENDIAN = 4,

    /// <summary>
    ///     A 32-bit number in big-endian format. Some UNIX systems support big-endian architectures.
    /// </summary>
    REG_DWORD_BIG_ENDIAN = 5,

    /// <summary>
    ///     A null-terminated Unicode string that contains the target path of a symbolic link that was created
    ///     by calling the RegCreateKeyEx() function with REG_OPTION_CREATE_LINK.
    /// </summary>
    REG_LINK = 6,

    /// <summary>
    ///     A sequence of null-terminated strings, terminated by an empty string (\0). The following is an example:
    ///     String1\0String2\0String3\0LastString\0\0
    ///     The first \0 terminates the first string, the second to the last \0 terminates the last string and the final \0
    ///     terminates the sequence. Note that the final terminator must be factored into the length of the string.
    /// </summary>
    REG_MULTI_SZ = 7,

    REG_RESOURCE_LIST = 8,
    REG_FULL_RESOURCE_DESCRIPTOR = 9,
    REG_RESOURCE_REQUIREMENTS_LIST = 10,

    /// <summary>
    ///     A 64-bit number.
    /// </summary>
    REG_QWORD = 11,

    /// <summary>
    ///     A 64-bit number in little-endian format. Windows is designed to run on little-endian computer architectures.
    ///     Therefore, this value is defined as REG_QWORD in the Windows header files.
    /// </summary>
    REG_QWORD_LITTLE_ENDIAN = 11
}
