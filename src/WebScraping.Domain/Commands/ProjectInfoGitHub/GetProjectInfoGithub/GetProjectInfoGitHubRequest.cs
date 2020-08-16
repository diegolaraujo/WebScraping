using MediatR;
using System;

namespace WebScraping.Domain.Commands.ProjectInfoGitHub.GetProjectInfoGithub
{
    public class GetProjectInfoGitHubRequest : IRequest<Response>
    {
        public Uri UrlGitHub{ get; protected set; }

        public GetProjectInfoGitHubRequest(Uri urlGitHub)
        {
            this.UrlGitHub = urlGitHub;
        }
    }
}
