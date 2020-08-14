using HtmlAgilityPack;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using prmToolkit.NotificationPattern;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WebScraping.Domain.Entities;
using WebScraping.Domain.Utils;

namespace WebScraping.Domain.Commands.ProjectInfoGitHub.GetProjectInfoGithub
{
    public class GetProjectInfoGitHubHandler : Notifiable, IRequestHandler<GetProjectInfoGitHubRequest, Response>
    {
        private readonly IMediator _mediator;
        private Dictionary<string, Entities.ProjectInfoGitHub> _dicProjectInfo;              
        private readonly HtmlWeb _httpClient;
        private readonly IMemoryCache _memoryCache;

        public GetProjectInfoGitHubHandler(IMediator mediator, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _dicProjectInfo = new Dictionary<string, Entities.ProjectInfoGitHub>();
            _httpClient = new HtmlWeb();
            _memoryCache = memoryCache;
        }

        public async Task<Response> Handle(GetProjectInfoGitHubRequest request, CancellationToken cancellationToken)
        {            
            if (request == null)
            {
                AddNotification("Request", "Request is required!");
                return new Response(this);
            }
            
            var urlProject = request.UrlGitHub.AbsoluteUri;



            // download page from url parameter            
            //HttpResponseMessage result = await _httpClient.GetAsync(urlProject);
            //Stream stream = await result.Content.ReadAsStreamAsync();
            var htmlDocument = _httpClient.Load(urlProject);

            var cacheKey = urlProject.Substring(urlProject.IndexOf("com/") +1);
            if (!_memoryCache.TryGetValue(cacheKey, out _dicProjectInfo))
            {
                if(_dicProjectInfo == null)
                {
                    _dicProjectInfo = new Dictionary<string, Entities.ProjectInfoGitHub>();
                }
                this.GetDetailsProjectGutHub(htmlDocument);

                // Keep in cache for this time, reset time if accessed.
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _memoryCache.Set(cacheKey, _dicProjectInfo, cacheEntryOptions);
            }

            //this.GetDetailsProjectGutHub(_htmlDocument);

            //Create object response
            var response = new Response(this, _dicProjectInfo);

            //Return result
            return await Task.FromResult(response);
        } 
        
        private void GetDetailsProjectGutHub(HtmlDocument htmlDocument)
        {
            //var elementMain = htmlDocument.GetElementbyId("js-repo-pjax-container");

            var files = htmlDocument.DocumentNode.SelectNodes("//*[contains(@class,'js-navigation-open link-gray-dark')]");

            foreach (HtmlNode file in files)
            {
                if (file.Attributes.Count > 0)
                {

                    var urlFile = new Uri(@"https://github.com/" + WebUtility.HtmlDecode(file.Attributes["href"].Value)).AbsoluteUri;                    
                    htmlDocument =  _httpClient.Load(urlFile);

                    var elementRoot = htmlDocument.DocumentNode;
                    var detailsFileElement = elementRoot.SelectNodes("//*[contains(@class,'text-mono f6 flex-auto pr-3 flex-order-2 flex-md-order-1 mt-2 mt-md-0')]");
                    if (detailsFileElement != null)
                    {


                        var detailsFile = WebUtility.HtmlDecode(detailsFileElement.Single().InnerText.Replace("\n", "").Trim());

                        var splitDetails = detailsFile.Split("lines");
                        string codeLines = string.Empty;
                        string bytesStr = string.Empty;
                        if (splitDetails.Length > 1)
                        {
                            var lines = splitDetails[0];
                            codeLines = Regex.Match(lines, @"\d+").Value;
                            bytesStr = splitDetails[1];
                        }
                        else
                        {
                            codeLines = "0";
                            bytesStr = splitDetails[0];
                        }
                        splitDetails = bytesStr.Trim().Split(" ");
                        bytesStr = splitDetails[splitDetails.Length - 2];
                        var bytesType = splitDetails.Last();

                        var bytes = ConvertToBytes.ConvertStringNumberToBytes(Convert.ToDouble(bytesStr, System.Globalization.CultureInfo.InvariantCulture), bytesType);

                        //Get extension file and store in dictionary of Project Info
                        var extension = file.InnerHtml.Split(".").Last();

                        Entities.ProjectInfoGitHub linesBytes;
                        if (_dicProjectInfo.TryGetValue(extension, out linesBytes))
                        {
                            linesBytes.SetBytes(linesBytes.Bytes + bytes);
                            linesBytes.SetCodeLines(linesBytes.CodeLines + Convert.ToInt64(codeLines));                            
                            _dicProjectInfo[extension] = linesBytes;
                        }
                        else
                        {
                            var projectInfoGitHub = new Entities.ProjectInfoGitHub(Convert.ToInt64(codeLines), bytes);                            
                            _dicProjectInfo.Add(extension, projectInfoGitHub);
                        }
                    }
                    else
                    {
                        GetDetailsProjectGutHub(htmlDocument);
                    }
                }
            }

        }

      

    }   
}
