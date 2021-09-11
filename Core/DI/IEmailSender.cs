using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DI
{
    public interface IEmailSender
    {
        void Send(string subject, string message, string receipientMail);
    }
}
