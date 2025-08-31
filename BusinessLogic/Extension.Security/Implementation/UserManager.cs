using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;
using Extension.Security.Interface;

namespace Extension.Security.Implementation
{
    public class UserManager : IUserManager
    {
        public bool TestConnection(string header, string tranMessage)
        {
            using (TiktokishContext context = new("Data Source=AS-BSD-RAZAMER\\\\RAZAMEER;Initial Catalog=Tiktokish;Persist Security Info=True;User ID=sa;Password=avanza@123;"))
            {

                var users = context.UserInfos.ToList();
                foreach (var user in users)
                {
                    Console.WriteLine($"{user.Id} - {user.UserName}");
                }
            }

            return true;
        }

		public bool TestConnection()
		{
			throw new NotImplementedException();
		}
	}
}
