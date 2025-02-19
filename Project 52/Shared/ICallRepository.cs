namespace Shared;
using System.Collections.Generic;

public interface ICallRepository
{
    void Save(Call call);
    List<Call> GetCalls();
}
