using GPol.Serialization;

public class Program
{
    public static void Main(string[] args)
    {
        var policyReader = new PolicyReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System),
            "GroupPolicy", "User", "Registry.pol"));
        var policies = policyReader.ReadPolicies();
        Console.WriteLine(policies);
    }
}
