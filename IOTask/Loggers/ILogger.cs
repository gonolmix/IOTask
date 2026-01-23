using IOTask.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTask.Loggers
{
    public interface ILogger
    {
        Task LogAsync(FileActions action, string filePath, string? @params = null, string? errorMessage = null);
    }
}
