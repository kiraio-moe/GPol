using Serilog;

namespace GPol;

public sealed class PolicyReader
{
    private BinaryReader? _reader;

    public PolFile ReadUserPolicies() => ReadPolicies(Path.Combine(GetPolicyFolder("User"), "Registry.pol"));
    public PolFile ReadMachinePolicies() => ReadPolicies(Path.Combine(GetPolicyFolder("Machine"), "Registry.pol"));

    public PolicyReader()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("log.txt")
            .CreateLogger();
    }

    private PolFile ReadPolicies(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                Log.Warning("{path} did not exist! No policies being applied to the system.", path);
                return new PolFile();
            }

            using var stream = new MemoryStream(File.ReadAllBytes(path));
            _reader = new BinaryReader(stream);

            var signature = _reader.ReadUInt32();
            if (signature != 0x67655250)
            {
                Log.Error("Invalid file signature. Expected 0x67655250, got {signature}", signature);
                throw new InvalidDataException("Invalid file! Not a .pol file.");
            }

            var version = _reader.ReadUInt32();

            var policies = new List<Policy>();
            while (_reader.BaseStream.Position < _reader.BaseStream.Length)
            {
                var policyData = new List<ushort>();
                bool isStart = false, isEnd = false; // Flags to start or end reading the data

                // Split the policy data into parts
                var policyParts = new List<ushort[]>();
                List<ushort> policyPart = [];
                var partsCount = 0;

                while (_reader.BaseStream.Position < _reader.BaseStream.Length)
                {
                    var chunk = _reader.ReadUInt16();
                    switch (chunk)
                    {
                        case 59: // ';'
                            partsCount++;
                            break;
                        case 91: // '['
                            isStart = true;
                            break;
                        case 93: // ']'
                            isEnd = true;
                            break;
                    }

                    if (isEnd)
                        break;

                    if (isStart && !isEnd)
                        policyData.Add(chunk);

                    if (partsCount == 4)
                    {
                        // Skip the opening by starting the index at 1
                        for (var i = 1; i < policyData.Count; i++)
                        {
                            // If reach the delimiter, add the current part to the list
                            if (policyData[i] == 59) // ';'
                            {
                                policyParts.Add(policyPart.ToArray());
                                policyPart.Clear();
                                continue;
                            }

                            policyPart.Add(policyData[i]);
                        }

                        // Convert policyParts[4] to byte[] first, then to int
                        var policyPartAsBytes = policyParts[3].Select(b => (byte)b).ToArray();
                        int userDataSize = BitConverter.ToInt16(policyPartAsBytes);
                        var userData = _reader.ReadBytes(userDataSize).Select(b => (ushort)b).ToArray();
                        policyParts.Add(userData);
                    }
                }

                if (policyParts.Count != 5)
                {
                    Log.Error(
                        "Error formatting the .pol file. Expected 5 parts to be formatted, got {policyParts.Count}",
                        policyParts.Count);
                    throw new InvalidDataException("Invalid policy data format.");
                }

                var policy = new Policy
                {
                    Key = new string(policyParts[0].Select(c => (char)c).ToArray()),
                    Name = new string(policyParts[1].Select(c => (char)c).ToArray()),
                    Type = (RegistryType)BitConverter.ToInt16(policyParts[2].Select(b => (byte)b).ToArray()),
                    DataSize = BitConverter.ToInt16(policyParts[3].Select(b => (byte)b).ToArray(), 0),
                    Data = new string(policyParts[4].Select(c => (char)c).ToArray())
                };

                policies.Add(policy);
            }

            return new PolFile
            {
                Signature = signature,
                Version = version,
                Policies = policies.ToArray()
            };
        }
        catch (InvalidDataException e)
        {
            Log.Error("Error reading .pol file. {e.Message}", e.Message);
            throw;
        }
        catch (IOException e)
        {
            Log.Error("IO error. {e.Message}", e.Message);
            throw;
        }
    }

    private static string GetPolicyFolder(string userType)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "GroupPolicy", userType);
    }
}
