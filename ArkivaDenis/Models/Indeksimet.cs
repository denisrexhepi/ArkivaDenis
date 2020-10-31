using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ArkivaDenis.Models
{
    public class Indeksimet
    {
      
        public int Id { get; set; }
        public string Emer { get; set; }
        public virtual ICollection<Dokumente> Dokumente { get; set; }

    }
}