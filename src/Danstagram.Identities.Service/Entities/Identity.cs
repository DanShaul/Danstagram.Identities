using System;
using Danstagram.Common;

namespace Danstagram.Identities.Service.Entities
{
    public class Identity : IEntity
    {
        #region Properties

        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }


        #endregion
    }
}