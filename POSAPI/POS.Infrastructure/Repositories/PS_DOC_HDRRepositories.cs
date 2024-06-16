using POS.Domain;
using POS.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Infrastructure.Repositories
{
    public class PS_DOC_HDRRepositories : Repository<PS_DOC_HDR>
    {
        public PS_DOC_HDRRepositories(string connectionString) : base(connectionString) { }
    }
}