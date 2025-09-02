using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
	public class ResponseModel<T>
	{
		public bool? Status { get; set; }

		public string? Message { get; set; }

		public T? Data { get; set; }

		public Exception? Exception { get; set; }
		
	}
}
