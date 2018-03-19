EF Core Setup = From Project to Database
Install Nuget Packages:
	Microsoft.EntityFrameworkCore.SqlServer in App.Data
	Microsoft.EntityFrameworkCore.Tools in App.Data
	Microsoft.EntityFrameworkCore.Design
	
Startup Project: App.UI
PM Console's Default Project: App.Data when migrating.
PM Console's Default Project: App.Data

Add-Migration Initial


EF Core Setup = From Database to Project
Same Nuget Installation

PM Console's Default Project: App.Model when scaffolding.
scaffold-dbcontext -provider Microsoft.EntityFrameworkCore.SqlServer -connection "Data Source=(localdb)\MSSQLLocalDb;Initial Catalog=MyFooApp;Integrated Security=True;Connect Timeout=30;"


EF Logging
Nuget: Microsoft.Extensions.Logging.Console

Queries are in SamuraiUI => program.cs