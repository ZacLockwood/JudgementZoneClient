using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JudgementZone.Interfaces
{
    public interface I_MobileClient
    {
        Task<MobileServiceUser> Authorize(MobileServiceAuthenticationProvider provider);
    }
}
