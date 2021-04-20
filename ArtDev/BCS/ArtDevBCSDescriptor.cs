using Microsoft.BusinessData.MetadataModel;
using Microsoft.SharePoint.BusinessData.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtDev.BCS
{
    public class ArtDevBCSDescriptor
    {
        private ArtDevBCS BCS;
        private ArtDevBCSMethod BCSMethod;
        private FilterDescriptor filterDescriptor;
        private Parameter parameter;
        private TypeDescriptor typeDescriptorCollection;
        private TypeDescriptor typeDescriptor;      


        public ArtDevBCSDescriptor(string DescriptorName, DirectionType directionType, ArtDevBCSMethod BCSMethod, ArtDevBCS BCS)
        { 
            // Create the return parameter.
            this.BCS = BCS;
            this.BCSMethod = BCSMethod;                       

            this.parameter = this.BCSMethod.method.Parameters.Create(
                DescriptorName, true, directionType);
        }

        public ArtDevBCSDescriptor TypeOfTable()
        {
            this.SetRowCollectionName("Rows");
            this.SetRowName("Row");
            return this;
        }

        public ArtDevBCSDescriptor SetRowCollectionName(string name)
        {
            // Create the TypeDescriptors for the Contacts return parameter.
            this.typeDescriptorCollection =
                this.parameter.CreateRootTypeDescriptor(
                name,
                true,
                "System.Data.IDataReader, System.Data, Version=4.0.0.0," +
                " Culture=neutral, PublicKeyToken=b77a5c561934e089",
                name,
                null,
                null,
                TypeDescriptorFlags.IsCollection,
                null,
                this.BCS.catalog);
            return this;
        }

        public ArtDevBCSDescriptor SetRowName(string name)
        {
            this.typeDescriptor =
                    this.typeDescriptorCollection.ChildTypeDescriptors.Create(
                    name,
                    true,
                    "System.Data.IDataRecord, System.Data, Version=4.0.0.0," +
                    " Culture=neutral, PublicKeyToken=b77a5c561934e089",
                    name,
                    null,
                    null,
                    TypeDescriptorFlags.None,
                    null);
            return this;
        }

        public ArtDevBCSDescriptor AddField(string name, string type)
        {
            this.typeDescriptor.ChildTypeDescriptors.Create(
                    name,
                    true,
                    type,
                    name,
                    null,
                    null,
                    TypeDescriptorFlags.None,
                    null);
            return this;
        }

        public ArtDevBCSDescriptor TypeOfFiltered(string name, string type)
        {
            // Create a Filter so that we can limit the number 
            // of rows returned;
            // otherwise we may exceed the list query size threshold.
            this.filterDescriptor =
                    this.BCSMethod.method.FilterDescriptors.Create(
                    name, true, FilterType.Limit, null);
            this.filterDescriptor.Properties.Add(
                "IsDefault", true);

            // Create the TypeDescriptor for the MaxRowsReturned parameter.
            // using the Filter we have created.
            this.typeDescriptor =
                this.parameter.CreateRootTypeDescriptor(
                name,
                true,
                type,
                name,
                null,
                this.filterDescriptor,
                TypeDescriptorFlags.None,
                null,
                this.BCS.catalog);

            return this;
        }

        public ArtDevBCSDescriptor TypeOfIdentier()
        {
            return this;
        }

        public ArtDevBCSDescriptor AddIdentifierField(string name, string type)
        {
            this.typeDescriptor.ChildTypeDescriptors.Create(
                    name,
                    true,
                    type,
                    name,
                    new IdentifierReference(name,
                        new EntityReference(this.BCS.lobSystemName, this.BCS.entity.Name, this.BCS.catalog),
                        this.BCS.catalog),
                    null,
                    TypeDescriptorFlags.None,
                    null);
            return this;
        }

        public ArtDevBCSDescriptor CreateMethodInstance(string name, MethodInstanceType Type)
        {
            // Create the finder method instance
            this.BCSMethod.MethodInstance =
                this.BCSMethod.method.MethodInstances.Create(
                name,
                true,
                this.typeDescriptorCollection,
                Type,
                true);

            this.BCSMethod.MethodInstance.Properties.Add("RootFinder", "");
            return this;
        }

        public ArtDevBCSDescriptor SetMethodInstanceLimit(string limitNumber)
        {
            // Set the default value for the number of rows 
            // to be returned filter.
            // NOTE: The method instance needs to be created first 
            // before we can set the default value.
            this.typeDescriptor.SetDefaultValue(
                     this.BCSMethod.MethodInstance.Id, Int64.Parse(limitNumber));
            return this;
        }

        public ArtDevBCSMethod MethodContext()
        {
            return this.BCSMethod;
        }
        public ArtDevBCS BCSContext()
        {
            return this.BCS;
        }
    }
}
