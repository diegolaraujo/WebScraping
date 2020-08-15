using WebScraping.Domain.Entities.Base;

namespace WebScraping.Domain.Entities
{
    public class ProjectInfoGitHub : EntityBase
    {

        public string Extension { get; set; }
        public long CodeLines { get; protected set; }
        public double Bytes { get; protected set; }


        protected ProjectInfoGitHub()
        {

        }

        public ProjectInfoGitHub(string extension, long codeLines, double bytes)
        {
            this.Extension = extension;
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
