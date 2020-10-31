using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ArkivaDenis.Models
{
    public class ArkivaDenisContext : DbContext
    {
       
        public DbSet<Subjekte> Subjekte { get; set; }
        public DbSet<Dokumente> Dokumente { get; set; }
        public DbSet<Inspektime> Inspektime { get; set; }
        public DbSet<LlojDokument> LlojDokument { get; set;}
        public DbSet< Indeksimet> Indeksimet { get; set; }
        public DbSet<Llogaria> Llogaria { get; set; }

    }

}

