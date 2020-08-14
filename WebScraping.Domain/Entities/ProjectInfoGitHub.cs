using prmToolkit.NotificationPattern;
using System;
using System.Collections.Generic;
using System.Text;
using WebScraping.Domain.Entities.Base;

namespace WebScraping.Domain.Entities
{
    public class ProjectInfoGitHub : EntityBase
    {        

        public long CodeLines { get; protected set; }
        public double Bytes { get; protected set; }


        protected ProjectInfoGitHub()
        {

        }

        public ProjectInfoGitHub(long codeLines, double bytes)
        {                                 
            this.CodeLines = codeLines;
            this.Bytes = bytes;
        }

        public void SetBytes(double bytes)
        {
            this.Bytes = bytes;
        }

        public void SetCodeLines(long codeLines)
        {
            this.CodeLines = codeLines;
        }
    }
}
