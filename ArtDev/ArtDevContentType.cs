using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtDev
{
    public class ArtDevContentType
    {
        public SPContentType type = null;
        public SPFieldLink link = null;
        public List<SPField> sPFields = null;
        
        public ArtDevContentType (SPContentType type)
        {
            this.type = type;
            this.sPFields = new List<SPField>();
        }

     
        public ArtDevContentType AddFieldLink (ArtDevField field)
        {            
            this.sPFields.Add(field.field);
            this.link = new SPFieldLink(field.field);
            return this;
        }

        public ArtDevContentType GetFieldLink(Guid guid)
        {
            this.link = this.type.FieldLinks[guid]; 
            return this;
        }

        public ArtDevContentType SetRequired (bool condition)
        {
            this.link.Required = condition;
            return this;
        }

        public ArtDevContentType SetDisplayName (string name)
        {
            this.link.DisplayName = name;
            return this;
        }

        public ArtDevContentType SetDocumentTemplate (string url)
        {
            this.type.DocumentTemplate = url;
            return this;
        }

        public ArtDevContentType Commit()
        {
         this.type.FieldLinks.Add(this.link);
         this.type.Update(true);
         return this;
        }   

        public ArtDevContentType RemoveOldLinks()
        {            
            var Links = this.type.FieldLinks;
            Links.Cast<SPFieldLink>().ToList().ForEach(delegate(SPFieldLink link)
            {                
                if (!link.Name.Equals("ContentType") && !link.Name.Equals("Title"))
                {
                    this.type.FieldLinks.Delete(link.Name);
                    this.type.Update(true);
                }        
            });            
            return this; 
            
        }

    }
}
