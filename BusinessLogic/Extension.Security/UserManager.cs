using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;

namespace Extension.Security
{
	public class UserManager
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
	}
}
