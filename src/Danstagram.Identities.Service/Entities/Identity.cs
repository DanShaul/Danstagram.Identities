using System;
using Danstagram.Common;
using Microsoft.AspNetCore.Mvc;

namespace Danstagram.Identities.Service.Entities{
    public class Identity : IEntity
    {
        #region Properties

        public Guid Id {get;set;}

        public string UserName{get;set;}

        public string Password{get;set;}


        #endregion
    }
}