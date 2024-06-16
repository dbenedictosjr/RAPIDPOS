using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain
{
    public class PS_DOC_HDR
    {
        public long DOC_ID { get; set; }
        public string STR_ID { get; set; }
        public string STA_ID { get; set; }
        public string TKT_NO { get; set; }
        public string DOC_TYP { get; set; }
        public string CUST_NO { get; set; }
        public int LINS { get; set; }
        public double LIN_TOT { get; set; }
        public List<PS_DOC_LIN> PS_DOC_LIN { get; set; }
    }
}
