﻿namespace LisovaAuditSystem.Workers.API.Interfaces.Services;

public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);
}
