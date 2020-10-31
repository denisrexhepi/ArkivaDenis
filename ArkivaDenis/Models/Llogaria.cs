using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ArkivaDenis.Models
{
    public class Llogaria
    {
        private string _username;

        public int ID { get; set; }
        public string Emer { get; set; }
        public string Mbiemer { get; set; }
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value.ToLowerInvariant();
            }

        }
        public ICollection<Dokumente> Dokumente { get; set; }

        public string Password { get; set; }
        
    }
}