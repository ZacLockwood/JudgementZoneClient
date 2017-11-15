using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
namespace JudgementZone.Interfaces
{
    public interface IAuthenticate
    {
        Task<MobileServiceUser> Authenticate();
    }
}
