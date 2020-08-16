using System;

namespace WebScraping.Domain.Entities.Base
{
    public abstract class EntityBase
    {
        public Guid Id { get; set; }

        protected EntityBase()
        {
            this.Id = Guid.NewGuid();
        }
    }
}
