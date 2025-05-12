using GPol;

public class Program
{
    public static void Main(string[] args)
    {
        var policyReader = new PolicyReader();
        PolFile userPolicies = policyReader.ReadUserPolicies();
        PolFile machinePolicies = policyReader.ReadMachinePolicies();
        Console.WriteLine($"{userPolicies}\n{machinePolicies}");
    }
}
