using Danstagram.Identities.Service.Entities;

namespace Danstagram.Identities.Service
{
    public static class Extentions
    {
        #region Methods
        public static IdentityDto AsDto(this Identity identity)
        {
            return new IdentityDto(identity.Id, identity.Username);
        }
        #endregion

    }
}