using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkivaDenis.Models
{
    public class LlojDokument
    {
        public string Emer { get; set; }
        public int Id { get; set; }
        public virtual ICollection<Inspektime> Inspektime { get; set; }
        public ICollection<Dokumente> Dokumente  { get; set; }

        public static implicit operator LlojDokument(List<LlojDokument> v)
        {
            throw new NotImplementedException();
        }
    }

}