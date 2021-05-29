using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUD_Assessment.model
{
    public class UserInfo
    {
        public string UserId { get; set; }
        public string PCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string UserAction { get; set; }

    }
}
