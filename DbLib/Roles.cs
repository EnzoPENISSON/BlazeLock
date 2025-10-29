using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLib
{
    public static class Roles
    {
        public const string User = "ROLE_USER";
        public const string Administrator = "ROLE_ADMIN";
        public const string User_Administrator = $"{User},{Administrator}";
    }
}
