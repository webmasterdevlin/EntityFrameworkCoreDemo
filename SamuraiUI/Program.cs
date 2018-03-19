using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SamuraiUI
{
    internal class Program
    {
        private static SamuraiContext _context = new SamuraiContext();

        private static void Main(string[] args)
        {
            InsertSamurai(); //Single
            InserMultipleSamurai(); // Batch
            InsertMultipleDifferentObjects(); // EF Core's feature only
            SimpleSamuraiQuery(); // 1 query
            MoreQueries();

            RetrieveAndUpdateSamurai();
            RetrieveAndUpdateMultipleSamurais();
            QueryAndUpdateBattle_Disconnected();

            DeleteWhileTracked();
            DeleteWhileNotTracked();
            DeleteUsingId(6);

            InsertNewPkFkGraph();
            InsertNewPkFkGraphMultipleChildren();
            AddChildToExistingObjectWhileTracked();
            AddChildToExistingObjectWhileNotTracked(1);
            EagerLoadSamuraiWithQuotes();
            var dynamicList = ProjectDynamic();
            ProjectSomeProperties();
            ProjectSamuraisWithQuotes();
            FilteringWithRelatedData();
            ModifyingRelatedDataWhenTracked();
            ModifyingRelatedDataWhenNotTracked();
        }

        private static void InsertSamurai()
        {
            var samurai = new Samurai { Name = "Dave" };
            using (var context = new SamuraiContext())
            {
                context.Samurais.Add(samurai);
                context.SaveChanges();
            }
        }

        private static void InserMultipleSamurai()
        {
            var samurai1 = new Samurai { Name = "Naruto" };
            var samurai2 = new Samurai { Name = "Lee" };
            var samurai3 = new Samurai { Name = "Sasuke" };
            var samurai4 = new Samurai { Name = "Gaara" };
            var samurai5 = new Samurai { Name = "Sai" };
            using (var context = new SamuraiContext())
            {
                context.Samurais.AddRange(
                    samurai1,
                    samurai2,
                    samurai3,
                    samurai4,
                    samurai5
                    );
                context.SaveChanges();
            }
        }

        private static void InsertMultipleDifferentObjects()
        {
            var samurai = new Samurai { Name = "Choji" };
            var battle = new Battle
            {
                Name = "Battle of Nagashino",
                StartDate = new DateTime(1575, 06, 16),
                EndDate = new DateTime(1575, 06, 28)
            };
            using (var context = new SamuraiContext())
            {
                context.AddRange(samurai, battle);
                context.SaveChanges();
            }
        }

        private static void SimpleSamuraiQuery()
        {
            using (var context = new SamuraiContext())
            {
                var samurais = context.Samurais.ToList();
                foreach (Samurai samurai in samurais)
                {
                    Console.WriteLine($"Name: {samurai.Name}");
                }
            }
        }

        private static void MoreQueries()
        {
            var name = "Devlin";
            var firstQuery = _context.Samurais.Where(s => s.Name == name).ToList();
            var secondQuery = _context.Samurais.FirstOrDefault(s => s.Name == name); // select the first result that matches the query
            var thirdQuery = _context.Samurais.Find(2); // retreive an object using its key value
            var fourthQuery = _context.Samurais.Where(s => EF.Functions.Like(s.Name, "D%")).ToList(); // All names start with D
        }

        private static void RetrieveAndUpdateSamurai()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            if (samurai != null) samurai.Name += " San";
            _context.SaveChanges();
        }

        private static void RetrieveAndUpdateMultipleSamurais()
        {
            var samurais = _context.Samurais.ToList();
            samurais.ForEach(s => s.Name += " San");
            _context.SaveChanges();
        }

        private static void InsertBattle()
        {
            _context.Battles.Add(new Battle { Name = "Battle of Okehazama", StartDate = new DateTime(1560, 05, 01) });
            _context.SaveChanges();
        }

        private static void QueryAndUpdateBattle_Disconnected()
        {
            var battle = _context.Battles.FirstOrDefault();
            if (battle == null) return;
            battle.EndDate = new DateTime(1560, 06, 30);
            using (var newContextInstance = new SamuraiContext())
            {
                newContextInstance.Battles.Update(battle);
                newContextInstance.SaveChanges();
            }
        }

        private static void DeleteWhileTracked()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name == "Gaara");
            _context.Samurais.Remove(samurai ?? throw new InvalidOperationException());
            _context.Remove(samurai); // another way
            _context.Samurais.Remove(_context.Samurais.Find(1)); // another way
            _context.SaveChanges();
        }

        private static void DeleteWhileNotTracked()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name == "Lee");
            using (var contextNewAppInstance = new SamuraiContext())
            {
                contextNewAppInstance.Samurais.Remove(samurai ?? throw new InvalidOperationException());
                contextNewAppInstance.SaveChanges();
            }
        }

        private static void DeleteUsingId(int samuraiId)
        {
            var samurai = _context.Samurais.Find(samuraiId);
            _context.Remove(samurai);
            _context.SaveChanges();
            _context.Database.ExecuteSqlCommand("exec DeleteById {0}", samuraiId); // or a stored procedure
        }

        private static void InsertNewPkFkGraph()
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote>
                {
                    new Quote{Text = "I've come to save you"}
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void InsertNewPkFkGraphMultipleChildren()
        {
            var samurai = new Samurai
            {
                Name = "Kyuzo",
                Quotes = new List<Quote>
                {
                    new Quote{Text = "Watch out for my sharp sword!"},
                    new Quote{Text = "I told you to watch out for the sharp sword! Oh well!"},
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void AddChildToExistingObjectWhileTracked()
        {
            var samurai = _context.Samurais.First();
            samurai.Quotes.Add(new Quote
            {
                Text = "I bet you're happy that I've saved you!"
            });
            _context.SaveChanges();
        }

        private static void AddChildToExistingObjectWhileNotTracked(int samuraiId)
        {
            var quote = new Quote
            {
                Text = "Now that I saved you, will you feed me dinner?",
                SamuraiId = samuraiId
            };
            using (var newContext = new SamuraiContext())
            {
                newContext.Quotes.Add(quote);
                newContext.SaveChanges();
            }
        }

        private static void EagerLoadSamuraiWithQuotes()
        {
            var samuraiWithQuotes = _context.Samurais.Where(s => s.Name.Contains("Kyūzō"))
                .Include(s => s.Quotes)
                .Include(s => s.SecretIdentity)
                .FirstOrDefault();
        }

        private static List<dynamic> ProjectDynamic()
        {
            var someProperties = _context.Samurais.Select(s => new { s.Id, s.Name }).ToList();
            return someProperties.ToList<dynamic>();
        }

        private static void ProjectSomeProperties()
        {
            var someProperties = _context.Samurais.Select(s => new { s.Id, s.Name }).ToList();
            var idsAndNames = _context.Samurais.Select(s => new IdAndName(s.Id, s.Name)).ToList();
        }

        public struct IdAndName
        {
            public IdAndName(int id, string name)
            {
                Id = id;
                Name = name;
            }

            public int Id;
            public string Name;
        }

        private static void ProjectSamuraisWithQuotes()
        {
            var somePropertiesWithQuotes = _context.Samurais
                .Select(s => new { s.Id, s.Name, s.Quotes.Count })
                .ToList();
        }

        private static void FilteringWithRelatedData()
        {
            var samurais = _context.Samurais
                .Where(s => s.Quotes.Any(q => q.Text.Contains("happy")))
                .ToList();
        }

        private static void ModifyingRelatedDataWhenNotTracked()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault();
            if (samurai == null) return;
            var quote = samurai.Quotes[0];
            quote.Text += " Did you hear that?";
            using (var newContext = new SamuraiContext())
            {
                //newContext.Quotes.Update(quote);
                newContext.Entry(quote).State = EntityState.Modified;
                newContext.SaveChanges();
            }
        }

        private static void ModifyingRelatedDataWhenTracked()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault();
            if (samurai != null) samurai.Quotes[0].Text += " Did you hear that?";
            _context.SaveChanges();
        }
    }
}