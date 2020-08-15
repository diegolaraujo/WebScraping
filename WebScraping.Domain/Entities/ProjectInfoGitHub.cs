using WebScraping.Domain.Entities.Base;

namespace WebScraping.Domain.Entities
{
    public class ProjectInfoGitHub : EntityBase
    {

        public string Extension { get; set; }
        public long Lines { get; protected set; }
        public double Bytes { get; protected set; }


        protected ProjectInfoGitHub()
        {

        }

        public ProjectInfoGitHub(string extension, long lines, double bytes)
        {
            this.Extension = extension;
            this.Lines = lines;
            this.Bytes = bytes;
        }

        public void SetBytes(double bytes)
        {
            this.Bytes = bytes;
        }

        public void SetCodeLines(long codeLines)
        {
            this.Lines = codeLines;
        }
    }
}
