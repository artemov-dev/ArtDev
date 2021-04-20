using Microsoft.BusinessData.MetadataModel;
using Microsoft.BusinessData.Runtime;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.BusinessData.Administration;
using Microsoft.SharePoint.BusinessData.SharedService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtDev.BCS
{
    public class ArtDevBCS
    {
        private string ModelName;
        private Model Model;
        public string lobSystemName;
        private LobSystem lobSystem;
        private LobSystemInstance lobSystemInstance;        
        public AdministrationMetadataCatalog catalog;
        public Entity entity;
       

        public ArtDevBCS (string ModelName)
        {
            this.ModelName = ModelName;
            // Get the Catalog for the SharePoint site
            BdcService service = SPFarm.Local.Services.GetValue<BdcService>(String.Empty);
            SPAdministrationWebApplication centralWeb = SPAdministrationWebApplication.Local;
            SPSite AdminSite = new SPSite(centralWeb.Sites.FirstOrDefault<SPSite>().Url);
            SPServiceContext context = SPServiceContext.GetContext(AdminSite);
            this.catalog =
                service.GetAdministrationMetadataCatalog(context);

            this.catalog.GetModels(ModelName)?.ToList().ForEach(m => m.Delete());
            // Create a new Model
            // NOTE: Assume that the "ModelName" Model 
            // does not already exist.
            this.Model = Model.Create(ModelName, true, catalog);
        }

        public ArtDevBCS LobSystemInit (string name)
        {
            this.lobSystemName = name;
            // Make a new Employee LobSystem
            // NOTE: Assume that the "LobSystemName" LobSystem 
            // does not already exist.                 
            this.lobSystem = this.Model.OwnedReferencedLobSystems.Create(name, true, SystemType.Database);

            // Make a new AdventureWorks LobSystemInstance.
            this.lobSystemInstance = this.lobSystem.LobSystemInstances.Create(name, true);

            return this;
        }

        public ArtDevBCS LobSystemConfigSqlWithSSS(string DataSource, string InitialCatalog, string SSOAppID)
        {
            // Set the connection properties.
            this.lobSystemInstance.Properties.Add(
                "ShowInSearchUI", "");
            this.lobSystemInstance.Properties.Add(
                "DatabaseAccessProvider", "SqlServer");
            this.lobSystemInstance.Properties.Add(
                "RdbConnection Data Source", DataSource);
            this.lobSystemInstance.Properties.Add(
                "RdbConnection Initial Catalog", InitialCatalog);
            this.lobSystemInstance.Properties.Add(
                "AuthenticationMode", "RdbCredentials");
            this.lobSystemInstance.Properties.Add(
                "SsoProviderImplementation", "Microsoft.Office.SecureStoreService.Server.SecureStoreProvider, Microsoft.Office.SecureStoreService, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c");
            this.lobSystemInstance.Properties.Add(
                "SsoApplicationId", SSOAppID);
            this.lobSystemInstance.Properties.Add(
                "RdbConnection Pooling", "true");
            return this;
        }

        public ArtDevBCS EntityCreate(string name, string version)
        {
            this.entity = Entity.Create(
                    name,
                    this.lobSystemName,
                    true,
                    new Version(version),
                    10000,
                    CacheUsage.Default,
                    this.lobSystem,
                    this.Model,
                    this.catalog);
            return this;
        }

        public ArtDevBCS EntityIdentifer(string ColumnIdentifier, string ColumnTypeIdentifier)
        {            
            // Set the identifier to the EmployeeID column.
            this.entity.Identifiers.Create(
                ColumnIdentifier, true, ColumnTypeIdentifier);
            return this;
        }

        public ArtDevBCSMethod NewMethod(string MethodName, string LobName)
        {
            // Create the Finder method
            return new ArtDevBCSMethod(this.entity.Methods.Create(
                    MethodName, true, false, LobName), this);
            
        }

        
    }

    
}
