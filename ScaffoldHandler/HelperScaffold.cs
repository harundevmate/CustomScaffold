namespace ScaffoldHandler
{
    public class HelperScaffold
    {
        public static string[] ExcludeField = {"Id", "CreatedBy", "CreatedAt", "ModifiedBy" , "ModifiedAt" , "DeletedBy" , "DeletedAt" };
        public static string DirInfrastructure = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}\\CustomScaffold\\Infrastructure";
        public static string DirBusinessCore = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}\\CustomScaffold\\BusinessCore";
        public static string DirApi = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}\\CustomScaffold\\Api";

        public static string NamespaceInfrastructure = "Infrastructure";
        public static string NamespaceBusinessCore = "BusinessCore";
    }
}
