﻿namespace Services.Contracts
{
    public interface ILoggerService
    {
        void LogInfo(string message);
        void LogError(string message);
        void LogWarning(string message);
        void LogDebug(string message);
    }
}
