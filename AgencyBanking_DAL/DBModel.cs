using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencyBanking_DAL
{
    public interface IDbModel
    {
        /// <summary>
        /// Returns the Tables Name associated with the model
        /// </summary>
        string TableName { get; }
    }

    public enum ModelOperation
    {
        AddNew,
        Update,
        Delete
    }
}
