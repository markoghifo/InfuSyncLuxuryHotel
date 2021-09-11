using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helpers
{
    public static class ModelStateExtension
    {
        public static string GetErrorMessage(this ModelStateDictionary dictionary)
        {
            List<string> err = dictionary.SelectMany(m => m.Value.Errors)
                .Select(m => m.ErrorMessage).ToList();
            return string.Join(",", err.ToArray());
        }
    }
}
