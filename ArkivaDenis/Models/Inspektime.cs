using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkivaDenis.Models
{
    public class Inspektime
    {
        public int Id { get; set; }
        public string NrInspek { get; set; }
        public virtual Subjekte Subjekte { get; set; }
        public ICollection<LlojDokument> LlojDokument { get; set; }
    }
}