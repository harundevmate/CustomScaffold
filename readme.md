# Scaffold PowerShell
### Open Terminal Project ScaffoldHanlder
dotnet ef dbcontext scaffold "Data Source=localhost;Database=Learning;Integrated Security=False;User Id=dev;Password=P@ssw0rd;MultipleActiveResultSets=True" Microsoft.EntityFrameworkCore.SqlServer -o Models -c AppDbContext -f --context-dir Contexts
