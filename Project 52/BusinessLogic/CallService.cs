namespace BusinessLogic;
using Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CallService : ICallService
{
    private readonly ICallRepository _callRepository;

    public CallService(ICallRepository callRepository)
    {
        _callRepository = callRepository;
    }

    public void AddCall(Call call)
    {
        _callRepository.Save(call);
    }

    public async Task<List<Call>> GetCallsAsync()
    {
        return await Task.Run(() => _callRepository.GetCalls());
    }
}
