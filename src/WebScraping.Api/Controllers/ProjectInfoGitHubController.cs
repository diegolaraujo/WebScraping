using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebScraping.Domain.Commands.ProjectInfoGitHub.GetProjectInfoGithub;

namespace WebScraping.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProjectInfoGitHubController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public ProjectInfoGitHubController(IMediator mediator, ILogger<ProjectInfoGitHubController> logger)
        {
            this._mediator = mediator;
            this._logger = logger;
        }

        [HttpGet]        
        public async Task<IActionResult> Get(string urlGitHub)
        {            
            try
            {
                var uri = new Uri(urlGitHub);
                var request = new GetProjectInfoGitHubRequest(uri);
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch( UriFormatException ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(400, "input parameter 'urlGitHub' is incorrect!");
            }
            catch (System.Exception ex)
            {

                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }            
        }   
        
      

    }
}