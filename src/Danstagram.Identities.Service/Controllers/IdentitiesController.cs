using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Danstagram.Common;
using Danstagram.Identities.Contracts;
using Danstagram.Identities.Service.Entities;
using MassTransit;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Mvc;

namespace Danstagram.Identities.Service.Controllers
{
    [ApiController]
    [Route("identities")]
    public class IdentitiesController : ControllerBase
    {

        #region Properties
        private readonly IRepository<Identity> repository;
        private readonly IPublishEndpoint publishEndpoint;
        #endregion

        #region Constructors
        public IdentitiesController(IRepository<Identity> repository, IPublishEndpoint publishEndpoint)
        {
            this.repository = repository;
            this.publishEndpoint = publishEndpoint;
        }

        #endregion

        #region Methods
        [Route("")]
        [HttpGet]
        public async Task<IEnumerable<IdentityDto>> GetAsync()
        {
            return (await repository.GetAllAsync()).Select(item => item.AsDto());
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult<IdentityDto>> GetByIdAsync(Guid id)
        {
            Identity identity = await repository.GetAsync(id);
            return identity == null ? (ActionResult<IdentityDto>)NotFound() : (ActionResult<IdentityDto>)identity.AsDto();
        }

        // Get /identities/auth
        [Route("auth")]
        [HttpPost]
        public async Task<ActionResult<IdentityDto>> PostAuthAsync(AuthIdentityDto authIdentityDto)
        {

            Identity identity = await repository.GetAsync(item => item.Username == authIdentityDto.Username);

            return identity == null
                ? (ActionResult<IdentityDto>)NotFound("Username doesnt exist in the system")
                : identity.Password.Equals(authIdentityDto.Password) == false ? (ActionResult<IdentityDto>)Unauthorized("Credential is incorrect") : (ActionResult<IdentityDto>)identity.AsDto();
        }

        [Route("")]
        [HttpPost]
        public async Task<ActionResult<CreateIdentityDto>> PostIdentityAsync(CreateIdentityDto createIdentityDto)
        {

            if ((await repository.GetAsync(item => item.Username == createIdentityDto.Username)) != null)
            {
                return BadRequest("A User with that username exists already in the system");
            }

            Identity identity = new()
            {
                Id = Guid.NewGuid(),
                Username = createIdentityDto.Username,
                Password = createIdentityDto.Password
            };

            await repository.CreateAsync(identity);
            await publishEndpoint.Publish(new IdentityCreated(identity.Id, identity.Username));

            return CreatedAtAction(nameof(GetByIdAsync), new { id = identity.Id }, identity.AsDto());
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            if ((await repository.GetAsync(id)) == null)
            {
                return NoContent();
            }

            await repository.RemoveAsync(id);
            await publishEndpoint.Publish(new IdentityDeleted(id));

            return NoContent();
        }
        #endregion
    }

}