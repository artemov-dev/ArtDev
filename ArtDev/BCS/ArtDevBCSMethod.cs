using Microsoft.BusinessData.MetadataModel;
using Microsoft.SharePoint.BusinessData.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtDev.BCS
{
    public class ArtDevBCSMethod
    {
        public Method method;
        public MethodInstance MethodInstance;
        private ArtDevBCS BCS;
        public ArtDevBCSMethod(Method method, ArtDevBCS BCSContext )
        {
            this.method = method;
            this.BCS = BCSContext;
        }

        public ArtDevBCSMethod SetRdbCommand(string Value)
        {
            this.method.Properties.Add("RdbCommandText", Value);
            return this;
        }

        public ArtDevBCSMethod SetRdbCommandType(string Value)
        {
            this.method.Properties.Add("RdbCommandType", Value);
            return this;
        }

        public ArtDevBCSMethod SetSchema(string Value)
        {
            this.method.Properties.Add("Schema", Value);
            return this;
        }

        public ArtDevBCSMethod SetObjectType(string Value)
        {
            this.method.Properties.Add("BackEndObjectType", Value);
            return this;
        }

        public ArtDevBCSMethod SetObject(string Value)
        {
            this.method.Properties.Add("BackEndObject", Value);
            return this;
        }

      

        public ArtDevBCSDescriptor NewDescriptor(string DescriptorName, DirectionType directionType)
        {
            return new ArtDevBCSDescriptor(DescriptorName, directionType, this, this.BCS);
        }

        public ArtDevBCS BCSContext()
        { return this.BCS; }

    }
}
