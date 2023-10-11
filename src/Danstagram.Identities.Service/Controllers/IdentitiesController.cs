using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Amazon.Runtime.EventStreams.Internal;
using Danstagram.Common;
using Danstagram.Identities.Contracts;
using Danstagram.Identities.Service.Entities;
using MassTransit;
using MassTransit.Initializers;
using MassTransit.Transports;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Danstagram.Identities.Service.Controllers{
    [ApiController]
    [Route("identities")]
    public class IdentitiesController : ControllerBase{

        #region Properties
        private readonly IRepository<Identity> repository;
        private readonly IPublishEndpoint publishEndpoint;
        #endregion

        #region Constructors
        public IdentitiesController(IRepository<Identity> repository,IPublishEndpoint publishEndpoint) {
            this.repository = repository;
            this.publishEndpoint = publishEndpoint;
        }

        #endregion

        #region Methods
        [Route("")]
        [HttpGet]
        public async Task<IEnumerable<IdentityDto>> GetAsync(){
            return (await this.repository.GetAllAsync()).Select(item => item.AsDto());
        }

        [Route("{id}")]
        [HttpGet()]
        public async Task<ActionResult<IdentityDto>> GetByIdAsync(Guid id){
            var identity = await this.repository.GetAsync(id);
            if (identity == null) return NotFound();

            return identity.AsDto();
        }

        // Get /identities/auth
        [Route("auth")]
        [HttpPost]
        public async Task<ActionResult<IdentityDto>> PostAuthAsync(AuthIdentityDto authIdentityDto){

            var identity = await this.repository.GetAsync(item => item.UserName == authIdentityDto.UserName);

            if(identity == null) return NotFound("Username doesnt exist in the system");

            if(identity.Password.Equals(authIdentityDto.Password) == false) return Unauthorized("Credential is incorrect");


            return identity.AsDto();
        }

        [Route("")]
        [HttpPost]
        public async Task<ActionResult<CreateIdentityDto>> PostIdentityAsync(CreateIdentityDto createIdentityDto){

            if((await this.repository.GetAsync(item => item.UserName == createIdentityDto.UserName)) != null) return BadRequest("A User with that username exists already in the system");
            
            Identity identity = new(){
                Id = Guid.NewGuid(),
                UserName = createIdentityDto.UserName,
                Password = createIdentityDto.Password
            };

            await this.repository.CreateAsync(identity);
            await this.publishEndpoint.Publish(new IdentityCreated(identity.Id,identity.UserName));

            return CreatedAtAction(nameof(GetByIdAsync),new {id = identity.Id},identity.AsDto());
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<ActionResult> DeleteAsync(Guid id){
            if ((await this.repository.GetAsync(id)) == null) return NoContent();

            await this.repository.RemoveAsync(id);
            await this.publishEndpoint.Publish(new IdentityDeleted(id));

            return NoContent();
        }
        #endregion
    }

}