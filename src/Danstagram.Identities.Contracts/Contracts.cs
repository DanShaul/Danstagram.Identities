using System;

namespace Danstagram.Identities.Contracts
{
    #region Properties

    public record IdentityCreated(Guid Id, string Username);

    public record IdentityDeleted(Guid Id);

    #endregion
}