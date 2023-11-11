using System;

namespace Danstagram.Identities.Service
{
    #region Properties

    public record IdentityDto(Guid Id, string Username);

    public record CreateIdentityDto(string Username, string Password);

    public record AuthIdentityDto(string Username, string Password);

    public record DeleteIdentityDto(Guid Id);

    #endregion
}