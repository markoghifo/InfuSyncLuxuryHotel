using Core.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helpers
{
    public class EmailSender : IEmailSender
    {
        public void Send(string subject, string message, string receipientMail)
        {
            //throw new NotImplementedException();
        }
    }
}
