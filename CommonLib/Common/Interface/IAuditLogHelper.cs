using DataAccessLayer.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common
{
    public interface IAuditLogHelper
    {
        public string GetAuditLogMessage(string action, string entityName, string entityId, string details);
        public void AddAuditLog(string username, string actionType, string actionEntity, object? actionDetail);
    }
}
