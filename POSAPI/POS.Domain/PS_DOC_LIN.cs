using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain
{
    public class PS_DOC_LIN
    {
        public int LIN_SEQ_NO { get; set; }
        public string LIN_TYP { get; set; }
        public string ITEM_NO { get; set; }
        public double QTY_SOLD { get; set; }
        public double QTY_NUMER { get; set; }
        public double QTY_DENOM { get; set; }
        public string SELL_UNIT { get; set; }
        public double EXT_PRC { get; set; }
        public bool IS_TXBL { get; set; }
        public string ITEM_TYP { get; set; }
        public string TRK_METH { get; set; }
        public double QTY_RET { get; set; }
        public double GROSS_EXT_PRC { get; set; }
        public bool HAS_PRC_OVRD { get; set; }
        public bool USR_ENTD_PRC { get; set; }
        public bool IS_DISCNTBL { get; set; }
        public double CALC_EXT_PRC { get; set; }
        public bool IS_WEIGHED { get; set; }
        public double TAX_AMT_ALLOC { get; set; }
        public double NORM_TAX_AMT_ALLOC { get; set; }
    }
}
