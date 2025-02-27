namespace Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICallService
{
    void AddCall(Call call);
    Task<List<Call>> GetCallsAsync();
}
