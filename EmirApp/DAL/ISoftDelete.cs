using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmirApp.DAL
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTime? DeleteDate { get; set; }
    }
}
