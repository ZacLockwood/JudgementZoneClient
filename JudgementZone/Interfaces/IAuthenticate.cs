using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JudgementZone.Interfaces
{
    public interface IAuthenticate
    {
        Task<bool> Authenticate();
    }
}