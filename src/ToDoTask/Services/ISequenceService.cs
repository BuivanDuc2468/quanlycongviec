using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoTask.Services
{
    public interface ISequenceService
    {
        Task<int> GetNewId();
    }
}