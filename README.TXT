Entify Framework Core is a total rewrite of Entity Framework 6.
There are 4 different libraries to simulate separation of concern.
This is a quick demo of EF Core. 2.0. Which is also different from EF Core 1.0

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


EF in UWP
Operating System: Windows 10 Fall's Creator's Update