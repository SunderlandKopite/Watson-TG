using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TangentWatson.Entities;

namespace TangentWatson.WatsonClient.Interfaces
{
    public interface IWatsonClient
    {
        Task<WatsonResponse> GetResponse(string message);
    }
}
