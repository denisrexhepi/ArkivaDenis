using ArkivaDenis.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace ArkivaDenis
{
    public class ArkivaDenisDbInitialize : DropCreateDatabaseIfModelChanges<ArkivaDenisContext>
    {
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void InitializeDatabase(ArkivaDenisContext context)
        {
            base.InitializeDatabase(context);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        protected override void Seed(ArkivaDenisContext context)
        {
            context.Subjekte.Add(
                  new Subjekte
                  {
                      Id = 1,
                      EmerSubjekt = "Telekom",
                  });

            context.Subjekte.Add(
                 new Subjekte
                 {
                     Id = 2,
                     EmerSubjekt= " Vodafone"
                 });

            context.SaveChanges();

            base.Seed(context);

        }
    }

}
