using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebScraping.Domain.Commands.ProjectInfoGitHub.GetProjectInfoGithub;

namespace WebScraping.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProjectInfoGitHubController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectInfoGitHubController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet]        
        public async Task<IActionResult> Get(string urlGitHub)
        {            
            try
            {                
                var request = new GetProjectInfoGitHubRequest(new Uri(urlGitHub));
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (System.Exception ex)
            {

                return NotFound(ex.Message);
            }            
        }   
        
      

    }
}