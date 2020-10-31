using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkivaDenis.helperclass
{
    public class result
    {
        public string Dokument { get; set; }
        public string Zyra { get; set; }
        public string Rafti { get; set; }
        public string Kutia { get; set; }
        public string Content { get; set; }
        public byte[] Bytes { get; set; }
        public List<string> Indeksime { get; set; }
    }
}