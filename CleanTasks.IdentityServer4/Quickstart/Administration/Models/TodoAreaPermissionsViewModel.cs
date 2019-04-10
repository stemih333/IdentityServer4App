using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace IdentityServer4.Quickstart.UI
{
    public class TodoAreaPermissionsViewModel
    {
        public List<IdName> UserTodoAreas { get; set; }
        public SelectList AvailableTodoAreas { get; set; }
        public string Username { get; set; }
        [HiddenInput]
        public string UserId { get; set; }
    }

    public class IdName
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class IdNameComparer : IEqualityComparer<IdName>
    {
        public bool Equals(IdName x, IdName y)
        {
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(IdName obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
