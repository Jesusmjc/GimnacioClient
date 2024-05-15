using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimnacioClient
{
    internal class UserSingleton
    {
        private static UserSingleton instance;
        private static readonly object lockObject = new object();

        public int UserId { get; set; }
        public string UserType { get; set; }

        private UserSingleton() { }

        public static UserSingleton Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new UserSingleton();
                    }
                }

                return instance;
            }
        }
    }
}
