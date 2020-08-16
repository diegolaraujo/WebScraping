using HtmlAgilityPack;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using prmToolkit.NotificationPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WebScraping.Core.Utils;
using WebScraping.Domain.Commands.ProjectInfoGitHub.GetProjectInfoGithub;

namespace WebScraping.Core.Services.Handlers.ProjectInfoGitHub.GetProjectInfoGithub
{
    public class GetProjectInfoGitHubHandler : Notifiable, IRequestHandler<GetProjectInfoGitHubRequest, Domain.Commands.Response>
    {        
        private Dictionary<string, Domain.Entities.ProjectInfoGitHub> _dicProjectInfo;              
        private readonly HtmlWeb _httpClient;
        private readonly IMemoryCache _memoryCache;

        public GetProjectInfoGitHubHandler(IMemoryCache memoryCache, HtmlWeb httpClient)
        {            
            _dicProjectInfo = new Dictionary<string, Domain.Entities.ProjectInfoGitHub>();
            _httpClient = httpClient;
            _memoryCache = memoryCache;
        }

        public async Task<Domain.Commands.Response> Handle(GetProjectInfoGitHubRequest request, CancellationToken cancellationToken)
        {            
            if (request == null)
            {
                AddNotification("Request", "Request is required!");
                return new Domain.Commands.Response(this);
            }
           
            var urlProject = request.UrlGitHub.AbsoluteUri;
            
            // download page from url parameter
            var htmlDocument = _httpClient.Load(urlProject);
            var cacheKey = urlProject.Substring(urlProject.IndexOf("com/") + 4);

            // get datetime of the last commit for verify if occurred changes
            var fragmentElement = htmlDocument.DocumentNode.SelectNodes("//include-fragment[@src]");
            if (fragmentElement != null && fragmentElement.FirstOrDefault() != null)
            {
                var fragment = fragmentElement.First();
                var documentLastCommit = _httpClient.Load(new Uri(@"https://github.com/" + WebUtility.HtmlDecode(fragment.Attributes["src"].Value.Split("/rollup")[0])).AbsoluteUri);
                var dateTimeElement = documentLastCommit.DocumentNode.SelectSingleNode("//relative-time[@datetime]");
                var dateTimeStr = dateTimeElement.Attributes["datetime"].Value;                
                cacheKey = cacheKey + "|" + dateTimeStr;
            }


            // Checks whether the data is already in memorycache. If not, we cache it for 12 hours with sliding expiration
            if (!_memoryCache.TryGetValue(cacheKey, out _dicProjectInfo))
            {
                if(_dicProjectInfo == null)
                {
                    _dicProjectInfo = new Dictionary<string, Domain.Entities.ProjectInfoGitHub>();
                }
                this.GetDetailsProjectGutHub(htmlDocument);

                // Keep in cache for this time.
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(12));

                _memoryCache.Set(cacheKey, _dicProjectInfo, cacheEntryOptions);
            }


            //Create object response
            var listInfoGrouped = _dicProjectInfo.Select(d => new Domain.Entities.ProjectInfoGitHub(d.Key, d.Value.Lines, d.Value.Bytes)).ToList();
            var response = new Domain.Commands.Response(this, listInfoGrouped);

            //Return result
            return await Task.FromResult(response);
        } 
        
        private void GetDetailsProjectGutHub(HtmlDocument htmlDocument)
        {
            // Nodes that have the file or folder page url
            var files = htmlDocument.DocumentNode.SelectNodes("//*[contains(@class,'js-navigation-open link-gray-dark')]");

            foreach (HtmlNode file in files)
            {
                if (file.Attributes.Count > 0)
                {
                    // Downloading the file page to get the data from the lines of code and bytes
                    var urlFile = new Uri(@"https://github.com/" + WebUtility.HtmlDecode(file.Attributes["href"].Value)).AbsoluteUri;                    
                    htmlDocument =  _httpClient.Load(urlFile);

                    var elementRoot = htmlDocument.DocumentNode;
                    var detailsFileElement = elementRoot.SelectNodes("//*[contains(@class,'text-mono f6 flex-auto pr-3 flex-order-2 flex-md-order-1 mt-2 mt-md-0')]");
                    // If detailsFileElement is not null, then it means it is a file, otherwise it is a folder and 
                    //it is necessary to repeat the process walking through the subfolders until you reach the files
                    if (detailsFileElement != null)
                    {
                        var detailsFile = WebUtility.HtmlDecode(detailsFileElement.Single().InnerText.Replace("\n", "").Trim());

                        var splitDetails = detailsFile.Split("lines");
                        string codeLines = string.Empty;
                        string bytesStr = string.Empty;
                        // Obtain codeLines and bytes from string
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

                        var bytes = ConvertToBytes.ConvertNumberToBytes(Convert.ToDouble(bytesStr, System.Globalization.CultureInfo.InvariantCulture), bytesType);

                        //Get extension file and store in dictionary of Project Info
                        var extension = file.InnerHtml.Split(".").Last();

                        Domain.Entities.ProjectInfoGitHub linesBytes;
                        if (_dicProjectInfo.TryGetValue(extension, out linesBytes))
                        {
                            linesBytes.SetBytes(linesBytes.Bytes + bytes);
                            linesBytes.SetCodeLines(linesBytes.Lines + Convert.ToInt64(codeLines));                            
                            _dicProjectInfo[extension] = linesBytes;
                        }
                        else
                        {
                            var projectInfoGitHub = new Domain.Entities.ProjectInfoGitHub(extension, Convert.ToInt64(codeLines), bytes);                            
                            _dicProjectInfo.Add(extension, projectInfoGitHub);
                        }
                    }
                    else
                    {
                        //Recursive method case the element be folder
                        GetDetailsProjectGutHub(htmlDocument);
                    }
                }
            }

        }

    }   
}
