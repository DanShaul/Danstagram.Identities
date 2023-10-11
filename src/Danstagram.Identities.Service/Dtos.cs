using System;

namespace Danstagram.Identities
{
    #region Properties

    public record IdentityDto(Guid Id, string UserName);

    public record CreateIdentityDto(Guid Id,string UserName,string Password);

    public record AuthIdentityDto(string UserName,string Password);

    public record DeleteIdentityDto(Guid Id);

    #endregion
}