namespace GPol.Serialization;

public sealed class PolicyReader : BinaryReader
{
    // private readonly Stream _stream;

    public PolicyReader(Stream stream) : base(stream)
    {
        // _stream = stream;
    }

    public PolicyReader(string fileName) : base(File.OpenRead(fileName))
    {
        try
        {
            // _stream = BaseStream;
        }
        catch (IOException e)
        {
            Console.WriteLine($"IO Error: {e.Message}");
            throw;
        }
    }

    // protected override void Dispose(bool disposing)
    // {
    //     if (disposing)
    //     {
    //         _stream?.Dispose();
    //     }
    //
    //     base.Dispose(disposing);
    // }

    public PolFile ReadPolicies()
    {
        try
        {
            var signature = ReadUInt32();
            if (signature != 0x67655250)
                throw new InvalidDataException("Invalid file! Not a .pol file.");

            var version = ReadUInt32();

            var policies = new List<Policy>();
            while (BaseStream.Position < BaseStream.Length)
            {
                var policyData = new List<ushort>();
                bool isStart = false, isEnd = false; // Flags to start or end reading the data

                // Split the policy data into parts
                var policyParts = new List<ushort[]>();
                List<ushort> policyPart = [];
                var partsCount = 0;

                while (BaseStream.Position < BaseStream.Length)
                {
                    var chunk = ReadUInt16();
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
                        var userData = ReadBytes(userDataSize).Select(b => (ushort)b).ToArray();
                        policyParts.Add(userData);
                    }
                }

                if (policyParts.Count != 5)
                    throw new InvalidDataException("Invalid policy data format. Expected 5 parts.");

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
            Console.WriteLine($"Data Error: {e.Message}");
            throw;
        }
        catch (IOException e)
        {
            Console.WriteLine($"IO Error: {e.Message}");
            throw;
        }
    }

    // internal Policy[] ReadPolicyRegistries()
    // {
    //     return new
    // }
}
