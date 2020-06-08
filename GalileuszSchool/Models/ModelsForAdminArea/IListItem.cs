using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.Models.ModelsForAdminArea
{
    public interface IListItem
    {
        public int Id { get; }
        string FirstName { get; }
        string LastName { get; }
    }
}
