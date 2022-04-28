using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AHKUpdater.Model
{
    [Serializable()]
    [DesignerCategory( "code" )]
    [XmlType( AnonymousType = true )]
    [XmlRoot( Namespace = "", IsNullable = false )]
    public partial class ahk
    {

        private ahkSetting[] settingsField;

        private ahkVariable[] variablesField;

        private ahkHotstring[] hotstringsField;

        private ahkFunction[] functionsField;

        private ahkChange[] changelogField;

        [XmlArrayItem( "setting", IsNullable = false )]
        public ahkSetting[] settings
        {
            get
            {
                return this.settingsField;
            }
            set
            {
                this.settingsField = value;
            }
        }

        [XmlArrayItem( "variable", IsNullable = false )]
        public ahkVariable[] variables
        {
            get
            {
                return this.variablesField;
            }
            set
            {
                this.variablesField = value;
            }
        }

        [XmlArrayItem( "hotstring", IsNullable = false )]
        public ahkHotstring[] hotstrings
        {
            get
            {
                return this.hotstringsField;
            }
            set
            {
                this.hotstringsField = value;
            }
        }

        [XmlArrayItem( "function", IsNullable = false )]
        public ahkFunction[] functions
        {
            get
            {
                return this.functionsField;
            }
            set
            {
                this.functionsField = value;
            }
        }

        [XmlArrayItem( "change", IsNullable = false )]
        public ahkChange[] changelog
        {
            get
            {
                return this.changelogField;
            }
            set
            {
                this.changelogField = value;
            }
        }
    }

    [Serializable()]
    [DesignerCategory( "code" )]
    [XmlType( AnonymousType = true )]
    public partial class ahkSetting
    {

        private string settingNameField;

        private string defaultValueField;

        private string valueField;

        [XmlAttribute()]
        public string settingName
        {
            get
            {
                return this.settingNameField;
            }
            set
            {
                this.settingNameField = value;
            }
        }

        [XmlAttribute()]
        public string defaultValue
        {
            get
            {
                return this.defaultValueField;
            }
            set
            {
                this.defaultValueField = value;
            }
        }

        [XmlText()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    [Serializable()]
    [DesignerCategory( "code" )]
    [XmlType( AnonymousType = true )]
    public partial class ahkVariable
    {

        private string variableNameField;

        private string valueField;

        [XmlAttribute()]
        public string variableName
        {
            get
            {
                return this.variableNameField;
            }
            set
            {
                this.variableNameField = value;
            }
        }

        [XmlText()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    [Serializable()]
    [DesignerCategory( "code" )]
    [XmlType( AnonymousType = true )]
    public partial class ahkHotstring
    {

        private string hotstringNameField;

        private string hotstringSystemField;

        private string hotstringMenuTitleField;

        private string valueField;

        [XmlAttribute()]
        public string hotstringName
        {
            get
            {
                return this.hotstringNameField;
            }
            set
            {
                this.hotstringNameField = value;
            }
        }

        [XmlAttribute()]
        public string hotstringSystem
        {
            get
            {
                return this.hotstringSystemField;
            }
            set
            {
                this.hotstringSystemField = value;
            }
        }

        [XmlAttribute()]
        public string hotstringMenuTitle
        {
            get
            {
                return this.hotstringMenuTitleField;
            }
            set
            {
                this.hotstringMenuTitleField = value;
            }
        }

        [XmlText()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    [Serializable()]
    [DesignerCategory( "code" )]
    [XmlType( AnonymousType = true )]
    public partial class ahkFunction
    {

        private string functionNameField;

        private string valueField;

        [XmlAttribute()]
        public string functionName
        {
            get
            {
                return this.functionNameField;
            }
            set
            {
                this.functionNameField = value;
            }
        }

        [XmlText()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    [Serializable()]
    [DesignerCategory( "code" )]
    [XmlType( AnonymousType = true )]
    public partial class ahkChange
    {

        private DateTime versionField;

        private string valueField;

        [XmlAttribute( DataType = "date" )]
        public DateTime version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        [XmlText()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }


}
