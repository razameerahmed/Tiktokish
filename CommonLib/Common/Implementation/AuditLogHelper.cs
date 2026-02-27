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
    public class AuditLogHelper : IAuditLogHelper
    {
        //public enum AuditLogType
        //{
        //    Create,
        //    Retreive,
        //    Update,
        //    Delete,
        //    Export,
        //    Import
        //}
        string _connectionString;
        public AuditLogHelper(string connectionString)
        {
            _connectionString = connectionString;
        }
        public string GetAuditLogMessage(string action, string entityName, string entityId, string details)
        {
            StringBuilder logMessage = new StringBuilder();
            logMessage.AppendLine($"Action: {action}");
            logMessage.AppendLine($"Entity: {entityName}");
            logMessage.AppendLine($"Entity ID: {entityId}");
            logMessage.AppendLine($"Details: {details}");
            logMessage.AppendLine($"Timestamp: {DateTime.UtcNow}");
            return logMessage.ToString();
        }
        public void AddAuditLog(string username, string actionType, string actionEntity, object? actionDetail)
        {
            string? actionDetailJson = actionDetail != null
                ? JsonSerializer.Serialize(actionDetail)
                : null;

            using (var context = new TiktokishContext(_connectionString))
            {
                var auditLog = new AuditLog
                {
                    Username = username,
                    ActionTimestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                    ActionType = actionType,
                    ActionEntity = actionEntity,
                    ActionDetail = actionDetailJson
                };

                context.AuditLogs.Add(auditLog);
                context.SaveChanges();
            }
        }

    }
}
