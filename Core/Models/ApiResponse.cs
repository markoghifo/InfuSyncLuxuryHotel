using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
	public class ApiResponse<T>
	{
		public string StatusMessage { get; set; }
		public string StatusCode { get; set; }
		public bool Successful => StatusCode == ResponseCode.Ok;
		public T ResponseObject { get; set; }
	}

	public static class ResponseUtil<T>
	{
		public static ApiResponse<T> Ok(T data, string message = "")
		{
			return new ApiResponse<T>()
			{
				StatusCode = ResponseCode.Ok,
				StatusMessage = message,
				ResponseObject = data
			};
		}

		public static ApiResponse<T> Error(T error, string message = "")
		{
			return new ApiResponse<T>()
			{
				StatusCode = ResponseCode.Error,
				StatusMessage = message,
				ResponseObject = error
			};
		}

		public static ApiResponse<T> Error400(T error, string message = "")
		{
			return new ApiResponse<T>()
			{
				StatusCode = ResponseCode.Error400,
				StatusMessage = message,
				ResponseObject = error
			};
		}

	}

	public static class ResponseCode
	{
		public static string Ok = "00";
		public static string Error = "99";
		public static string Error400 = "89";
	}
}
