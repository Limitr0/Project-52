using Shared.Models;
using BusinessLogic.Services;
using DataAccess.Repositories;
using NUnit.Framework;
using Moq;

namespace DataAccess;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;

public class CallRepository : ICallRepository
{
    private List<Call> calls = new List<Call>();

    public void Save(Call call)
    {
        calls.Add(call);
    }

    public List<Call> GetCalls()
    {
        return calls.OrderByDescending(c => c.CallTime).ToList();
    }
