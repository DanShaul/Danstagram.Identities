using System;

namespace Danstagram.Identities.Contracts{
    #region Properties

    public record IdentityCreated(Guid Id,string UserName);

    public record IdentityDeleted(Guid Id);

    #endregion
}