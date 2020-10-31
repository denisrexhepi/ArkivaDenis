using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace ArkivaDenis.Models
{
    public class Dokumente
    {
        public int Id { get; set; }

        public string EmerDok { get; set; }

        public int Subjekte_Id { get; set; }

        public int Inspektime_Id { get; set; }

        public string Zyra { get; set; }

        public string NrKutise { get; set; }

        public string Rafti { get; set; }

        public string Data { get; set; }

        public virtual Llogaria  Llogaria {get; set;}

        public virtual LlojDokument LlojDokument { get; set; }

        public virtual ICollection<Indeksimet> Indeksimet { get; set; }


    }
}