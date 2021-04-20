using Microsoft.SharePoint;
using Microsoft.SharePoint.WebPartPages;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI.WebControls.WebParts;

namespace ArtDev
{
   
    public class ArtDevList
    {
        public bool mode;
        public bool FirstDeploy;
        public SPField Field;
        public SPListItem Item;
        public List<ArtDevContentType> artDevContentTypes;
        public List<ArtDevField> artDevFields;
        public List<SPField> sPFields;
        public SPList list
        {
            get;
            set;
        }

        public ArtDevList (SPList list)
        {
            this.list = list;
            this.artDevContentTypes = new List<ArtDevContentType>();
            this.artDevFields = new List<ArtDevField>();
            this.sPFields = new List<SPField>();
        }
        public ArtDevList addContentType(ArtDevContentType type)
        {
            this.artDevContentTypes.Add(type);
            SPContentType CType = this.list.ContentTypes.Cast<SPContentType>().FirstOrDefault(c => c.Name.Equals(type.type.Name)) ?? this.list.ContentTypes.Add(type.type);                   
            this.list.Update();
            return this;
        }

        public ArtDevList addContentType(SPContentTypeId typeID)
        {
            SPContentType CType = this.list.ParentWeb.AvailableContentTypes[typeID];
            CType = this.list.ContentTypes.Cast<SPContentType>().FirstOrDefault(c => c.Name.Equals(CType.Name)) ?? this.list.ContentTypes.Add(CType);            
            this.list.Update();            
            return this;
        }


        public ArtDevList removeContentTypeID(SPContentTypeId typeID)
        {
            SPContentType CType = this.list.ParentWeb.AvailableContentTypes[typeID];
            CType = this.list.ContentTypes.Cast<SPContentType>().FirstOrDefault(c => c.Name.Equals(CType.Name)) ?? null;
            if (CType != null) { this.list.ContentTypes.Delete(CType.Id);  }
            this.list.Update();
            return this;
        }  

        public ArtDevList RemoveStandartContentType()
        {

            SPContentType DocType = this.list.ParentWeb.AvailableContentTypes[SPBuiltInContentTypeId.Document];
            SPContentType ItemType = this.list.ParentWeb.AvailableContentTypes[SPBuiltInContentTypeId.Item];
            DocType = this.list.ContentTypes.Cast<SPContentType>().FirstOrDefault(c => c.Name.Equals(DocType.Name)) ?? null;
            ItemType = this.list.ContentTypes.Cast<SPContentType>().FirstOrDefault(c => c.Name.Equals(ItemType.Name)) ?? null;
            if (DocType != null) { this.list.ContentTypes.Delete(DocType.Id); }
            if (ItemType != null) { this.list.ContentTypes.Delete(ItemType.Id); }
            this.list.Update();
            return this;
        }
        
        public ArtDevList removeFieldID(Guid guid)
        {
            SPField Field = this.list.ParentWeb.AvailableFields[guid];
            Field = this.list.Fields.GetFieldByInternalName(Field.InternalName) ?? null;
            if (Field != null) { this.list.Fields.Delete(Field.InternalName); }
            this.list.Update();
            return this;
        }  

        public ArtDevList GetFieldLink(Guid guid)
        {
            SPField Field = this.list.ParentWeb.AvailableFields[guid];            
            this.Field = this.list.Fields.TryGetFieldByStaticName(Field.InternalName) ?? null;  
            if (this.Field != null) { this.sPFields.Add(this.Field); this.Field = this.list.Fields.TryGetFieldByStaticName(Field.InternalName); }
            return this;
        }

        public SPField GetField(Guid guid)
        {
            SPField Field =  this.list.ParentWeb.AvailableFields[guid];
            return this.list.Fields.GetFieldByInternalName(Field.InternalName) ?? null;
        }

        public ArtDevList AddFieldLink(ArtDevField field)
        {
            this.artDevFields.Add(field);
            this.sPFields.Add(field.field);
            SPField sPField = this.list.Fields.TryGetFieldByStaticName(field.field.StaticName) ?? null;
            if (sPField == null) { this.list.Fields.Add(field.field); this.Field = this.list.Fields.TryGetFieldByStaticName(field.field.StaticName); }
            else { this.Field = sPField; }            
            return this;
        }

        public ArtDevList AddFieldLink(Guid guid)
        {
            SPField field = this.list.ParentWeb.AvailableFields[guid];
            this.sPFields.Add(field);
            SPField sPField = this.list.Fields.TryGetFieldByStaticName(field.StaticName) ?? null;
            if (sPField == null) { this.list.Fields.Add(field); this.Field = this.list.Fields.TryGetFieldByStaticName(field.StaticName);   }
            else { this.Field = sPField; }
            return this;
        }

        public ArtDevList SetFieldRequired(bool condition)
        {
            this.Field.Required = condition;
            this.Field.Update();
            return this;
        }

        public ArtDevList SetFieldTitle(string Title)
        {
            this.Field.Title = Title;
            this.Field.Update();
            return this;
        }

        public ArtDevList Commit()
        {
            this.list.Update();            
            return this;
        }

        public ArtDevList SetListTitle(string Name)
        {
            this.list.Title = Name;
            this.list.Update();
            return this;
        }

        public ArtDevList GetItemByTitle(string Value)
        {
            this.Item = null;
            SPQuery query = new SPQuery
            {
                Query = @"<Where>
		                         <Eq>
				                      <FieldRef Name='Title' />
				                      <Value Type='Text'>" + Value + @"</Value>
			                     </Eq>
                          </Where>"
            };
            this.Item = list.GetItems(query).Cast<SPListItem>().FirstOrDefault();
            return this;
        }

        public ArtDevList SetItem(Guid FieldGuid, string Value)
        {
            this.Field = this.GetField(FieldGuid);
            if (this.Item != null && this.Field != null) { this.Item[this.Field.Id] = Value; this.Item.Update(); }             
            return this;
        }

        public ArtDevList GetItem(Guid FieldGuid, string Value)
        {
            this.Item = null;
            this.Field = this.GetField(FieldGuid);
            SPQuery query = new SPQuery
            {
                Query = @"<Where>
		                         <Eq>
				                      <FieldRef Name='" + this.Field.InternalName + @"' />
				                      <Value Type='Text'>" + Value + @"</Value>
			                     </Eq>
                          </Where>"
            };
            this.Item = list.GetItems(query).Cast<SPListItem>().FirstOrDefault();
            return this;
        }

        public ArtDevList AddItemIfModeDevOrFirstDeploy(params string[] Values)
        {
            if ((string)this.list.ParentWeb.AllProperties["DEV"] == "true" || this.FirstDeploy )
            {
                SPListItem sPListItem = this.list.Items.Add();
                int i = 0;
                Values.ToList<string>().ForEach(value =>
                {

                    string Name = this.sPFields[i].InternalName;
                    sPListItem[Name] = value;
                    i++;
                });
                sPListItem.Update();
            }
            return this;
        }

        public ArtDevList AddItemIfTitleNotExsist(params string[] Values)
        {
            this.GetItem(SPBuiltInFieldId.Title, Values[0]);
            if (this.Item == null)
            {
                SPListItem sPListItem = this.list.Items.Add();
                int i = 0;
                Values.ToList<string>().ForEach(value =>
                {
                    string Name = this.sPFields[i].InternalName;
                    sPListItem[Name] = value;
                    i++;
                });
                sPListItem.Update();
            }
            return this;
        }

        public ArtDevList AddOrUpdateItemIfTitleNotExsist(params string[] Values)
        {
            this.GetItem(SPBuiltInFieldId.Title, Values[0]);
            if (this.Item == null)
            {
                SPListItem sPListItem = this.list.Items.Add();
                int i = 0;
                Values.ToList<string>().ForEach(value =>
                {
                    string Name = this.sPFields[i].InternalName;
                    sPListItem[Name] = value;
                    i++;
                });
                sPListItem.Update();
            }
            else
            {
                int i = 0;
                Values.ToList<string>().ForEach(value => 
                {
                    string Name = this.sPFields[i].InternalName;                    
                    if ((string)this.Item[Name] == "" || this.Item[Name] == null)
                    {
                        this.Item[Name] = value;
                    }
                    i++;
                });
                this.Item.Update();
            }
            return this;
        }

        public ArtDevList AddOrUpdateItem (params string[] Values)
        {
            this.GetItem(SPBuiltInFieldId.Title, Values[0]);
            if (this.Item == null)
            {
                SPListItem sPListItem = this.list.Items.Add();
                int i = 0;
                Values.ToList<string>().ForEach(value =>
                {
                    string Name = this.sPFields[i].InternalName;
                    sPListItem[Name] = value;
                    i++;
                });
                sPListItem.Update();
            }
            else
            {
                int i = 0;
                Values.ToList<string>().ForEach(value =>
                {
                    string Name = this.sPFields[i].InternalName;
                    this.Item[Name] = value;                    
                    i++;
                });
                this.Item.Update();
            }
            return this;
        }

        public ArtDevList CreateListView(string Name, bool gridMode)
        {           
            
            SPView view = this.list.Views.Cast<SPView>().FirstOrDefault(c => c.Title.Equals(Name));
            if (view == null)
            {
                StringCollection viewFields = new StringCollection();
                sPFields.ForEach((SPField field) =>
                {
                    viewFields.Add(field.InternalName);
                });                
                string strQuery = "";
                this.list.Views.Add(Name, viewFields, strQuery, 30, true, false, gridMode ? SPViewCollection.SPViewType.Grid : SPViewCollection.SPViewType.Html, false);        
                
            }
            else
            {
                view.ViewFields.DeleteAll();
                sPFields.ForEach((SPField field) =>
                {
                    view.ViewFields.Add(field.InternalName);
                });
                view.Update();
            }
         this.list.Update();
         return this;
        }

        public ArtDevList SetJSLinkView(string Url)
        {
            var file = this.list.ParentWeb.GetFile(this.list.DefaultView.Url);
            file.CheckOut();

            using (var manager = file.GetLimitedWebPartManager(PersonalizationScope.Shared))
            {
                var webPart = manager.WebParts.OfType<XsltListViewWebPart>().FirstOrDefault();
                if (webPart != null && this.list.ParentWeb.AllProperties["DEV"].ToString() != "true")
                {
                    webPart.JSLink = Url ?? string.Empty;
                    manager.SaveChanges(webPart);
                }
                else {
                    webPart.JSLink = string.Empty;
                    manager.SaveChanges(webPart);
                }
            }

            file.CheckIn("Added JSLink to the Form");
            return this;
        }

        public ArtDevList ClearItemsIfModeDev()
        {
            if((string)this.list.ParentWeb.AllProperties["DEV"] == "true")
            {
                if (this.list.BaseType == SPBaseType.GenericList)
                {
                    SPListItemCollection coll = this.list.Items;

                    foreach (SPListItem listitem in coll)
                    {
                        SPListItem itemToDelete = this.list.GetItemById(listitem.ID);
                        itemToDelete.Recycle();
                    }
                }

                if (this.list.BaseType == SPBaseType.DocumentLibrary)
                {
                    string libURL = this.list.ParentWeb.Url + "/" + this.list.RootFolder.Url;
                    SPFolderCollection folders = this.list.ParentWeb.Folders[libURL].SubFolders;

                    foreach (SPFolder folder in folders)
                    {
                        if (!folder.Name.Contains("Forms"))
                        {
                            SPListItem itemToDelete = this.list.GetItemById(folder.Item.ID);
                            itemToDelete.Recycle();
                        }
                    }
                }

            }


            return this;
        }
    }
}
 