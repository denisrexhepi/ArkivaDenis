using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkivaDenis.Models
{
    public class Subjekte
    {
        public int Id { get; set; }
        public string EmerSubjekt { get; set; }
        public ICollection<Inspektime> Inspektime { get; set; }

    }
}