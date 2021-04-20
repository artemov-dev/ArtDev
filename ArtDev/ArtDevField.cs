using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtDev
{
    public class ArtDevField
    {
        public SPField field = null;
        public ArtDevField(SPField field)
        {
            this.field = field;
        }

        public ArtDevField SetTitle(string Title)
        {
            this.field.Title = Title;
            this.field.Update(true);
            return this;
        }

        public ArtDevField SetDisplayFormat (Enum Format)
        {            
            if (this.field.Type == SPFieldType.Currency) { ((SPFieldCurrency)this.field).DisplayFormat = (SPNumberFormatTypes)Format; }
            if (this.field.Type == SPFieldType.DateTime) { ((SPFieldDateTime)this.field).DisplayFormat = (SPDateTimeFieldFormatType)Format; }
            this.field.Update(true);
            return this;
        }

        public ArtDevField SetMinimumValue(int Value)
        {
            if (this.field.Type == SPFieldType.Currency) { ((SPFieldCurrency)this.field).MinimumValue = Value; }
            this.field.Update(true);
            return this;
        }

        public ArtDevField SetMultipleValue(bool condition)
        {
            if (this.field.Type == SPFieldType.User) { ((SPFieldUser)this.field).AllowMultipleValues = condition; }
            if (this.field.Type == SPFieldType.Lookup) { ((SPFieldLookup)this.field).AllowMultipleValues = condition; }            
            this.field.Update(true);
            return this;
        }

        public ArtDevField SetDefaultValue(string Value)
        {
            this.field.DefaultValue = Value;
            this.field.Update(true);
            return this;
        }

        public ArtDevField SetChoices(params string[] Choices)
        {
            if (this.field.Type == SPFieldType.Choice)
            {
                ((SPFieldChoice)this.field).Choices.Clear();
                Choices.ToList<string>().ForEach(value => ((SPFieldChoice)this.field).Choices.Add(value));
            }            
            return this; 
        }

        public ArtDevField SetLookupField(string field)
        {
            ((SPFieldLookup)this.field).LookupField = field;
            this.field.Update(true);
            return this;
        }

        public string GetInternalName()
        {
            return this.field.InternalName;
        }

        

    }
}
